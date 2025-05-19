using CTMS.Share.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CTMS.DataModel.Models.ClinicalInformation
{
    public class 臨床資料化學治療Node
    {
        public VisitCodeModel VisitCode { get; set; } = new();
        public string SubjectNo { get; set; }
        public List<臨床資料化學治療Item> Items { get; set; } = new();
    }

    public class 臨床資料化學治療Item
    {
        public DateTime TreatmentDate { get; set; }
        public string BSA { get; set; }
        public string RegimenPaclitaxel { get; set; }
        public string RegimenCarboplatin { get; set; }
        public string Reduction { get; set; }
        public string Bevacizumab { get; set; }
    }
}
