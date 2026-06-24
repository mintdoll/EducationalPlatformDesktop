namespace EducationalPlatformDesktop.Api.Contracts
{
    public sealed class LoginResponseDto
    {
        public string? Message { get; set; }
        public string? Token { get; set; }
        public int UserId { get; set; }
        public string? Role { get; set; }
    }
}