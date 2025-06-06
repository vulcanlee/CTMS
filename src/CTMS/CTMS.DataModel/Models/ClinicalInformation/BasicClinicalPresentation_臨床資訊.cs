using CTMS.Share.Extensions;
using CTMS.Share.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTMS.DataModel.Models.ClinicalInformation;
public class MMRProteinSetting :ICloneable
{
    public ProteinInfo MSH2 { get; set; } = new();
    public ProteinInfo MSH6 { get; set; } = new();
    public ProteinInfo PMS2 { get; set; } = new();
    public ProteinInfo MLH1 { get; set; } = new();

    public MMRProteinSetting Clone()
    {
        var result =  ((ICloneable)this).Clone() as MMRProteinSetting;
        result.MSH2 = MSH2.Clone();
        result.MSH6 = MSH6.Clone();
        result.PMS2 = PMS2.Clone();
        result.MLH1 = MLH1.Clone();
        return result;
    }
    object ICloneable.Clone()
    {
        return this.MemberwiseClone();
    }

    public string GetProteinResult()
    {
        string MSH2Result = MSH2.GetResult();
        string MSH6Result = MSH6.GetResult();
        string PMS2Result = PMS2.GetResult();
        string MLH1Result = MLH1.GetResult();

        if (MSH2Result == MagicObjectHelper.ProteinUnknown && MSH6Result == MagicObjectHelper.ProteinUnknown &&
            PMS2Result == MagicObjectHelper.ProteinUnknown && MLH1Result == MagicObjectHelper.ProteinUnknown)
        {
            return MagicObjectHelper.ProteinUnknown;
        }
        else if (MSH2Result == MagicObjectHelper.ProteinLoss || MSH6Result == MagicObjectHelper.ProteinLoss ||
                 PMS2Result == MagicObjectHelper.ProteinLoss || MLH1Result == MagicObjectHelper.ProteinLoss)
        {
            return MagicObjectHelper.ProteinLoss;
        }
        else
        {
            return MagicObjectHelper.ProteinPreserve;
        }
    }
}

public class ProteinInfo : ICloneable
{
    public bool? HasLoss { get; set; } = null;
    public bool? HasPreserve { get; set; } = null;

    public void SetLoss()
    {
        bool? item = HasLoss;
        if (item.HasValue == false)
        {
            HasLoss = true;
            HasPreserve = false;
        }
        else if (item.Value == true)
        {
            HasLoss = false;
            HasPreserve = true;
        }
        else
        {
            HasLoss = true;
            HasPreserve = false;
        }
    }

    public void SetPreserve()
    {
        bool? item = HasPreserve;
        if (item.HasValue == false)
        {
            HasPreserve = true;
            HasLoss = false;
        }
        else if (item.Value == true)
        {
            HasPreserve = false;
            HasLoss = true;
        }
        else
        {
            HasPreserve = true;
            HasLoss = false;
        }
    }

    public string GetProteinPreserveIcon()
    {
        if (HasPreserve.HasValue == false)
        {
            return MagicObjectHelper.CheckBoxUnknownIcon;
        }
        else if (HasPreserve.Value == true)
        {
            return MagicObjectHelper.CheckBoxIcon;
        }
        else
        {
            return MagicObjectHelper.CheckBoxBlankIcon;
        }
    }

    public string GetProteinLossIcon()
    {
        if (HasLoss.HasValue == false)
        {
            return MagicObjectHelper.CheckBoxUnknownIcon;
        }
        else if (HasLoss.Value == true)
        {
            return MagicObjectHelper.CheckBoxIcon;
        }
        else
        {
            return MagicObjectHelper.CheckBoxBlankIcon;
        }
    }

    public ProteinInfo Clone()
    {
        return ((ICloneable)this).Clone() as ProteinInfo;
    }
    object ICloneable.Clone()
    {
        return this.MemberwiseClone();
    }

    public string GetResult()
    {
        if (HasLoss == true && HasPreserve == true)
        {
            return MagicObjectHelper.ProteinUnknown;
        }
        else if (HasLoss == true)
        {
            return MagicObjectHelper.ProteinLoss;
        }
        else if (HasPreserve == true)
        {
            return MagicObjectHelper.ProteinPreserve;
        }
        else
        {
            return MagicObjectHelper.ProteinUnknown;
        }
    }
}

public class BasicClinicalPresentation_臨床資訊
{
    // 左側欄位
    /// <summary>
    /// 病人編號
    /// </summary>
    public string SubjectNo { get; set; }
    /// <summary>
    /// 臨床資訊 癌別 EC or OCC(自填)
    /// </summary>
    public string ECorOC { get; set; }
    /// <summary>
    /// 年齡 (Age) 20歲-80歲
    /// </summary>
    public string Age { get; set; }
    /// <summary>
    /// 月經狀態 (0停經, 1未停經)
    /// </summary>
    public string MenstrualStatus { get; set; }
    /// <summary>
    /// 身高(cm) 140cm-180cm
    /// </summary>
    public string Height { get; set; }
    /// <summary>
    /// 體重(kg) 30kg-120kg
    /// </summary>
    public string Weight { get; set; }
    /// <summary>
    /// BMI (Kg/m²)
    /// </summary>
    public string BMI { get; set; }
    /// <summary>
    /// 體表面積(BSA) m²
    /// </summary>
    public string BSA { get; set; }
    /// <summary>
    /// 腰圍 (AC) cm
    /// </summary>
    public string AbdominalCircumference { get; set; }
    /// <summary>
    /// 日常體能狀態(PS) 0, 1, 2
    /// </summary>
    public string PerformanceStatus { get; set; }
    // 右側欄位

    /// <summary>
    /// 癌症狀態 癌別 卵巢/子宮
    /// </summary>
    public string CancerType { get; set; }
    /// <summary>
    /// 癌症分期(2023 FIGO)
    /// </summary>
    public string FIGOStaging { get; set; }
    /// <summary>
    /// AJCC c stage
    /// </summary>
    public string AJCCClinicalStage { get; set; }
    public string AJCCClinicalStageT { get; set; }
    public string AJCCClinicalStageN { get; set; }
    public string AJCCClinicalStageM { get; set; }
    /// <summary>
    /// AJCC p stage
    /// </summary>
    public string AJCCPathologicalStage { get; set; }
    public string AJCCPathologicalStageT { get; set; }
    public string AJCCPathologicalStageN { get; set; }
    public string AJCCPathologicalStageM { get; set; }
    /// <summary>
    /// 組織型態
    /// </summary>
    public string HistologicalType { get; set; }
    /// <summary>
    /// 組織型態 Type I  (2A )or Type II (2B)
    /// </summary>
    public string HistologicalTypeDetail { get; set; }
    /// <summary>
    /// MMR protein
    /// </summary>
    public string MMRProtein { get; set; }
    /// <summary>
    /// MMR protein 蛋白質的設定
    /// </summary>
    public MMRProteinSetting MMRProteinSetting { get; set; } = new();
    /// <summary>
    /// MMR protein Detail
    /// </summary>
    public string MMRProteinDetail { get; set; }
    /// <summary>
    /// p53
    /// </summary>
    public string P53 { get; set; }
    /// <summary>
    /// Hormon status
    /// </summary>
    public string HormonStatus { get; set; }
    /// <summary>
    /// Hormon status Positive Percentage
    /// </summary>
    public string HormonStatusPositivePercentage { get; set; }

    public void BuildStage()
    {
        AJCCClinicalStage = $"c{AJCCClinicalStageT}{AJCCClinicalStageN}{AJCCClinicalStageM}";
        AJCCPathologicalStage = $"p{AJCCPathologicalStageT}{AJCCPathologicalStageN}{AJCCPathologicalStageM}";
    }

    public void CalculateCancerType()
    {
        if (ECorOC == "EC")
        {
            CancerType = "子宮內膜癌";
        }
        else if (ECorOC == "OC")
        {
            CancerType = "卵巢癌";
        }
        else
        {
            CancerType = "未知";
        }
        //ECorOC = CancerType;
    }
    // 計算BMI和BSA的方法
    public void CalculateBMI()
    {
        if (Height.ToDouble() > 0 && Weight.ToDouble() > 0)
        {
            // BMI = 體重(kg) / 身高(m)²
            BMI = (Weight.ToDouble() / Math.Pow(Height.ToDouble() / 100, 2)).ToString("F2");
        }
    }

    public void CalculateBSA()
    {
        if (Height.ToDouble() > 0 && Weight.ToDouble() > 0)
        {
            // BSA = (體重(kg) × 身高(cm)) ÷ 3600 的開根號
            BSA = Math.Sqrt((Weight.ToDouble() * Height.ToDouble()) / 3600).ToString("F2");
        }
    }
    public void CalculateBMIAndBSA()
    {
        CalculateBMI();
        CalculateBSA();
    }
}
