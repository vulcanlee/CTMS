using CTMS.Share.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CTMS.DataModel.Models.ClinicalInformation
{
    public class OtherTreatmentNode
    {
        public VisitCodeModel VisitCode { get; set; } = new();
        public string SubjectNo { get; set; }
        public List<OtherTreatmentItem> Items { get; set; } = new();
    }

    public class OtherTreatmentItem
    {
        public DateTime Visit { get; set; }
        public string Treatment { get; set; }
        public string Lab { get; set; }
        public string Image { get; set; }
        public string Others { get; set; }
        public string First { get; set; }
    }
}
