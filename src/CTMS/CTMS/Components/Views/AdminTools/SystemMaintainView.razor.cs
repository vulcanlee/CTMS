using AntDesign;
using CTMS.Business.Helpers;
using CTMS.DataModel.Models;
using CTMS.DataModel.Models.Systems;
using CTMS.Services;
using Microsoft.AspNetCore.Components;

namespace CTMS.Components.Views.AdminTools;

public partial class SystemMaintainView
{
    [Inject]
    public ILogger<SystemMaintainView> logger { get; set; }
    [Inject]
    public SystemMaintainServices systemMaintainServices { get; set; }
    [Inject]
    public ModalService modalService { get; set; }
    public async Task OnFix成大抽血生化eGFR參考區間()
    {
        var ok = await modalService.ConfirmAsync(new ConfirmOptions
        {
            Title = "再次確認",
            Content = "確定要刪除這個 成大抽血生化_eGFR參考區間修正 需求嗎？",
            OkText = "是",
            CancelText = "取消",
            OkButtonProps = new ButtonProps { Danger = true },
            MaskClosable = false
        });

        if (ok)
        {
            await systemMaintainServices.Fix_20260326_成大抽血生化_eGFR參考區間修正();
        }
    }
}
