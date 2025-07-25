﻿using CTMS.DataModel.Models.ClinicalInformation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTMS.DataModel.Models.Systems;

public class BrowseSearchingModel
{
    public List<string> 院別 { get; set; } = new List<string>();
    public List<string> 癌別 { get; set; } = new List<string>();
    public string SearchKeyword { get; set; } = string.Empty;
}
