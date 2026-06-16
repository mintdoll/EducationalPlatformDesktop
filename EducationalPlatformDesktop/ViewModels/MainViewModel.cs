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
        private string _currentSection = "home";

        private readonly ObservableCollection<Course> _allCourses = DemoEducationData.GetCourses();

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

        public string CurrentSection
        {
            get => _currentSection;
            set
            {
                if (_currentSection != value)
                {
                    _currentSection = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsHomeVisible));
                    OnPropertyChanged(nameof(IsCoursesVisible));
                    OnPropertyChanged(nameof(IsProfileVisible));
                }
            }
        }

        public bool IsHomeVisible => CurrentSection == "home";
        public bool IsCoursesVisible => CurrentSection == "courses";
        public bool IsProfileVisible => CurrentSection == "profile";

        public UserProfile Profile { get; } = DemoEducationData.GetProfile();

        public ObservableCollection<Course> AllCourses => _allCourses;

        public ObservableCollection<Course> PurchasedCourses { get; } = new();
        public ObservableCollection<Course> AvailableCourses { get; } = new();

        public RelayCommand ShowHomeCommand { get; }
        public RelayCommand ShowCoursesCommand { get; }
        public RelayCommand ShowProfileCommand { get; }
        public RelayCommand ExitCommand { get; }

        public event Action? RequestClose;

        public MainViewModel()
        {
            ShowHomeCommand = new RelayCommand(_ => ShowHome());
            ShowCoursesCommand = new RelayCommand(_ => ShowCourses());
            ShowProfileCommand = new RelayCommand(_ => ShowProfile());
            ExitCommand = new RelayCommand(_ => RequestClose?.Invoke());

            BuildCourseLists();
            ShowHome();
        }

        private void BuildCourseLists()
        {
            PurchasedCourses.Clear();
            AvailableCourses.Clear();

            foreach (var course in AllCourses)
            {
                if (course.IsPurchased)
                {
                    PurchasedCourses.Add(course);
                }
                else
                {
                    AvailableCourses.Add(course);
                }
            }
        }

        private void ShowHome()
        {
            CurrentSection = "home";
            PageTitle = "Главная";
            PageDescription = "Стартовый экран демонстрационного приложения по ТЗ.";
        }

        private void ShowCourses()
        {
            CurrentSection = "courses";
            PageTitle = "Курсы";
            PageDescription = "Список доступных и купленных курсов.";
        }

        private void ShowProfile()
        {
            CurrentSection = "profile";
            PageTitle = "Профиль";
            PageDescription = "Информация о пользователе.";
        }
    }
}