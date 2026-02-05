using CTMS.AdapterModels;
using CTMS.DataModel.Models;
using CTMS.DataModel.Models.ClinicalInformation;
using CTMS.DataModel.Models.Systems;
using CTMS.Helper;
using CTMS.Share.Helpers;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Syncfusion.Blazor.DropDowns;

namespace CTMS.Components.Views.ClinicalInformation;

public partial class ChemotherapySideEffectsSurveyView
{
    [Parameter]
    public string Code { get; set; }
    [Parameter]
    public EventCallback<SurveyTabDisplayModel> SurveyTabDisplayModelChanged { get; set; }

    PatientData patientData = new();
    PatientAdapterModel patientAdapterModel = new();
    bool editMode = false;

    Survey化療副作用Node data = new();
    Survey化療副作用 header = new();

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

    string sourceObjectJson = "{}";
    string targetObjectJson = "{}";

    SurveyTabDisplayModel surveyTabDisplayModel;

    protected override async Task OnInitializedAsync()
    {
        surveyTabDisplayModel = SurveyTabDisplayHelper.Build();
        patientAdapterModel = await PatientService.GetAsync(Code);
        patientData.FromJson(patientAdapterModel.JsonData);

        await InitDataAsync(true);
    }

    async Task InitDataAsync(bool isFirst = true)
    {
        header = patientData.臨床資料.Survey化療副作用;
        if (isFirst)
        {
            sourceObjectJson = JsonConvert.SerializeObject(patientData.臨床資料.Survey化療副作用);

            data = header.Items.FirstOrDefault();

            if (data != null)
            {
                SurveyService.Reset(data.Questions);
                foreach (var item in data.Questions)
                {
                    SurveyService.RefreshByQuestionChanged(data.Questions, item);
                }
            }

            ListVisitCode = VisitCodeCollectionHelper.GetAll問卷VisitCodes(patientData.臨床資料);
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

        SurveyTabDisplayHelper.Get(patientData, SelectVisitCode, surveyTabDisplayModel);
        await SurveyTabDisplayModelChanged.InvokeAsync(surveyTabDisplayModel);
    }

    void OnChangeEditMode()
    {
        editMode = !editMode;
    }

    async Task OnSaveAsync()
    {
        // BloodExameService.CheckBloodExame(data);
        // data = patientData.臨床資料.問卷.化療副作用;
        patientAdapterModel.JsonData = patientData.ToJson();
        await PatientService.UpdateAsync(patientAdapterModel);

        targetObjectJson = JsonConvert.SerializeObject(patientData.臨床資料.Survey化療副作用);

        #region 更新操作日誌
        MyUserAdapterModel myUserAdapterModel = await AuthenticationStateHelper
        .GetUserInformation(authStateProvider);

        await OperationHistoryTraceService.AddAsync(OperationHistoryTraceAdapterModel.Build(myUserAdapterModel.Name, patientData.臨床資訊.SubjectNo, MagicObjectHelper.OperationCategory問卷_化療副作用, "-", "-"), sourceObjectJson, targetObjectJson, MagicObjectHelper.OperationCategory問卷_化療副作用);
        #endregion

        await InitDataAsync(false);
        editMode = false;

        MessageBox.Show("400px", "200px", "資訊", "儲存成功", MessageBox.HiddenAsync);
    }

    async Task OnCancel()
    {
        patientData.FromJson(patientAdapterModel.JsonData);
        editMode = false;
        await InitDataAsync(false);
    }

    void OnOptionChange(Question question, List<Option> options, Option option)
    {
        question.Answer = "";
        foreach (var item in options)
        {
            item.CheckBoxIcon = MagicObjectHelper.CheckBoxBlankIcon;
        }

        option.CheckBoxIcon = MagicObjectHelper.CheckBoxIcon;
        question.Answer = option.Value.ToString();

        SurveyService.RefreshByQuestionChanged(data.Questions, question);
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
                await InitDataAsync(false);
            }
        }
    }

    void RefreshVisitCode(bool reset = true)
    {
        #region VisitCode
        {
            // ListVisitCode.Clear();
            // if (data != null)
            // {

            // }
            // foreach (var nodeItem in header.Items)
            // {
            //     ListVisitCode.Add(new DropDownListDataModel()
            //     {
            //         Key = nodeItem.VisitCode.Id,
            //         Name = nodeItem.VisitCode.VisitCodeTitle,
            //     });
            // }
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
