using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Windows.Controls;
using WinBaseSoftwareInstall.Interfaces;
using WinBaseSoftwareInstall.Views;

namespace WinBaseSoftwareInstall.ViewModels;

public class MainWindowViewModel : IMainWindowViewModel
{
    public string Title
    {
        get
        {
            Assembly? entryAssembly = Assembly.GetEntryAssembly();
            string appName = entryAssembly?.GetName().Name ?? "Unknown App";
            string companyName = entryAssembly?.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company ?? "Unknown Company";
            string version = entryAssembly?.GetName().Version?.ToString() ?? "Unknown Version";

            string title = $"{appName} | by {companyName} | v{version}";
            return title;
        }
    }

    public UserControl TitleBarUserControl
    {
        get
        {
            TitleBarNonClientView titleBarNonClientView = App.ServiceProvider!.GetRequiredService<TitleBarNonClientView>();
            return titleBarNonClientView;
        }
    }

    public UserControl ApplicationsMenuUserControl
    {
        get
        {
            ApplicationMenuView applicationMenuView = App.ServiceProvider!.GetRequiredService<ApplicationMenuView>();
            return applicationMenuView;
        }
    }
}