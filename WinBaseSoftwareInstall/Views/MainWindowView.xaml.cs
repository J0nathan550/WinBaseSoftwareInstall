using System.Windows;
using WinBaseSoftwareInstall.Interfaces;

namespace WinBaseSoftwareInstall.Views
{
    public partial class MainWindowView : Window
    {
        public MainWindowView(IMainWindowViewModel mainWindowViewModel)
        {
            InitializeComponent();
            DataContext = mainWindowViewModel;
        }
    }
}