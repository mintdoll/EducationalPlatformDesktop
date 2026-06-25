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
    public class MainViewModel : ViewModelBase
    {

        private readonly HomeView _homeView = new();
        private readonly CoursesView _coursesView = new();
        private readonly ProfileView _profileView = new();
        private readonly LessonView _lessonView = new();
        private readonly ProgressView _progressView = new();
        private readonly TestView _testView = new();
        private readonly TestResultView _testResultView = new();
        private readonly CertificateView _certificateView = new();
        private readonly OnlineSchoolApiClient _apiClient = new(new HttpClient());
        private readonly Dictionary<int, UserCourseDto> _userCourseStates = new();

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


        public UserProfile Profile { get; }
        public ObservableCollection<Course> Courses { get; }
        public ObservableCollection<Progress> ProgressItems { get; }
        public ObservableCollection<Certificate> Certificates { get; }
        public ObservableCollection<Module> Modules { get; } = new();
        public ObservableCollection<Lesson> Lessons { get; } = new();


        public RelayCommand ShowHomeCommand { get; }
        public RelayCommand ShowCoursesCommand { get; }
        public RelayCommand ShowProfileCommand { get; }
        public RelayCommand ShowProgressCommand { get; }
        public RelayCommand OpenLessonCommand { get; }
        public RelayCommand CompleteLessonCommand { get; }
        public RelayCommand OpenTestCommand { get; }
        public RelayCommand BackToCoursesCommand { get; }
        public RelayCommand ExitCommand { get; }
        public RelayCommand ShowCertificatesCommand { get; }

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
        }


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
        }


        public bool IsHomeVisible => CurrentView == _homeView;
        public bool IsCoursesVisible => CurrentView == _coursesView;
        public bool IsProfileVisible => CurrentView == _profileView;
        public bool IsProgressVisible => CurrentView == _progressView;
        public bool IsLessonVisible => CurrentView == _lessonView;
        public bool IsTestVisible => CurrentView == _testView;
        public bool IsTestResultVisible => CurrentView == _testResultView;
        public bool IsCertificatesVisible => CurrentView == _certificateView;

        public int ActiveCoursesCount => Courses.Count(course => course.Progress > 0 && course.Progress < 100);
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

        public event Action? RequestClose;


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
            PageDescription = "Просмотр курсов, модулей и уроков.";
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
        private void OpenLesson(Lesson? lesson)
        {
            if (lesson == null)
                return;

            SelectedLesson = lesson;
            LessonContent = lesson.Content;
            CurrentView = _lessonView;
            PageTitle = lesson.Title;
            PageDescription = SelectedCourse != null
                ? $"{SelectedCourse.Title} · {SelectedModule?.Title}"
                : "Текст лекции";
        }

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
            _testViewModel.TestCompleted += OnTestCompleted;

            _testViewModel.BackRequested += ShowCourses;


            _testView.DataContext = _testViewModel;

            CurrentView = _testView;
            PageTitle = _testViewModel.Title;
            PageDescription =
                "Выберите один ответ. Можно использовать клавиши 1–4 и Enter.";
        }
        private async void OnTestCompleted(TestResult result)
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

            UpdateProgressForCourse(course);
            NotifyProgressSummaryChanged();
            await GenerateCertificateIfEligibleAsync(course, result);

            _testResultViewModel = new TestResultViewModel(
                result,
                course.CanReceiveCertificate);

            _testResultViewModel.RetryRequested += RetryCurrentTest;
            _testResultViewModel.BackToCoursesRequested += ShowCourses;

            _testResultView.DataContext = _testResultViewModel;

            CurrentView = _testResultView;
            PageTitle = "Результат теста";
            PageDescription = course.Title;
        }

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
        }

        private void UpdateLessonContent()
        {
            LessonContent = SelectedLesson?.Content ?? "Выберите урок, чтобы увидеть текст лекции.";
        }
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

                course.Modules.Clear();
                course.Modules.Add(module);

                Modules.Clear();
                Modules.Add(module);

                if (_userCourseStates.TryGetValue(course.Id, out var userCourse))
                {
                    ApplyUserCourseProgress(module, userCourse.Progress);
                    course.IsPurchased = true;
                    course.RefreshProgress();
                }

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
            catch
            {
                Modules.Clear();
                Lessons.Clear();
                SelectedModule = null;
                SelectedLesson = null;
                LessonContent = "Не удалось загрузить лекции курса.";
            }
        }

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
                ModuleId = dto.CourseId,
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
                Group = AppSession.Role ?? "student",
                Role = AppSession.Role ?? "student"
            };
        }

        private void NotifyCourseSummaryChanged()
        {
            OnPropertyChanged(nameof(HasCourses));
            OnPropertyChanged(nameof(IsCoursesEmpty));
            OnPropertyChanged(nameof(ActiveCoursesCount));
            OnPropertyChanged(nameof(LessonsCompletedCount));
            OpenTestCommand.RaiseCanExecuteChanged();
        }
        private void CompleteLesson(Lesson? lesson)
        {
            if (lesson == null || lesson.IsCompleted)
            {
                return;
            }

            lesson.IsCompleted = true;

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
        private async void RetryCurrentTest()
        {
            if (_activeTestCourseId == null)
            {
                return;
            }

            SelectedCourse = Courses.FirstOrDefault(
                course => course.Id == _activeTestCourseId.Value);

            await OpenTestAsync();
        }
        private async Task LoadUserProgressFromApiAsync()
        {
            if (AppSession.UserId == null ||
                string.IsNullOrWhiteSpace(AppSession.Token))
            {
                ApiStatusMessage =
                    "API-режим недоступен: пользователь не авторизован. Используются локальные данные.";
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

                    ApiStatusMessage = "У вас пока нет приобретённых курсов.";
                    HasApiStatusMessage = true;

                    NotifyProgressSummaryChanged();
                    return;
                }

                ApiStatusMessage = string.Empty;
                HasApiStatusMessage = false;

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

                NotifyCourseSummaryChanged();
                NotifyProgressSummaryChanged();
            }
            catch
            {
                ApiStatusMessage =
                    "Не удалось подключиться к API. Сейчас используются локальные данные.";
                HasApiStatusMessage = true;
            }
        }

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
        }
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
