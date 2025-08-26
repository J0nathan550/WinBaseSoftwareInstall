using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Reflection;
using WinBaseSoftwareInstall.Interfaces;

namespace WinBaseSoftwareInstall.ViewModels;

public class MainWindowViewModel : IMainWindowViewModel
{
    private readonly ILogger<MainWindowViewModel> _logger;
    private readonly IUserService _userService;

    public MainWindowViewModel(ILogger<MainWindowViewModel> logger, IUserService userService)
    {
        _logger = logger;
        _userService = userService;
    }

    private IAsyncRelayCommand? _saveRelayCommand;
    public IAsyncRelayCommand SaveRelayCommand
    {
        get
        {
            _saveRelayCommand ??= new AsyncRelayCommand(ExecuteSaveAsyncCommand);
            return _saveRelayCommand;
        }
    }

    public static string Title
    {
        get
        {
            string title = $"{Assembly.GetExecutingAssembly().GetName().Name} | by {FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).CompanyName} | v{Assembly.GetExecutingAssembly().GetName().Version}";
            return title;
        }
    }

    private async Task ExecuteSaveAsyncCommand()
    {
        try
        {
            _logger.LogInformation("Save command executed");
            await _userService.SaveUserDataAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during save operation");
        }
    }
}