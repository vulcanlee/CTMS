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

namespace CTMS.Components.Views.ClinicalInformation;

public partial class OtherMedicationView
{
    [Inject]
    public ModalService modalService { get; set; }
    [Parameter]
    public string Code { get; set; }

    PatientData patientData = new();
    PatientAdapterModel patientAdapterModel = new();
    bool editMode = false;

    OtherMedicationNode data = new();
    OtherMedication header = new();

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
        header = patientData.臨床資料.其他治療藥物;
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

    async Task OnGetMedicationApiAsync(List<MedicationApiModel> reportApiData)
    {
        ShowCallApiDialog = false;
        await InvokeAsync(StateHasChanged);

        if (reportApiData != null && reportApiData.Count > 0)
        {
            string visitCodeTitle = data?.VisitCode?.VisitCodeTitle;
            var ok = await modalService.ConfirmAsync(new ConfirmOptions
            {
                Title = "再次確認",
                Content = $"確定要匯入這裡選取的成大藥品方面的資料到該 Visit Code {visitCodeTitle} 內嗎?",
                OkText = "是",
                CancelText = "取消",
                OkButtonProps = new ButtonProps { Danger = true },
                MaskClosable = false
            });

            if (ok)
            {
                //BloodExameService.MatchApiChemistrydResult(data.抽血檢驗生化, reportApiData);
                //await OnSave();
                await InvokeAsync(StateHasChanged);
            }
        }
    }

    void OnShowApiDialog()
    {
        ShowCallApiDialog = true;
    }

    void OnChangeEditMode()
    {
        editMode = !editMode;

        sourceObjectJson = JsonConvert.SerializeObject(patientData.臨床資料.其他治療藥物);
    }

    async Task OnSave()
    {
        patientAdapterModel.JsonData = patientData.ToJson();
        await PatientService.UpdateAsync(patientAdapterModel);
        editMode = false;

        targetObjectJson = JsonConvert.SerializeObject(patientData.臨床資料.其他治療藥物);

        #region 更新操作日誌
        MyUserAdapterModel myUserAdapterModel = await AuthenticationStateHelper
        .GetUserInformation(authStateProvider);

        await OperationHistoryTraceService.AddAsync(OperationHistoryTraceAdapterModel.Build(myUserAdapterModel.Name, patientData.臨床資訊.SubjectNo, MagicObjectHelper.OperationCategory追蹤資料其他治療藥物, "-", "-"), sourceObjectJson, targetObjectJson, MagicObjectHelper.OperationCategory追蹤資料其他治療藥物);
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
        data.Items.Add(new OtherMedicationItem());
        await OnSave();
    }

    async Task OnDeleteAsync(OtherMedicationItem item)
    {
        // Assuming we need to remove the last item for deletion
        if (data.Items.Count > 0)
        {
            data.Items.Remove(item);
            await OnSave();
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
