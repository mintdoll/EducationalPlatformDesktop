using System.Collections.ObjectModel;

namespace EducationalPlatformDesktop.Models
{
    public class Question
    {
        public int Id { get; set; }

        public int TestId { get; set; }

        public string Text { get; set; } = string.Empty;

        public ObservableCollection<AnswerOption> Options { get; set; } = new();
    }
}