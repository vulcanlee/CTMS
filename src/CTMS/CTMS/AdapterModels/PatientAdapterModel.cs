﻿using CTMS.DataModel.Models;
using CTMS.EntityModel.Models;
using System.ComponentModel.DataAnnotations;

namespace CTMS.AdapterModels;

public class PatientAdapterModel : ICloneable
{
    public int Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string 醫院 { get; set; }
    public string 癌別 { get; set; }
    public string JsonData { get; set; }

    public PatientAdapterModel Clone()
    {
        return ((ICloneable)this).Clone() as PatientAdapterModel;
    }
    object ICloneable.Clone()
    {
        return this.MemberwiseClone();
    }
}
