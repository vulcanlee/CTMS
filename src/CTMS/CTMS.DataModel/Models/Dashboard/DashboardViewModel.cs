namespace CTMS.DataModel.Models.Dashboard;

/// <summary>
/// 醫療數據儀表板的主要 ViewModel
/// </summary>
public class DashboardViewModel
{
    // 1. 頂部摘要卡片區塊
    public DashboardSummary Summary { get; set; } = new();

    // 2. 中左：醫院個數統計 (Bar Chart)
    public List<HospitalCaseStat> HospitalStats { get; set; } = new();

    // 3. 中右：分期統計 (Bar Chart)
    public List<CancerStageStat> StageStats { get; set; } = new();

    // 4. 左下：卵巢癌 / 內膜癌分佈 (Donut/Pie Chart)
    public CancerTypeDistribution CancerTypeStats { get; set; } = new();

    // 5. 右下：完成率分佈 (Pie Chart)
    public CompletionDistribution CompletionStats { get; set; } = new();
}

/// <summary>
/// 頂部四張卡片的數據
/// </summary>
public class DashboardSummary
{
    // 卡片 1: 合作醫院
    public int PartnerHospitalCount { get; set; }
    public List<string> HospitalNames { get; set; } = new(); // e.g., ["成大", "郭綜合", "奇美"]

    // 卡片 2: 總病例數
    public int TotalCases { get; set; }
    public int TargetCases { get; set; } = 294;
    public int MonthlyGrowthRate { get; set; }
    public int NewCasesThisMonth { get; set; }

    // 卡片 3: 完成率
    public double CompletionRate { get; set; }
    public double CompletionRateGrowth { get; set; }

    // 卡片 4: 分析報告
    public int AnalysisReportCount { get; set; }

    #region Method
    public string GetHospitalNamesDisplay() => string.Join("、", HospitalNames); // 用於顯示醫院名稱的輔助方法
    public string GetMonthlyGrowthDescription() => $"+{MonthlyGrowthRate} 本月新增{NewCasesThisMonth}例"; 
    public string GetCompletionRateDisplay() => $"{CompletionRate:F1}%";
    public string GetCompletionRateDescription() => $"+{CompletionRateGrowth:F1}%)";
    #endregion
}

/// <summary>
/// 醫院個數長條圖數據項
/// </summary>
public class HospitalCaseStat
{
    public string HospitalName { get; set; } 
    public int CaseCount { get; set; }       
}

/// <summary>
/// 分期統計長條圖數據項 (1期, 2期...)
/// </summary>
public class CancerStageStat
{
    public string StageName { get; set; } 
    public int Count { get; set; }
}

/// <summary>
/// 癌症類型分佈 (卵巢癌 vs 內膜癌)
/// </summary>
public class CancerTypeDistribution
{
    public int OvarianCancerCount { get; set; }  
    public int EndometrialCancerCount { get; set; }

    // 方便計算總數或百分比的唯讀屬性
    public int Total => OvarianCancerCount + EndometrialCancerCount;
}

/// <summary>
/// 完成率圓餅圖數據
/// </summary>
public class CompletionDistribution
{
    public int CompletedCount { get; set; }   // 已完成: 907
    public int IncompleteCount { get; set; }  // 未完成: 255

    public int Total => CompletedCount + IncompleteCount;
}
