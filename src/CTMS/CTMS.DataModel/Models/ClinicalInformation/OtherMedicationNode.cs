using CTMS.Share.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CTMS.DataModel.Models.ClinicalInformation
{
    public class OtherMedicationNode
    {
        public VisitCodeModel VisitCode { get; set; } = new();
        public string SubjectNo { get; set; }
        public List<OtherMedicationItem> Items { get; set; } = new();
    }

    public class OtherMedicationItem
    {
        public string Order_Code { get; set; }
        public string Pharmacy_Name { get; set; }
        public string Frequency_Code { get; set; }
        public string Totally_Dosage_Unit { get; set; }
        public string Dosage_Unit { get; set; }
        public string Usage_Code { get; set; }
        public string Order_Effect_Date { get; set; }
        public string Order_End_Date { get; set; }
    }
}
