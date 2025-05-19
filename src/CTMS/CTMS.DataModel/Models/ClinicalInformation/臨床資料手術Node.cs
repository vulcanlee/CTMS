using CTMS.Share.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CTMS.DataModel.Models.ClinicalInformation
{
    public class 臨床資料手術Node
    {
        public VisitCodeModel VisitCode { get; set; }=new();
        public string SubjectNo { get; set; }
        public DateTime 手術日期 { get; set; }
        public string 術式 { get; set; }
        public string OPOutcome { get; set; }
        public string Ascites { get; set; }
        public string Uterus { get; set; }
        public string UterusSite { get; set; }
        public string UterusTumorNumber { get; set; }
        public string UterusTumorSize { get; set; }
        public string Cervix { get; set; }
        public string CervixSite { get; set; }
        public string CervixTumorNumber { get; set; }
        public string Endometrium { get; set; }
        public string Myometrium { get; set; }
        public string CulDeSac { get; set; }
        public string OvarianSurfaceRuptureOrNotRightOvary { get; set; }
        public string OvarianSurfaceRuptureOrNotLeftOvary { get; set; }
        public string LeftAdnexa { get; set; }
        public string LeftAdnexaTumorNumber { get; set; }
        public string LeftAdnexaTumorSize { get; set; }
        public string RightAdnexa { get; set; }
        public string RightAdnexaTumorNumber { get; set; }
        public string RightAdnexaTumorSize { get; set; }
        public string PelvicPeritonealCavity { get; set; }
        public string PelvicPeritonealCavityTumorSize { get; set; }
        public string ExtrapelvicPeritonealCavity { get; set; }
        public string ExtrapelvicPeritonealCavityOtherFinding { get; set; }
        public string OtherOrganInvolvementGrossLooking { get; set; }
        public string Optimal { get; set; }
        public string ResidualTumor { get; set; }

    }
}
