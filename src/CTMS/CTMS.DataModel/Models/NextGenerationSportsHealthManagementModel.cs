using CTMS.DataModel.Models;

namespace CTMS.ExcelUtility.Services;

public class NextGenerationSportsCTMSModel
{
    public HomePageModel HomePageModel { get; set; } = new HomePageModel();
    public BodyMuscleMassModel肌肉質量Model BodyMuscleMassModel肌肉質量Model { get; set; } = new BodyMuscleMassModel肌肉質量Model();
    public BodyMotionAnalysis動作分析Model BodyMotionAnalysis動作分析Model { get; set; } = new BodyMotionAnalysis動作分析Model();
    public BodyMuscleQuality肌肉品質Model BodyMuscleQuality肌肉品質Model { get; set; } = new BodyMuscleQuality肌肉品質Model();
    public BodyFatAnalysis脂肪分析Model BodyFatAnalysis脂肪分析Model { get; set; } = new BodyFatAnalysis脂肪分析Model();
    public 抽血檢驗_血液Model 抽血檢驗_血液 { get; set; } = new();
    public 抽血檢驗_生化Model 抽血檢驗_生化 { get; set; } = new();
    public 抽血檢驗_特殊Model 抽血檢驗_特殊 { get; set; } = new();
    public List<抽血檢驗_血液生化特殊Model> 抽血檢驗_血液Items { get; set; } = new();
    public List<抽血檢驗_血液生化特殊Model> 抽血檢驗_生化Items { get; set; } = new();
    public List<抽血檢驗_血液生化特殊Model> 抽血檢驗_特殊Items { get; set; } = new();
    public ComprehensiveAssessmentRecommendation綜合評估建議Model ComprehensiveAssessmentRecommendation綜合評估建議Model { get; set; } = new ComprehensiveAssessmentRecommendation綜合評估建議Model();
    public 報告摘要Model 報告摘要Model { get; set; } = new 報告摘要Model();

    public HomePageModel Home首頁2 { get; set; } = new HomePageModel();
    public BodyMotionAnalysis動作分析Model MotionAnalysis動作分析2 { get; set; } = new BodyMotionAnalysis動作分析Model();
    public 心肺功能Model 心肺功能 { get; set; } = new ();
    public 心理韌性Model 心理韌性 { get; set; } = new ();
    public 代謝物含量壓力情緒指標Model 代謝物含量壓力情緒指標 { get; set; } = new ();
    public GenomeAnalysis基因體分析Model 基因體分析 { get; set; } = new ();
    public GenomeDetail基因體細項Model 基因體細項 { get; set; } = new ();
    public 代謝体分析Model 代謝体分析 { get; set; } = new ();
    public 數值縱向軌跡肌肉崩解Model 數值縱向軌跡肌肉崩解 { get; set; } = new ();
    public 數值縱向軌跡發炎反應Model 數值縱向軌跡發炎反應 { get; set; } = new ();
    public 數值縱向軌跡甲基化Model 數值縱向軌跡甲基化 { get; set; } = new ();
}
