using System.Windows;
using EducationalPlatformDesktop.ViewModels;

namespace EducationalPlatformDesktop.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();

            var viewModel = new LoginViewModel();
            viewModel.LoginSucceeded += OnLoginSucceeded;

            DataContext = viewModel;
        }

        private void OnLoginSucceeded()
        {
            var mainWindow = new MainWindow();
            Application.Current.MainWindow = mainWindow;
            mainWindow.Show();
            Close();
        }
    }
}
