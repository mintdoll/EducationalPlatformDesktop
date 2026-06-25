using System;
using System.Net.Http;
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

        public LoginViewModel()
        {
            _apiClient = new OnlineSchoolApiClient(new HttpClient());
            LoginCommand = new RelayCommand(
                async parameter => await ExecuteLoginAsync(parameter),
                _ => !IsBusy);
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
                    LoginCommand.RaiseCanExecuteChanged();
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
                AppSession.Clear();
                _apiClient.SetBearerToken(null);

                var response = await _apiClient.LoginAsync(new LoginRequestDto
                {
                    Email = email,
                    Password = Password
                });

                if (response == null || string.IsNullOrWhiteSpace(response.Token))
                {
                    ErrorMessage = "Неверный email или пароль.";
                    return;
                }

                AppSession.UserId = response.UserId;
                AppSession.Role = response.Role;
                AppSession.Email = response.Email ?? email;
                AppSession.Token = response.Token;
                AppSession.Name = response.Name;
                AppSession.LastName = response.LastName;

                _apiClient.SetBearerToken(response.Token);

                SuccessMessage = "Вход выполнен успешно.";
                LoginSucceeded?.Invoke();
            }
            catch (HttpRequestException)
            {
                ErrorMessage = "Не удалось подключиться к API. Проверь, что backend запущен.";
            }
            catch (TaskCanceledException)
            {
                ErrorMessage = "Превышено время ожидания ответа от API.";
            }
            catch (Exception)
            {
                ErrorMessage = "Не удалось выполнить вход.";
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}