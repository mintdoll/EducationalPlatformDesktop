using System.Collections.Generic;

namespace EducationalPlatformDesktop.Api
{
    public static class AppSession
    {
        public static int? UserId { get; set; }
        public static string? Role { get; set; }
        public static string? Email { get; set; }
        public static string? Token { get; set; }
        public static string? Name { get; set; }
        public static string? LastName { get; set; }

        public static string FullName
        {
            get
            {
                var parts = new List<string>();

                if (!string.IsNullOrWhiteSpace(Name))
                {
                    parts.Add(Name!);
                }

                if (!string.IsNullOrWhiteSpace(LastName))
                {
                    parts.Add(LastName!);
                }

                return string.Join(" ", parts);
            }
        }

        public static void Clear()
        {
            UserId = null;
            Role = null;
            Email = null;
            Token = null;
            Name = null;
            LastName = null;
        }
    }
}