using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WinBaseSoftwareInstall.Interfaces;
using WinBaseSoftwareInstall.Views;

namespace WinBaseSoftwareInstall.ViewModels;

public class TitleBarNonClientViewModel : ITitleBarNonClientViewModel
{
    private readonly ILogger<TitleBarNonClientViewModel> _logger;

    public TitleBarNonClientViewModel(ILogger<TitleBarNonClientViewModel> logger)
    {
        _logger = logger;
    }

    private IAsyncRelayCommand? _openConfigRelayCommand;
    public IAsyncRelayCommand OpenConfigRelayCommand
    {
        get
        {
            return _openConfigRelayCommand ??= new AsyncRelayCommand(OpenConfigAsyncRelayCommand_Execute);
        }
    }

    private async Task OpenConfigAsyncRelayCommand_Execute()
    {
        _logger.LogInformation("Opening Config Dialog...");
        ConfigDialogView configDialogView = App.ServiceProvider!.GetRequiredService<ConfigDialogView>();
        string dialogResult = string.Empty;
        await Dialog.Show(configDialogView).Initialize<ConfigDialogViewModel>(vm => vm.Result = dialogResult)
            .GetResultAsync<string>()
            .ContinueWith(str => dialogResult = str.Result);
        _logger.LogInformation("Closing Config Dialog...");
    }
}