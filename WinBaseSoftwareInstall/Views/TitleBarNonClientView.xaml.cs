using System.Windows.Controls;
using WinBaseSoftwareInstall.Interfaces;

namespace WinBaseSoftwareInstall.Views;

public partial class TitleBarNonClientView : UserControl
{
    public TitleBarNonClientView(ITitleBarNonClientViewModel titleBarNonClientViewModel)
    {
        InitializeComponent();
        DataContext = titleBarNonClientViewModel;
    }
}