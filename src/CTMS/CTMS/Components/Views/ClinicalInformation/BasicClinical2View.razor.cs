using CTMS.AdapterModels;
using CTMS.DataModel.Models;
using CTMS.DataModel.Models.AIAgent;
using CTMS.DataModel.Models.ClinicalInformation;
using CTMS.EntityModel.Models;
using CTMS.Share.Extensions;
using CTMS.Share.Helpers;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Syncfusion.Blazor.DropDowns;

namespace CTMS.Components.Views.ClinicalInformation;

public partial class BasicClinical2View
{
    [Parameter]
    public string Code { get; set; }

    PatientData patientData = new();
    PatientData beforePatientData = new();
    PatientAdapterModel patientAdapterModel = new();
    bool editMode = false;

    bool ShowMMRProteinSettingDialog = false;
    bool ShowVisitCodeDialog = false;
    bool ShowUploadDicomDialog = false;

    ConfirmBoxModel ConfirmMessageBox { get; set; } = new ConfirmBoxModel();
    UploadDicomModel UploadDicomModel { get; set; } = new UploadDicomModel();

    MessageBoxModel MessageBox { get; set; } = new MessageBoxModel();

    VisitCodeSetModel VisitCodeSetModel = new();

    string SubjectNo = "";
    string image = "";
    string imageVersion = DateTime.Now.Ticks.ToString();
    bool showNotyfyDicomDialog = false;

    string sourceObjectJson = "{}";
    string targetObjectJson = "{}";
    string source臨床資訊ObjectJson = "{}";
    string target臨床資訊ObjectJson = "{}";

    async Task OnNotifyDicomDialog(NotifyDicomResponse notifyDicomResponse)
    {
        showNotyfyDicomDialog = false;
        if (notifyDicomResponse == null)
        {
            return;
        }
        if (string.IsNullOrEmpty(notifyDicomResponse.單號))
        {
            return;
        }

        await SendEmailService.SendNotifyEmailAsync(notifyDicomResponse.單號, notifyDicomResponse.病歷號, NavigationManager.Uri);
    }

    CancellationToken token = new CancellationToken();
    CancellationTokenSource cts = new CancellationTokenSource();
    Task checkTask;
    protected override async Task OnInitializedAsync()
    {
    }
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await GetDataAsync();
            cts = new CancellationTokenSource();
            token = cts.Token;

            // if (patientAdapterModel.AI處理 == MagicObjectHelper.AI處理處理中)
            // if (patientAdapterModel.AI處理 == MagicObjectHelper.NA)
            {
                try
                {
                    var checkCompleted = false;
                    checkTask = Task.Run(async () =>
                    {
                        PatientAdapterModel taskPatientAdapterModel = new();
                        PatientData taskPatientData = new();

                        while (!token.IsCancellationRequested)
                        {
                            await Task.Delay(1000, token);

                            if (editMode)
                                continue;

                            #region GetData
                            CancerStageService.ReadAll();
                            taskPatientAdapterModel = await PatientService.GetAsync(Code);
                            taskPatientData.FromJson(taskPatientAdapterModel.JsonData);
                            #endregion
                            //await GetData();

                            if (taskPatientAdapterModel.AI處理 != MagicObjectHelper.AI處理處理中)
                                continue;

                            bool isCompletion = await AIIntegrateService.CheckAIProcess(taskPatientData.臨床資訊.KeyName);
                            if (isCompletion)
                            {
                                checkCompleted = true;
                                await InvokeAsync(async () =>
                                {
                                    var task = ShowAICompletionAsync();
                                    StateHasChanged();
                                    await Task.Delay(100);
                                    await task;
                                });
                            }
                        }

                        if (checkCompleted)
                        {
                        }
                    }, token);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            StateHasChanged();
        }
    }

    async Task GetDataAsync()
    {
        CancerStageService.ReadAll();
        patientAdapterModel = await PatientService.GetAsync(Code);
        patientData.FromJson(patientAdapterModel.JsonData);
        patientData.臨床資訊.CalculateCancerType();
        DropDownListDataInit();
        patientData.臨床資訊.BuildStage();

        SubjectNo = patientData.臨床資訊.SubjectNo;
        image = Path.Combine(MagicObjectHelper.UploadFinalPath, patientData.臨床資訊.ImagePng);
    }

    #region 拉霸選單物件宣告
    List<DropDownListDataModel> ListECOrOCC = new List<DropDownListDataModel>();
    DropDownListDataModel SelectECOrOCC = new DropDownListDataModel();
    List<DropDownListDataModel> List年齡Age = new List<DropDownListDataModel>();
    DropDownListDataModel Select年齡Age = new DropDownListDataModel();
    List<DropDownListDataModel> List月經狀態 = new List<DropDownListDataModel>();
    DropDownListDataModel Select月經狀態 = new DropDownListDataModel();
    List<DropDownListDataModel> List身高Height = new List<DropDownListDataModel>();
    DropDownListDataModel Select身高Height = new DropDownListDataModel();
    List<DropDownListDataModel> List體重BW = new List<DropDownListDataModel>();
    DropDownListDataModel Select體重BW = new DropDownListDataModel();
    List<DropDownListDataModel> List日常體能狀態PS = new List<DropDownListDataModel>();
    DropDownListDataModel Select日常體能狀態PS = new DropDownListDataModel();
    List<DropDownListDataModel> ListFIGO癌症分期 = new List<DropDownListDataModel>();
    DropDownListDataModel SelectFIGO癌症分期 = new DropDownListDataModel();
    List<DropDownListDataModel> ListAJCC_C_Stage = new List<DropDownListDataModel>();
    DropDownListDataModel SelectAJCC_C_Stage = new DropDownListDataModel();
    List<DropDownListDataModel> ListAJCC_C_StageT = new List<DropDownListDataModel>();
    DropDownListDataModel SelectAJCC_C_StageT = new DropDownListDataModel();
    List<DropDownListDataModel> ListAJCC_C_StageN = new List<DropDownListDataModel>();
    DropDownListDataModel SelectAJCC_C_StageN = new DropDownListDataModel();
    List<DropDownListDataModel> ListAJCC_C_StageM = new List<DropDownListDataModel>();
    DropDownListDataModel SelectAJCC_C_StageM = new DropDownListDataModel();
    List<DropDownListDataModel> ListAJCC_P_Stage = new List<DropDownListDataModel>();
    DropDownListDataModel SelectAJCC_P_Stage = new DropDownListDataModel();
    List<DropDownListDataModel> ListAJCC_P_StageT = new List<DropDownListDataModel>();
    DropDownListDataModel SelectAJCC_P_StageT = new DropDownListDataModel();
    List<DropDownListDataModel> ListAJCC_P_StageN = new List<DropDownListDataModel>();
    DropDownListDataModel SelectAJCC_P_StageN = new DropDownListDataModel();
    List<DropDownListDataModel> ListAJCC_P_StageM = new List<DropDownListDataModel>();
    DropDownListDataModel SelectAJCC_P_StageM = new DropDownListDataModel();
    List<DropDownListDataModel> ListMMRProtein = new List<DropDownListDataModel>();
    DropDownListDataModel SelectMMRProtein = new DropDownListDataModel();
    List<DropDownListDataModel> ListMMRProteinDetail = new List<DropDownListDataModel>();
    DropDownListDataModel SelectMMRProteinDetail = new DropDownListDataModel();
    List<DropDownListDataModel> ListP53 = new List<DropDownListDataModel>();
    DropDownListDataModel SelectP53 = new DropDownListDataModel();
    List<DropDownListDataModel> ListHormonStatus = new List<DropDownListDataModel>();
    DropDownListDataModel SelectHormonStatus = new DropDownListDataModel();
    List<DropDownListDataModel> ListHormonStatusER = new List<DropDownListDataModel>();
    DropDownListDataModel SelectHormonStatusER = new DropDownListDataModel();
    List<DropDownListDataModel> ListHormonStatusPR = new List<DropDownListDataModel>();
    DropDownListDataModel SelectHormonStatusPR = new DropDownListDataModel();
    List<DropDownListDataModel> List組織型態 = new List<DropDownListDataModel>();
    DropDownListDataModel Select組織型態 = new DropDownListDataModel();
    List<DropDownListDataModel> List組織型態Detail = new List<DropDownListDataModel>();
    DropDownListDataModel Select組織型態Detail = new DropDownListDataModel();
    #endregion

    void DropDownListDataInit()
    {
        #region 癌別
        {
            ListECOrOCC = DropDownListDataService.Get癌別Code();
            var item = ListECOrOCC.FirstOrDefault(x => x.Key == patientData.臨床資訊.ECorOC);
            if (item != null)
                SelectECOrOCC = item;
        }
        #endregion

        #region 年齡Age
        {
            List年齡Age = DropDownListDataService.GetAge();
            var item = List年齡Age.FirstOrDefault(x => x.Key == patientData.臨床資訊.Age);
            if (item != null)
                Select年齡Age = item;
        }
        #endregion

        #region 月經狀態
        {
            List月經狀態 = DropDownListDataService.Get月經狀態();
            var item = List月經狀態.FirstOrDefault(x => x.Key == patientData.臨床資訊.MenstrualStatus);
            if (item != null)
                Select月經狀態 = item;
        }
        #endregion

        #region 身高Height
        {
            List身高Height = DropDownListDataService.Get身高Height();
            var item = List身高Height.FirstOrDefault(x => x.Key == patientData.臨床資訊.Height);
            if (item != null)
                Select身高Height = item;
        }
        #endregion

        #region 體重BW
        {
            List體重BW = DropDownListDataService.Get體重BW();
            var item = List體重BW.FirstOrDefault(x => x.Key == patientData.臨床資訊.Weight);
            if (item != null)
                Select體重BW = item;
        }
        #endregion

        #region 日常體能狀態PS
        {
            List日常體能狀態PS = DropDownListDataService.Get日常體能狀態PS();
            var item = List日常體能狀態PS.FirstOrDefault(x => x.Key == patientData.臨床資訊.PerformanceStatus);
            if (item != null)
                Select日常體能狀態PS = item;
        }
        #endregion

        #region FIGO癌症分期
        {
            RefreshFIGO癌症分期();
        }
        #endregion

        #region FIGOListAJCC_C_Stage
        {
            ListAJCC_C_Stage = DropDownListDataService.GetAJCC_CP_Stage();
            var item = ListAJCC_C_Stage.FirstOrDefault(x =>
                x.Key == patientData.臨床資訊.AJCCClinicalStage);
            if (item != null)
                SelectAJCC_C_Stage = item;
        }
        #endregion

        #region FIGOListAJCC_C_StageT
        {
            ListAJCC_C_StageT = DropDownListDataService.GetAJCC_CP_StageT();
            var item = ListAJCC_C_StageT.FirstOrDefault(x =>
                x.Key == patientData.臨床資訊.AJCCClinicalStageT);
            if (item != null)
                SelectAJCC_C_StageT = item;
        }
        #endregion

        #region FIGOListAJCC_C_StageN
        {
            ListAJCC_C_StageN = DropDownListDataService.GetAJCC_CP_StageN();
            var item = ListAJCC_C_StageN.FirstOrDefault(x =>
                x.Key == patientData.臨床資訊.AJCCClinicalStageN);
            if (item != null)
                SelectAJCC_C_StageN = item;
        }
        #endregion

        #region FIGOListAJCC_C_StageM
        {
            ListAJCC_C_StageM = DropDownListDataService.GetAJCC_CP_StageM();
            var item = ListAJCC_C_StageM.FirstOrDefault(x =>
                x.Key == patientData.臨床資訊.AJCCClinicalStageM);
            if (item != null)
                SelectAJCC_C_StageM = item;
        }
        #endregion

        #region FIGOListAJCC_C_Stage
        {
            ListAJCC_P_Stage = DropDownListDataService.GetAJCC_CP_Stage();
            var item = ListAJCC_P_Stage.FirstOrDefault(x =>
                x.Key == patientData.臨床資訊.AJCCPathologicalStage);
            if (item != null)
                SelectAJCC_P_Stage = item;
        }
        #endregion

        #region FIGOListAJCC_C_StageT
        {
            ListAJCC_P_StageT = DropDownListDataService.GetAJCC_CP_StageT();
            var item = ListAJCC_P_StageT.FirstOrDefault(x =>
                x.Key == patientData.臨床資訊.AJCCPathologicalStageT);
            if (item != null)
                SelectAJCC_P_StageT = item;
        }
        #endregion

        #region FIGOListAJCC_C_StageN
        {
            ListAJCC_P_StageN = DropDownListDataService.GetAJCC_CP_StageN();
            var item = ListAJCC_P_StageN.FirstOrDefault(x =>
                x.Key == patientData.臨床資訊.AJCCPathologicalStageN);
            if (item != null)
                SelectAJCC_P_StageN = item;
        }
        #endregion

        #region FIGOListAJCC_C_StageM
        {
            ListAJCC_P_StageM = DropDownListDataService.GetAJCC_CP_StageM();
            var item = ListAJCC_P_StageM.FirstOrDefault(x =>
                x.Key == patientData.臨床資訊.AJCCPathologicalStageM);
            if (item != null)
                SelectAJCC_P_StageM = item;
        }
        #endregion

        #region MMRProtein
        {
            ListMMRProtein = DropDownListDataService.GetMMRProtein();
            var item = ListMMRProtein.FirstOrDefault(x =>
                x.Key == patientData.臨床資訊.MMRProtein);
            if (item != null)
            {
                SelectMMRProtein = item;
                RefreshMMRProteinDetail();
            }
        }
        #endregion

        #region P53
        {
            ListP53 = DropDownListDataService.GetP53();
            var item = ListP53.FirstOrDefault(x =>
                x.Key == patientData.臨床資訊.P53);
            if (item != null)
                SelectP53 = item;
        }
        #endregion

        #region HormonStatus
        {
            ListHormonStatusER = DropDownListDataService.GetHormonStatusER();
            var item = ListHormonStatusER.FirstOrDefault(x =>
                x.Key == patientData.臨床資訊.HormonStatusER);
            if (item != null)
                SelectHormonStatusER = item;
            ListHormonStatusPR = DropDownListDataService.GetHormonStatusPR();
            var itemPR = ListHormonStatusPR.FirstOrDefault(x =>
                x.Key == patientData.臨床資訊.HormonStatusPR);
            if (itemPR != null)
                SelectHormonStatusPR = itemPR;
        }

        InitList組織型態();
        #endregion
    }

    void InitList組織型態()

    #region 組織型態
    {
        List組織型態Detail?.Clear();
        List組織型態 = DropDownListDataService.Get組織型態();

        if (patientData.臨床資訊.ECorOC == MagicObjectHelper.EC)
        {
            var item = List組織型態.FirstOrDefault(x =>
                x.Key == patientData.臨床資訊.HistologicalType);
            if (item != null)
            {
                Select組織型態 = item;

                Refresh組織型態Detail();
            }
        }
        else
        {
            var item = List組織型態.FirstOrDefault(x =>
                x.Key == MagicObjectHelper.TypeII);

            if (item != null)
            {
                patientData.臨床資訊.HistologicalType = item.Key;
                Select組織型態 = item;

                Refresh組織型態Detail();
            }
        }
    }
    #endregion

    #region 下拉選單的事件

    void RefreshFIGO癌症分期()
    {
        if (patientData.臨床資訊.ECorOC == "EC")
        {
            ListFIGO癌症分期 = DropDownListDataService.GetFIGO癌症分期子宮內膜癌EndometrialCancer();
            var item = ListFIGO癌症分期.FirstOrDefault(x =>
                x.Key == patientData.臨床資訊.FIGOStaging);
            if (item != null)
                SelectFIGO癌症分期 = item;
        }
        else if (patientData.臨床資訊.ECorOC == "OC")
        {
            ListFIGO癌症分期 = DropDownListDataService.GetFIGO癌症分期卵巢癌OvarianCancer();
            var item = ListFIGO癌症分期.FirstOrDefault(x =>
                x.Key == patientData.臨床資訊.FIGOStaging);
            if (item != null)
                SelectFIGO癌症分期 = item;
        }
    }

    void Refresh組織型態Detail()
    {
        if (Select組織型態 == null) return;
        List組織型態Detail = DropDownListDataService.Get組織型態Detail(Select組織型態.Key, patientData.臨床資訊.ECorOC);
        var itemDetail = List組織型態Detail.FirstOrDefault(x =>
            x.Key == patientData.臨床資訊.HistologicalTypeDetail);
        if (itemDetail != null)
        {
            Select組織型態Detail = itemDetail;
        }
    }

    void RefreshMMRProteinDetail()
    {
        // if (SelectMMRProtein == null) return;
        ListMMRProteinDetail = DropDownListDataService.GetMMRProteinDetail();
        var itemDetail = ListMMRProteinDetail.FirstOrDefault(x =>
            x.Key == patientData.臨床資訊.MMRProteinDetail);
        if (itemDetail != null)
        {
            SelectMMRProteinDetail = itemDetail;
        }
    }

    private async Task OnFIGO癌症分期Changed(ChangeEventArgs<DropDownListDataModel, DropDownListDataModel> args)
    {
        if (args.Value == null) return;
        patientData.臨床資訊.FIGOStaging = args.Value.Key;
    }

    private async Task On組織型態Changed(ChangeEventArgs<DropDownListDataModel, DropDownListDataModel> args)
    {
        if (args.Value == null) return;
        patientData.臨床資訊.HistologicalType = args.Value.Key;
        Refresh組織型態Detail();
    }

    private async Task On組織型態DetailChanged(ChangeEventArgs<DropDownListDataModel, DropDownListDataModel> args)
    {
        if (args.Value == null) return;
        patientData.臨床資訊.HistologicalTypeDetail = args.Value.Key;
    }

    private async Task OnHormonStatusChanged(ChangeEventArgs<DropDownListDataModel, DropDownListDataModel> args)
    {
        if (args.Value == null) return;
        patientData.臨床資訊.HormonStatus = args.Value.Key;
    }

    private async Task OnHormonStatusERChanged(ChangeEventArgs<DropDownListDataModel, DropDownListDataModel> args)
    {
        if (args.Value == null) return;
        patientData.臨床資訊.HormonStatusER = args.Value.Key;
    }

    private async Task OnHormonStatusPRChanged(ChangeEventArgs<DropDownListDataModel, DropDownListDataModel> args)
    {
        if (args.Value == null) return;
        patientData.臨床資訊.HormonStatusPR = args.Value.Key;
    }

    private async Task OnP53Changed(ChangeEventArgs<DropDownListDataModel, DropDownListDataModel> args)
    {
        if (args.Value == null) return;
        patientData.臨床資訊.P53 = args.Value.Key;
    }

    private async Task OnMMRProteinChanged(ChangeEventArgs<DropDownListDataModel, DropDownListDataModel> args)
    {
        if (args.Value == null) return;
        patientData.臨床資訊.MMRProtein = args.Value.Key;
        RefreshMMRProteinDetail();
    }

    private async Task OnMMRProteinDetailChanged(ChangeEventArgs<DropDownListDataModel, DropDownListDataModel> args)
    {
        if (args.Value == null) return;
        patientData.臨床資訊.MMRProteinDetail = args.Value.Key;
    }

    private async Task OnAJCC_P_StageChanged(ChangeEventArgs<DropDownListDataModel, DropDownListDataModel> args)
    {
        if (args.Value == null) return;
        patientData.臨床資訊.AJCCPathologicalStage = args.Value.Key;
    }

    private async Task OnAJCC_P_StageTChanged(ChangeEventArgs<DropDownListDataModel, DropDownListDataModel> args)
    {
        if (args.Value == null) return;
        patientData.臨床資訊.AJCCPathologicalStageT = args.Value.Key;
        patientData.臨床資訊.BuildStage();
        patientData.臨床資訊.AJCCPathologicalStage = CancerStageService
    .GetStageNames(patientData.臨床資訊.ECorOC, MagicObjectHelper.StageP,
        patientData.臨床資訊.AJCCPathologicalStageT,
        patientData.臨床資訊.AJCCPathologicalStageN,
        patientData.臨床資訊.AJCCPathologicalStageM);
    }

    private async Task OnAJCC_P_StageNChanged(ChangeEventArgs<DropDownListDataModel, DropDownListDataModel> args)
    {
        if (args.Value == null) return;
        patientData.臨床資訊.AJCCPathologicalStageN = args.Value.Key;
        patientData.臨床資訊.BuildStage();
        patientData.臨床資訊.AJCCPathologicalStage = CancerStageService
        .GetStageNames(patientData.臨床資訊.ECorOC, MagicObjectHelper.StageP,
            patientData.臨床資訊.AJCCPathologicalStageT,
            patientData.臨床資訊.AJCCPathologicalStageN,
            patientData.臨床資訊.AJCCPathologicalStageM);
    }

    private async Task OnAJCC_P_StageMChanged(ChangeEventArgs<DropDownListDataModel, DropDownListDataModel> args)
    {
        if (args.Value == null) return;
        patientData.臨床資訊.AJCCPathologicalStageM = args.Value.Key;
        patientData.臨床資訊.BuildStage();
        patientData.臨床資訊.AJCCPathologicalStage = CancerStageService
    .GetStageNames(patientData.臨床資訊.ECorOC, MagicObjectHelper.StageP,
        patientData.臨床資訊.AJCCPathologicalStageT,
        patientData.臨床資訊.AJCCPathologicalStageN,
        patientData.臨床資訊.AJCCPathologicalStageM);
    }

    private async Task OnAJCC_C_StageChanged(ChangeEventArgs<DropDownListDataModel, DropDownListDataModel> args)
    {
        if (args.Value == null) return;
        patientData.臨床資訊.AJCCClinicalStage = args.Value.Key;
    }

    private async Task OnAJCC_C_StageTChanged(ChangeEventArgs<DropDownListDataModel, DropDownListDataModel> args)
    {
        if (args.Value == null) return;
        patientData.臨床資訊.AJCCClinicalStageT = args.Value.Key;
        patientData.臨床資訊.BuildStage();
        patientData.臨床資訊.AJCCClinicalStage =
        CancerStageService.GetStageNames(patientData.臨床資訊.ECorOC, MagicObjectHelper.StageC,
            patientData.臨床資訊.AJCCClinicalStageT,
            patientData.臨床資訊.AJCCClinicalStageN,
            patientData.臨床資訊.AJCCClinicalStageM);
    }

    private async Task OnAJCC_C_StageNChanged(ChangeEventArgs<DropDownListDataModel, DropDownListDataModel> args)
    {
        if (args.Value == null) return;
        patientData.臨床資訊.AJCCClinicalStageN = args.Value.Key;
        patientData.臨床資訊.BuildStage();
        patientData.臨床資訊.AJCCClinicalStage =
    CancerStageService.GetStageNames(patientData.臨床資訊.ECorOC, MagicObjectHelper.StageC,
        patientData.臨床資訊.AJCCClinicalStageT,
        patientData.臨床資訊.AJCCClinicalStageN,
        patientData.臨床資訊.AJCCClinicalStageM);
    }

    private async Task OnAJCC_C_StageMChanged(ChangeEventArgs<DropDownListDataModel, DropDownListDataModel> args)
    {
        if (args.Value == null) return;
        patientData.臨床資訊.AJCCClinicalStageM = args.Value.Key;
        patientData.臨床資訊.BuildStage();
        patientData.臨床資訊.AJCCClinicalStage =
    CancerStageService.GetStageNames(patientData.臨床資訊.ECorOC, MagicObjectHelper.StageC,
        patientData.臨床資訊.AJCCClinicalStageT,
        patientData.臨床資訊.AJCCClinicalStageN,
        patientData.臨床資訊.AJCCClinicalStageM);
    }

    private async Task On日常體能狀態PSChanged(ChangeEventArgs<DropDownListDataModel, DropDownListDataModel> args)
    {
        if (args.Value == null) return;
        patientData.臨床資訊.PerformanceStatus = args.Value.Key;
    }

    private async Task On體重BWChanged(ChangeEventArgs<DropDownListDataModel, DropDownListDataModel> args)
    {
        if (args.Value == null) return;
        patientData.臨床資訊.Weight = args.Value.Key;
        patientData.臨床資訊.CalculateBMIAndBSA();
    }

    private async Task On身高HeightChanged(ChangeEventArgs<DropDownListDataModel, DropDownListDataModel> args)
    {
        if (args.Value == null) return;
        patientData.臨床資訊.Height = args.Value.Key;
        patientData.臨床資訊.CalculateBMIAndBSA();
    }

    private async Task On月經狀態Changed(ChangeEventArgs<DropDownListDataModel, DropDownListDataModel> args)
    {
        if (args.Value == null) return;
        patientData.臨床資訊.MenstrualStatus = args.Value.Key;
    }

    private async Task On年齡AgeChanged(ChangeEventArgs<DropDownListDataModel, DropDownListDataModel> args)
    {
        if (args.Value == null) return;
        patientData.臨床資訊.Age = args.Value.Key;
    }

    private async Task OnECOrOCCChanged(ChangeEventArgs<DropDownListDataModel, DropDownListDataModel> args)
    {
        if (args.Value == null) return;
        patientData.臨床資訊.ECorOC = args.Value.Key;
        patientData.臨床資訊.CalculateCancerType();
        RefreshFIGO癌症分期();
        if (patientData.臨床資訊.ECorOC == MagicObjectHelper.OC)
        {
            var type1 = List組織型態.FirstOrDefault(x => x.Key == MagicObjectHelper.TypeI);
            var type2 = List組織型態.FirstOrDefault(x => x.Key == MagicObjectHelper.TypeII);
            Select組織型態 = type1;
            StateHasChanged();
            Select組織型態 = type2;
            Refresh組織型態Detail();
            StateHasChanged();
        }
        else
        {
            var type = List組織型態.FirstOrDefault(x => x.Key == MagicObjectHelper.TypeI);
            Select組織型態 = type;
            StateHasChanged();
            Refresh組織型態Detail();
            StateHasChanged();
        }
    }

    #endregion

    void OnUploadBtn()
    {
        if (AuthenticationStateHelper.CheckAccessPage(MagicObjectHelper.ROLEAI操作) == true)
        {
            ShowUploadDicomDialog = true;
        }
    }

    void OnUploadEditAnnotationsBtn()
    {
    }

    async Task OnPushToAIBtn()
    {
        if (AuthenticationStateHelper.CheckAccessPage(MagicObjectHelper.ROLEAI操作) == false)
        {
            return;
        }
        Patient patient = mapper.Map<Patient>(patientAdapterModel);

        await GetDataAsync();
        var currentRootPath = Directory.GetCurrentDirectory();
        var dicomImage = Path.Combine(currentRootPath, MagicObjectHelper.UploadFinalPath, patientData.臨床資訊.ImageDicom);

        string preCheckMessage = await AIIntegrateService.PushToAICheck(patient, patientData, dicomImage);
        if (string.IsNullOrEmpty(preCheckMessage) == false)
        {
            StateHasChanged();
            await Task.Delay(100);
            MessageBox.Show("400px", "200px", "錯誤", preCheckMessage, MessageBox.HiddenAsync);
            StateHasChanged();
            return;
        }

        PatientAIInfo patientAIInfo = await AIIntegrateService.PushToAI(patient, patientData, dicomImage);

        patientData.臨床資訊.ObstetricianGynecologistConfirmation = new();
        patientData.臨床資訊.RadiologistConfirmation = new();

        patientData.臨床資訊.KeyName = patientAIInfo.KeyName;
        patientAdapterModel.JsonData = patientData.ToJson();
        patientAdapterModel.AI處理 = MagicObjectHelper.AI處理處理中;
        patientAdapterModel.AI評估 = MagicObjectHelper.NA;
        // patientAdapterModel.組別 = "Dr";
        await PatientService.UpdateAsync(patientAdapterModel);

        #region 更新操作日誌
        MyUserAdapterModel myUserAdapterModel = await AuthenticationStateHelper
        .GetUserInformation(authStateProvider);

        await OperationHistoryTraceService.AddAsync(OperationHistoryTraceAdapterModel.Build(myUserAdapterModel.Name, patientData.臨床資訊.SubjectNo, MagicObjectHelper.OperationCategoryAI推論, MagicObjectHelper.OperationMessageAI推論, $"使用者 {myUserAdapterModel.Name} 觸發 AI推論 需求"));

        #endregion

        MessageBox.Show("400px", "200px", "資訊", $"已經將此紀錄送至 AI 推論中 (ID: {patientAIInfo.KeyName})", MessageBox.HiddenAsync);

    }

    async Task OnConfirmAIBtn()
    {
        if (AuthenticationStateHelper.CheckAccessPage(MagicObjectHelper.ROLEAI操作) == false)
        {
            return;
        }
        Patient patient = mapper.Map<Patient>(patientAdapterModel);

        await GetDataAsync();
        bool isCompletion = await AIIntegrateService.CheckAIProcess(patientData.臨床資訊.KeyName);

        #region 更新操作日誌
        MyUserAdapterModel myUserAdapterModel = await AuthenticationStateHelper
        .GetUserInformation(authStateProvider);

        await OperationHistoryTraceService.AddAsync(OperationHistoryTraceAdapterModel.Build(myUserAdapterModel.Name, patientData.臨床資訊.SubjectNo, MagicObjectHelper.OperationCategory推論狀態呈現, MagicObjectHelper.OperationMessage推論狀態呈現, $"使用者 {myUserAdapterModel.Name} 要求進行推論狀態呈現結果檢查"));

        #endregion

        string msg = "";
        if (isCompletion == false)
        {
            msg = "AI處理  尚未  完成，請稍後再確認";
            MessageBox.Show("400px", "200px", "資訊", msg, MessageBox.HiddenAsync);
            return;
        }
        else
        {
            await ShowAICompletionAsync();
        }
    }

    async Task ShowAICompletionAsync()
    {
        var msg = "AI處理  已經  完成，可以在 風險評估 頁籤看到推論結果";

        #region 取得腰圍
        RiskAssessmentAIResult riskAssessmentAIResult = await AIIntegrateService.Get腰圍ACAsync(patientData.臨床資訊.KeyName);
        if (riskAssessmentAIResult != null)
        {
            patientData.臨床資訊.AbdominalCircumference = riskAssessmentAIResult.腰圍ACcm;
        }
        #endregion

        InputCsvModel inputCsvModel = await AIIntegrateService.GetInputCsv(patientData.臨床資訊.KeyName);

        string imageRootPath = MagicObjectHelper.UploadFinalPath;
        var keyName = patientData.臨床資訊.KeyName;
        // http://localhost:5272/UploadFiles/202509111436154559/Phase1Result/202509111436154559.png
        var imageFilename = $"{keyName}/Phase1Result/{keyName}.png";
        patientData.臨床資訊.RiskAssessmentResult.ImagePng = imageFilename;
        patientData.臨床資訊.RiskAssessmentResult.SMD骨骼肌密度 = inputCsvModel.Total_SMD.ToFloat().ToString("F2");
        patientData.臨床資訊.RiskAssessmentResult.IMAT肌間肌肉脂肪組織 = inputCsvModel.Total_ImatA.ToFloat().ToString("F2");
        patientData.臨床資訊.RiskAssessmentResult.LAMA低密度肌肉區域 = inputCsvModel.Total_LamaA.ToFloat().ToString("F2");
        patientData.臨床資訊.RiskAssessmentResult.NAMA正常密度肌肉區域 = inputCsvModel.Total_NamaA.ToFloat().ToString("F2");
        // SMA : SMA (Skeletal Muscle Area) TotalLamaA + TotalNamaA 骨骼肌面積
        patientData.臨床資訊.RiskAssessmentResult.SMA骨骼肌面積 = (inputCsvModel.Total_LamaA.ToFloat() + inputCsvModel.Total_NamaA.ToFloat()).ToString("F2");
        //SMI= LAMA+NAMA/(身高的平方(公尺))
        patientData.臨床資訊.RiskAssessmentResult.SMI骨骼肌指標 = ((inputCsvModel.Total_LamaA.ToFloat() + inputCsvModel.Total_NamaA.ToFloat())
            / (patientData.臨床資訊.Height.ToFloat() / 100.0 * patientData.臨床資訊.Height.ToFloat() / 100.0)).ToString("F2");

        patientData.臨床資訊.RiskAssessmentResult.Myosteatosis肌肉脂肪變性 = (inputCsvModel.Total_ImatA.ToFloat() + inputCsvModel.Total_LamaA.ToFloat()).ToString("F2");

        (string risk, string reducedRisk) = await AIIntegrateService.GetOnputCsv(patientData.臨床資訊.KeyName);

        patientData.臨床資訊.RiskAssessmentResult.風險程度 = risk;
        patientData.臨床資訊.RiskAssessmentResult.是否需要降15Percent劑量 = reducedRisk;

        if (patientAdapterModel.組別 == "AI")
        {
            patientData.臨床資訊.RiskAssessmentResult.ExperimentalControl = MagicObjectHelper.實驗組;
        }
        else
        {
            patientData.臨床資訊.RiskAssessmentResult.ExperimentalControl = MagicObjectHelper.對照組;
        }

        patientAdapterModel.JsonData = patientData.ToJson();

        patientAdapterModel.AI處理 = MagicObjectHelper.AI處理已完成;
        // patientAdapterModel.組別 = "AI";
        await PatientService.UpdateAsync(patientAdapterModel);

        MessageBox.Show("400px", "200px", "資訊", msg, MessageBox.HiddenAsync);
        return;
    }

    void OnShowMMRProteinSetting()
    {
        ShowMMRProteinSettingDialog = true;
    }

    void OnShowVisitCode()
    {
        patientData.臨床資料.CollectVisitCode(VisitCodeSetModel);

        sourceObjectJson = JsonConvert.SerializeObject(VisitCodeSetModel);

        ShowVisitCodeDialog = true;
    }

    async Task OnMMRProteinSettingAsync(MMRProteinSetting mMRProteinSetting)
    {
        if (mMRProteinSetting != null)
        {
            patientData.臨床資訊.MMRProteinSetting = mMRProteinSetting;
        }
        ShowMMRProteinSettingDialog = false;
    }

    async Task OnVisitCodeAsync(VisitCodeSetModel visitCodeSetModel)
    {
        var task = GetDataAsync();
        await Task.Delay(100);
        StateHasChanged();
        await task;
        if (visitCodeSetModel != null)
        {
            targetObjectJson = JsonConvert.SerializeObject(VisitCodeSetModel);


            Main臨床資料HelperService.SyncData(patientData.臨床資訊.SubjectNo, patientData.臨床資料, visitCodeSetModel);
            patientData.SyncData();
            patientAdapterModel.JsonData = patientData.ToJson();
            await PatientService.UpdateAsync(patientAdapterModel);

            #region 更新操作日誌
            MyUserAdapterModel myUserAdapterModel = await AuthenticationStateHelper
            .GetUserInformation(authStateProvider);

            await OperationHistoryTraceService.AddAsync(OperationHistoryTraceAdapterModel.Build(myUserAdapterModel.Name, patientData.臨床資訊.SubjectNo, MagicObjectHelper.OperationCategoryVisitCode, "-", "-"), sourceObjectJson, targetObjectJson, MagicObjectHelper.OperationCategoryVisitCode);
            #endregion

        }
        ShowVisitCodeDialog = false;
    }

    async Task OnUploadDicomAsync(string filename)
    {
        if (filename != null)
        {
            patientData.臨床資訊.Image = patientData.臨床資訊.SubjectNo;
            patientAdapterModel.JsonData = patientData.ToJson();
            await PatientService.UpdateAsync(patientAdapterModel);
            imageVersion = DateTime.Now.Ticks.ToString();
            StateHasChanged();
            await Task.Delay(200);

            #region 更新操作日誌
            MyUserAdapterModel myUserAdapterModel = await AuthenticationStateHelper
            .GetUserInformation(authStateProvider);

            await OperationHistoryTraceService.AddAsync(OperationHistoryTraceAdapterModel.Build(myUserAdapterModel.Name, patientData.臨床資訊.SubjectNo, MagicObjectHelper.OperationCategory上傳影像, MagicObjectHelper.OperationMessage完成影像上傳, $"使用者 {myUserAdapterModel.Name} 已經完成新影像上傳"));

            #endregion
        }
        imageVersion = DateTime.Now.Ticks.ToString();
        ShowUploadDicomDialog = false;
        await GetDataAsync();
        StateHasChanged();
    }

    void OnChangeEditMode()
    {
        source臨床資訊ObjectJson = JsonConvert.SerializeObject(patientData.臨床資訊); ;

        editMode = !editMode;
        var beforeJson = patientData.ToJson();
        PatientData temp = new PatientData();
        temp.FromJson(beforeJson);
        beforePatientData = temp;
    }

    async Task OnSave()
    {
        RandomParameterMode randomParameterModeAfter = new RandomParameterMode()
        {
            SubjectNo = patientData.臨床資訊.SubjectNo,
            FIGO = patientData.臨床資訊.FIGOStaging,
            ECorOC = patientData.臨床資訊.ECorOC,
        };
        randomParameterModeAfter.Parse();
        RandomParameterMode randomParameterModeBefore = new RandomParameterMode()
        {
            SubjectNo = beforePatientData.臨床資訊.SubjectNo,
            FIGO = beforePatientData.臨床資訊.FIGOStaging,
            ECorOC = beforePatientData.臨床資訊.ECorOC,
        };
        randomParameterModeBefore.Parse();
        var Treatment = await RandomListService.AssignRandomToStudyCodeAsync(
            randomParameterModeBefore, randomParameterModeAfter);

        patientAdapterModel.組別 = Treatment;
        patientAdapterModel.JsonData = patientData.ToJson();
        await PatientService.UpdateAsync(patientAdapterModel);
        editMode = false;

        target臨床資訊ObjectJson = JsonConvert.SerializeObject(patientData.臨床資訊); ;

        #region 更新操作日誌
        MyUserAdapterModel myUserAdapterModel = await AuthenticationStateHelper
        .GetUserInformation(authStateProvider);

        await OperationHistoryTraceService.AddAsync(OperationHistoryTraceAdapterModel.Build(myUserAdapterModel.Name, patientData.臨床資訊.SubjectNo, MagicObjectHelper.OperationCategory臨床資訊, "-", "-"), source臨床資訊ObjectJson, target臨床資訊ObjectJson, MagicObjectHelper.OperationCategory臨床資訊);
        #endregion
    }

    async Task OnRecomputeRandomListAsync()
    {
        bool result = false;
        var task = ConfirmMessageBox.ShowAsync("400px", "200px",
             "再次確認", $"確定要重新計算亂數表中的組別嗎？",
             ConfirmMessageBox.HiddenAsync);
        StateHasChanged();
        result = await task;
        if (result == false)
        {
            return;
        }

        RandomParameterMode randomParameterMode = new RandomParameterMode()
        {
            SubjectNo = patientData.臨床資訊.SubjectNo,
            FIGO = patientData.臨床資訊.FIGOStaging,
            ECorOC = patientData.臨床資訊.ECorOC,

        };
        randomParameterMode.Parse();

        var Treatment = await RandomListService.ReComputeRandomListAsync(randomParameterMode);

        patientAdapterModel.組別 = Treatment;
        patientAdapterModel.JsonData = patientData.ToJson();
        await PatientService.UpdateAsync(patientAdapterModel);
    }

    async Task OnCancel()
    {
        patientData.FromJson(patientAdapterModel.JsonData);
        editMode = false;
    }

    async Task OnNotifyRadiologistAsync()
    {
        // await ConfirmSendNotification();
        showNotyfyDicomDialog = true;
    }

    async Task ConfirmSendNotification()
    {
        bool result = false;
        var task = ConfirmMessageBox.ShowAsync("400px", "200px",
             "再次確認", $"確定要寄送通知給放射科醫師嗎？",
             ConfirmMessageBox.HiddenAsync);
        StateHasChanged();
        result = await task;
    }

    public void Dispose()
    {
        Console.WriteLine("Disposing resources");
        cts.Cancel();
    }

    #region 頁籤切換
    void On臨床資料()
    {
        if (AuthenticationStateHelper.CheckShowPage(MagicObjectHelper.ROLE臨床資訊, ConfirmMessageBox) == false)
            return;

        var url = $"/ClinicalInformation/{Code}";
        NavigationManager.NavigateTo(url);
    }

    void On抽血資料()
    {
        if (AuthenticationStateHelper.CheckShowPage(MagicObjectHelper.ROLE抽血資料, ConfirmMessageBox) == false)
            return;

        var url = $"/BloodTest/{Code}";
        NavigationManager.NavigateTo(url);
    }

    void On副作用()
    {
        if (AuthenticationStateHelper.CheckShowPage(MagicObjectHelper.ROLE副作用, ConfirmMessageBox) == false)
            return;

        var url = $"/SideEffectPage/{Code}";
        NavigationManager.NavigateTo(url);
    }

    void On問卷()
    {
        if (AuthenticationStateHelper.CheckShowPage(MagicObjectHelper.ROLE問卷, ConfirmMessageBox) == false)
            return;

        var url = $"/Survey/{Code}";
        NavigationManager.NavigateTo(url);
    }

    void On追蹤資料()
    {
        if (AuthenticationStateHelper.CheckShowPage(MagicObjectHelper.ROLE追蹤資料, ConfirmMessageBox) == false)
            return;

        var url = $"/TrackingData/{Code}";
        NavigationManager.NavigateTo(url);
    }

    void On風險評估()
    {
        if (AuthenticationStateHelper.CheckShowPage(MagicObjectHelper.ROLE風險評估, ConfirmMessageBox) == false)
            return;

        var url = $"/RiskAssessment/{Code}";
        NavigationManager.NavigateTo(url);
    }
    #endregion
}
