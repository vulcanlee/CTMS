using CTMS.Share.Helpers;
using System.Drawing;

namespace CTMS.DataModel.Models.ClinicalInformation
{
    public class Survey2SideEffects副作用Node
    {
        public VisitCodeModel VisitCode { get; set; } = new();
        public GradeItemSideEffectsItem PeripheralNeuropathy周邊感覺神經異常 { get; set; } = new();
        public GradeItemSideEffectsItem Fatigue疲倦 { get; set; } = new();
        public GradeItemSideEffectsItem SkinRash紅疹 { get; set; } = new();
        public GradeItemSideEffectsItem HandFootSyndrome手足症候群 { get; set; } = new();
        public GradeItemSideEffectsItem Alopecia掉髮 { get; set; } = new();
    }
}
