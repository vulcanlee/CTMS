using AntDesign;
using CTMS.Business.Services.ClinicalInformation;
using CTMS.DataModel.Models;
using CTMS.DataModel.Models.Apis;
using CTMS.Services;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;

namespace CTMS.Components.Views.Commons;

public partial class QueryMedicationApiDialog
{
    [Inject]
    public ApiConditionService ApiConditionService { get; set; }
    [Inject]
    public NckuhApiService NckuhApiService { get; set; }
    [Inject]
    public ModalService modalService { get; set; }
    [Parameter]
    public bool OpenPicker { get; set; } = false;
    [Parameter]
    public EventCallback<List<MedicationApiModel>> OnConfirmCallback { get; set; }

    ConfirmBoxModel ConfirmMessageBox { get; set; } = new ConfirmBoxModel();
    MessageBoxModel MessageBox { get; set; } = new MessageBoxModel();

    string DialogTitle = "查詢藥品紀錄";
    public bool ShowMessageBox { get; set; } = false;
    public string MessageBoxBody { get; set; } = "";
    public string MessageBoxTitle { get; set; } = "";

    List<MedicationApiModel> data = new List<MedicationApiModel>();
    public string ApiTestChartNo { get; set; } = string.Empty;
    public string ApiTestBeginTime { get; set; } = string.Empty;
    public string ApiTestEndTime { get; set; } = string.Empty;
    bool IsApiCalling = false;

    protected override async Task OnInitializedAsync()
    {
        if(string.IsNullOrEmpty(ApiTestChartNo))
        {
            ApiTestChartNo = ApiConditionService.ApiTestChartNo;
            ApiTestBeginTime = ApiConditionService.ApiTestBeginTime;
            ApiTestEndTime = ApiConditionService.ApiTestEndTime;
        }
        await GetData();
    }

    async Task GetData()
    {
    }

    public async Task OnApi呼叫()
    {
        if (string.IsNullOrEmpty(ApiTestChartNo) || string.IsNullOrEmpty(ApiTestBeginTime) || string.IsNullOrEmpty(ApiTestEndTime))
        {
            var checkTask = MessageBox.ShowAsync("400px", "200px", "錯誤", "請確認輸入的資料是否完整，病歷號、起訖日期必須要有值", MessageBox.HiddenAsync);
            StateHasChanged();
            await checkTask;
            return;
        }

        IsApiCalling = true;

        ApiTestBeginTime = ApiTestBeginTime.Replace("-", "").Replace("/", "");
        ApiTestEndTime = ApiTestEndTime.Replace("-", "").Replace("/", "");
        ApiConditionService.ApiTestChartNo = ApiTestChartNo;
        ApiConditionService.ApiTestBeginTime = ApiTestBeginTime;
        ApiConditionService.ApiTestEndTime = ApiTestEndTime;

        var apiResult = await NckuhApiService.GetMedicationAsync(ApiTestChartNo, ApiTestBeginTime, ApiTestEndTime);

        var apiResultJson = JsonConvert.SerializeObject(apiResult);

        IsApiCalling = false;

        data.Clear();
        foreach (var item in apiResult)
        {
            data.Add(item);
        }
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

}
