using System;
using System.Collections.Generic;
using System.Text;
using System;

namespace EducationalPlatformDesktop.Api.Contracts
{
    public sealed class UserCourseDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int CourseId { get; set; }

        public int Progress { get; set; }

        public bool Completed { get; set; }

        public DateTime DatePurchase { get; set; }

        public CourseDto? Course { get; set; }
    }
}