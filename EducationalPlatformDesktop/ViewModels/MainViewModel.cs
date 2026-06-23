using System;
using System.Collections.ObjectModel;
using System.Linq;
using EducationalPlatformDesktop.Commands;
using EducationalPlatformDesktop.Mocks;
using EducationalPlatformDesktop.Models;
using EducationalPlatformDesktop.Views.Sections;

namespace EducationalPlatformDesktop.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly HomeView _homeView = new();
        private readonly CoursesView _coursesView = new();
        private readonly ProfileView _profileView = new();
        private readonly LessonView _lessonView = new();

        private object _currentView = null!;
        private string _pageTitle = "Главная";
        private string _pageDescription = "Стартовый экран демонстрационной оболочки.";

        private Course? _selectedCourse;
        private Module? _selectedModule;
        private Lesson? _selectedLesson;
        private string _lessonContent = "Выберите урок, чтобы увидеть текст лекции.";

        public MainViewModel()
        {
            Profile = DemoEducationData.GetProfile();
            Courses = DemoEducationData.GetCourses();
            ProgressItems = MockTestData.GetProgress();
            Certificates = MockTestData.GetCertificates();

            ShowHomeCommand = new RelayCommand(_ => ShowHome());
            ShowCoursesCommand = new RelayCommand(_ => ShowCourses());
            ShowProfileCommand = new RelayCommand(_ => ShowProfile());
            OpenLessonCommand = new RelayCommand(param => OpenLesson(param as Lesson));
            BackToCoursesCommand = new RelayCommand(_ => ShowCourses());
            ExitCommand = new RelayCommand(_ => RequestClose?.Invoke());

            CurrentView = _homeView;

            if (Courses.Count > 0)
            {
                SelectedCourse = Courses.First();
            }
        }

        public event Action? RequestClose;

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
                    OnPropertyChanged(nameof(IsLessonVisible));
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
        public bool IsLessonVisible => CurrentView == _lessonView;

        public UserProfile Profile { get; }

        public ObservableCollection<Course> Courses { get; }
        public ObservableCollection<Progress> ProgressItems { get; }
        public ObservableCollection<Certificate> Certificates { get; }
        public ObservableCollection<Module> Modules { get; } = new();
        public ObservableCollection<Lesson> Lessons { get; } = new();

        public int ActiveCoursesCount => Courses.Count(course => course.Progress > 0 && course.Progress < 100);
        public int LessonsCompletedCount => Courses.Sum(course => course.CompletedLessons);
        public int CertificatesCount => Certificates.Count;

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

        public RelayCommand ShowHomeCommand { get; }
        public RelayCommand ShowCoursesCommand { get; }
        public RelayCommand ShowProfileCommand { get; }
        public RelayCommand OpenLessonCommand { get; }
        public RelayCommand BackToCoursesCommand { get; }
        public RelayCommand ExitCommand { get; }

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

        private void OpenLesson(Lesson? lesson)
        {
            if (lesson == null)
            {
                return;
            }

            SelectedLesson = lesson;
            LessonContent = lesson.Content;

            CurrentView = _lessonView;
            PageTitle = lesson.Title;

            PageDescription = SelectedCourse != null
                ? $"{SelectedCourse.Title} · {SelectedModule?.Title}"
                : "Текст лекции";
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

            foreach (var module in SelectedCourse.Modules)
            {
                Modules.Add(module);
            }

            if (Modules.Count > 0)
            {
                SelectedModule = Modules.First();
            }
        }

        private void UpdateLessonsForSelectedModule()
        {
            Lessons.Clear();

            _selectedLesson = null;
            OnPropertyChanged(nameof(SelectedLesson));

            LessonContent = "Выберите урок, чтобы увидеть текст лекции.";

            if (SelectedModule == null)
            {
                return;
            }

            foreach (var lesson in SelectedModule.Lessons)
            {
                Lessons.Add(lesson);
            }
        }

        private void UpdateLessonContent()
        {
            LessonContent = SelectedLesson?.Content ?? "Выберите урок, чтобы увидеть текст лекции.";
        }
    }
}