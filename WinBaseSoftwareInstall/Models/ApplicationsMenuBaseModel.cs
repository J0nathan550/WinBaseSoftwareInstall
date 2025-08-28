using System.Windows.Media;

namespace WinBaseSoftwareInstall.Models;

public class ApplicationsMenuBaseModel
{
    public string Title { get; set; } = string.Empty;
    public Geometry? Icon { get; set; }
}