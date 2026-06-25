namespace EducationalPlatformDesktop.Api.Contracts
{
    public sealed class TestDto
    {
        public int Id { get; set; }

        public int CourseId { get; set; }

        public string Name { get; set; } = string.Empty;
    }
}