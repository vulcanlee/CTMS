using CTMS.Share.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CTMS.DataModel.Models.ClinicalInformation
{
    public class 臨床資料合併用藥
    {
        public string VisitCode { get; set; }
        public string SubjectNo { get; set; }
        public List<臨床資料合併用藥Item> Items { get; set; } = new();
    }

    public class 臨床資料合併用藥Item
    {
        public int Index { get; set; }
        public DateTime TreatmentDate { get; set; }
        public string Durg { get; set; }
        public string Dose { get; set; }
        public string RouteCode { get; set; }
        public string UnitCode { get; set; }
    }
}
