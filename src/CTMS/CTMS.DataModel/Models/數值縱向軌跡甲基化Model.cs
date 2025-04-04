using System.Drawing;
using System.Text;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CTMS.DataModel.Models;

public class 數值縱向軌跡甲基化Model
{
    public 甲基化胺基酸軌跡Model 甲基化胺基酸軌跡 { get; set; } = new();
}

public class 甲基化胺基酸軌跡Model
{
    public 軌跡Model TMAO { get; set; } = new();
    public 軌跡Model TMAO職棒平均 { get; set; } = new();
    public 軌跡Model Serine { get; set; } = new();
    public 軌跡Model Serine職棒平均 { get; set; } = new();
    public 軌跡Model Glycine { get; set; } = new();
    public 軌跡Model Glycine職棒平均 { get; set; } = new();
    public 軌跡Model Sarcosine { get; set; } = new();
    public 軌跡Model Sarcosine職棒平均 { get; set; } = new();
    public 軌跡Model Choline { get; set; } = new();
    public 軌跡Model Choline職棒平均 { get; set; } = new();
}





