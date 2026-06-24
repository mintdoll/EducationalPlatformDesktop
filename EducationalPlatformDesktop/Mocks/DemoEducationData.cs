using System.Collections.ObjectModel;
using EducationalPlatformDesktop.Models;

namespace EducationalPlatformDesktop.Mocks
{
    public static class DemoEducationData
    {
        public static UserProfile GetProfile()
        {
            return new UserProfile
            {
                FullName = "Арина Бутакова",
                EmailOrMax = "arina.butakova@example.com",
                Group = "БИ-24-1",
                Role = "Студент"
            };
        }

        
    }
}