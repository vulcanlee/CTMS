namespace CTMS.DataModel.Models.Dashboard;

/// <summary>
/// 收案進度統計頁的主要 ViewModel（醫院 × 癌別 × 期別 × 組別 交叉統計）
/// </summary>
public class EnrollmentProgressViewModel
{
    /// <summary>
    /// 各醫院統計區塊（固定順序：成大、郭綜合、奇美）
    /// </summary>
    public List<HospitalEnrollment> Hospitals { get; set; } = new();

    /// <summary>
    /// 總計收案數（所有格子加總）
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// 已收案且已分組、但醫院/癌別/期別任一無法歸類者（頁尾註記用）
    /// </summary>
    public int UnclassifiedCount { get; set; }
}

/// <summary>
/// 單一醫院的收案統計區塊
/// </summary>
public class HospitalEnrollment
{
    public string HospitalName { get; set; }

    /// <summary>
    /// 癌別區塊（固定順序：卵巢癌、內膜癌）
    /// </summary>
    public List<CancerEnrollment> Cancers { get; set; } = new();

    /// <summary>
    /// 小計收案數（本院所有格子加總）
    /// </summary>
    public int Subtotal { get; set; }
}

/// <summary>
/// 單一癌別的收案統計區塊
/// </summary>
public class CancerEnrollment
{
    public string CancerName { get; set; }

    /// <summary>
    /// 期別格子（固定順序：I、II、III、IV）
    /// </summary>
    public List<StageCell> Stages { get; set; } = new();
}

/// <summary>
/// 單一期別的對照組/實驗組人數
/// </summary>
public class StageCell
{
    public string StageName { get; set; }
    public int ControlCount { get; set; }
    public int ExperimentalCount { get; set; }
}
