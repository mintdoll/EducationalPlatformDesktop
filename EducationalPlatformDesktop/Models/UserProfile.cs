namespace EducationalPlatformDesktop.Models
{
    public class UserProfile
    {
        public string FullName { get; set; } = string.Empty;
        public string EmailOrMax { get; set; } = string.Empty;
        public string Group { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;

        public string Initials
        {
            get
            {
                if (string.IsNullOrWhiteSpace(FullName))
                {
                    return string.Empty;
                }

                var parts = FullName.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length == 1)
                {
                    return parts[0][0].ToString().ToUpper();
                }

                var first = parts[0][0].ToString().ToUpper();
                var second = parts[1][0].ToString().ToUpper();

                return $"{first}{second}";
            }
        }
    }
}