using System.Collections.ObjectModel;

namespace EducationalPlatformDesktop.Models
{
    public class Test
    {
        public int Id { get; set; }

        public int CourseId { get; set; }

        public string Title { get; set; } = string.Empty;

        public int PassingScore { get; set; } = 70;

        public ObservableCollection<Question> Questions { get; set; } = new();
    }
}