using System.Collections.ObjectModel;

namespace EducationalPlatformDesktop.Models
{
    public class Question
    {
        public string Text { get; set; } = string.Empty;
        public ObservableCollection<string> Options { get; set; } = new();
        public int CorrectIndex { get; set; }
    }
}