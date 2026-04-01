using CTMS.Business.Services.ClinicalInformation;
using CTMS.DataModel.Dtos;
using CTMS.DataModel.Models.ClinicalInformation;
using CTMS.DataModel.Models.Dashboard;
using CTMS.EntityModel;
using CTMS.EntityModel.Models;
using CTMS.Share.Helpers;
using Microsoft.EntityFrameworkCore;

namespace CTMS.Business.Services;

public class DashboardService
{
    private readonly Microsoft.Extensions.Logging.ILogger<DashboardService> logger;
    private readonly BackendDBContext context;
    private readonly AIIntegrateService aiIntegrateService;

    public DashboardViewModel Dashboard { get; set; } = new();

    public DashboardService(Microsoft.Extensions.Logging.ILogger<DashboardService> logger,
        BackendDBContext context, AIIntegrateService aIIntegrateService)
    {
        this.logger = logger;
        this.context = context;
        this.aiIntegrateService = aIIntegrateService;
    }

    public void Build()
    {
        Dashboard = new DashboardViewModel();
        #region 儀錶板初始化
        #region Row 1
        #region 合作醫院
        Dashboard.Summary.HospitalNames = new List<string>
        {
            MagicObjectHelper.PrefixSheetName成大醫院,
            MagicObjectHelper.PrefixSheetName奇美醫院,
            MagicObjectHelper.PrefixSheetName郭綜合醫院,
        };
        Dashboard.Summary.PartnerHospitalCount = Dashboard.Summary.HospitalNames.Count;
        #endregion

        #region 總病例數
        Dashboard.Summary.TotalCases = 0;
        Dashboard.Summary.MonthlyGrowthRate = 0;
        Dashboard.Summary.NewCasesThisMonth = 0;
        #endregion

        #region 完成率
        Dashboard.Summary.CompletionRate = 0;
        Dashboard.Summary.CompletionRateGrowth = 0;
        #endregion

        #region 分析報告
        Dashboard.Summary.AnalysisReportCount = 0;
        #endregion

        #region 第二排摘要統計
        Dashboard.Summary.ExperimentalGroupCount = 0;
        Dashboard.Summary.ControlGroupCount = 0;
        Dashboard.Summary.HighRiskCount = 0;
        #endregion

        #endregion

        #region Row 2
        #region 醫院個數
        Dashboard.HospitalStats = new List<HospitalCaseStat>
        {
            new HospitalCaseStat
            {
                HospitalName = MagicObjectHelper.PrefixSheetName成大醫院,
                CaseCount = 0,
                ExperimentalGroupCount = 0,
                ControlGroupCount = 0
            },
            new HospitalCaseStat
            {
                HospitalName = MagicObjectHelper.PrefixSheetName奇美醫院,
                CaseCount = 0,
                ExperimentalGroupCount = 0,
                ControlGroupCount = 0
            },
            new HospitalCaseStat
            {
                HospitalName = MagicObjectHelper.PrefixSheetName郭綜合醫院,
                CaseCount = 0,
                ExperimentalGroupCount = 0,
                ControlGroupCount = 0
            },
        };
        #endregion

        #region 分期統計
        Dashboard.StageStats = new List<CancerStageStat>
        {
            new CancerStageStat { StageName = "I", Count = 0, ExperimentalGroupCount = 0, ControlGroupCount = 0 },
            new CancerStageStat { StageName = "II", Count = 0, ExperimentalGroupCount = 0, ControlGroupCount = 0 },
            new CancerStageStat { StageName = "III", Count = 0, ExperimentalGroupCount = 0, ControlGroupCount = 0 },
            new CancerStageStat { StageName = "IV", Count = 0, ExperimentalGroupCount = 0, ControlGroupCount = 0 },
        };
        #endregion

        #endregion

        #region Row 3
        #region 癌別統計
        Dashboard.CancerTypeStats.OvarianCancerCount = 0;
        Dashboard.CancerTypeStats.EndometrialCancerCount = 0;
        Dashboard.CancerTypeStats.OvarianCancerExperimentalGroupCount = 0;
        Dashboard.CancerTypeStats.OvarianCancerControlGroupCount = 0;
        Dashboard.CancerTypeStats.EndometrialCancerExperimentalGroupCount = 0;
        Dashboard.CancerTypeStats.EndometrialCancerControlGroupCount = 0;
        #endregion

        #region 完成度統計
        Dashboard.CompletionStats.CompletedCount = 0;
        Dashboard.CompletionStats.IncompleteCount = 0;
        #endregion
        #endregion

        #endregion
    }

    public async System.Threading.Tasks.Task RefreshAsync()
    {
        PatientData patientData = new();
        int pageSize = 10;
        int page = 1;
        var today = DateTime.Today;

        while (true)
        {
            var patients = await context.Patient
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

            // 沒有資料時退出
            if (patients.Count == 0)
                break;

            page++;

            foreach (var patient in patients)
            {
                patientData.FromJson(patient.JsonData);

                #region 儀錶板初始化
                #region Row 1
                #region 合作醫院
                #endregion

                #region 總病例數
                if (patient.狀態 == MagicObjectHelper.Patient狀態_收案)
                {
                    Dashboard.Summary.TotalCases++;
                    Dashboard.Summary.MonthlyGrowthRate++;

                    if (patientData.臨床資訊.收案日期.Year == today.Year &&
                        patientData.臨床資訊.收案日期.Month == today.Month)
                    {
                        Dashboard.Summary.NewCasesThisMonth++;
                    }
                }
                #endregion

                #region 完成率
                if (patient.狀態 == MagicObjectHelper.Patient狀態_收案)
                {
                    Dashboard.Summary.CompletionRate++;

                    if (patientData.臨床資訊.收案日期.Year == today.Year &&
         patientData.臨床資訊.收案日期.Month == today.Month)
                    {
                        Dashboard.Summary.CompletionRateGrowth++;
                    }
                }
                #endregion

                #region 分析報告
                if (string.IsNullOrEmpty(patientData.臨床資訊.KeyName) == false)
                {
                    bool isCompletion = await aiIntegrateService.CheckAIProcess(patientData.臨床資訊.KeyName, onlyCheck:true);
                    if (isCompletion)
                    {
                        Dashboard.Summary.AnalysisReportCount++;
                    }
                }
                #endregion

                #region 第二排摘要統計
                if (patient.組別 == MagicObjectHelper.組別實驗組英文)
                {
                    Dashboard.Summary.ExperimentalGroupCount++;
                }
                else if (patient.組別 == MagicObjectHelper.組別對照組英文)
                {
                    Dashboard.Summary.ControlGroupCount++;
                }

                if (patientData.臨床資訊?.RiskAssessmentResult?.風險程度 == "高風險")
                {
                    Dashboard.Summary.HighRiskCount++;
                }
                #endregion

                #endregion

                #region Row 2
                #region 醫院個數
                var normalizedHospitalName = GetHospitalName(patient.醫院);
                if (string.IsNullOrEmpty(normalizedHospitalName) == false)
                {
                    var hospitalStat = Dashboard.HospitalStats
                        .FirstOrDefault(a => a.HospitalName == normalizedHospitalName);

                    if (hospitalStat is not null)
                    {
                        if (patient.組別 == MagicObjectHelper.組別實驗組英文)
                        {
                            hospitalStat.ExperimentalGroupCount++;
                            hospitalStat.CaseCount++;
                        }
                        else if (patient.組別 == MagicObjectHelper.組別對照組英文)
                        {
                            hospitalStat.ControlGroupCount++;
                            hospitalStat.CaseCount++;
                        }
                    }
                }
                #endregion

                #region 分期統計
                var cancerStage = patientData.臨床資訊.FIGOStaging;
                if (string.IsNullOrEmpty(cancerStage) == false)
                {
                    CancerStageStat? stageStat = null;

                    if (cancerStage.Contains("IV") && cancerStage.IndexOf("IV") == 0)
                    {
                        stageStat = Dashboard.StageStats.FirstOrDefault(a => a.StageName == "IV");
                    }
                    else if (cancerStage.Contains("III") && cancerStage.IndexOf("III") == 0)
                    {
                        stageStat = Dashboard.StageStats.FirstOrDefault(a => a.StageName == "III");
                    }
                    else if (cancerStage.Contains("II") && cancerStage.IndexOf("II") == 0)
                    {
                        stageStat = Dashboard.StageStats.FirstOrDefault(a => a.StageName == "II");
                    }
                    else if (cancerStage.Contains("I") && cancerStage.IndexOf("I") == 0)
                    {
                        stageStat = Dashboard.StageStats.FirstOrDefault(a => a.StageName == "I");
                    }

                    if (stageStat is not null)
                    {
                        if (patient.組別 == MagicObjectHelper.組別實驗組英文)
                        {
                            stageStat.ExperimentalGroupCount++;
                            stageStat.Count++;
                        }
                        else if (patient.組別 == MagicObjectHelper.組別對照組英文)
                        {
                            stageStat.ControlGroupCount++;
                            stageStat.Count++;
                        }
                    }
                }
                #endregion

                #endregion

                #region Row 3
                #region 癌別統計
                if (patientData.臨床資訊?.CancerType?.Contains("卵巢癌") == true)
                {
                    Dashboard.CancerTypeStats.OvarianCancerCount++;

                    if (patient.組別 == MagicObjectHelper.組別實驗組英文)
                    {
                        Dashboard.CancerTypeStats.OvarianCancerExperimentalGroupCount++;
                    }
                    else if (patient.組別 == MagicObjectHelper.組別對照組英文)
                    {
                        Dashboard.CancerTypeStats.OvarianCancerControlGroupCount++;
                    }
                }
                else if (patientData.臨床資訊?.CancerType?.Contains("子宮內膜癌") == true)
                {
                    Dashboard.CancerTypeStats.EndometrialCancerCount++;

                    if (patient.組別 == MagicObjectHelper.組別實驗組英文)
                    {
                        Dashboard.CancerTypeStats.EndometrialCancerExperimentalGroupCount++;
                    }
                    else if (patient.組別 == MagicObjectHelper.組別對照組英文)
                    {
                        Dashboard.CancerTypeStats.EndometrialCancerControlGroupCount++;
                    }
                }
                #endregion

                #region 完成度統計
                Dashboard.CompletionStats.CompletedCount = 0;
                Dashboard.CompletionStats.IncompleteCount = 0;
                #endregion
                #endregion

                #endregion
            }
        }

        Dashboard.ComputeCompletion();
    }

    private static string? GetHospitalName(string? hospital)
    {
        if (string.IsNullOrEmpty(hospital))
        {
            return null;
        }

        if (hospital.Contains(MagicObjectHelper.PrefixSheetName成大醫院))
        {
            return MagicObjectHelper.PrefixSheetName成大醫院;
        }

        if (hospital.Contains(MagicObjectHelper.PrefixSheetName奇美醫院))
        {
            return MagicObjectHelper.PrefixSheetName奇美醫院;
        }

        if (hospital.Contains(MagicObjectHelper.PrefixSheetName郭綜合醫院))
        {
            return MagicObjectHelper.PrefixSheetName郭綜合醫院;
        }

        return null;
    }
}

