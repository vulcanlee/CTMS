using System.Drawing;
using System.Text;
using System;

namespace CTMS.DataModel.Models;

public class 代謝物含量壓力情緒指標Model
{
    public 代謝物含量壓力情緒指標ItemModel 血清素 { get; set; } = new();
    public 代謝物含量壓力情緒指標ItemModel 多巴胺 { get; set; } = new();
    public 代謝物含量壓力情緒指標ItemModel 血清素原料 { get; set; } = new();
    public 代謝物含量壓力情緒指標ItemModel 抗憂鬱指標 { get; set; } = new();
}

public class 代謝物含量壓力情緒指標ItemModel
{
    public string 運動前 { get; set; }
    public string 運動前ClassText { get; set; } = "";
    public string 運動後15分 { get; set; }
    public string 運動後15分ClassText { get; set; } = "";
    public string 運動後30分 { get; set; }
    public string 運動後30分ClassText { get; set; } = "";
}






