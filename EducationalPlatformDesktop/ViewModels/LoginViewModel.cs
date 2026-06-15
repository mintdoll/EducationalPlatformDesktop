using System.Windows.Controls;
using EducationalPlatformDesktop.Commands;

namespace EducationalPlatformDesktop.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private string _login = string.Empty;
        private string _password = string.Empty;
        private string _errorMessage = string.Empty;
        private string _successMessage = string.Empty;

        public string Login
        {
            get => _login;
            set
            {
                if (_login != value)
                {
                    _login = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                if (_password != value)
                {
                    _password = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                if (_errorMessage != value)
                {
                    _errorMessage = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsErrorVisible));
                }
            }
        }

        public string SuccessMessage
        {
            get => _successMessage;
            set
            {
                if (_successMessage != value)
                {
                    _successMessage = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsSuccessVisible));
                }
            }
        }

        public bool IsErrorVisible => !string.IsNullOrWhiteSpace(ErrorMessage);
        public bool IsSuccessVisible => !string.IsNullOrWhiteSpace(SuccessMessage);

        public RelayCommand LoginCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(ExecuteLogin);
        }

        private void ExecuteLogin(object? parameter)
        {
            var passwordBox = parameter as PasswordBox;
            Password = passwordBox?.Password ?? string.Empty;

            if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Введите логин и пароль.";
                SuccessMessage = string.Empty;
                return;
            }

            ErrorMessage = string.Empty;
            SuccessMessage = "Вход выполнен успешно.";
        }

    }
}