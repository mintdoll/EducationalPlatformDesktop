using System;
using System.Collections.ObjectModel;
using EducationalPlatformDesktop.Commands;
using EducationalPlatformDesktop.Mocks;
using EducationalPlatformDesktop.Models;

namespace EducationalPlatformDesktop.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private string _pageTitle = "Главная";
        private string _pageDescription = "Стартовый экран демонстрационной версии приложения.";

        private UserProfile _profile = DemoEducationData.GetProfile();
        private ObservableCollection<Course> _courses = DemoEducationData.GetCourses();

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

        public UserProfile Profile
        {
            get => _profile;
            set
            {
                if (_profile != value)
                {
                    _profile = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<Course> Courses
        {
            get => _courses;
            set
            {
                if (_courses != value)
                {
                    _courses = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<string> DemoItems { get; } = new();

        public RelayCommand ShowHomeCommand { get; }
        public RelayCommand ShowCoursesCommand { get; }
        public RelayCommand ShowProfileCommand { get; }

        public event Action? RequestClose;

        public MainViewModel()
        {
            ShowHomeCommand = new RelayCommand(_ => SetHome());
            ShowCoursesCommand = new RelayCommand(_ => SetCourses());
            ShowProfileCommand = new RelayCommand(_ => SetProfile());

            SetHome();
        }

        private void SetHome()
        {
            PageTitle = "Главная";
            PageDescription = "Демонстрационная оболочка приложения для просмотра профиля и курсов.";

            DemoItems.Clear();
            DemoItems.Add("Добро пожаловать в образовательную платформу.");
            DemoItems.Add("Здесь будет показан доступ к курсам, модулям и урокам.");
            DemoItems.Add("Пока данные берутся из mock-источника.");
        }

        private void SetCourses()
        {
            PageTitle = "Курсы";
            PageDescription = "Список доступных и купленных курсов.";

            DemoItems.Clear();

            foreach (var course in Courses)
            {
                var status = course.IsPurchased ? "куплен" : "доступен";
                DemoItems.Add($"{course.Title} - {status}");
            }
        }

        private void SetProfile()
        {
            PageTitle = "Профиль";
            PageDescription = "Информация о пользователе.";

            DemoItems.Clear();
            DemoItems.Add($"ФИО: {Profile.FullName}");
            DemoItems.Add($"Email/MAX: {Profile.EmailOrMax}");
            DemoItems.Add($"Группа: {Profile.Group}");
            DemoItems.Add($"Роль: {Profile.Role}");
        }
    }
}