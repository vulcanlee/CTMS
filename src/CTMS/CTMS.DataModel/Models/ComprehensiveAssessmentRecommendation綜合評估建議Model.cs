namespace CTMS.DataModel.Models;

public class ComprehensiveAssessmentRecommendation綜合評估建議Model
{
    public string VisceralFatPercentile內臟脂肪百分位 { get; set; }
    public string SubcutaneousFatPercentile皮下脂肪百分位 { get; set; }
    public string MetabolicDisorderRisk代謝失調風險 { get; set; }
    public string WaistCircumferencePercentile腰圍百分位 { get; set; }
    public string MuscleHealth肌肉健康度 { get; set; }
    public string MuscleStrength肌力 { get; set; }
    public string MuscleEndurance肌耐力 { get; set; }
    public string CoreBalance核心均衡力 { get; set; }
    public string 核心肌群校正肌力 { get; set; }
    public string 心肺能力 { get; set; }


    public string 飲食補充 { get; set; } = string.Empty;
    public string 體能訓練 { get; set; } = string.Empty;
    public string 運動潛能 { get; set; } = string.Empty;
    public string 健康風險 { get; set; } = string.Empty;
    public string 飲食補充簡短建議 { get; set; } = string.Empty;
    public string 體能訓練簡短建議 { get; set; } = string.Empty;
    public string 運動潛能簡短建議 { get; set; } = string.Empty;
    public string 健康風險簡短建議 { get; set; } = string.Empty;

    public void CopyFrom(ComprehensiveAssessmentRecommendation綜合評估建議Model source)
    {
        this.飲食補充 = source.飲食補充;
        this.體能訓練 = source.體能訓練;
        this.運動潛能 = source.運動潛能;
        this.健康風險 = source.健康風險;

        this.飲食補充簡短建議 = source.飲食補充簡短建議;
        this.體能訓練簡短建議 = source.體能訓練簡短建議;
        this.運動潛能簡短建議 = source.運動潛能簡短建議;
        this.健康風險簡短建議 = source.健康風險簡短建議;
    }
}
