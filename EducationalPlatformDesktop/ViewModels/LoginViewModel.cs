using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Controls;
using EducationalPlatformDesktop.Api;
using EducationalPlatformDesktop.Api.Contracts;
using EducationalPlatformDesktop.Api.Services;
using EducationalPlatformDesktop.Commands;

namespace EducationalPlatformDesktop.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly OnlineSchoolApiClient _apiClient;

        private string _login = string.Empty;
        private string _password = string.Empty;
        private string _errorMessage = string.Empty;
        private string _successMessage = string.Empty;
        private bool _isBusy;

        // Локальный fallback оставляем только на случай, если API временно недоступен.
        private static readonly Dictionary<string, string> DemoAccounts = new(StringComparer.OrdinalIgnoreCase)
        {
            { "arina@mail", "1234" },
            { "student@mail", "1234" },
            { "demo@mail", "demo123" }
        };
        public LoginViewModel()
        {
            _apiClient = new OnlineSchoolApiClient(new System.Net.Http.HttpClient());
            LoginCommand = new RelayCommand(async parameter => await ExecuteLoginAsync(parameter));
        }
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
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (_isBusy != value)
                {
                    _isBusy = value;
                    OnPropertyChanged();
                }
            }
        }
        public RelayCommand LoginCommand { get; }
        public event Action? LoginSucceeded;
        private async Task ExecuteLoginAsync(object? parameter)
        {
            if (IsBusy)
            {
                return;
            }

            var passwordBox = parameter as PasswordBox;
            Password = passwordBox?.Password ?? string.Empty;

            var email = Login.Trim();

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Введите email и пароль.";
                SuccessMessage = string.Empty;
                return;
            }
            IsBusy = true;
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;
            try
            {
                var response = await _apiClient.LoginAsync(new LoginRequestDto
                {
                    Email = email,
                    Password = Password
                });

                if (response != null)
                {
                    AppSession.UserId = response.UserId;
                    AppSession.Role = response.Role;
                    AppSession.Email = email;
                    AppSession.Token = response.Token;

                    _apiClient.SetBearerToken(response.Token);

                    SuccessMessage = "Вход выполнен успешно.";
                    LoginSucceeded?.Invoke();
                    return;
                }
                ErrorMessage = "Неверный email или пароль.";
                return;
            }
            catch
            {
                // Если backend временно недоступен, можно войти в демо-режиме.
                if (!DemoAccounts.TryGetValue(email, out var expectedPassword) || expectedPassword != Password)
                {
                    ErrorMessage = "Не удалось подключиться к API, и локальная проверка не прошла.";
                    return;
                }
                AppSession.UserId = 1;
                AppSession.Role = "student";
                AppSession.Email = email;
                AppSession.Token = null;

                SuccessMessage = "Вход выполнен успешно в демо-режиме.";
                LoginSucceeded?.Invoke();
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}