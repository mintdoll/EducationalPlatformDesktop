using System.Collections.Generic;
using System.Text;
using System;

namespace EducationalPlatformDesktop.Api.Contracts
{
    public sealed class CertificateDto
    {
        public string CertificateNumber { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string CourseName { get; set; } = string.Empty;

        public DateTime IssueDate { get; set; }
    }
}