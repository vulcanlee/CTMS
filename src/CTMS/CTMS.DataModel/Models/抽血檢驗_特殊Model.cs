using System.Drawing;
using System.Text;
using System;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;

namespace CTMS.DataModel.Models;

public class 抽血檢驗_特殊Model
{
    public 抽血檢驗_特殊ItemModel 運動前 { get; set; } = new();
    public 抽血檢驗_特殊ItemModel 運動後15分鐘 { get; set; } = new();
    public 抽血檢驗_特殊ItemModel 運動後30分鐘 { get; set; } = new();
}

public class 抽血檢驗_特殊ItemModel
{
    public string 肌酸酐_CR { get; set; }
    public string 乳酸脫氫酶_LDH { get; set; }
    public string 葡萄糖_Glucose { get; set; }
    public string 鹼性磷酸酶_ALKP { get; set; }
    public string 醣化血紅素_HbA1C { get; set; }
    public string 高敏感C反應性蛋白質_HS_CRP { get; set; }
    public string 結合蛋白_Haptoglobin { get; set; }
    public string 血清前白蛋白_Prealbumin { get; set; }
    public string 胰島素_Insulin { get; set; }
    public string 類胰島素成長因子_IGF_1 { get; set; }
    public string C_胜鏈胰島素_C_peptide { get; set; }
    public string 介白素_6_IL_6 { get; set; }
    public string 皮質醇_Cortisol { get; set; }

}





