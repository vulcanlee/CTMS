using CTMS.AdapterModels;
using CTMS.DataModel.Models;
using CTMS.DataModel.Models.ClinicalInformation;
using CTMS.Share.Helpers;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;

namespace CTMS.Components.Views.Commons;

public partial class VisitCodeDialog
{
    [Parameter]
    public bool OpenPicker { get; set; } = false;
    [Parameter]
    public EventCallback<VisitCodeSetModel> OnConfirmCallback { get; set; }
    [Parameter]
    public VisitCodeSetModel data { get; set; }
    [Parameter]
    public string Code { get; set; }

    ConfirmBoxModel ConfirmMessageBox { get; set; } = new ConfirmBoxModel();
    MessageBoxModel MessageBox { get; set; } = new MessageBoxModel();

    string DialogTitle = "設定 Visit Code";
    public bool ShowMessageBox { get; set; } = false;
    public string MessageBoxBody { get; set; } = "";
    public string MessageBoxTitle { get; set; } = "";

    bool ShowVisitCodeEditorDialog = false;
    VisitCodeModel VisitCode = new();
    VisitCodeModel VisitCodeBackup = new();
    string VisitCodeMode = "";

    // List<VisitCodeModel> VisitCodes = new();
    PatientData patientData = new();
    PatientAdapterModel patientAdapterModel = new();

    bool editMode = false;
    string sourceObjectJson = "{}";
    string targetObjectJson = "{}";

    protected override async Task OnInitializedAsync()
    {
        await GetData();
    }

    async Task GetData()
    {
        patientAdapterModel = await PatientService.GetAsync(Code);
        patientData.FromJson(patientAdapterModel.JsonData);
    }

    async Task Init()
    {
        //patientData.臨床資料.CollectVisitCode(data);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
        }
    }

    async Task OnPickerOK()
    {
        if (data == null)
        {
            var checkTask = ConfirmMessageBox.ShowAsync("400px", "200px", "警告",
                 "沒有傳入任何 Protein 物件", ConfirmMessageBox.HiddenAsync);
            StateHasChanged();
            await checkTask;
            OpenPicker = false;
        }
        OpenPicker = false;
        await OnConfirmCallback.InvokeAsync(data);
    }

    async Task OnPickerCancel()
    {
        OpenPicker = false;
        await OnConfirmCallback.InvokeAsync(null);
    }

    async Task OnVisiteCodeEditorAsync(VisitCodeModel visitCodeModel)
    {
        if (visitCodeModel != null)
        {
            if (VisitCodeMode == "Edit")
            {
                await GetData();
                patientData.臨床資料.ReplaceVisitCode(VisitCodeBackup, visitCodeModel);

                patientData.SyncData();
                patientAdapterModel = await PatientService.GetAsync(Code);
                patientAdapterModel.JsonData = patientData.ToJson();

                await PatientService.UpdateAsync(patientAdapterModel);

                OpenPicker = false;
                await OnConfirmCallback.InvokeAsync(null);
            }
            else if (VisitCodeMode == "Add")
            {
                data.AddVisitCode(visitCodeModel);
            }
            else
            {
                // MessageBox.Show("未知的模式: " + VisitCodeMode, "錯誤");
                // return;
            }
        }
        ShowVisitCodeEditorDialog = false;
    }

    void OnShowVisitCodeEditor()
    {
        VisitCode = new();
        VisitCodeMode = "Add";
        ShowVisitCodeEditorDialog = true;
    }

    async Task OnEditVisitCodeAsync(VisitCodeModel itemVisitCode)
    {
        VisitCode = itemVisitCode;
        VisitCodeBackup = itemVisitCode.Clone();
        VisitCodeMode = "Edit";
        ShowVisitCodeEditorDialog = true;
    }

    async Task OnDeleteVisitCodeAsync(VisitCodeModel itemVisitCode)
    {
        var task = ConfirmMessageBox.ShowAsync("400px", "200px",
        "警告", $"確定要刪除 Visit Code : {itemVisitCode.GetVisitCodeTitle()} 嗎？",
        ConfirmMessageBox.HiddenAsync);
        StateHasChanged();
        var confirmDelete = await task;
        if (confirmDelete == true)
        {
            sourceObjectJson = JsonConvert.SerializeObject(data);
            await GetData();
            patientData.臨床資料.RemoveVisitCode(itemVisitCode);

            patientData.SyncData();
            patientAdapterModel = await PatientService.GetAsync(Code);
            patientAdapterModel.JsonData = patientData.ToJson();

            await PatientService.UpdateAsync(patientAdapterModel);

            VisitCodeSetModel VisitCodeSetModel = new();
            patientData.臨床資料.CollectVisitCode(VisitCodeSetModel);
            targetObjectJson = JsonConvert.SerializeObject(VisitCodeSetModel);

            #region 更新操作日誌
            MyUserAdapterModel myUserAdapterModel = await AuthenticationStateHelper
            .GetUserInformation(authStateProvider);

            await OperationHistoryTraceService.AddAsync(OperationHistoryTraceAdapterModel.Build(myUserAdapterModel.Name, patientData.臨床資訊.SubjectNo, MagicObjectHelper.OperationCategoryVisitCode, "-", "-"), sourceObjectJson, targetObjectJson, MagicObjectHelper.OperationCategoryVisitCode);
            #endregion

            OpenPicker = false;
            await OnConfirmCallback.InvokeAsync(null);
        }
        else
        {
            // 使用者取消刪除操作
        }

    }

    void ToggleExpanded(VisitCodeModel itemVisitCode)
    {
        data.ToggleExpanded(itemVisitCode);
    }
}
