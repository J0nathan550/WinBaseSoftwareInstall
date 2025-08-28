namespace WinBaseSoftwareInstall.Models;

public class ApplicationsMenuModel : ApplicationsMenuBaseModel
{
    public ApplicationsMenuSubModel[] ApplicationsMenuChilds { get; set; } = [];
}