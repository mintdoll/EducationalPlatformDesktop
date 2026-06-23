using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using EducationalPlatformDesktop.Commands;

namespace EducationalPlatformDesktop.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private static readonly Dictionary<string, string> DemoAccounts = new(StringComparer.OrdinalIgnoreCase)
        {
            { "arina@mail", "1234" },
            { "student@mail", "1234" },
            { "demo@mail", "demo123" }
        };

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

        public event Action? LoginSucceeded;

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(ExecuteLogin);
        }

        private void ExecuteLogin(object? parameter)
        {
            var passwordBox = parameter as PasswordBox;
            Password = passwordBox?.Password ?? string.Empty;

            var login = Login.Trim();

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Введите логин и пароль.";
                SuccessMessage = string.Empty;
                return;
            }

            if (!DemoAccounts.TryGetValue(login, out var expectedPassword) || expectedPassword != Password)
            {
                ErrorMessage = "Неверный логин или пароль.";
                SuccessMessage = string.Empty;
                return;
            }

            ErrorMessage = string.Empty;
            SuccessMessage = $"Вход выполнен успешно. Добро пожаловать, {login}.";
            LoginSucceeded?.Invoke();
        }
    }
}