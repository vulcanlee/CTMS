using System.Drawing;
using System.Text;
using System;
using System.Reflection;

namespace CTMS.DataModel.Models;

public class BodyMuscleMassModel肌肉質量Model
{
    public string Waistline腰圍 { get; set; }
    public string VertebralBody椎體 { get; set; }
    public string Skeleton骨架 { get; set; }
    public string Area面積 { get; set; }
    public string Density密度 { get; set; }
    public string Index指標 { get; set; }
    public string CoreMuscleMass核心肌群肌肉量 { get; set; }
    public string CoreMuscleEndurance核心肌群肌肉品質 { get; set; }
    public string CorrectCoreMuscleStrength校正肌力 { get; set; }
    public CoreMuscleRadarChartModel CoreMuscleMass核心肌群肌肉量RadarChartModel { get; set; } = new();
    public CoreMuscleRadarChartModel CoreMuscleEndurance核心肌群肌耐力RadarChartModel { get; set; } = new();
    public CoreMuscleRadarChartModel CorrectCoreMuscleStrength核心肌群校正肌力強度RadarChartModel { get; set; } = new();
}





