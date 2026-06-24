using System;

namespace EducationalPlatformDesktop.Models
{
    public class Certificate
    {
        public string Id { get; set; } = string.Empty;

        public string Number { get; set; } = string.Empty;

        public int CourseId { get; set; }

        public string CourseName { get; set; } = string.Empty;

        public string StudentName { get; set; } = string.Empty;

        public DateTime IssuedAt { get; set; }

        public int Score { get; set; }

        public string TestResultId { get; set; } = string.Empty;

        public string IssuedDate =>
            IssuedAt.ToString("dd.MM.yyyy");

        public string ScoreText =>
            $"Итоговый результат: {Score}%";
    }
}