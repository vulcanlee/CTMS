using CTMS.AdapterModels;
using CTMS.Business.Services.ClinicalInformation;
using CTMS.DataModel.Models;
using CTMS.DataModel.Models.ClinicalInformation;
using CTMS.Helper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Syncfusion.Blazor.DropDowns;

namespace CTMS.Components.Views.ClinicalInformation;

public partial class Survey2SideEffectsView
{
    [Parameter]
    public string Code { get; set; }
    [Inject]
    public SurveySideEffectsService SurveySideEffectsService { get; set; }

    PatientData patientData = new();
    PatientAdapterModel patientAdapterModel = new();
    bool editMode = false;
    Survey2SideEffects副作用Node data = new();
    Survey2SideEffects副作用 header = new();
    Main臨床資料 Main臨床資料 = null;

    List<DropDownListDataModel> ListCardiovascularIncludeHtnYesNo = new List<DropDownListDataModel>();
    List<string> ListYesNo = new List<string>();

    #region 操作 Visit Code 用到的物件
    bool ShowVisitCodeDialog = false;
    ConfirmBoxModel ConfirmMessageBox { get; set; } = new ConfirmBoxModel();
    MessageBoxModel MessageBox { get; set; } = new MessageBoxModel();
    string VisitCodeOperateMode = string.Empty;
    VisitCodeModel VisitCode = new();
    SfDropDownList<DropDownListDataModel, DropDownListDataModel> VisitCodeDropDown;
    DropDownListDataModel SelectVisitCode = new DropDownListDataModel();
    List<DropDownListDataModel> ListVisitCode = new List<DropDownListDataModel>();
    #endregion

    protected override async Task OnInitializedAsync()
    {
        patientAdapterModel = await PatientService.GetAsync(Code);
        patientData.FromJson(patientAdapterModel.JsonData);
        InitData(true);
    }

    void InitData(bool isFirst = true)
    {
        header = patientData.臨床資料.SurveySideEffects副作用2;
        Main臨床資料 = patientData.臨床資料;
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

    #region 針對 VisitCode 的方法
    private async Task OnVisitCodeChanged(ChangeEventArgs<DropDownListDataModel, DropDownListDataModel> args)
    {
        if (args.Value != null)
        {
            var item = header.Items.FirstOrDefault(x => x.VisitCode.Id == args.Value.Key);
            if (item != null)
            {
                data = item;
                SelectVisitCode = args.Value;
            }
            else
            {
                SelectVisitCode = null;
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

    async Task OnRefreshAsync()
    {
        // SurveySideEffectsService
        if (SelectVisitCode == null || Main臨床資料 == null) return;

        var item = Main臨床資料.SurveySideEffects副作用2.Items.FirstOrDefault(x => x.VisitCode.Id == SelectVisitCode.Key);

        SurveySideEffectsService.Update副作用2All(Main臨床資料, item);

        patientAdapterModel.JsonData = patientData.ToJson();
        await PatientService.UpdateAsync(patientAdapterModel);

        InitData();
    }
}
