using System;
using EducationalPlatformDesktop.Commands;
using EducationalPlatformDesktop.Models;

namespace EducationalPlatformDesktop.ViewModels
{
    public class TestResultViewModel : ViewModelBase
    {
        public TestResultViewModel(
            TestResult result,
            bool isCertificateAvailable)
        {
            Result = result ??
                throw new ArgumentNullException(nameof(result));

            IsCertificateAvailable = isCertificateAvailable;

            RetryCommand =
                new RelayCommand(_ => RetryRequested?.Invoke());

            BackToCoursesCommand =
                new RelayCommand(_ => BackToCoursesRequested?.Invoke());
        }

        public TestResult Result { get; }

        public bool IsCertificateAvailable { get; }

        public string ResultTitle => Result.IsPassed
            ? "Тест успешно пройден"
            : "Тест пока не пройден";

        public string ScoreText =>
            $"{Result.ScorePercent}%";

        public string AnswersText =>
            $"Правильных ответов: " +
            $"{Result.CorrectAnswers} из {Result.TotalQuestions}";

        public string PassingScoreText =>
            $"Проходной балл: {Result.PassingScore}%";

        public string CompletedAtText =>
            $"Дата прохождения: " +
            $"{Result.CompletedAt:dd.MM.yyyy HH:mm}";

        public string ResultDescription => Result.IsPassed
            ? "Результат сохранён в демонстрационном прогрессе."
            : "Повторите материал и попробуйте пройти тест ещё раз.";

        public string CertificateMessage => IsCertificateAvailable
            ? "Все условия выполнены. Доступна выдача сертификата."
            : "Для сертификата необходимо завершить не менее 90% уроков и сдать итоговый тест.";

        public RelayCommand RetryCommand { get; }

        public RelayCommand BackToCoursesCommand { get; }

        public event Action? RetryRequested;

        public event Action? BackToCoursesRequested;
    }
}