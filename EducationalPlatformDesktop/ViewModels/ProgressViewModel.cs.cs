using System.Collections.ObjectModel;
using EducationalPlatformDesktop.Mocks;
using EducationalPlatformDesktop.Models;

namespace EducationalPlatformDesktop.ViewModels
{
    public class ProgressViewModel : ViewModelBase
    {
        public ObservableCollection<Progress> Items { get; }
            = MockTestData.GetProgress();
    }
}