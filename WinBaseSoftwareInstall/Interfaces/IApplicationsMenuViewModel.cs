using WinBaseSoftwareInstall.Models;

namespace WinBaseSoftwareInstall.Interfaces;

public interface IApplicationsMenuViewModel
{
    ApplicationsMenuModel[] ApplicationsMenus { get; }
}