using CTMS.Share.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CTMS.DataModel.Models.ClinicalInformation
{
    public class OtherTreatmentImageNode
    {
        public VisitCodeModel VisitCode { get; set; } = new();
        public string SubjectNo { get; set; }
        public OtherTreatmentImageItem Item { get; set; } = new();
    }

    public class OtherTreatmentImageItem
    {
        public string First { get; set; }
        public string ChestXRay { get; set; }
        public string LeadEKG12 { get; set; }
        public string ChestCT { get; set; }
        public string AbdCT { get; set; }
        public string BrainMRI { get; set; }
        public string BoneScan { get; set; }
        public DateTime? ChestXRayDate { get; set; }
        public DateTime? LeadEKG12Date { get; set; }
        public DateTime? ChestCTDate { get; set; }
        public DateTime? AbdCTDate { get; set; }
        public DateTime? BrainMRIDate { get; set; }
        public DateTime? BoneScanDate { get; set; }
    }
}
