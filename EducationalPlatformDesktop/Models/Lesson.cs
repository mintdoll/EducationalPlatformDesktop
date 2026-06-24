using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EducationalPlatformDesktop.Models
{
    public class Lesson : INotifyPropertyChanged
    {
        private bool _isCompleted;

        public int Id { get; set; }

        public int CourseId { get; set; }

        public int ModuleId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public string Url { get; set; } = string.Empty;

        public int DurationMinutes { get; set; }

        public bool IsCompleted
        {
            get => _isCompleted;
            set
            {
                if (_isCompleted == value)
                {
                    return;
                }

                _isCompleted = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CompletionButtonText));
                OnPropertyChanged(nameof(CompletionStatus));
                OnPropertyChanged(nameof(CanBeCompleted));
            }
        }

        public bool CanBeCompleted => !IsCompleted;

        public string CompletionButtonText => IsCompleted
            ? "Урок пройден"
            : "Отметить пройденным";

        public string CompletionStatus => IsCompleted
            ? "✓ Пройден"
            : "Не пройден";

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