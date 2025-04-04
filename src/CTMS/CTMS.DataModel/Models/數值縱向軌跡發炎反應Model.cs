using System.Drawing;
using System.Text;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CTMS.DataModel.Models;

public class 數值縱向軌跡發炎反應Model
{
    public KynurenineTyptophan軌跡Model KynurenineTyptophan軌跡 { get; set; } = new();
    public KynurenineSlashgTyptophan軌跡Model KynurenineSlashgTyptophan軌跡 { get; set; } = new();
}

public class KynurenineTyptophan軌跡Model
{
    public 軌跡Model Tryptophan { get; set; } = new();
    public 軌跡Model Tryptophan職棒平均 { get; set; } = new();
    public 軌跡Model Kynurenine { get; set; } = new();
    public 軌跡Model Kynurenine職棒平均 { get; set; } = new();
}

public class KynurenineSlashgTyptophan軌跡Model
{
    public 軌跡Model KynTrp { get; set; } = new();
    public 軌跡Model KynTrp職棒平均 { get; set; } = new();
}





