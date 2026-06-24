using System.Collections.ObjectModel;

namespace EducationalPlatformDesktop.Models
{
    public class Module
    {
        public int Id { get; set; }
        public int CourseId { get; set; }

        public string Title { get; set; } = string.Empty;

        public ObservableCollection<Lesson> Lessons { get; set; } = new();
    }
}