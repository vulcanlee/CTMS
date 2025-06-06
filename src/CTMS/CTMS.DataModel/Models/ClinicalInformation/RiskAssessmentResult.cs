using CTMS.Share.Helpers;

namespace CTMS.DataModel.Models.ClinicalInformation;

public class RiskAssessmentResult
{
    public void Demo實驗組()
    {
        ExperimentalControl = MagicObjectHelper.實驗組;
        SMI骨骼肌指標 = "低於正常值";
        SMD骨骼肌密度 = "低於正常值";
        IMAT肌間肌肉脂肪組織 = "高於正常值";
        LAMA低密度肌肉區域 = "正常";
        NAMA正常密度肌肉區域 = "正常";
        Myosteatosis肌肉脂肪變性 = "正常";
    }
    public void Demo對照組()
    {
        ExperimentalControl = MagicObjectHelper.對照組;
        SMI骨骼肌指標 = "高於正常值";
        SMD骨骼肌密度 = "高於正常值";
        IMAT肌間肌肉脂肪組織 = "低於正常值";
        LAMA低密度肌肉區域 = "異常";
        NAMA正常密度肌肉區域 = "異常";
        Myosteatosis肌肉脂肪變性 = "異常";
    }
    /// <summary>
    /// 實驗組或對照組
    /// </summary>
    public string ExperimentalControl { get; set; } = MagicObjectHelper.實驗組;
    public string ExperimentalControlMessage
    {
        get
        {
            return ExperimentalControl switch
            {
                MagicObjectHelper.實驗組 => MagicObjectHelper.實驗組Message,
                MagicObjectHelper.對照組 => MagicObjectHelper.對照組Message,
                _ => "未知組別"
            };
        }
    }
    public string 風險程度 { get; set; } = "高風險";
    public string 是否需要降15Percent劑量 { get; set; } = "需要";
    public string SMI骨骼肌指標 { get; set; } = "低於正常值";
    public string SMD骨骼肌密度 { get; set; } = "低於正常值";
    public string IMAT肌間肌肉脂肪組織 { get; set; } = "高於正常值";
    public string LAMA低密度肌肉區域 { get; set; } = "正常";
    public string NAMA正常密度肌肉區域 { get; set; } = "正常";
    public string Myosteatosis肌肉脂肪變性 { get; set; } = "正常";
    public string CTImage1 { get; set; }
    public string CTImage2 { get; set; }
}
