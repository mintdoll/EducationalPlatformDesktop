namespace EducationalPlatformDesktop.Models
{
    public class Progress
    {
        public string CourseName { get; set; } = string.Empty;
        public int CompletedLessons { get; set; }
        public int TotalLessons { get; set; }
        public int TestScore { get; set; }
        public bool IsCompleted { get; set; }

        public double LessonsPercentage => TotalLessons == 0
            ? 0
            : (double)CompletedLessons / TotalLessons * 100;

        public string StatusText => IsCompleted ? "Курс завершён" : "Обучение продолжается";
    }
}
