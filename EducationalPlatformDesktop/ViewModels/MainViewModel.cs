using System;
using System.Collections.ObjectModel;
using System.Linq;
using EducationalPlatformDesktop.Commands;
using EducationalPlatformDesktop.Mocks;
using EducationalPlatformDesktop.Models;

namespace EducationalPlatformDesktop.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private string _pageTitle = "Мои курсы";
        private string _pageDescription = "Выберите курс, затем модуль и урок для просмотра лекции.";

        private Course? _selectedCourse;
        private Module? _selectedModule;
        private Lesson? _selectedLesson;
        private string _lessonContent = "Выберите урок, чтобы увидеть текст лекции.";

        public MainViewModel()
        {
            Courses = DemoEducationData.GetCourses();

            ActiveCoursesCount = Courses.Count(c => c.IsPurchased);
            LessonsCompletedCount = Courses.Sum(c => c.Modules.Sum(m => m.Lessons.Count)) / 2;
            CertificatesCount = 1;

            ShowCoursesCommand = new RelayCommand(_ => ShowCourses());
            ShowCatalogCommand = new RelayCommand(_ => ShowCatalog());
            ShowTestingCommand = new RelayCommand(_ => ShowTesting());
            ShowProfileCommand = new RelayCommand(_ => ShowProfile());
            ExitCommand = new RelayCommand(_ => RequestClose?.Invoke());

            SelectCourseCommand = new RelayCommand(course => SelectCourse(course as Course));
            SelectModuleCommand = new RelayCommand(module => SelectModule(module as Module));
            SelectLessonCommand = new RelayCommand(lesson => SelectLesson(lesson as Lesson));

            SelectedCourse = Courses.FirstOrDefault();
        }

        public event Action? RequestClose;

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

        public ObservableCollection<Course> Courses { get; }

        public ObservableCollection<Module> Modules
        {
            get;
            private set;
        } = new();

        public ObservableCollection<Lesson> Lessons
        {
            get;
            private set;
        } = new();

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

        public int ActiveCoursesCount { get; }
        public int LessonsCompletedCount { get; }
        public int CertificatesCount { get; }

        public RelayCommand ShowCoursesCommand { get; }
        public RelayCommand ShowCatalogCommand { get; }
        public RelayCommand ShowTestingCommand { get; }
        public RelayCommand ShowProfileCommand { get; }
        public RelayCommand ExitCommand { get; }

        public RelayCommand SelectCourseCommand { get; }
        public RelayCommand SelectModuleCommand { get; }
        public RelayCommand SelectLessonCommand { get; }

        private void ShowCourses()
        {
            PageTitle = "Мои курсы";
            PageDescription = "Выберите курс, затем модуль и урок для просмотра лекции.";
        }

        private void ShowCatalog()
        {
            PageTitle = "Каталог";
            PageDescription = "Список доступных курсов.";
        }

        private void ShowTesting()
        {
            PageTitle = "Тестирование";
            PageDescription = "Этот раздел будет делать студент 2.";
        }

        private void ShowProfile()
        {
            PageTitle = "Профиль";
            PageDescription = "Информация об авторизованном пользователе.";
        }

        private void SelectCourse(Course? course)
        {
            if (course == null)
            {
                return;
            }

            SelectedCourse = course;
            PageTitle = course.Title;
            PageDescription = $"{course.Track} · {course.Teacher}";
        }

        private void SelectModule(Module? module)
        {
            if (module == null)
            {
                return;
            }

            SelectedModule = module;
        }

        private void SelectLesson(Lesson? lesson)
        {
            if (lesson == null)
            {
                return;
            }

            SelectedLesson = lesson;
        }

        private void UpdateModulesForSelectedCourse()
        {
            Modules.Clear();
            Lessons.Clear();

            SelectedModule = null;
            SelectedLesson = null;
            LessonContent = "Выберите урок, чтобы увидеть текст лекции.";

            if (SelectedCourse == null)
            {
                return;
            }

            foreach (var module in SelectedCourse.Modules)
            {
                Modules.Add(module);
            }

            SelectedModule = Modules.FirstOrDefault();
        }

        private void UpdateLessonsForSelectedModule()
        {
            Lessons.Clear();
            SelectedLesson = null;
            LessonContent = "Выберите урок, чтобы увидеть текст лекции.";

            if (SelectedModule == null)
            {
                return;
            }

            foreach (var lesson in SelectedModule.Lessons)
            {
                Lessons.Add(lesson);
            }

            SelectedLesson = Lessons.FirstOrDefault();
        }

        private void UpdateLessonContent()
        {
            if (SelectedLesson == null)
            {
                LessonContent = "Выберите урок, чтобы увидеть текст лекции.";
                return;
            }

            LessonContent = SelectedLesson.Content;
        }
    }
}