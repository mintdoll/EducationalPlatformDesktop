namespace EducationalPlatformDesktop.Api.Contracts
{
    public sealed class CourseDto
    {
        public int Id { get; set; }
        public string Teacher { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Price { get; set; }
        public string Purpose { get; set; } = string.Empty;
    }
}