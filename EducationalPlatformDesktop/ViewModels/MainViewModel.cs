using System;
using System.Collections.ObjectModel;
using System.Linq;
using EducationalPlatformDesktop.Commands;
using EducationalPlatformDesktop.Mocks;
using EducationalPlatformDesktop.Models;
using EducationalPlatformDesktop.Views;
using EducationalPlatformDesktop.Views.Sections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using EducationalPlatformDesktop.Api;
using EducationalPlatformDesktop.Api.Services;
using EducationalPlatformDesktop.Api.Contracts;

namespace EducationalPlatformDesktop.ViewModels
{
    public class MainViewModel : ViewModelBase // Центральная ViewModel, управляющая навигацией и состоянием приложения
    {
        //Здесь заранее создаются экземпляры экранов
        private readonly HomeView _homeView = new();
        private readonly CoursesView _coursesView = new();
        private readonly ProfileView _profileView = new();
        private readonly LessonView _lessonView = new();
        private readonly ProgressView _progressView = new();
        private readonly TestView _testView = new();
        private readonly TestResultView _testResultView = new();
        private readonly CertificateView _certificateView = new();
        // Создание клиента API для взаимодействия с сервером
        private readonly OnlineSchoolApiClient _apiClient = new(new HttpClient());
        // Словари для хранения состояния курсов пользователя, завершённых уроков и лучших результатов тестов
        private readonly Dictionary<int, UserCourseDto> _userCourseStates = new();
        private readonly Dictionary<int, HashSet<int>> _completedLessonsByCourse = new();
        private readonly Dictionary<int, int> _bestTestScoreByCourse = new();
        private readonly Dictionary<int, Module> _lessonModuleCacheByCourse = new();
        //Часть состояния пока хранится локально в ViewModel, потому что API сервера не предоставляет endpoint’ов и структур для сохранения
        //прогресса по каждому уроку и точного результата теста. Поэтому клиент вынужден временно кэшировать эти данные в памяти текущей сессии

        //Служебные поля состояния
        private TestViewModel? _testViewModel;
        private TestResultViewModel? _testResultViewModel;
        private readonly CertificateViewModel _certificateViewModel;
        private int? _activeTestCourseId;
        private object _currentView = null!;
        private string _pageTitle = "Главная";
        private string _pageDescription = "Стартовый экран платформы EduPrime.";
        private Course? _selectedCourse;
        private Module? _selectedModule;
        private Lesson? _selectedLesson;
        private string _lessonContent = "Выберите урок, чтобы увидеть текст лекции.";
        private string _apiStatusMessage = string.Empty;
        private bool _hasApiStatusMessage;

        // Публичные свойства для привязки к представлениям
        public UserProfile Profile { get; }
        public ObservableCollection<Course> Courses { get; }
        public ObservableCollection<Progress> ProgressItems { get; }
        public ObservableCollection<Certificate> Certificates { get; }
        public ObservableCollection<Module> Modules { get; } = new();
        public ObservableCollection<Lesson> Lessons { get; } = new();

        //Команды позволяют связать действия интерфейса с методами ViewModel
        //без прямой логики в XAML.cs, что соответствует паттерну MVVM
        public RelayCommand ShowHomeCommand { get; }
        public RelayCommand ShowCoursesCommand { get; }
        public RelayCommand ShowProfileCommand { get; }
        public RelayCommand ShowProgressCommand { get; }
        public RelayCommand OpenLessonCommand { get; }
        public RelayCommand CompleteLessonCommand { get; }
        public RelayCommand OpenNextLessonCommand { get; }
        public RelayCommand OpenTestCommand { get; }
        public RelayCommand BackToCoursesCommand { get; }
        public RelayCommand ExitCommand { get; }
        public RelayCommand ShowCertificatesCommand { get; }
        //Команды позволяют связать действия интерфейса с методами ViewModel без прямой логики в XAML.cs, что соответствует паттерну MVVM

        //Свойство CurrentView и навигация. Хранит текущий отображаемый экран
        public object CurrentView
        {
            get => _currentView;
            set
            {
                if (_currentView != value)
                {
                    _currentView = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsHomeVisible));
                    OnPropertyChanged(nameof(IsCoursesVisible));
                    OnPropertyChanged(nameof(IsProfileVisible));
                    OnPropertyChanged(nameof(IsProgressVisible));
                    OnPropertyChanged(nameof(IsLessonVisible));
                    OnPropertyChanged(nameof(IsTestVisible));
                    OnPropertyChanged(nameof(IsTestResultVisible));
                    OnPropertyChanged(nameof(IsCertificatesVisible));
                }
            }
        } //Выбрана более простая схема на UserControl и переключении состояния через CurrentView,
          //потому что приложение небольшое и не требует сложной маршрутизации

        public string PageTitle
        {
            get => _pageTitle;
            set
            {
                if (_pageTitle != value)
                {
                    _pageTitle = value;
                    OnPropertyChanged();
                }
            }
        }
        public string PageDescription
        {
            get => _pageDescription;
            set
            {
                if (_pageDescription != value)
                {
                    _pageDescription = value;
                    OnPropertyChanged();
                }
            }
        } //интерфейс не просто меняет экран, а еще и текстово сообщает, где находится пользователь.

        // Производные свойства для определения видимости различных экранов
        public bool IsHomeVisible => CurrentView == _homeView;
        public bool IsCoursesVisible => CurrentView == _coursesView;
        public bool IsProfileVisible => CurrentView == _profileView;
        public bool IsProgressVisible => CurrentView == _progressView;
        public bool IsLessonVisible => CurrentView == _lessonView;
        public bool IsTestVisible => CurrentView == _testView;
        public bool IsTestResultVisible => CurrentView == _testResultView;
        public bool IsCertificatesVisible => CurrentView == _certificateView;
        //Вместо хранения нескольких независимых флагов я использую единый источник истины CurrentView,
        //а свойства видимости вычисляются от него. Это снижает риск рассинхронизации состояния

        //Сводные счетчики
        public int ActiveCoursesCount =>
            Courses.Count(course => course.Progress > 0 && !course.IsCourseCompleted);
        public int LessonsCompletedCount => ProgressItems.Sum(item => item.CompletedLessons);
        public int CertificatesCount => Certificates.Count;
        public int CompletedCoursesCount => ProgressItems.Count(item => item.IsCompleted);
        public bool HasCourses => Courses.Count > 0;
        public bool HasProgressItems => ProgressItems.Count > 0;
        public bool HasCertificates => Certificates.Count > 0;
        public bool IsCoursesEmpty => !HasCourses;
        public bool IsProgressEmpty => !HasProgressItems;
        public bool IsCertificatesEmpty => !HasCertificates;
        public int AverageProgress => ProgressItems.Count == 0
       ? 0
       : (int)Math.Round(
           ProgressItems.Average(item => item.OverallPercentage));
        public string TestAvailabilityMessage
        {
            get
            {
                if (SelectedCourse == null)
                {
                    return "Выберите курс, чтобы увидеть доступность теста.";
                }
                if (SelectedCourse.CanTakeFinalTest)
                {
                    return "Итоговый тест доступен.";
                }
                return "Итоговый тест откроется после прохождения не менее 90% уроков.";
            }
        }
        public bool HasNextLesson => GetNextLesson() != null;
        public string NextLessonButtonText
        {
            get
            {
                var nextLesson = GetNextLesson();

                return nextLesson == null
                    ? "Следующего урока нет"
                    : $"Следующий урок: {nextLesson.Title}";
            }
        }

        // Свойства для выбранного курса, модуля и урока
        public Course? SelectedCourse
        {
            get => _selectedCourse;
            set
            {
                if (_selectedCourse != value)
                {
                    _selectedCourse = value;
                    OnPropertyChanged();
                    UpdateModulesForSelectedCourse();
                    OpenTestCommand.RaiseCanExecuteChanged();
                    OnPropertyChanged(nameof(TestAvailabilityMessage));
                }
            }
        }
        public Module? SelectedModule
        {
            get => _selectedModule;
            set
            {
                if (_selectedModule != value)
                {
                    _selectedModule = value;
                    OnPropertyChanged();
                    UpdateLessonsForSelectedModule();
                }
            }
        }
        public Lesson? SelectedLesson
        {
            get => _selectedLesson;
            set
            {
                if (_selectedLesson != value)
                {
                    _selectedLesson = value;
                    OnPropertyChanged();
                    UpdateLessonContent();
                }
            }
        }
        public string LessonContent
        {
            get => _lessonContent;
            set
            {
                if (_lessonContent != value)
                {
                    _lessonContent = value;
                    OnPropertyChanged();
                }
            }
        }
        public string ApiStatusMessage
        {
            get => _apiStatusMessage;
            set
            {
                if (_apiStatusMessage != value)
                {
                    _apiStatusMessage = value;
                    OnPropertyChanged();
                }
            }
        }
        public bool HasApiStatusMessage
        {
            get => _hasApiStatusMessage;
            set
            {
                if (_hasApiStatusMessage != value)
                {
                    _hasApiStatusMessage = value;
                    OnPropertyChanged();
                }
            }
        }
        public event Action? RequestClose; //Закрытие окна реализовано через событие,
                                           //чтобы ViewModel не зависела от конкретного экземпляра окна и не управляла UI напрямую


        // Конструктор инициализирует состояние ViewModel, загружает данные и настраивает команды
        public MainViewModel()
        {
            Profile = CreateProfileFromSession();
            Courses = new ObservableCollection<Course>();
            Certificates = new ObservableCollection<Certificate>();
            _apiClient.SetBearerToken(AppSession.Token);
            ProgressItems = new ObservableCollection<Progress>(
                Courses.Select(CreateProgressItem));
            _certificateViewModel = new CertificateViewModel(Certificates);
            _certificateView.DataContext =
                _certificateViewModel;
            // Инициализация команд
            ShowHomeCommand = new RelayCommand(_ => ShowHome());
            ShowCoursesCommand = new RelayCommand(_ => ShowCourses());
            ShowProfileCommand = new RelayCommand(_ => ShowProfile());
            ShowProgressCommand = new RelayCommand(_ => ShowProgress());
            OpenLessonCommand = new RelayCommand(param => OpenLesson(param as Lesson));
            ShowCertificatesCommand = new RelayCommand(_ => ShowCertificates());
            CompleteLessonCommand =
                new RelayCommand(param => CompleteLesson(param as Lesson));

            OpenNextLessonCommand =
                new RelayCommand(
                    _ => OpenNextLesson(),
                    _ => HasNextLesson);
            //Команда итогового теста использует предикат CanExecute, поэтому доступность
            //действия определяется состоянием модели курса, а не вручную через интерфейс
            OpenTestCommand =
                new RelayCommand(
                 async _ => await OpenTestAsync(),
                 _ => SelectedCourse?.CanTakeFinalTest == true);
            BackToCoursesCommand = new RelayCommand(_ => ShowCourses());
            ExitCommand = new RelayCommand(_ => RequestClose?.Invoke());
            // Установка стартового представления
            CurrentView = _homeView;

            // Выбор первого курса если есть
            if (Courses.Count > 0)
            {
                SelectedCourse = Courses.First();
            }
            _ = InitializeCoursesAsync();
        }

        //Методы навигации по разделам
        private void ShowHome()
        {
            CurrentView = _homeView;
            PageTitle = "Главная";
            PageDescription = "Стартовый экран платформы EduPrime.";
        }
        private void ShowCourses()
        {
            CurrentView = _coursesView;
            PageTitle = "Курсы";
            PageDescription = "Просмотр курсов и уроков.";
        }
        private void ShowProfile()
        {
            CurrentView = _profileView;
            PageTitle = "Профиль";
            PageDescription = "Информация об авторизованном пользователе.";
        }
        private void ShowProgress()
        {
            CurrentView = _progressView;
            PageTitle = "Прогресс";
            PageDescription = "Отслеживайте ваш прогресс по всем курсам.";
        }
        private void ShowCertificates()
        {
            CurrentView = _certificateView;
            PageTitle = "Сертификаты";
            PageDescription =
                "Просмотр сертификатов за завершённые курсы.";
        }

        //Открытие урока
        private void OpenLesson(Lesson? lesson)
        {
            if (lesson == null)
                return;

            SelectedLesson = lesson;
            LessonContent = lesson.Content;
            CurrentView = _lessonView;
            PageTitle = lesson.Title;
            PageDescription = SelectedCourse != null
                ? SelectedCourse.Title
                : "Текст лекции";
        }

        //Переход к следующему уроку
        private void OpenNextLesson()
        {
            var nextLesson = GetNextLesson();

            if (nextLesson == null)
            {
                return;
            }

            OpenLesson(nextLesson);
        } //новая функция переиспользует уже готовую логику, а не дублирует код.

        //Открытие теста
        private async Task OpenTestAsync()
        {
            if (SelectedCourse == null)
            {
                return;
            }
            if (!SelectedCourse.CanTakeFinalTest)
            {
                return;
            }

            _activeTestCourseId = SelectedCourse.Id;

            var testsResult = await _apiClient.GetTestsByCourseIdAsync(SelectedCourse.Id);

            if (!testsResult.IsSuccess)
            {
                ApiStatusMessage = testsResult.Message;
                HasApiStatusMessage = true;
                return;
            }

            if (testsResult.Data == null || testsResult.Data.Count == 0)
            {
                ApiStatusMessage = "Для этого курса пока нет тестов.";
                HasApiStatusMessage = true;
                return;
            }

            var test = MockTestData.GetTestForCourse(SelectedCourse.Id);
            test.Title = testsResult.Data[0].Name;

            _testViewModel = new TestViewModel(test);
            _testViewModel.TestCompleted += result => _ = HandleTestCompletedAsync(result);

            _testViewModel.BackRequested += ShowCourses;


            _testView.DataContext = _testViewModel;

            CurrentView = _testView;
            PageTitle = _testViewModel.Title;
            PageDescription =
                "Выберите один ответ. Можно использовать клавиши 1–4 и Enter.";
        } //На текущем этапе backend API предоставляет только метаданные теста по курсу, но не предоставляет структуру вопросов и вариантов ответов.
          //Поэтому клиентская часть использует серверный факт наличия теста и его название, а содержимое теста временно загружает из локальных мок-данных

        //Завершение теста
        private async Task HandleTestCompletedAsync(TestResult result)
        {
            var course = Courses.FirstOrDefault(
                item => item.Id == result.CourseId);
            if (course == null)
            {
                return;
            }
            result.CourseName = course.Title;
            await FinishTestOnApiAsync(course.Id);
            course.ApplyTestResult(result.ScorePercent);
            _bestTestScoreByCourse[course.Id] = course.BestTestScore;
            UpdateProgressForCourse(course);
            NotifyProgressSummaryChanged();
            await GenerateCertificateIfEligibleAsync(course, result);
            _testResultViewModel = new TestResultViewModel(
                result,
                course.CanReceiveCertificate);
            _testResultViewModel.RetryRequested += () => _ = RetryCurrentTestAsync();
            _testResultViewModel.BackToCoursesRequested += ShowCourses;
            _testResultView.DataContext = _testResultViewModel;
            CurrentView = _testResultView;
            PageTitle = "Результат теста";
            PageDescription = course.Title;
        } //Серверу отправляется факт завершения теста и завершения курса. Но точный процент результата клиент сейчас рассчитывает сам,
          //потому что серверный контракт пока не возвращает полную оценку попытки в пригодном для восстановления виде

        //Обновление курса при смене выбора
        private void UpdateModulesForSelectedCourse()
        {
            Modules.Clear();
            Lessons.Clear();
            _selectedModule = null;
            _selectedLesson = null;
            OnPropertyChanged(nameof(SelectedModule));
            OnPropertyChanged(nameof(SelectedLesson));
            LessonContent = "Выберите урок, чтобы увидеть текст лекции.";
            if (SelectedCourse == null)
            {
                return;
            }
            _ = LoadLessonsForSelectedCourseAsync();
        }

        //Обновление уроков выбранного модуля
        private void UpdateLessonsForSelectedModule()
        {
            Lessons.Clear();
            _selectedLesson = null;
            OnPropertyChanged(nameof(SelectedLesson));
            LessonContent = "Выберите урок, чтобы увидеть текст лекции.";
            if (SelectedModule == null)
                return;
            foreach (var lesson in SelectedModule.Lessons)
                Lessons.Add(lesson);
            OnPropertyChanged(nameof(HasNextLesson));
            OnPropertyChanged(nameof(NextLessonButtonText));
            OpenNextLessonCommand.RaiseCanExecuteChanged();
        }
        private void UpdateLessonContent()
        {
            LessonContent = SelectedLesson?.Content ?? "Выберите урок, чтобы увидеть текст лекции.";
            OnPropertyChanged(nameof(HasNextLesson));
            OnPropertyChanged(nameof(NextLessonButtonText));
            OpenNextLessonCommand.RaiseCanExecuteChanged();
        }

        //Навигация по урокам реализована через текущую плоскую коллекцию уроков активного модуля,
        //без дополнительных структур.
        private Lesson? GetNextLesson()
        {
            if (SelectedLesson == null || Lessons.Count == 0)
            {
                return null;
            }
            var currentLessonIndex = Lessons.IndexOf(SelectedLesson);
            if (currentLessonIndex < 0 || currentLessonIndex >= Lessons.Count - 1)
            {
                return null;
            }
            return Lessons[currentLessonIndex + 1];
        }

        //Инициализация данных приложения
        private async Task InitializeCoursesAsync()
        {
            await LoadCoursesAsync();
            await LoadUserProgressFromApiAsync();
            if (Courses.Count > 0)
            {
                SelectedCourse = Courses.First();
            }
            else
            {
                Modules.Clear();
                Lessons.Clear();
                SelectedCourse = null;
                SelectedModule = null;
                SelectedLesson = null;
                LessonContent = "Курсы не найдены.";
                ApiStatusMessage = "Курсы не найдены.";
                HasApiStatusMessage = true;
            }
        }

        //Загрузка списка курсов из API
        private async Task LoadCoursesAsync()
        {
            try
            {
                var courseDtos = await _apiClient.GetCoursesAsync();
                Courses.Clear();
                foreach (var dto in courseDtos)
                {
                    Courses.Add(MapCourse(dto));
                }
                NotifyCourseSummaryChanged();
                if (Courses.Count == 0)
                {
                    ApiStatusMessage = "Курсы не найдены.";
                    HasApiStatusMessage = true;
                }
                else
                {
                    ApiStatusMessage = string.Empty;
                    HasApiStatusMessage = false;
                }
            }
            catch
            {
                Courses.Clear();
                NotifyCourseSummaryChanged();

                ApiStatusMessage = "Не удалось загрузить курсы из API.";
                HasApiStatusMessage = true;
            }
        }

        //Загрузка уроков выбранного курса
        private async Task LoadLessonsForSelectedCourseAsync()
        {
            var course = SelectedCourse;
            if (course == null)
            {
                return;
            }
            try
            {
                var lessonDtos = await _apiClient.GetLessonsByCourseIdAsync(course.Id);
                var module = new Module
                {
                    Id = course.Id,
                    CourseId = course.Id,
                    Title = "Лекции курса"
                };
                foreach (var dto in lessonDtos)
                {
                    module.Lessons.Add(MapLesson(dto));
                }
                ApplyProgressToModule(course, module);
                _lessonModuleCacheByCourse[course.Id] = module;
                ShowModuleForCourse(course, module);
            }
            catch
            {
                if (_lessonModuleCacheByCourse.TryGetValue(course.Id, out var cachedModule))
                {
                    ShowModuleForCourse(course, cachedModule);
                    ApiStatusMessage =
                        "Не удалось обновить данные из API. Показаны ранее загруженные уроки курса.";
                    HasApiStatusMessage = true;
                    return;
                }

                Modules.Clear();
                Lessons.Clear();
                SelectedModule = null;
                SelectedLesson = null;
                LessonContent = "Не удалось загрузить лекции курса.";
            }
        }
        //Backend API возвращает уроки в разрезе курса, без отдельной сущности модулей в контракте, поэтому на клиенте используется упрощенное представление: все лекции курса объединяются в один модуль.
        //Это решение позволяет сохранить понятную структуру интерфейса без искусственного усложнения

        //Применение серверного прогресса к урокам
        private static void ApplyUserCourseProgress(Module module, int progressPercent)
        {
            if (module.Lessons.Count == 0)
            {
                return;
            }
            var completedCount = (int)Math.Round(
                module.Lessons.Count * progressPercent / 100.0);
            foreach (var lesson in module.Lessons.Take(completedCount))
            {
                lesson.IsCompleted = true;
            }
        }

        private void ApplyProgressToModule(Course course, Module module)
        {
            foreach (var lesson in module.Lessons)
            {
                lesson.IsCompleted = false;
            }

            if (_userCourseStates.TryGetValue(course.Id, out var userCourse))
            {
                ApplyUserCourseProgress(module, userCourse.Progress);
                course.IsPurchased = true;
                course.RefreshProgress();
            }

            if (_completedLessonsByCourse.TryGetValue(course.Id, out var completedLessons))
            {
                foreach (var lesson in module.Lessons.Where(
                             lesson => completedLessons.Contains(lesson.Id)))
                {
                    lesson.IsCompleted = true;
                }

                course.SetProgressOverride(null);
                course.RefreshProgress();
            }
        }

        private void ShowModuleForCourse(Course course, Module module)
        {
            course.Modules.Clear();
            course.Modules.Add(module);

            Modules.Clear();
            Modules.Add(module);

            SelectedModule = module;

            if (module.Lessons.Count > 0)
            {
                SelectedLesson = module.Lessons.First();
            }
            else
            {
                SelectedLesson = null;
                LessonContent = "Для этого курса пока нет лекций.";
            }
        }
        //Поскольку API хранит агрегированный процент прохождения курса, а не список конкретных завершенных уроков, клиент реконструирует прогресс приближенно, помечая завершенными первые уроки в количестве, соответствующем серверному проценту.
        //Для точного восстановления нужен отдельный endpoint или структура хранения прогресса по урокам

        //Маппинг DTO в клиентские модели
        private static Course MapCourse(CourseDto dto)
        {
            return new Course
            {
                Id = dto.Id,
                Title = dto.Name ?? string.Empty,
                Track = dto.Purpose ?? string.Empty,
                Teacher = dto.Teacher ?? string.Empty,
                Description = dto.Description ?? string.Empty,
                IsPurchased = false
            };
        }

        private static Lesson MapLesson(LessonDto dto)
        {
            return new Lesson
            {
                Id = dto.Id,
                CourseId = dto.CourseId,
                Title = dto.Name ?? string.Empty,
                Description = dto.Description ?? string.Empty,
                Content = !string.IsNullOrWhiteSpace(dto.Content)
                    ? dto.Content
                    : dto.Description ?? string.Empty,
                Url = dto.Url ?? string.Empty,
                DurationMinutes = EstimateDurationMinutes(dto.Content)
            };
        }

        private static int EstimateDurationMinutes(string? content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return 5;
            }
            return Math.Max(5, (int)Math.Ceiling(content.Length / 240.0) * 5);
        }
        //Длительность урока вычисляется локально, потому что API не предоставляет длительность в явном виде,
        //поэтому клиент вычисляет приблизительное значение для интерфейса

        //Формирование профиля из сессии: профиль в текущей версии не загружается отдельным API-запросом, а собирается из данных авторизации.
        private static UserProfile CreateProfileFromSession()
        {
            var fullName = AppSession.FullName;
            if (string.IsNullOrWhiteSpace(fullName))
            {
                fullName = "Студент";
            }
            return new UserProfile
            {
                FullName = fullName,
                EmailOrMax = AppSession.Email ?? string.Empty,
                Role = AppSession.Role ?? "student"
            };
        }

        //Методы уведомления о пересчете сводных данных
        private void NotifyCourseSummaryChanged()
        {
            OnPropertyChanged(nameof(HasCourses));
            OnPropertyChanged(nameof(IsCoursesEmpty));
            OnPropertyChanged(nameof(ActiveCoursesCount));
            OnPropertyChanged(nameof(LessonsCompletedCount));
            OpenTestCommand.RaiseCanExecuteChanged();
        }

        //Завершение урока
        private void CompleteLesson(Lesson? lesson)
        {
            if (lesson == null || lesson.IsCompleted)
            {
                return;
            }
            lesson.IsCompleted = true;

            if (!_completedLessonsByCourse.TryGetValue(lesson.CourseId, out var completedLessons))
            {
                completedLessons = new HashSet<int>();
                _completedLessonsByCourse[lesson.CourseId] = completedLessons;
            }
            completedLessons.Add(lesson.Id);
            var course = Courses.FirstOrDefault(
                item => item.Id == lesson.CourseId);

            if (course == null)
            {
                return;
            }
            course.SetProgressOverride(null);
            course.RefreshProgress();
            UpdateProgressForCourse(course);
            NotifyProgressSummaryChanged();
            OpenTestCommand.RaiseCanExecuteChanged();
            OnPropertyChanged(nameof(TestAvailabilityMessage));
        }
        private void UpdateProgressForCourse(Course course)
        {
            UpdateProgressForCourse(CreateProgressItem(course));
        }
        private void NotifyProgressSummaryChanged()
        {
            OnPropertyChanged(nameof(ActiveCoursesCount));
            OnPropertyChanged(nameof(LessonsCompletedCount));
            OnPropertyChanged(nameof(CompletedCoursesCount));
            OnPropertyChanged(nameof(AverageProgress));
            OpenTestCommand.RaiseCanExecuteChanged();
            OnPropertyChanged(nameof(HasCourses));
            OnPropertyChanged(nameof(HasProgressItems));
            OnPropertyChanged(nameof(HasCertificates));
            OnPropertyChanged(nameof(IsCoursesEmpty));
            OnPropertyChanged(nameof(IsProgressEmpty));
            OnPropertyChanged(nameof(IsCertificatesEmpty));
        }
        //Отметка урока как пройденного в текущей реализации обновляет локальное состояние клиента и интерфейса, но не отправляется на сервер,
        //так как серверный API не содержит отдельного механизма фиксации прохождения уроков поштучно

        //Повтор теста
        private async Task RetryCurrentTestAsync()
        {
            if (_activeTestCourseId == null)
            {
                return;
            }
            SelectedCourse = Courses.FirstOrDefault(
                course => course.Id == _activeTestCourseId.Value);
            await OpenTestAsync();
        }

        //Загрузка пользовательского прогресса из API
        private async Task LoadUserProgressFromApiAsync()
        {
            if (AppSession.UserId == null ||
                string.IsNullOrWhiteSpace(AppSession.Token))
            {
                ApiStatusMessage =
                    "Пользователь не авторизован. Данные API недоступны.";
                HasApiStatusMessage = true;
                return;
            }
            try
            {
                var result = await _apiClient.GetUserCoursesAsync(
                    AppSession.UserId.Value);
                if (!result.IsSuccess)
                {
                    ApiStatusMessage = result.Message;
                    HasApiStatusMessage = true;
                    return;
                }
                _userCourseStates.Clear();
                if (result.Data == null || result.Data.Count == 0)
                {
                    Courses.Clear();
                    ProgressItems.Clear();
                    _bestTestScoreByCourse.Clear();
                    Certificates.Clear();

                    ApiStatusMessage = "У вас пока нет приобретённых курсов.";
                    HasApiStatusMessage = true;

                    NotifyProgressSummaryChanged();
                    return;
                }
                ApiStatusMessage = string.Empty;
                HasApiStatusMessage = false;
                Certificates.Clear();
                var allowedCourseIds = result.Data
                    .Select(item => item.CourseId)
                    .ToHashSet();
                var visibleCourses = Courses
                    .Where(course => allowedCourseIds.Contains(course.Id))
                    .ToList();
                Courses.Clear();
                foreach (var course in visibleCourses)
                {
                    course.IsPurchased = false;
                    course.SetProgressOverride(null);
                    course.RefreshProgress();
                    Courses.Add(course);
                }
                ProgressItems.Clear();
                foreach (var userCourse in result.Data)
                {
                    _userCourseStates[userCourse.CourseId] = userCourse;
                    var course = Courses.FirstOrDefault(
                        item => item.Id == userCourse.CourseId);
                    if (course == null)
                    {
                        continue;
                    }
                    course.IsPurchased = true;
                    course.SetProgressOverride(userCourse.Progress);
                    if (_bestTestScoreByCourse.TryGetValue(course.Id, out var cachedScore))
                    {
                        course.BestTestScore = cachedScore;
                    }
                    var lessonCount = await TryGetLessonCountAsync(course.Id);
                    var completedLessons = lessonCount == 0
                        ? 0
                        : (int)Math.Round(lessonCount * userCourse.Progress / 100.0);
                    UpdateProgressForCourse(new Progress
                    {
                        CourseId = course.Id,
                        CourseName = course.Title,
                        CompletedLessons = completedLessons,
                        TotalLessons = lessonCount,
                        TestScore = course.BestTestScore,
                        OverallPercentage = userCourse.Progress,
                        IsCompleted = userCourse.Completed
                    });
                }
                await LoadCertificatesFromApiAsync(
                    result.Data
                        .Where(item => item.Completed)
                        .Select(item => item.CourseId));
                NotifyCourseSummaryChanged();
                NotifyProgressSummaryChanged();
                OnPropertyChanged(nameof(CertificatesCount));
                OnPropertyChanged(nameof(HasCertificates));
                OnPropertyChanged(nameof(IsCertificatesEmpty));
            }
            catch
            {
                ApiStatusMessage =
                    "Не удалось подключиться к API.";
                HasApiStatusMessage = true;
            }
        }

        //Создание объекта прогресса
        private static Progress CreateProgressItem(Course course)
        {
            return new Progress
            {
                CourseId = course.Id,
                CourseName = course.Title,
                CompletedLessons = course.CompletedLessons,
                TotalLessons = course.TotalLessons,
                TestScore = course.BestTestScore,
                OverallPercentage = course.Progress,
                IsCompleted = course.IsCourseCompleted
            };
        }
        private void UpdateProgressForCourse(Progress item)
        {
            var oldItem = ProgressItems.FirstOrDefault(
                progress => progress.CourseId == item.CourseId);

            if (oldItem == null)
            {
                ProgressItems.Add(item);
                return;
            }

            var index = ProgressItems.IndexOf(oldItem);
            ProgressItems[index] = item;
        }

        //Подсчет числа уроков через API. Метод защищен, если backend недоступен, приложение не падает.
        private async Task<int> TryGetLessonCountAsync(int courseId)
        {
            try
            {
                var lessons = await _apiClient.GetLessonsByCourseIdAsync(courseId);
                return lessons.Count;
            }
            catch
            {
                return 0;
            }
        }

        //Загрузка сертификатов из API.ет единого endpoint списка сертификатов пользователя.
        //Поэтому клиент вынужден запрашивать сертификат по каждому курсу отдельно.
        private async Task LoadCertificatesFromApiAsync(IEnumerable<int> completedCourseIds)
        {
            if (AppSession.UserId == null ||
                string.IsNullOrWhiteSpace(AppSession.Token))
            {
                return;
            }
            foreach (var courseId in completedCourseIds.Distinct())
            {
                try
                {
                    var response = await _apiClient.GetCertificateAsync(
                        AppSession.UserId.Value,
                        courseId);

                    if (!response.IsSuccess || response.Data == null)
                    {
                        continue;
                    }
                    var certificate = new Certificate
                    {
                        Id = Guid.NewGuid().ToString("N"),
                        Number = response.Data.CertificateNumber,
                        CourseId = courseId,
                        CourseName = response.Data.CourseName,
                        StudentName = response.Data.FullName,
                        IssuedAt = response.Data.IssueDate,
                        // API не предоставляет точную оценку теста, только состояние завершения.
                        Score = 100
                    };
                    Certificates.Add(certificate);
                    var course = Courses.FirstOrDefault(item => item.Id == courseId);
                    if (course != null)
                    {
                        course.BestTestScore = Math.Max(course.BestTestScore, certificate.Score);
                        var progressItem = ProgressItems.FirstOrDefault(
                            item => item.CourseId == courseId);
                        if (progressItem != null)
                        {
                            progressItem.TestScore = course.BestTestScore;
                            progressItem.IsCompleted = true;
                            UpdateProgressForCourse(progressItem);
                        }
                    }
                }
                catch
                {
                    // Игнорируем сбой загрузки одного сертификата, чтобы остальная часть страницы по-прежнему отображалась.
                }
            }
        }

        //Отправка завершения теста на сервер
        private async Task<int?> FinishTestOnApiAsync(int courseId)
        {
            if (AppSession.UserId == null ||
                string.IsNullOrWhiteSpace(AppSession.Token))
            {
                ApiStatusMessage =
                    "Тест завершён локально. Для отправки результата на сервер нужно войти в систему.";
                HasApiStatusMessage = true;
                return null;
            }
            try
            {
                var result = await _apiClient.FinishTestAsync(
                    new EducationalPlatformDesktop.Api.Contracts.TestFinishRequestDto
                    {
                        UserId = AppSession.UserId.Value,
                        CourseId = courseId
                    });
                if (!result.IsSuccess)
                {
                    ApiStatusMessage = result.Message;
                    HasApiStatusMessage = true;
                    return null;
                }
                ApiStatusMessage = string.Empty;
                HasApiStatusMessage = false;
                return result.Data?.Score;
            }
            catch
            {
                ApiStatusMessage =
                    "Не удалось отправить результат теста на сервер. Показан локальный результат.";
                HasApiStatusMessage = true;
                return null;
            }
        } //по backend этот endpoint в основном фиксирует завершение курса, а не хранит полноценную историю результатов.
          //Поэтому это не полноценное серверное сохранение тестовой попытки.

        //Генерация сертификата при успешном завершении курса
        private async Task GenerateCertificateIfEligibleAsync(
     Course course,
     TestResult result)
        {
            if (!course.CanReceiveCertificate ||
                !result.IsPassed)
            {
                return;
            }

            var alreadyExists = Certificates.Any(
                certificate => certificate.CourseId == course.Id);

            if (alreadyExists)
            {
                return;
            }

            var certificateFromApi =
                await TryGetCertificateFromApiAsync(course.Id, result);

            if (certificateFromApi != null)
            {
                Certificates.Add(certificateFromApi);

                OnPropertyChanged(nameof(CertificatesCount));
                OnPropertyChanged(nameof(HasCertificates));
                OnPropertyChanged(nameof(IsCertificatesEmpty));

                return;
            }

            var localCertificate = new Certificate
            {
                Id = Guid.NewGuid().ToString("N"),
                Number =
                    $"CERT-{DateTime.Now:yyyyMMdd}-" +
                    $"{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}",
                CourseId = course.Id,
                CourseName = course.Title,
                StudentName = !string.IsNullOrWhiteSpace(AppSession.FullName)
                    ? AppSession.FullName
                    : Profile.FullName,
                IssuedAt = DateTime.Now,
                Score = result.ScorePercent,
                TestResultId = result.AttemptId
            };

            Certificates.Add(localCertificate);

            OnPropertyChanged(nameof(CertificatesCount));
            OnPropertyChanged(nameof(HasCertificates));
            OnPropertyChanged(nameof(IsCertificatesEmpty));
        }

        //Получение сертификата из API. сертификат в WPF сейчас не “загружается как готовый документ”.
        //Он формируется как экран по данным.
        private async Task<Certificate?> TryGetCertificateFromApiAsync(
    int courseId,
    TestResult result)
        {
            if (AppSession.UserId == null ||
                string.IsNullOrWhiteSpace(AppSession.Token))
            {
                ApiStatusMessage =
                    "Сертификат создан локально. Для получения серверного сертификата нужно войти в систему.";
                HasApiStatusMessage = true;
                return null;
            }

            try
            {
                var response = await _apiClient.GetCertificateAsync(
                    AppSession.UserId.Value,
                    courseId);

                if (!response.IsSuccess)
                {
                    ApiStatusMessage = response.Message;
                    HasApiStatusMessage = true;
                    return null;
                }

                if (response.Data == null)
                {
                    ApiStatusMessage =
                        "Сервер не вернул данные сертификата. Создан локальный сертификат.";
                    HasApiStatusMessage = true;
                    return null;
                }

                ApiStatusMessage = string.Empty;
                HasApiStatusMessage = false;

                return new Certificate
                {
                    Id = Guid.NewGuid().ToString("N"),
                    Number = response.Data.CertificateNumber,
                    CourseId = courseId,
                    CourseName = response.Data.CourseName,
                    StudentName = response.Data.FullName,
                    IssuedAt = response.Data.IssueDate,
                    Score = result.ScorePercent,
                    TestResultId = result.AttemptId
                };
            }
            catch
            {
                ApiStatusMessage =
                    "Не удалось получить сертификат с сервера. Создан локальный сертификат.";
                HasApiStatusMessage = true;
                return null;
            }
        }
    }
    
}
