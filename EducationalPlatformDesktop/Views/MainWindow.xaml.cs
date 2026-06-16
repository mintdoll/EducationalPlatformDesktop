using System.Windows;
using EducationalPlatformDesktop.ViewModels;

namespace EducationalPlatformDesktop.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var viewModel = new MainViewModel();
            viewModel.RequestClose += Close;

            DataContext = viewModel;
        }
    }
}