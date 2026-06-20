using System.Collections.ObjectModel;
using EducationalPlatformDesktop.Commands;
using EducationalPlatformDesktop.Mocks;
using EducationalPlatformDesktop.Models;

namespace EducationalPlatformDesktop.ViewModels
{
    public class TestViewModel : ViewModelBase
    {
        private readonly Test _test;
        private int _currentIndex = 0;
        private int _score = 0;
        private bool _isFinished = false;
        private int _selectedOption = -1;

        public Question CurrentQuestion => _test.Questions[_currentIndex];
        public int QuestionNumber => _currentIndex + 1;
        public int TotalQuestions => _test.Questions.Count;

        public bool IsFinished
        {
            get => _isFinished;
            set { _isFinished = value; OnPropertyChanged(); }
        }
        public int Score
        {
            get => _score;
            set { _score = value; OnPropertyChanged(); }
        }
        public int SelectedOption
        {
            get => _selectedOption;
            set { _selectedOption = value; OnPropertyChanged(); }
        }

        public RelayCommand AnswerCommand { get; }
        public RelayCommand SelectOptionCommand { get; }

        public TestViewModel()
        {
            _test = MockTestData.GetTest();
            AnswerCommand = new RelayCommand(Answer);
            SelectOptionCommand = new RelayCommand(p =>
            {
                if (int.TryParse(p?.ToString(), out int idx))
                    SelectedOption = idx;
            });
        }

        private void Answer(object? parameter)
        {
            if (SelectedOption < 0) return;

            if (SelectedOption == CurrentQuestion.CorrectIndex)
                Score++;

            if (_currentIndex < _test.Questions.Count - 1)
            {
                _currentIndex++;
                SelectedOption = -1;
                OnPropertyChanged(nameof(CurrentQuestion));
                OnPropertyChanged(nameof(QuestionNumber));
            }
            else
            {
                IsFinished = true;
            }
        }
    }
}