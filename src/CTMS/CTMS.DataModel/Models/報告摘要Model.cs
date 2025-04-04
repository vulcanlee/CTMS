namespace CTMS.DataModel.Models;

public class 報告摘要Model
{
    public bool 前測結果摘要 { get; set; } = false;
    public bool 後測結果摘要 { get; set; } = false;
    public string 基因體學檢查分析結果 { get; set; }
    public string 肌肉組成檢查分析結果 { get; set; }
    public string 代謝失調風險分析結果 { get; set; }
    public string 運動代謝體分析結果 { get; set; }
    public string 生化檢查結果分析結果 { get; set; }

    public string 基因體學檢查建議 { get; set; }
    public string 肌肉組成檢查建議 { get; set; }
    public string 代謝失調風險建議 { get; set; }
    public string 運動代謝體建議 { get; set; }
    public string 生化檢查結果建議 { get; set; }

    public string 解說醫師 { get; set; }
    public string 輸入日期 { get; set; }

    public void CopyFrom(報告摘要Model source)
    {
        this.前測結果摘要 = source.前測結果摘要;
        this.後測結果摘要 = source.後測結果摘要;
        this.基因體學檢查分析結果 = source.基因體學檢查分析結果;
        this.肌肉組成檢查分析結果 = source.肌肉組成檢查分析結果;
        this.代謝失調風險分析結果 = source.代謝失調風險分析結果;
        this.運動代謝體分析結果 = source.運動代謝體分析結果;
        this.生化檢查結果分析結果 = source.生化檢查結果分析結果;

        this.基因體學檢查建議 = source.基因體學檢查建議;
        this.肌肉組成檢查建議 = source.肌肉組成檢查建議;
        this.代謝失調風險建議 = source.代謝失調風險建議;
        this.運動代謝體建議 = source.運動代謝體建議;
        this.生化檢查結果建議 = source.生化檢查結果建議;

        this.解說醫師 = source.解說醫師;
        this.輸入日期 = source.輸入日期;
    }
}
