using AntDesign;
using CTMS.AdapterModels;
using CTMS.DataModel.Models;
using CTMS.DataModel.Models.ClinicalInformation;
using CTMS.DataModel.ViewModels;
using CTMS.EntityModel.Models;
using CTMS.Share.Helpers;
using Syncfusion.Blazor.DropDowns;

namespace CTMS.Components.Views.Commons;

public partial class BrowseView
{
    HomePageModel HomePageModel = new();
    string imagefile = "";

    ConfirmBoxModel ConfirmMessageBox { get; set; } = new ConfirmBoxModel();
    MessageBoxModel MessageBox { get; set; } = new MessageBoxModel();
    string RoleMessage = string.Empty;

    bool ShowAddPatientDialog = false;

    List<Patient> patients = new();
    List<PatientAdapterModel> patientsAdapterModel = new();
    PatientData patientData = new();

    BrowseFilterCondition browseFilterCondition = new();
    string Keyword = string.Empty;
    int FoundCount = 0;

    #region 拉霸選單物件宣告
    List<DropDownListDataModel> List癌別 = new List<DropDownListDataModel>();
    DropDownListDataModel Select癌別 = new DropDownListDataModel();
    List<DropDownListDataModel> List院別 = new List<DropDownListDataModel>();
    DropDownListDataModel Select院別 = new DropDownListDataModel();
    List<DropDownListDataModel> List狀態 = new List<DropDownListDataModel>();
    DropDownListDataModel Select狀態 = new DropDownListDataModel();
    #endregion

    ITable table;
    int _pageIndex = 1;
    int _pageSize = 5;
    int _total = 0;

    bool isShowFilter = false;
    bool isAdmin = false;

    void On篩選條件()
    {
        isShowFilter = !isShowFilter;
    }

    void On儀表板()
    {
        NavigationManager.NavigateTo("/Dashboard");
    }

    async Task OnTableChange(AntDesign.TableModels.QueryModel<PatientAdapterModel> args)
    {
        BrowseSearchingService.BrowseSearchingModel.PageIndex = args.PageIndex;
        await ReloadAsync();
    }

    private async Task PageSizeChange(PaginationEventArgs args)
    {
        _pageSize = args.PageSize;
        await ReloadAsync();
    }

    protected override async Task OnInitializedAsync()
    {
        var checkResult = await AuthenticationStateHelper
        .Check(authStateProvider, NavigationManager);
        if (checkResult == true)
        {
            isAdmin = AuthenticationStateHelper.CheckIsAdmin();
        }

        Keyword = BrowseSearchingService.BrowseSearchingModel.SearchKeyword;
        foreach (var item in BrowseSearchingService.BrowseSearchingModel.院別)
        {
            browseFilterCondition.Items.Add(new BrowseFilterConditionNode()
            {
                Category = MagicObjectHelper.院別,
                Name = item,
            });
        }
        foreach (var item in BrowseSearchingService.BrowseSearchingModel.癌別)
        {
            browseFilterCondition.Items.Add(new BrowseFilterConditionNode()
            {
                Category = MagicObjectHelper.癌別,
                Name = item,
            });
        }
        foreach (var item in BrowseSearchingService.BrowseSearchingModel.狀態)
        {
            browseFilterCondition.Items.Add(new BrowseFilterConditionNode()
            {
                Category = MagicObjectHelper.狀態,
                Name = item,
            });
        }
        DropDownListDataInit();
    }

    async Task OnSetKeyword()
    {
        BrowseSearchingService.SetSearchKeyword(Keyword);
        await ReloadAsync();
    }

    async Task ReloadAsync()
    {
        DataRequestResult<PatientAdapterModel> dataRequestResult = await PatientService.GetAsync(BrowseSearchingService.BrowseSearchingModel);

        patientsAdapterModel = dataRequestResult.Result.ToList();
        _total = dataRequestResult.Count;

        foreach (var item in patientsAdapterModel)
        {
            patientData.FromJson(item.JsonData);
            item.SubjectNo = patientData.臨床資訊.SubjectNo; // Accessing PatientId property
            item.期別 = patientData.臨床資訊.FIGOStaging;
            item.Get組別DisplayName();
        }
        FoundCount = dataRequestResult.Count;
        StateHasChanged(); // 更新 UI
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
        }
    }

    async Task OnChangeStatusAsync(PatientAdapterModel patientAdapterModel)
    {
        bool confirmChange;

        patientData.FromJson(patientAdapterModel.JsonData);
        string subjectNo = patientData.臨床資訊.SubjectNo;
        string FIGO = patientData.臨床資訊.FIGOStaging;

        string stringFromTo = string.Empty;
        if (patientAdapterModel.狀態 == MagicObjectHelper.Patient狀態_收案)
        {
            stringFromTo = $"{MagicObjectHelper.Patient狀態_收案} 變更為 {MagicObjectHelper.Patient狀態_退出}";
        }
        else if (patientAdapterModel.狀態 == MagicObjectHelper.Patient狀態_退出)
        {
            stringFromTo = $"{MagicObjectHelper.Patient狀態_退出} 變更為 {MagicObjectHelper.Patient狀態_收案}";
        }

        if (patientAdapterModel.狀態 == MagicObjectHelper.Patient狀態_收案)
        {
            var task = ConfirmMessageBox.ShowAsync("400px", "200px",
     "變更受測者狀態", $"確定要變更受測者 {patientData.臨床資訊.SubjectNo} 的狀態從 {stringFromTo} 嗎？", ConfirmMessageBox.HiddenAsync);
            StateHasChanged(); // 更新 UI
            confirmChange = await task;
            if (confirmChange == true)
            {
                // 變更受測者狀態的邏輯
                await PatientService.ChangeStatusData(patientAdapterModel);

                #region 更新操作日誌
                MyUserAdapterModel myUserAdapterModel = await AuthenticationStateHelper
                .GetUserInformation(authStateProvider);

                await OperationHistoryTraceService.AddAsync(OperationHistoryTraceAdapterModel.Build(myUserAdapterModel.Name, subjectNo, MagicObjectHelper.OperationCategory變更狀態, MagicObjectHelper.OperationMessage受測者狀態變動, $"修改受測者狀態資料發生變動，Subject No={subjectNo} 狀態為 {MagicObjectHelper.Patient狀態_退出}"));
                #endregion

                await ReloadAsync();
            }
        }
        else if (patientAdapterModel.狀態 == MagicObjectHelper.Patient狀態_退出)
        {
            var task = ConfirmMessageBox.ShowAsync("400px", "200px",
"變更受測者狀態", $"確定要變更受測者 {patientData.臨床資訊.SubjectNo} 的狀態從 {stringFromTo} 嗎？", ConfirmMessageBox.HiddenAsync);
            StateHasChanged(); // 更新 UI
            confirmChange = await task;
            if (confirmChange == true)
            {
                // 變更受測者狀態的邏輯
                await PatientService.ChangeStatusData(patientAdapterModel);

                #region 更新操作日誌
                MyUserAdapterModel myUserAdapterModel = await AuthenticationStateHelper
                .GetUserInformation(authStateProvider);

                await OperationHistoryTraceService.AddAsync(OperationHistoryTraceAdapterModel.Build(myUserAdapterModel.Name, subjectNo, MagicObjectHelper.OperationCategory變更狀態, MagicObjectHelper.OperationMessage受測者狀態變動, $"修改受測者狀態資料發生變動，Subject No={subjectNo} 狀態為 {MagicObjectHelper.Patient狀態_收案}"));
                #endregion
                await ReloadAsync();
            }
        }
    }

    async Task OnEditAsync(PatientAdapterModel patientAdapterModel)
    {
        patientData.FromJson(patientAdapterModel.JsonData);
        var url = $"/BasicClinical/{patientAdapterModel.Code}";
        NavigationManager.NavigateTo(url);

    }

    async Task OnDeleteAsync(PatientAdapterModel patientAdapterModel)
    {

        patientData.FromJson(patientAdapterModel.JsonData);
        string subjectNo = patientData.臨床資訊.SubjectNo;
        string FIGO = patientData.臨床資訊.FIGOStaging;
        // 使用 ConfirmMessageBox 來確認刪除操作
        var task = ConfirmMessageBox.ShowAsync("400px", "200px",
        "刪除受測者資料", $"確定要刪除受測者 {patientData.臨床資訊.SubjectNo} 的資料嗎？",
        ConfirmMessageBox.HiddenAsync);
        StateHasChanged(); // 更新 UI
        var confirmDelete = await task;
        if (confirmDelete == true)
        {
            // 刪除受測者資料的邏輯
            var deleteResult = await PatientService.DeleteAsync(patientAdapterModel.Id);
            if (deleteResult.Success)
            {

                RandomParameterMode randomParameterModeAfter = new RandomParameterMode()
                {
                    SubjectNo = subjectNo,
                    FIGO = FIGO,
                };
                randomParameterModeAfter.Parse();
                await RandomListService.RemoveAsync(randomParameterModeAfter);


                // 刪除成功，顯示提示訊息
                // await MessageBox.ShowAsync("400px", "200px", "成功", "受測者資料已成功刪除。", MessageBox.HiddenAsync);
                await ReloadAsync();
            }
            else
            {
                // 刪除失敗，顯示錯誤訊息
                var task3 = MessageBox.ShowAsync("400px", "200px", "錯誤", deleteResult.Message, MessageBox.HiddenAsync);
                StateHasChanged(); // 更新 UI
                await task3;
            }
            StateHasChanged(); // 更新 UI
        }
        else
        {
            // 使用者取消刪除操作
        }
    }

    async Task OnAddAsync()
    {
        if (AuthenticationStateHelper.CheckShowPage(MagicObjectHelper.ROLE新增病患, ConfirmMessageBox) == false)
            return;

        ShowAddPatientDialog = true;
    }

    async Task OnAddPatientAsync(AddNewPatientViewModel addNewPatientViewModel)
    {
        if (addNewPatientViewModel.院別 != null)
        {
            await AddNewPatientAsync(addNewPatientViewModel);
        }
        else
        {
            ShowAddPatientDialog = false;
            StateHasChanged(); // 更新 UI
            await Task.Delay(100); // 等待對話框關閉動畫完成
                                   // 使用者取消新增操作
        }
    }

    async Task AddNewPatientAsync(AddNewPatientViewModel addNewPatientViewModel)
    {
        // 呼叫新增受測者的服務方法
        (var result, PatientAdapterModel patientAdapterModel, string newSubjectNo) = await PatientService.AddEmptyAsync(addNewPatientViewModel);
        if (result.Success)
        {
            ShowAddPatientDialog = false;
            StateHasChanged(); // 更新 UI
            await Task.Delay(100); // 等待對話框關閉動畫完成
                                   // 新增成功，顯示提示訊息

            #region 更新操作日誌
            MyUserAdapterModel myUserAdapterModel = await AuthenticationStateHelper
            .GetUserInformation(authStateProvider);

            await OperationHistoryTraceService.AddAsync(OperationHistoryTraceAdapterModel.Build(myUserAdapterModel.Name, newSubjectNo, MagicObjectHelper.OperationCategory建立紀錄, MagicObjectHelper.OperationMessage建立一筆受測者資料, $"新增受測者 {patientAdapterModel.醫院} 的 Subject Code : {newSubjectNo} "));
            #endregion

            MessageBox.Show("400px", "200px", "成功", "受測者資料已成功新增。", MessageBox.HiddenAsync);
            await ReloadAsync();
        }
        else
        {
            // 新增失敗，顯示錯誤訊息
            var task1 = MessageBox.ShowAsync("400px", "200px", "錯誤", result.Message, MessageBox.HiddenAsync);
            StateHasChanged(); // 更新 UI
            await task1;
        }
    }


    async Task OnRefreshAsync()
    {
        await ReloadAsync();
    }

    void DropDownListDataInit()
    {
        #region 癌別
        {
            List癌別 = DropDownListDataService.Get癌別();
        }
        #endregion

        #region 院別
        {
            List院別 = DropDownListDataService.Get院別();
        }
        #endregion

        #region 狀態
        {
            List狀態 = DropDownListDataService.Get狀態();
        }
        #endregion

    }

    async Task RemoveFilterAsync(BrowseFilterConditionNode item)
    {
        if (item == null) return;
        browseFilterCondition.Items.Remove(item);
        if (item.Category == MagicObjectHelper.院別)
            BrowseSearchingService.RemoveHospital(item.Name);
        else if (item.Category == MagicObjectHelper.癌別)
            BrowseSearchingService.RemoveCancerType(item.Name);
        else if (item.Category == MagicObjectHelper.狀態)
            BrowseSearchingService.RemoveStatus(item.Name);
        await ReloadAsync();
    }

    async Task RemovellFilterAsync()
    {
        BrowseSearchingService.Reset();
        browseFilterCondition.Items.Clear();
        BrowseSearchingService.Reset();
        await ReloadAsync();
    }

    async Task On院別Changed(ChangeEventArgs<DropDownListDataModel, DropDownListDataModel> args)
    {
        if (args.Value == null) return;
        // patientData.臨床資訊.ECorOC = args.Value.Key;

        var item = browseFilterCondition.Items.FirstOrDefault(i => i.Name == args.Value.Name);
        if (item == null)
        {
            browseFilterCondition.Items.Add(new BrowseFilterConditionNode
            {
                Category = MagicObjectHelper.院別,
                Name = args.Value.Name
            });
            BrowseSearchingService.AddHospital(args.Value.Name);
        }

        await ReloadAsync();
        StateHasChanged();
    }

    async Task On癌別Changed(ChangeEventArgs<DropDownListDataModel, DropDownListDataModel> args)
    {
        if (args.Value == null) return;

        var item = browseFilterCondition.Items.FirstOrDefault(i => i.Name == args.Value.Name);
        if (item == null)
        {
            browseFilterCondition.Items.Add(new BrowseFilterConditionNode
            {
                Category = MagicObjectHelper.癌別,
                Name = args.Value.Name
            });
            BrowseSearchingService.AddCancerType(args.Value.Name);
        }

        await ReloadAsync();
    }

    async Task On狀態Changed(ChangeEventArgs<DropDownListDataModel, DropDownListDataModel> args)
    {
        if (args.Value == null) return;

        var item = browseFilterCondition.Items.FirstOrDefault(i => i.Name == args.Value.Name);
        if (item == null)
        {
            browseFilterCondition.Items.Add(new BrowseFilterConditionNode
            {
                Category = MagicObjectHelper.狀態,
                Name = args.Value.Name
            });
            BrowseSearchingService.AddStatus(args.Value.Name);
        }

        await ReloadAsync();
    }
}
