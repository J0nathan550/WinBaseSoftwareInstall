using CommunityToolkit.Mvvm.Input;

namespace WinBaseSoftwareInstall.Interfaces;

public interface IMainWindowViewModel
{
    IAsyncRelayCommand SaveRelayCommand { get; }
}