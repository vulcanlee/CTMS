using System.Drawing;
using System.Text;
using System;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;

namespace CTMS.DataModel.Models;

public class 抽血檢驗_生化Model
{
    public string 血中尿素氮_BUN { get; set; }
    public string 天門冬胺酸轉胺酶_AST { get; set; }
    public string 丙胺酸轉胺酶_ALT { get; set; }
    public string 膽紅素總量_Total_Bilirubin { get; set; }
    public string 白蛋白_球蛋白_AG { get; set; }
    public string 總膽固醇_CHOL { get; set; }
    public string 高密度脂蛋白膽固醇_HDL { get; set; }
    public string 低密度脂蛋白膽固醇_LDL { get; set; }
    public string 三酸甘油脂_TG { get; set; }
    public string 醣化血紅素_HbA1C { get; set; }
}





