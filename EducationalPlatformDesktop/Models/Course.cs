using System.Collections.ObjectModel;

namespace EducationalPlatformDesktop.Models
{
    public class Course
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsPurchased { get; set; }
        public ObservableCollection<Module> Modules { get; set; } = new();
    }
}
