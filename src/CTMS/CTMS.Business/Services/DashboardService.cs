using Azure.AI.OpenAI;
using CTMS.DataModel.Models.Dashboard;
using CTMS.Share.Helpers;
using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTMS.Business.Services;

public class DashboardService
{
    public DashboardViewModel Dashboard { get; set; } = new();
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
}

