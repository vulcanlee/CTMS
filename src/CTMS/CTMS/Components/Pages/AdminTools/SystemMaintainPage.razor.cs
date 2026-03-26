using CTMS.DataModel.Models;
using CTMS.Share.Helpers;

namespace CTMS.Components.Pages.AdminTools;

public partial class SystemMaintainPage
{
    ConfirmBoxModel ConfirmMessageBox { get; set; } = new ConfirmBoxModel();
    MessageBoxModel MessageBox { get; set; } = new MessageBoxModel();
    string RoleMessage = string.Empty;

    protected override void OnInitialized()
    {
    }

    protected override async Task OnInitializedAsync()
    {
        var checkResult = await AuthenticationStateHelper
        .Check(authStateProvider, NavigationManager);
        if (checkResult == true)
        {
            if (AuthenticationStateHelper.CheckIsAdmin() == false)
            {
                RoleMessage = MagicObjectHelper.你沒有權限存取此頁面;
            }
        }
    }
}
