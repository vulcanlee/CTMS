using System.Drawing;
using System.Text;
using System;

namespace CTMS.DataModel.Models;

public class BodyMuscleQuality肌肉品質Model
{
    public string Skewness偏度 { get; set; }
    public string Kurtosis峰度 { get; set; }
    public string HealthyMuscleRatio健康肌肉比 { get; set; }
    public string MuscleHealthScore肌肉健康度 { get; set; }
    public string HighQualityMuscleHealthScore高品質肌肉健康度 { get; set; }
    public MuscleQualityRadarChartModel MuscleHealthScore肌肉健康度RadarChartModel { get; set; } = new();
    public MuscleQualityRadarChartModel HighQualityMuscleHealthScore高品質肌肉健康度RadarChartModel { get; set; } = new();
}





