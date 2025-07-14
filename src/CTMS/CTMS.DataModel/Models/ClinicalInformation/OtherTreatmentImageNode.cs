using CTMS.Share.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CTMS.DataModel.Models.ClinicalInformation
{
    public class OtherTreatmentImageNode
    {
        public VisitCodeModel VisitCode { get; set; } = new();
        public string SubjectNo { get; set; }
        public List<OtherTreatmentImageItem> Items { get; set; } = new();
    }

    public class OtherTreatmentImageItem
    {
        public string First { get; set; }
        public DateTime Visit { get; set; }
        public string ChestXRay { get; set; }
        public string LeadEKG12 { get; set; }
        public string ChestCT { get; set; }
        public string AbdCT { get; set; }
        public string BrainMRI { get; set; }
        public string BoneScan { get; set; }
        public string Other { get; set; }
    }
}
