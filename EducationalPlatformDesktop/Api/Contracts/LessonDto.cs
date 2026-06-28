using System;
using System.Collections.Generic;
using System.Text;

namespace EducationalPlatformDesktop.Api.Contracts
{
    public sealed class LessonDto
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }
}
