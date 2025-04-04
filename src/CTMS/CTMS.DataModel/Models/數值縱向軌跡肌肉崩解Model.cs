using System.Drawing;
using System.Text;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CTMS.DataModel.Models;

public class 數值縱向軌跡肌肉崩解Model
{
    public 肌肉能量及耗損_BCAAs軌跡Model BCAAs軌跡 { get; set; } = new();
    public 肌肉能量及耗損_組胺酸Model 組胺酸 { get; set; } = new();
}

public class 肌肉能量及耗損_BCAAs軌跡Model
{
    public 軌跡Model Leucine { get; set; } = new();
    public 軌跡Model Leucine職棒平均 { get; set; } = new();
    public 軌跡Model Isoleucine { get; set; } = new();
    public 軌跡Model Isoleucine職棒平均 { get; set; } = new();
    public 軌跡Model Valine { get; set; } = new();
    public 軌跡Model Valine職棒平均 { get; set; } = new();
    public 軌跡Model BCAAs { get; set; } = new();
    public 軌跡Model BCAAs職棒平均 { get; set; } = new();
}

public class 肌肉能量及耗損_組胺酸Model
{
    public 軌跡Model Histidine { get; set; } = new();
    public 軌跡Model Histidine職棒平均 { get; set; } = new();
    public 軌跡Model x1Methylhistidine { get; set; } = new();
    public 軌跡Model x1Methylhistidine職棒平均 { get; set; } = new();
    public 軌跡Model x3Methylhistidine { get; set; } = new();
    public 軌跡Model x3Methylhistidine職棒平均 { get; set; } = new();
}





