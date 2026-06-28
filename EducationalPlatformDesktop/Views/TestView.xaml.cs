using System.Windows.Controls;
using System.Windows.Input;

namespace EducationalPlatformDesktop.Views
{
    public partial class TestView : UserControl
    {
        public TestView()
        {
            InitializeComponent();
            Loaded += (_, _) => Keyboard.Focus(this);
        }
    }
}
