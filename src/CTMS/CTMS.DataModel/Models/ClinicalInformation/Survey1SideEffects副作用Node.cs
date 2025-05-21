using CTMS.Share.Helpers;
using System.Drawing;

namespace CTMS.DataModel.Models.ClinicalInformation
{
    public class Survey1SideEffects副作用Node
    {
        public VisitCodeModel VisitCode { get; set; } = new();
        public GradeItemSideEffectsItem Nausea噁心 { get; set; } = new();
        public GradeItemSideEffectsItem Vomiting嘔吐 { get; set; } = new();
        public GradeItemSideEffectsItem MucositisOral口腔炎 { get; set; } = new();
        public GradeItemSideEffectsItem Diarrhea拉肚子 { get; set; } = new();
        public GradeItemSideEffectsItem Constipation便秘 { get; set; } = new();
        public GradeItemSideEffectsItem Anorexia食慾不振 { get; set; } = new();
    }
}
