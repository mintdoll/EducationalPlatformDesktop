namespace EducationalPlatformDesktop.Models
{
    public class Progress
    {
        public int CourseId { get; set; }

        public string CourseName { get; set; } = string.Empty;

        public int CompletedLessons { get; set; }

        public int TotalLessons { get; set; }

        public int TestScore { get; set; }

        public int OverallPercentage { get; set; }

        public bool IsCompleted { get; set; }

        public double LessonsPercentage => TotalLessons == 0
            ? 0
            : (double)CompletedLessons / TotalLessons * 100;

        public string StatusText
        {
            get
            {
                if (IsCompleted)
                {
                    return "Курс завершён";
                }

                if (CompletedLessons == 0 && TestScore == 0)
                {
                    return "Курс не начат";
                }

                return "Обучение продолжается";
            }
        }
    }
}