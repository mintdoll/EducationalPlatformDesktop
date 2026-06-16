using System.Collections.ObjectModel;

namespace EducationalPlatformDesktop.Models
{
    public class Test
    {
        public string Title { get; set; } = string.Empty;
        public ObservableCollection<Question> Questions { get; set; } = new();
    }
}