using CTMS.Share.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CTMS.DataModel.Models.ClinicalInformation
{
    public class 臨床資料BaselineMedicalHistoryForm
    {
        public string VisitCode { get; set; }
        public DateTime AssessmentDate { get; set; }
        public string SubjectNo { get; set; }
        public string CardiovascularIncludeHtn { get; set; }
        public string PeripheralVascular { get; set; }
        public string Respiratory { get; set; }
        public string Gastrointestinal { get; set; }
        public string Renal { get; set; }
        public string Genitourinary { get; set; }
        public string EndocrineMetabolicIncludeDiabetes { get; set; }
        public string HematologicLymphatic { get; set; }
        public string Musculoskeletal { get; set; }
        public string Dermatologic { get; set; }
        public string DrugAbuse { get; set; }
        public string Tobacco { get; set; }
        public string Neurologic { get; set; }
        public string Psychiatric { get; set; }
        public string Allergies { get; set; }
        public string Neoplasia { get; set; }
        public string AlcoholUse { get; set; }
        public string ImmunityIncludeHiv { get; set; }
        public string HepatobiliaryIncludeHbvHcv { get; set; }
        public string OtherSpecify { get; set; }
    }
}
