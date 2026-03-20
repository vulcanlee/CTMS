using CTMS.DataModel.Models;
using CTMS.DataModel.Models.ClinicalInformation;
using CTMS.DataModel.ViewModels;
using Microsoft.AspNetCore.Components;

namespace CTMS.Components.Views.Commons;

public partial class AddPatientDialog
{
    [Parameter]
    public bool OpenPicker { get; set; } = false;
    [Parameter]
    public EventCallback<AddNewPatientViewModel> OnConfirmCallback { get; set; }

    ConfirmBoxModel ConfirmMessageBox { get; set; } = new ConfirmBoxModel();
    MessageBoxModel MessageBox { get; set; } = new MessageBoxModel();

    string DialogTitle = "新增病患資料";
    public bool ShowMessageBox { get; set; } = false;
    public string MessageBoxBody { get; set; } = "";
    public string MessageBoxTitle { get; set; } = "";

    bool ShowAddPatientDialog = false;

    List<DropDownListDataModel> List院別 = new List<DropDownListDataModel>();
    DropDownListDataModel Select院別 = new DropDownListDataModel();
    DateTime Select收案日期 { get; set; } = DateTime.Now;

    protected override async Task OnInitializedAsync()
    {
        await GetData();
    }

    async Task GetData()
    {
        List院別 = DropDownListDataService.Get院別();
        Select院別 = List院別.FirstOrDefault();
    }

    async Task Init()
    {
        //patientData.臨床資料.CollectVisitCode(data);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            //Init();
        }
    }

    async Task OnPickerOK()
    {
        OpenPicker = false;
        await OnConfirmCallback.InvokeAsync(new()
        {
            院別 = Select院別.Key,
            Select收案日期 = Select收案日期
        });
    }

    async Task OnPickerCancel()
    {
        OpenPicker = false;
        await OnConfirmCallback.InvokeAsync(null);
    }
}
