using System.Collections.ObjectModel;
using System.Linq;

namespace EducationalPlatformDesktop.Models
{
    public class Course
    {
        public string Title { get; set; } = string.Empty;
        public string Track { get; set; } = string.Empty;
        public string Teacher { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public int Progress { get; set; }
        public bool IsPurchased { get; set; }
        public int CompletedLessons { get; set; }

        public ObservableCollection<Module> Modules { get; set; } = new();

        public int TotalLessons => Modules.Sum(module => module.Lessons.Count);

        public string Status
        {
            get
            {
                if (Progress <= 0)
                {
                    return "Не начат";
                }

                if (Progress >= 100)
                {
                    return "Завершён";
                }

                return "В процессе";
            }
        }
    }
}