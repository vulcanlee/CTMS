using CTMS.AdapterModels;
using CTMS.DataModel.Models;
using CTMS.DataModel.Models.ClinicalInformation;
using CTMS.Share.Helpers;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;

namespace CTMS.Components.Views.ClinicalInformation;

public partial class RiskAssessmentView
{
    [Parameter]
    public string Code { get; set; }

    ConfirmBoxModel ConfirmMessageBox { get; set; } = new ConfirmBoxModel();
    MessageBoxModel MessageBox { get; set; } = new MessageBoxModel();

    RiskAssessmentResult data = new RiskAssessmentResult();
    PatientData patientData = new();
    PatientAdapterModel patientAdapterModel = new();
    MyUserAdapterModel myUserAdapterModel = new();
    bool hasRiskAssessment = false;
    string imageVersion = DateTime.Now.Ticks.ToString();

    SignatureNode signatureNode婦產科醫師確認 = new SignatureNode();
    SignatureNode signatureNode放射科醫師確認 = new SignatureNode();

    bool isShowHistory = false;

    string sourceObjectJson = "{}";
    string targetObjectJson = "{}";
    signature婦產科放射科Model signature婦產科放射科Model = new signature婦產科放射科Model();

    protected override async Task OnInitializedAsync()
    {
        myUserAdapterModel = await AuthenticationStateHelper.GetUserInformation(authStateProvider);
        if (myUserAdapterModel != null)
        {
        }

        // Simulate fetching data from a service
        patientAdapterModel = await PatientService.GetAsync(Code);
        patientData.FromJson(patientAdapterModel.JsonData);
        InitData();
    }

    void InitData()
    {
        signatureNode婦產科醫師確認 = patientData.臨床資訊.ObstetricianGynecologistConfirmation;
        signatureNode放射科醫師確認 = patientData.臨床資訊.RadiologistConfirmation;

        data = patientData.臨床資訊.RiskAssessmentResult;

        if (data != null && string.IsNullOrEmpty(data.風險程度) == false)
        {
            hasRiskAssessment = true;
        }
    }

    void OnShowHistory()
    {
        isShowHistory = true;
    }

    private async Task OnShowHistoryClosed(string args)
    {
        if (string.IsNullOrEmpty(args))
        {
        }
        isShowHistory = false;
        patientAdapterModel = await PatientService.GetAsync(Code);
        patientData.FromJson(patientAdapterModel.JsonData);
        InitData();
        StateHasChanged();
    }

    /// <summary>
    ///  婦產科醫師確認
    /// </summary>
    /// <returns></returns>
    async Task On婦產科醫師確認SignAsync()
    {
        if (AuthenticationStateHelper.CheckAccessPage(MagicObjectHelper.ROLE風險評估結果確認) == false)
        {
            MessageBox.Show("400px", "200px", "資訊", MagicObjectHelper.你沒有權限操作此功能, MessageBox.HiddenAsync);
            return;
        }


        var task = ConfirmMessageBox.ShowAsync("400px", "200px",
        "再次確認", $"確定要以婦產科醫師身分確認這些影像與數據正確無誤嗎？",
        ConfirmMessageBox.HiddenAsync);
        StateHasChanged();
        bool result = await task;
        if (result == false)
        {
            return;
        }

        if (myUserAdapterModel != null && myUserAdapterModel.Id > 0)
        {
            sourceObjectJson = JsonConvert.SerializeObject(patientData.臨床資訊.ObstetricianGynecologistConfirmationList);

            SignatureNode signatureNode = new SignatureNode
            {
                SignatureId = myUserAdapterModel.Id,
                SignatureName = myUserAdapterModel.Name,
                SignatureDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
            };
            patientData.臨床資訊.ObstetricianGynecologistConfirmation = signatureNode;
            patientData.臨床資訊.ObstetricianGynecologistConfirmationList.Add(signatureNode);
            patientAdapterModel.JsonData = patientData.ToJson();

            patientAdapterModel.AI評估 = MagicObjectHelper.AI評估完成;
            await PatientService.UpdateAsync(patientAdapterModel);

            signatureNode婦產科醫師確認 = signatureNode;

            targetObjectJson = JsonConvert.SerializeObject(patientData.臨床資訊.ObstetricianGynecologistConfirmationList);

            #region 更新操作日誌
            await OperationHistoryTraceService.AddAsync(OperationHistoryTraceAdapterModel.Build(myUserAdapterModel.Name, patientData.臨床資訊.SubjectNo, MagicObjectHelper.OperationCategory婦產科風險評估確認, "-", "-"), sourceObjectJson, targetObjectJson, MagicObjectHelper.OperationCategory婦產科風險評估確認);
            #endregion
            return;
        }
    }

    /// <summary>
    ///  放射科醫師確認
    /// </summary>
    /// <returns></returns>
    async Task On放射科醫師確認SignAsync()
    {
        if (AuthenticationStateHelper.CheckAccessPage(MagicObjectHelper.ROLE風險評估影像確認) == false)
        {
            MessageBox.Show("400px", "200px", "資訊", MagicObjectHelper.你沒有權限操作此功能, MessageBox.HiddenAsync);
            return;
        }

        var task = ConfirmMessageBox.ShowAsync("400px", "200px",
        "再次確認", $"確定要以放射科醫師身分確認這些影像與數據正確無誤嗎？",
        ConfirmMessageBox.HiddenAsync);
        StateHasChanged();
        bool result = await task;
        if (result == false)
        {
            return;
        }

        if (myUserAdapterModel != null && myUserAdapterModel.Id > 0)
        {
            // signature婦產科放射科Model.signature婦產科 = patientData.臨床資訊.ObstetricianGynecologistConfirmationList;
            // signature婦產科放射科Model.signature放射科 = patientData.臨床資訊.RadiologistConfirmationList;
            sourceObjectJson = JsonConvert.SerializeObject(patientData.臨床資訊.RadiologistConfirmationList);

            SignatureNode signatureNode = new SignatureNode
            {
                SignatureId = myUserAdapterModel.Id,
                SignatureName = myUserAdapterModel.Name,
                SignatureDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
            };
            patientData.臨床資訊.RadiologistConfirmation = signatureNode;
            patientData.臨床資訊.RadiologistConfirmationList.Add(signatureNode);
            patientAdapterModel.JsonData = patientData.ToJson();

            patientAdapterModel.AI評估 = MagicObjectHelper.AI評估完成;
            await PatientService.UpdateAsync(patientAdapterModel);

            signatureNode放射科醫師確認 = patientData.臨床資訊.RadiologistConfirmation;

            targetObjectJson = JsonConvert.SerializeObject(patientData.臨床資訊.RadiologistConfirmationList);

            #region 更新操作日誌
            // MyUserAdapterModel myUserAdapterModel = await AuthenticationStateHelper
            // .GetUserInformation(authStateProvider);

            await OperationHistoryTraceService.AddAsync(OperationHistoryTraceAdapterModel.Build(myUserAdapterModel.Name, patientData.臨床資訊.SubjectNo, MagicObjectHelper.OperationCategory放射科風險評估確認, "-", "-"), sourceObjectJson, targetObjectJson, MagicObjectHelper.OperationCategory放射科風險評估確認);
            #endregion
            return;
        }
    }
}
