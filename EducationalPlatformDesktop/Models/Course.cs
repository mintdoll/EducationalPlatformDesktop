using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EducationalPlatformDesktop.Models
{
    public class Course : INotifyPropertyChanged
    {
        private int _bestTestScore;
        private int? _progressOverride;
        private bool _isPurchased;

        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Track { get; set; } = string.Empty;

        public string Teacher { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public bool IsPurchased
        {
            get => _isPurchased;
            set
            {
                if (_isPurchased == value)
                {
                    return;
                }

                _isPurchased = value;
                OnPropertyChanged(nameof(IsPurchased));
            }
        }

        public int PassingScore { get; set; } = 70;

        public ObservableCollection<Lesson> Lessons { get; set; } = new();

        public int BestTestScore
        {
            get => _bestTestScore;
            set
            {
                if (_bestTestScore == value)
                {
                    return;
                }

                _bestTestScore = value;
                RefreshProgress();
            }
        }

        public int TotalLessons => Lessons.Count;

        public int CompletedLessons =>
            Lessons.Count(lesson => lesson.IsCompleted);

       

        public int LessonProgressPercent
        {
            get
            {
                if (TotalLessons == 0)
                {
                    return 0;
                }

                return (int)Math.Round(
                    (double)CompletedLessons / TotalLessons * 100);
            }
        }

        public int Progress => _progressOverride ?? LessonProgressPercent;

        public bool IsTestPassed => BestTestScore >= PassingScore;


        public bool IsCourseCompleted =>
            Progress >= 90 &&
            IsTestPassed;

        public bool CanTakeFinalTest =>
            Progress >= 90;

        public bool CanReceiveCertificate =>
            IsCourseCompleted;

        public string Status
        {
            get
            {
                if (Progress == 0 && BestTestScore == 0)
                {
                    return "Не начат";
                }

                if (IsCourseCompleted)
                {
                    return "Завершён";
                }

                return "В процессе";
            }
        }

        public void ApplyTestResult(int scorePercent)
        {
            if (scorePercent > BestTestScore)
            {
                BestTestScore = scorePercent;
            }
            else
            {
                RefreshProgress();
            }
        }

        public void SetProgressOverride(int? progressPercent)
        {
            if (_progressOverride == progressPercent)
            {
                return;
            }

            _progressOverride = progressPercent;
            RefreshProgress();
        }

        public void RefreshProgress()
        {
            OnPropertyChanged(nameof(BestTestScore));
            OnPropertyChanged(nameof(TotalLessons));
            OnPropertyChanged(nameof(CompletedLessons));
            OnPropertyChanged(nameof(LessonProgressPercent));
            OnPropertyChanged(nameof(Progress));
            OnPropertyChanged(nameof(IsTestPassed));
            OnPropertyChanged(nameof(IsCourseCompleted));
            OnPropertyChanged(nameof(CanTakeFinalTest));
            OnPropertyChanged(nameof(CanReceiveCertificate));
            OnPropertyChanged(nameof(Status));
  
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(
            [CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(propertyName));
        }
    }
}
