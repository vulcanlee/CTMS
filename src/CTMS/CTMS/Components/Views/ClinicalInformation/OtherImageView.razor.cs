using AntDesign;
using CTMS.AdapterModels;
using CTMS.DataModel.Models;
using CTMS.DataModel.Models.Apis;
using CTMS.DataModel.Models.ClinicalInformation;
using CTMS.Helper;
using CTMS.Share.Helpers;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Syncfusion.Blazor.DropDowns;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CTMS.Components.Views.ClinicalInformation;

public partial class OtherImageView
{
    [Inject]
    public ModalService modalService { get; set; }
    [Parameter]
    public string Code { get; set; }

    PatientData patientData = new();
    PatientAdapterModel patientAdapterModel = new();
    bool editMode = false;

    OtherTreatmentImageNode data = new();
    OtherTreatmentImage header = new();

    #region 操作 Visit Code 用到的物件
    bool ShowCallApiDialog = false;
    ConfirmBoxModel ConfirmMessageBox { get; set; } = new ConfirmBoxModel();
    MessageBoxModel MessageBox { get; set; } = new MessageBoxModel();
    string VisitCodeOperateMode = string.Empty;
    VisitCodeModel VisitCode = new();
    SfDropDownList<DropDownListDataModel, DropDownListDataModel> VisitCodeDropDown;
    DropDownListDataModel SelectVisitCode = new DropDownListDataModel();
    List<DropDownListDataModel> ListVisitCode = new List<DropDownListDataModel>();
    #endregion

    string sourceObjectJson = "{}";
    string targetObjectJson = "{}";

    protected override async Task OnInitializedAsync()
    {
        patientAdapterModel = await PatientService.GetAsync(Code);
        patientData.FromJson(patientAdapterModel.JsonData);
        InitData(true);
    }

    void InitData(bool isFirst = true)
    {
        header = patientData.臨床資料.其他治療影像;
        if (isFirst)
        {
            data = header.Items.FirstOrDefault();
            RefreshVisitCode();
            RefreshDropDwonVisitCode();
            var itemx = ListVisitCode.FirstOrDefault(x => x.Key == data?.VisitCode.Id);
            if (itemx != null)
                SelectVisitCode = itemx;
        }
        else
        {
            if (SelectVisitCode != null)
            {
                data = header.Items.FirstOrDefault(x => x.VisitCode.Id == SelectVisitCode.Key);
            }
        }
    }

    async Task OnGetReportApiAsync(List<ReportApiModel> reportApiData)
    {
        ShowCallApiDialog = false;
        await InvokeAsync(StateHasChanged);

        if (reportApiData != null && reportApiData.Count > 0)
        {
            string visitCodeTitle = data?.VisitCode?.VisitCodeTitle;
            var ok = await modalService.ConfirmAsync(new ConfirmOptions
            {
                Title = "再次確認",
                Content = $"確定要匯入這裡選取的成大檢查報告方面的資料到該 Visit Code {visitCodeTitle} 內嗎?",
                OkText = "是",
                CancelText = "取消",
                OkButtonProps = new ButtonProps { Danger = true },
                MaskClosable = false
            });

            if (ok)
            {
                foreach (var report in reportApiData)
                {
                    OtherTreatmentImageItem tItem = new()
                    {
                        ExecuteTime = report.ExecuteTime,
                        OrderCode = report.OrderCode,
                        OrderName = report.OrderName,
                        ReportText = report.ReportText
                    };
                    data.Items.Add(tItem);
                }
                await OnSave();
                await InvokeAsync(StateHasChanged);
            }
        }
    }

    async Task OnShowApiDialog()
    {
        if(data == null || data.VisitCode == null || data.VisitCode.AssessmentDate == null)
        {
            var ok = await modalService.InfoAsync(new ConfirmOptions
            {
                Title = "警告",
                Content = $"沒有發現可用的 Visit Code",
                OkText = "確定",
                OkButtonProps = new ButtonProps { Danger = true },
                MaskClosable = false
            });

            return;
        }

        ShowCallApiDialog = true;
    }

    void OnChangeEditMode()
    {
        editMode = !editMode;

        sourceObjectJson = JsonConvert.SerializeObject(patientData.臨床資料.其他治療影像);
    }

    async Task OnSave()
    {
        patientAdapterModel.JsonData = patientData.ToJson();
        await PatientService.UpdateAsync(patientAdapterModel);
        editMode = false;

        targetObjectJson = JsonConvert.SerializeObject(patientData.臨床資料.其他治療影像);

        #region 更新操作日誌
        MyUserAdapterModel myUserAdapterModel = await AuthenticationStateHelper
        .GetUserInformation(authStateProvider);

        await OperationHistoryTraceService.AddAsync(OperationHistoryTraceAdapterModel.Build(myUserAdapterModel.Name, patientData.臨床資訊.SubjectNo, MagicObjectHelper.OperationCategory追蹤資料其他治療影像, "-", "-"), sourceObjectJson, targetObjectJson, MagicObjectHelper.OperationCategory追蹤資料其他治療影像);
        #endregion
    }

    async Task OnCancel()
    {
        patientData.FromJson(patientAdapterModel.JsonData);
        InitData(false);
        editMode = false;
    }

    async Task OnAddAsync()
    {
    }

    async Task OnDeleteAsync(OtherTreatmentImageItem item)
    {
        var ok = await modalService.ConfirmAsync(new ConfirmOptions
        {
            Title = "再次確認",
            Content = $"確定要刪除該篇檢查報告 Order Code {item.OrderCode} / Order Name {item.OrderName} 嗎?",
            OkText = "確定",
            CancelText = "取消",
            OkButtonProps = new ButtonProps { Danger = true },
            MaskClosable = false
        });

        if (ok)
        {
            data.Items.Remove(item);
            await OnSave();
            await InvokeAsync(StateHasChanged);
        }

    }


    #region 針對 VisitCode 的方法
    private async Task OnVisitCodeChanged(ChangeEventArgs<DropDownListDataModel, DropDownListDataModel> args)
    {
        if (args.Value != null)
        {
            var item = header.Items.FirstOrDefault(x => x.VisitCode.Id == args.Value.Key);
            if (item != null)
            {
                data = item;
            }
        }
    }

    void RefreshVisitCode(bool reset = true)
    {
        #region VisitCode
        {
            ListVisitCode.Clear();
            if (data != null)
            {

            }
            foreach (var nodeItem in header.Items)
            {
                ListVisitCode.Add(new DropDownListDataModel()
                {
                    Key = nodeItem.VisitCode.Id,
                    Name = nodeItem.VisitCode.VisitCodeTitle,
                });
            }
            VisitCodeHelper.Sort(ListVisitCode);
            if (reset)
            {
                SelectVisitCode = null;
            }
        }
        #endregion
    }
    void RefreshDropDwonVisitCode()
    {
        // 重置選擇項目
        SelectVisitCode = null;

        // 手動刷新 DropDownList 元件
        if (VisitCodeDropDown != null)
        {
            InvokeAsync(() => VisitCodeDropDown.RefreshDataAsync());
        }
    }
    #endregion
}
