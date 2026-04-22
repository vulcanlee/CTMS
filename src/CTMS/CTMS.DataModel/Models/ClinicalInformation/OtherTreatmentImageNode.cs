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
        public List<OtherTreatmentImageItem> Items { get; set; } = new();
    }

    public class OtherTreatmentImageItem
    {
        public string ExecuteTime { get; set; }
        public string OrderCode { get; set; }
        public string OrderName { get; set; }
        public string ReportText { get; set; }
    }
}
