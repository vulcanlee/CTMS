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
        public string Drug { get; set; }
        public DateTime? TreatmentDate  { get; set; }
        public string dose { get; set; }
        public string RouteCode { get; set; }
        public string UnitCode { get; set; }
    }
}
