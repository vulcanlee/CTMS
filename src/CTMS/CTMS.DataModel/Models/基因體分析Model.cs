using System.Drawing;
using System.Text;
using System;

namespace CTMS.DataModel.Models;

public class GenomeAnalysis基因體分析Model
{
    public string 名稱 { get; set; }
    public string 疾病 { get; set; }
    public string 基因變異 { get; set; }
    public string 致病性 { get; set; }
    public string 基因頻率 { get; set; }
    public string 遺傳模式 { get; set; }

    public string 名稱1 { get; set; }
    public string 疾病1 { get; set; }
    public string 基因變異1 { get; set; }
    public string 致病性1 { get; set; }
    public string 基因頻率1 { get; set; }
    public string 遺傳模式1 { get; set; }

    public string 心臟單基因 { get; set; }
    public string 肌功能多基因 { get; set; }
    public string 心理韌性基因 { get; set; }
    public GenomeAnalysis基因體分析RadarChartModel 基因體分析Radar { get; set; } = new();
}





