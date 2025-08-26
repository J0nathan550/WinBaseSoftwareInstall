using CommunityToolkit.Mvvm.Input;

namespace WinBaseSoftwareInstall.Interfaces;

public interface ITitleBarNonClientViewModel
{
    IAsyncRelayCommand OpenConfigRelayCommand { get; }
}