using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CTMS.DataModel.Models.AIAgent;

public class RiskAssessmentAIResult
{
    /// <summary>
    /// Excel (B2)  
    /// </summary>
    public string ID { get; set; }
    /// <summary>
    ///  Excel (D2)
    /// </summary>
    public string Age { get; set; }
    /// <summary>
    ///  Excel (F2)   體重    kg    通常來自量測或資料庫，不由影像計算
    /// </summary>
    public string BodyHeight { get; set; }
    /// <summary>
    ///  Excel (G2)
    /// </summary>
    public string BodyWeight { get; set; }
    /// <summary>
    ///  Excel (B41)   椎體截面積    cm²   由 AI 偵測出椎體區域後自動計算
    /// </summary>
    public string VertebralBodyAreaCm2 { get; set; }
    /// <summary>
    ///  Excel (C13)   Skeletal Muscle Density    HU(Hounsfield Unit)    骨骼肌密度    取肌肉區域 CT 像素平均值
    /// </summary>
    public string TotalSMD { get; set; }
    /// <summary>
    ///  Excel (I13)    Intermuscular Adipose Tissue Area cm²  肌間/肌內脂肪組織面積    分割肌肉區域間的脂肪像素面積總和
    /// </summary>
    public string TotalImatA { get; set; }
    /// <summary>
    ///  Excel (N13)    Low Attenuation Muscle Area    cm²   低密度肌肉面積   HU 介於 -29～29 的肌肉區域面積
    /// </summary>
    public string TotalLamaA { get; set; }
    /// <summary>
    ///  Excel (S13)    Normal Attenuation Muscle Area    cm²    正常密度肌肉面積    HU 介於 30～150 的肌肉區域面積
    /// </summary>
    public string TotalNamaA { get; set; }
    /// <summary>
    ///  Excel (B13)    Visceral Adipose Tissue Area    cm²   內臟脂肪面積，由影像自動分割
    /// </summary>
    public string VatA { get; set; }
    /// <summary>
    ///  Excel (G13)    Subcutaneous Adipose Tissue Area    cm²   皮下脂肪面積
    /// </summary>
    public string SatA { get; set; }
}