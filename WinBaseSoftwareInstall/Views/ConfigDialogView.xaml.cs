using System.Windows.Controls;
using WinBaseSoftwareInstall.Interfaces;

namespace WinBaseSoftwareInstall.Views;

public partial class ConfigDialogView : UserControl
{
    public IConfigDialogViewModel ConfigDialogViewModel { get; }

    public ConfigDialogView(IConfigDialogViewModel configDialogViewModel)
    {
        InitializeComponent();
        ConfigDialogViewModel = configDialogViewModel;
        DataContext = ConfigDialogViewModel;
    }
}