using System.Collections.ObjectModel;

namespace EducationalPlatformDesktop.Models
{
    public class Module
    {
        public string Title { get; set; } = string.Empty;
        public ObservableCollection<Lesson> Lessons { get; set; } = new();
    }
}