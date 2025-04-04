using System.Drawing;
using System.Text;
using System;
using System.Runtime.Intrinsics.Arm;
using CTMS.Share.Helpers;

namespace CTMS.DataModel.Models;

public class 抽血檢驗_血液生化特殊Model
{
    public string 檢驗項目 { get; set; }
    public string 參考區間 { get; set; }
    public double 參考區間開始 { get; set; }
    public double 參考區間結束 { get; set; }
    public string 參考區間類型 { get; set; }
    public string 檢驗數據 { get; set; }
    public string 運動前 { get; set; }
    public string 運動後15分鐘 { get; set; }
    public string 運動後30分鐘 { get; set; }
    public string TextClassName { get; set; } = MagicObjectHelper.正常檢驗數計類別;
    public string TextClassName運動前 { get; set; } = MagicObjectHelper.正常檢驗數計類別;
    public string TextClassName運動後15分鐘 { get; set; } = MagicObjectHelper.正常檢驗數計類別;
    public string TextClassName運動後30分鐘 { get; set; } = MagicObjectHelper.正常檢驗數計類別;
}





