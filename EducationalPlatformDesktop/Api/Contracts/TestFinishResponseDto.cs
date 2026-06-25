using System;
using System.Collections.Generic;
using System.Text;

namespace EducationalPlatformDesktop.Api.Contracts
{
    public sealed class TestFinishResponseDto
    {
        public string Message { get; set; } = string.Empty;

        public int Score { get; set; }
    }
}