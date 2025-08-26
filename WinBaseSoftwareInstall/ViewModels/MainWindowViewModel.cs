using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using WinBaseSoftwareInstall.Interfaces;

namespace WinBaseSoftwareInstall.ViewModels
{
    public class MainWindowViewModel : IMainWindowViewModel
    {
        private readonly ILogger<MainWindowViewModel> _logger;
        private readonly IUserService _userService;

        public MainWindowViewModel(ILogger<MainWindowViewModel> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        private IAsyncRelayCommand _saveRelayCommand;
        public IAsyncRelayCommand SaveRelayCommand
        {
            get
            {
                if (_saveRelayCommand == null)
                {
                    _saveRelayCommand = new AsyncRelayCommand(ExecuteSaveAsyncCommand);
                }
                return _saveRelayCommand;
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
}