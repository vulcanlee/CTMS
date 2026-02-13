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

        #endregion

        #region Row 2
        #region 醫院個數
        Dashboard.HospitalStats = new List<HospitalCaseStat>
        {
            new HospitalCaseStat { HospitalName = MagicObjectHelper.PrefixSheetName成大醫院, CaseCount = 0 },
            new HospitalCaseStat { HospitalName = MagicObjectHelper.PrefixSheetName奇美醫院, CaseCount = 0 },
            new HospitalCaseStat { HospitalName = MagicObjectHelper.PrefixSheetName郭綜合醫院, CaseCount = 0 },
        };
        #endregion

        #region 分期統計
        Dashboard.StageStats = new List<CancerStageStat>
        {
            new CancerStageStat { StageName = "I", Count = 0 },
            new CancerStageStat { StageName = "II", Count = 0 },
            new CancerStageStat { StageName = "III", Count = 0 },
            new CancerStageStat { StageName = "IV", Count = 0 },
        };
        #endregion

        #endregion

        #region Row 3
        #region 癌別統計
        Dashboard.CancerTypeStats.OvarianCancerCount = 0;
        Dashboard.CancerTypeStats.EndometrialCancerCount = 0;
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
                    Dashboard.Summary.NewCasesThisMonth++;
                }
                #endregion

                #region 完成率
                if (patient.狀態 == MagicObjectHelper.Patient狀態_收案)
                {
                    Dashboard.Summary.CompletionRate++;
                    Dashboard.Summary.CompletionRateGrowth++;
                }
                #endregion

                #region 分析報告
                if (string.IsNullOrEmpty(patientData.臨床資訊.KeyName) == false)
                {
                    bool isCompletion = await aiIntegrateService.CheckAIProcess(patientData.臨床資訊.KeyName);
                    if (isCompletion)
                    {
                        Dashboard.Summary.AnalysisReportCount++;
                    }
                }
                #endregion

                #endregion

                #region Row 2
                #region 醫院個數
                var hospital = patient.醫院;
                if (hospital.Contains(MagicObjectHelper.PrefixSheetName成大醫院))
                {
                    Dashboard.HospitalStats.FirstOrDefault(a => a.HospitalName == MagicObjectHelper.PrefixSheetName成大醫院).CaseCount++;
                }
                else if (hospital.Contains(MagicObjectHelper.PrefixSheetName奇美醫院))
                {
                    Dashboard.HospitalStats.FirstOrDefault(a => a.HospitalName == MagicObjectHelper.PrefixSheetName奇美醫院).CaseCount++;
                }
                else if (hospital.Contains(MagicObjectHelper.PrefixSheetName郭綜合醫院))
                {
                    Dashboard.HospitalStats.FirstOrDefault(a => a.HospitalName == MagicObjectHelper.PrefixSheetName郭綜合醫院).CaseCount++;
                }
                #endregion

                #region 分期統計
                var cancerStage = patientData.臨床資訊.FIGOStaging;
                if (string.IsNullOrEmpty(cancerStage) == false)
                {
                    if (cancerStage.Contains("IV") && cancerStage.IndexOf("IV") == 0)
                    {
                        Dashboard.StageStats.FirstOrDefault(a => a.StageName == "IV").Count++;
                    } else if (cancerStage.Contains("III") && cancerStage.IndexOf("III") == 0)
                    {
                        Dashboard.StageStats.FirstOrDefault(a => a.StageName == "III").Count++;
                    }
                    else if (cancerStage.Contains("II") && cancerStage.IndexOf("II") == 0)
                    {
                        Dashboard.StageStats.FirstOrDefault(a => a.StageName == "II").Count++;
                    }
                    else if (cancerStage.Contains("I") && cancerStage.IndexOf("I") == 0)
                    {
                        Dashboard.StageStats.FirstOrDefault(a => a.StageName == "I").Count++;
                    }
                }
                #endregion

                #endregion

                #region Row 3
                #region 癌別統計
                if(patientData.臨床資訊.CancerType.Contains("卵巢癌"))
                {
                    Dashboard.CancerTypeStats.OvarianCancerCount++;
                }
                else if (patientData.臨床資訊.CancerType.Contains("子宮內膜癌"))
                {
                    Dashboard.CancerTypeStats.EndometrialCancerCount++;
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
    }
}

