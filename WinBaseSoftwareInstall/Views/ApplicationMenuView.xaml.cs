using System.Windows.Controls;
using WinBaseSoftwareInstall.Interfaces;

namespace WinBaseSoftwareInstall.Views;

public partial class ApplicationMenuView : UserControl
{
    public ApplicationMenuView(IApplicationsMenuViewModel applicationsMenuViewModel)
    {
        InitializeComponent();
        DataContext = applicationsMenuViewModel;
    }
}