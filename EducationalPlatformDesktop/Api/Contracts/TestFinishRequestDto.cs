using System;
using System.Collections.Generic;
using System.Text;

namespace EducationalPlatformDesktop.Api.Contracts
{
    public sealed class TestFinishRequestDto
    {
        public int UserId { get; set; }

        public int CourseId { get; set; }
    }
}