using System;
using System.Collections.ObjectModel;
using System.Linq;
using EducationalPlatformDesktop.Commands;
using EducationalPlatformDesktop.Mocks;
using EducationalPlatformDesktop.Models;
using EducationalPlatformDesktop.Views;
using EducationalPlatformDesktop.Views.Sections;

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
        private TestViewModel? _testViewModel;
        private TestResultViewModel? _testResultViewModel;
        private readonly CertificateViewModel _certificateViewModel;
        private int? _activeTestCourseId;
        

        private object _currentView = null!;
        private string _pageTitle = "Главная";
        private string _pageDescription = "Стартовый экран демонстрационной оболочки.";
        private Course? _selectedCourse;
        private Module? _selectedModule;
        private Lesson? _selectedLesson;
        private string _lessonContent = "Выберите урок, чтобы увидеть текст лекции.";


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
        public int LessonsCompletedCount => Courses.Sum(course => course.CompletedLessons);
        public int CertificatesCount => Certificates.Count;
        public int CompletedCoursesCount => ProgressItems.Count(item => item.IsCompleted);
        public int AverageProgress => ProgressItems.Count == 0
       ? 0
       : (int)Math.Round(
           ProgressItems.Average(item => item.OverallPercentage));

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


        public event Action? RequestClose;


        public MainViewModel()
        {

            Profile = DemoEducationData.GetProfile();
            Courses = MockCourseData.GetCourses();
            ProgressItems = new ObservableCollection<Progress>(
    Courses.Select(CreateProgressItem));
            Certificates = MockTestData.GetCertificates();
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
                new RelayCommand(_ => OpenTest());
            BackToCoursesCommand = new RelayCommand(_ => ShowCourses());
            ExitCommand = new RelayCommand(_ => RequestClose?.Invoke());

            // Установка стартового представления
            CurrentView = _homeView;

            // Выбор первого курса если есть
            if (Courses.Count > 0)
            {
                SelectedCourse = Courses.First();
            }
        }


        private void ShowHome()
        {
            CurrentView = _homeView;
            PageTitle = "Главная";
            PageDescription = "Стартовый экран демонстрационной оболочки.";
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

        private void OpenTest()
        {
            if (SelectedCourse == null)
            {
                return;
            }

            _activeTestCourseId = SelectedCourse.Id;

            var test = MockTestData.GetTestForCourse(SelectedCourse.Id);

            _testViewModel = new TestViewModel(test);
            _testViewModel.TestCompleted += OnTestCompleted;

            _testViewModel.BackRequested += ShowCourses;


            _testView.DataContext = _testViewModel;

            CurrentView = _testView;
            PageTitle = _testViewModel.Title;
            PageDescription =
                "Выберите один ответ. Можно использовать клавиши 1–4 и Enter.";
        }
        private void OnTestCompleted(TestResult result)
        {
            var course = Courses.FirstOrDefault(
                item => item.Id == result.CourseId);

            if (course == null)
            {
                return;
            }

            result.CourseName = course.Title;

            course.ApplyTestResult(result.ScorePercent);

            UpdateProgressForCourse(course);
            NotifyProgressSummaryChanged();
            GenerateCertificateIfEligible(course, result);

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
                return;

            foreach (var module in SelectedCourse.Modules)
                Modules.Add(module);

            if (Modules.Count > 0)
                SelectedModule = Modules.First();
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

            course.RefreshProgress();

            UpdateProgressForCourse(course);
            NotifyProgressSummaryChanged();
        }

        private Progress CreateProgressItem(Course course)
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
        private void UpdateProgressForCourse(Course course)
        {
            var oldItem = ProgressItems.FirstOrDefault(
                item => item.CourseId == course.Id);

            var newItem = CreateProgressItem(course);

            if (oldItem == null)
            {
                ProgressItems.Add(newItem);
                return;
            }

            var index = ProgressItems.IndexOf(oldItem);
            ProgressItems[index] = newItem;
        }
        private void NotifyProgressSummaryChanged()
        {
            OnPropertyChanged(nameof(ActiveCoursesCount));
            OnPropertyChanged(nameof(LessonsCompletedCount));
            OnPropertyChanged(nameof(CompletedCoursesCount));
            OnPropertyChanged(nameof(AverageProgress));
        }
        private void RetryCurrentTest()
        {
            if (_activeTestCourseId == null)
            {
                return;
            }

            SelectedCourse = Courses.FirstOrDefault(
                course => course.Id == _activeTestCourseId.Value);

            OpenTest();
        }
        private void GenerateCertificateIfEligible(
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

            var certificate = new Certificate
            {
                Id = Guid.NewGuid().ToString("N"),
                Number =
                    $"CERT-{DateTime.Now:yyyyMMdd}-" +
                    $"{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}",
                CourseId = course.Id,
                CourseName = course.Title,
                StudentName = Profile.FullName,
                IssuedAt = DateTime.Now,
                Score = result.ScorePercent,
                TestResultId = result.AttemptId
            };

            Certificates.Add(certificate);

            OnPropertyChanged(nameof(CertificatesCount));
        }
    }
    
}
