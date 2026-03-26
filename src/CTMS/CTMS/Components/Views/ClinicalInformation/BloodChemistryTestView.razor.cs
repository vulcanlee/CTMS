using CTMS.AdapterModels;
using CTMS.DataModel.Models;
using CTMS.DataModel.Models.ClinicalInformation;
using CTMS.Helper;
using CTMS.Share.Helpers;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Syncfusion.Blazor.DropDowns;

namespace CTMS.Components.Views.ClinicalInformation;

public partial class BloodChemistryTestView
{
    [Parameter]
    public string Code { get; set; }

    PatientData patientData = new();
    PatientAdapterModel patientAdapterModel = new();
    bool editMode = false;

    BloodTest抽血檢驗生化Node data = new();
    BloodTest抽血檢驗生化 header = new();
    BloodTest抽血檢驗生化Node dataOriginal = new();

    #region 操作 Visit Code 用到的物件
    bool ShowVisitCodeDialog = false;
    ConfirmBoxModel ConfirmMessageBox { get; set; } = new ConfirmBoxModel();
    MessageBoxModel MessageBox { get; set; } = new MessageBoxModel();
    string VisitCodeOperateMode = string.Empty;
    VisitCodeModel VisitCode = new();
    SfDropDownList<DropDownListDataModel, DropDownListDataModel> VisitCodeDropDown;
    DropDownListDataModel SelectVisitCode = new DropDownListDataModel();
    List<DropDownListDataModel> ListVisitCode = new List<DropDownListDataModel>();

    DateTime? resetDate = null;
    #endregion

    bool isLockEdit = true;

    string sourceObjectJson = "{}";
    string targetObjectJson = "{}";

    protected override async Task OnInitializedAsync()
    {
        patientAdapterModel = await PatientService.GetAsync(Code);
        patientData.FromJson(patientAdapterModel.JsonData);
        InitData(true);
        if (data != null)
        {
            var item1 = data.抽血檢驗生化.FirstOrDefault(z => z.項目名稱 == "LDH U/L");
            var item2 = data.抽血檢驗生化.FirstOrDefault(z => z.項目名稱 == "乳酸脫氫酶 (LDH)");
            if (item1 != null)
            {
                item1.項目名稱 = "乳酸脫氫酶 (LDH) U/L";
            }
            if (item2 != null)
            {
                data.抽血檢驗生化.Remove(item2);
            }
            BloodExameService.CheckBloodExame(data.抽血檢驗生化);
        }
    }

    void InitData(bool isFirst = true)
    {
        header = patientData.臨床資料.抽血檢驗生化;
        if (isFirst)
        {
            data = header.Items.FirstOrDefault();
            var dataJson = JsonConvert.SerializeObject(data);
            dataOriginal = JsonConvert.DeserializeObject<BloodTest抽血檢驗生化Node>(dataJson);

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
                var dataJson = JsonConvert.SerializeObject(data);
                dataOriginal = JsonConvert.DeserializeObject<BloodTest抽血檢驗生化Node>(dataJson);
            }
        }
    }

    void OnResetDate()
    {
        foreach (var item in data.抽血檢驗生化)
        {
            if (string.IsNullOrEmpty(item.檢驗數值) == true)
                continue;
            item.SamplingDate = resetDate;
        }
    }

    void OnChangeEditMode()
    {
        editMode = !editMode;

        sourceObjectJson = JsonConvert.SerializeObject(patientData.臨床資料.抽血檢驗生化);
    }

    void OnLockEdit()
    {
        isLockEdit = !isLockEdit;
    }

    async Task OnSave()
    {
        if (data != null)
            BloodExameService.CheckBloodExame(data.抽血檢驗生化);
        patientAdapterModel.JsonData = patientData.ToJson();
        await PatientService.UpdateAsync(patientAdapterModel);
        editMode = false;

        var dataJson = JsonConvert.SerializeObject(data);
        dataOriginal = JsonConvert.DeserializeObject<BloodTest抽血檢驗生化Node>(dataJson);

        targetObjectJson = JsonConvert.SerializeObject(patientData.臨床資料.抽血檢驗生化);

        #region 更新操作日誌
        MyUserAdapterModel myUserAdapterModel = await AuthenticationStateHelper
        .GetUserInformation(authStateProvider);

        await OperationHistoryTraceService.AddAsync(OperationHistoryTraceAdapterModel.Build(myUserAdapterModel.Name, patientData.臨床資訊.SubjectNo, MagicObjectHelper.OperationCategory抽血檢驗生化, "-", "-"), sourceObjectJson, targetObjectJson, MagicObjectHelper.OperationCategory抽血檢驗生化);
        #endregion
    }

    async Task OnCancel()
    {
        patientData.FromJson(patientAdapterModel.JsonData);
        editMode = false;
        InitData(false);
        BloodExameService.CheckBloodExame(data.抽血檢驗生化);
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

                var dataJson = JsonConvert.SerializeObject(data);
                dataOriginal = JsonConvert.DeserializeObject<BloodTest抽血檢驗生化Node>(dataJson);
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
