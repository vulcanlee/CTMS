using CTMS.Share.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CTMS.DataModel.Models.ClinicalInformation
{
    public class 臨床資料病理報告
    {
        public string VisitCode { get; set; } 
        public DateTime AssessmentDate { get; set; }
        public string SubjectNo { get; set; }
        public DateTime 切片日期 { get; set; }
        public string Histology { get; set; }
        public string TnmStage { get; set; }
        public string Myometrium { get; set; }
        public string UterineSerosaInvolvement        { get; set; }
        public string BloodLymphaticVesselInvasion { get; set; }
        public string Cervix { get; set; }
        public string Parametrium { get; set; }
        public string OvaryRight { get; set; }
        public string OvaryLeft { get; set; }
        public string FallopianTubeRight { get; set; }
        public string FallopianTubeLeft { get; set; }
        public string Vagina { get; set; }
        public string RegionalLymphNodes { get; set; }
        public string IsolatedTumorCells { get; set; }
        public string AdditionalPathologicalFindings { get; set; }
        public string ImmunohistochemicalTest  { get; set; }

    }
}
