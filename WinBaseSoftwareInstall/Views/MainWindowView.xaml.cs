using System.Windows.Media;
using WinBaseSoftwareInstall.Interfaces;
namespace WinBaseSoftwareInstall.Views;

public partial class MainWindowView : HandyControl.Controls.Window
{
    public MainWindowView(IMainWindowViewModel mainWindowViewModel)
    {
        InitializeComponent();
        DataContext = mainWindowViewModel;
    }

    private void GridSplitter_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e) => DividerElement.LineStroke = new SolidColorBrush(Colors.White);
    private void GridSplitter_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e) => DividerElement.LineStroke = new SolidColorBrush(Colors.Transparent);
}