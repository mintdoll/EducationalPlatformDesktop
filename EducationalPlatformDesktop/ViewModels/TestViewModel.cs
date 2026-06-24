using System;
using EducationalPlatformDesktop.Commands;
using EducationalPlatformDesktop.Models;

namespace EducationalPlatformDesktop.ViewModels
{
    public class TestViewModel : ViewModelBase
    {
        private readonly Test _test;
        private int _currentIndex;
        private int _correctAnswers;
        private int _selectedOption = -1;
        private bool _isFinished;

        public TestViewModel(Test test)
        {
            _test = test ?? throw new ArgumentNullException(nameof(test));

            AnswerCommand = new RelayCommand(_ => Answer(), _ => SelectedOption >= 0 && !IsFinished);
            SelectOptionCommand = new RelayCommand(SelectOption, _ => !IsFinished);
            RestartCommand = new RelayCommand(_ => Restart());
            BackCommand = new RelayCommand(_ => BackRequested?.Invoke());
        }

        public string Title => _test.Title;
        public Question CurrentQuestion => _test.Questions[_currentIndex];
        public int QuestionNumber => _currentIndex + 1;
        public int TotalQuestions => _test.Questions.Count;
        public int CorrectAnswers => _correctAnswers;
        public int ScorePercent => TotalQuestions == 0
            ? 0
            : (int)Math.Round((double)CorrectAnswers / TotalQuestions * 100);
        public double QuestionProgress => TotalQuestions == 0
            ? 0
            : (double)QuestionNumber / TotalQuestions * 100;
        public bool IsPassed => ScorePercent >= 70;
        public string ResultTitle => IsPassed ? "Тест пройден" : "Попробуйте ещё раз";
        public string ResultDescription => IsPassed
            ? "Отличная работа — результат сохранён в разделе «Прогресс»."
            : "Для зачёта нужно набрать не менее 70%. Повторите материал и пройдите тест снова.";

        public int SelectedOption
        {
            get => _selectedOption;
            set
            {
                if (_selectedOption == value)
                    return;

                _selectedOption = value;
                OnPropertyChanged();
                AnswerCommand.RaiseCanExecuteChanged();
            }
        }

        public bool IsFinished
        {
            get => _isFinished;
            private set
            {
                if (_isFinished == value)
                    return;

                _isFinished = value;
                OnPropertyChanged();
                AnswerCommand.RaiseCanExecuteChanged();
            }
        }

        public RelayCommand AnswerCommand { get; }
        public RelayCommand SelectOptionCommand { get; }
        public RelayCommand RestartCommand { get; }
        public RelayCommand BackCommand { get; }

        public event Action<int>? TestCompleted;
        public event Action? BackRequested;

        private void SelectOption(object? parameter)
        {
            if (int.TryParse(parameter?.ToString(), out var index) &&
                index >= 0 && index < CurrentQuestion.Options.Count)
            {
                SelectedOption = index;
            }
        }

        private void Answer()
        {
            if (SelectedOption == CurrentQuestion.CorrectIndex)
                _correctAnswers++;

            if (_currentIndex < TotalQuestions - 1)
            {
                _currentIndex++;
                SelectedOption = -1;
                OnPropertyChanged(nameof(CurrentQuestion));
                OnPropertyChanged(nameof(QuestionNumber));
                OnPropertyChanged(nameof(QuestionProgress));
                return;
            }

            IsFinished = true;
            OnPropertyChanged(nameof(CorrectAnswers));
            OnPropertyChanged(nameof(ScorePercent));
            OnPropertyChanged(nameof(IsPassed));
            OnPropertyChanged(nameof(ResultTitle));
            OnPropertyChanged(nameof(ResultDescription));
            TestCompleted?.Invoke(ScorePercent);
        }

        private void Restart()
        {
            _currentIndex = 0;
            _correctAnswers = 0;
            SelectedOption = -1;
            IsFinished = false;

            OnPropertyChanged(nameof(CurrentQuestion));
            OnPropertyChanged(nameof(QuestionNumber));
            OnPropertyChanged(nameof(QuestionProgress));
            OnPropertyChanged(nameof(CorrectAnswers));
            OnPropertyChanged(nameof(ScorePercent));
        }
    }
}
