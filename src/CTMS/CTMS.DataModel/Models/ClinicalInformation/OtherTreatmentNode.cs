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

        public void BuildItem()
        {
            OtherTreatmentItem item;
            item = new OtherTreatmentItem
            {
                First = "Admission",
                Visit = null,
                Treatment = "",
                Lab = "",
                Image = "",
                Others = ""
            };
            Items.Add(item);
            item = new OtherTreatmentItem
            {
                First = "ER",
                Visit = null,
                Treatment = "",
                Lab = "",
                Image = "",
                Others = ""
            };
            Items.Add(item);
            item = new OtherTreatmentItem
            {
                First = "Clinics",
                Visit = null,
                Treatment = "",
                Lab = "",
                Image = "",
                Others = ""
            };
            Items.Add(item);
        }
    }

    public class OtherTreatmentItem
    {
        public string First { get; set; }
        public DateTime? Visit { get; set; }
        public string Treatment { get; set; }
        public string Lab { get; set; }
        public string Image { get; set; }
        public string Others { get; set; }
    }
}
