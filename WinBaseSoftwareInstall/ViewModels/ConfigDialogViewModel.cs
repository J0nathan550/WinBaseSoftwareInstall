using Microsoft.Extensions.Logging;
using WinBaseSoftwareInstall.Interfaces;

namespace WinBaseSoftwareInstall.ViewModels;

public class ConfigDialogViewModel : IConfigDialogViewModel
{
    private readonly ILogger<ConfigDialogViewModel> _logger;

    public ConfigDialogViewModel(ILogger<ConfigDialogViewModel> logger)
    {
        _logger = logger;
        Result = string.Empty;
    }

    public string Result { get; set; }
    public Action? CloseAction { get; set; }
}