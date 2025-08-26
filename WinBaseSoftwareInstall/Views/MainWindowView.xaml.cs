using WinBaseSoftwareInstall.Interfaces;
namespace WinBaseSoftwareInstall.Views;

public partial class MainWindowView : HandyControl.Controls.Window
{
    public MainWindowView(IMainWindowViewModel mainWindowViewModel)
    {
        InitializeComponent();
        DataContext = mainWindowViewModel;
    }
}