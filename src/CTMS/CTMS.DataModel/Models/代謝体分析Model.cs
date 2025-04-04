using System.Drawing;
using System.Text;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CTMS.DataModel.Models;

public class 代謝体分析Model
{
    public string 肌肉崩解    { get; set; }
    public string 發炎反應 { get; set; }
    public string 甲基化 { get; set; }

    public 代謝體分析_肌肉能量及耗損RadarChartModel 肌肉能量及耗損 { get; set; } = new();
    public 代謝體分析_肌肉能量及耗損RadarChartModel 肌肉能量及耗損_職業平均 { get; set; } = new();
    public 代謝體分析_肌肉能量及耗損RadarChartModel 肌肉能量及耗損_校隊平均 { get; set; } = new();
    public 代謝體分析_發炎狀態RadarChartModel 發炎狀態 { get; set; } = new();
    public 代謝體分析_發炎狀態RadarChartModel 發炎狀態_職業平均 { get; set; } = new();
    public 代謝體分析_發炎狀態RadarChartModel 發炎狀態_校隊平均 { get; set; } = new();
    public 代謝體分析_甲基化胺基酸RadarChartModel 甲基化胺基酸 { get; set; } = new();
    public 代謝體分析_甲基化胺基酸RadarChartModel 甲基化胺基酸_職業平均 { get; set; } = new();
    public 代謝體分析_甲基化胺基酸RadarChartModel 甲基化胺基酸_校隊平均 { get; set; } = new();
}





