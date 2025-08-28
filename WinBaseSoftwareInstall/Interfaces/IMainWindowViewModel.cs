using System.Windows.Controls;

namespace WinBaseSoftwareInstall.Interfaces;

public interface IMainWindowViewModel
{
    string Title { get; }
    UserControl TitleBarUserControl { get; }
    UserControl ApplicationsMenuUserControl { get; }
}