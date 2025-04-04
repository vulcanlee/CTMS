using System.Drawing;
using System.Text;
using System;
using System.Runtime.Intrinsics.Arm;

namespace CTMS.DataModel.Models;

public class 抽血檢驗_血液Model
{
    public string 白血球計數_WBC { get; set; }
    public string 紅血球計數_RHC { get; set; }
    public string 血色素_Hb { get; set; }
    public string 血比容_Hct { get; set; }
    public string 平均紅血球容積_MCV { get; set; }
    public string 紅血球血紅素量_MCH { get; set; }
    public string 平均紅血球血紅素濃度_MCHC { get; set; }
    public string 網狀紅血球_RDW { get; set; }
    public string 血小板_Plt { get; set; }
    public string 平均血小板容積_MPV { get; set; }
    public string 嗜中性白血球_Seg { get; set; }
    public string 嗜酸性白血球_Eos { get; set; }
    public string 嗜鹼性白血球_Baso { get; set; }
    public string 單核球_Mono { get; set; }
    public string 淋巴球_Lymph { get; set; }
}





