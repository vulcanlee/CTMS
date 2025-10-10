using CTMS.DataModel.Models;
using CTMS.EntityModel.Models;
using CTMS.Share.Helpers;
using System.ComponentModel.DataAnnotations;

namespace CTMS.AdapterModels;

public class PatientAdapterModel : ICloneable
{
    public int Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string 醫院 { get; set; }
    public string 癌別 { get; set; }
    public string 組別 { get; set; }
    public string AI評估 { get; set; }
    public string AI處理 { get; set; }
    public string JsonData { get; set; }

    public PatientAdapterModel Clone()
    {
        return ((ICloneable)this).Clone() as PatientAdapterModel;
    }
    object ICloneable.Clone()
    {
        return this.MemberwiseClone();
    }

    public string Get組別DisplayName()
    {
        if (組別 == MagicObjectHelper.組別對照組英文)
            return MagicObjectHelper.組別對照組中文;
        else if (組別 == MagicObjectHelper.組別實驗組英文)
            return MagicObjectHelper.組別實驗組中文;
        else
            return MagicObjectHelper.NA;
    }
}
