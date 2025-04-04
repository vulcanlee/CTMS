using System.Drawing;
using System.Text;
using System;

namespace CTMS.DataModel.Models;

public class BodyFatAnalysis脂肪分析Model
{
    public string VisceralFatPercentile內臟脂肪百分位 { get; set; }
    public string SubcutaneousFatPercentile皮下脂肪百分位 { get; set; }
    public string WaistCircumferencePercentile腰圍百分位 { get; set; }
    public string MetabolicSyndromeRisk代謝失調風險 { get; set; }
    public string SpineSkeleton脊椎骨架 { get; set; }
    public string Height身高 { get; set; }
    public string Weight體重 { get; set; }
    public string BMI { get; set; }
    public string BodyFatPercentage體脂率 { get; set; }
    public string BasalMetabolicRate基礎代謝率 { get; set; }
    public string TotalDailyEnergyExpenditure每日消耗總熱量 { get; set; }
    public string BodyWater身體水分 { get; set; }
    public MuscleQualityRadarChartModel FatDegenerationPercentileInMuscleGroups各肌群脂肪變性百分位RadarChartModel { get; set; } = new();
}





