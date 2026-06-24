using System;

namespace EducationalPlatformDesktop.Models
{
    public class TestResult
    {
        public string AttemptId { get; set; } = string.Empty;

        public int CourseId { get; set; }

        public int TestId { get; set; }

        public string CourseName { get; set; } = string.Empty;

        public string TestTitle { get; set; } = string.Empty;

        public int CorrectAnswers { get; set; }

        public int TotalQuestions { get; set; }

        public int ScorePercent { get; set; }

        public int PassingScore { get; set; }

        public bool IsPassed { get; set; }

        public DateTime CompletedAt { get; set; }
    }
}