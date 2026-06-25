using System;
using System.Collections.Generic;
using System.Text;

namespace EducationalPlatformDesktop.Models
{
    public class DemoAppState
    {
        public List<CourseProgressState> CourseProgress { get; set; } = new();

        public List<Certificate> Certificates { get; set; } = new();
    }

    public class CourseProgressState
    {
        public int CourseId { get; set; }

        public List<int> CompletedLessonIds { get; set; } = new();

        public int BestTestScore { get; set; }
    }
}