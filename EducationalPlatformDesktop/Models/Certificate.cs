namespace EducationalPlatformDesktop.Models
{
    public class Certificate
    {
        public string CourseName { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string IssuedDate { get; set; } = string.Empty;
        public int Score { get; set; }
    }
}