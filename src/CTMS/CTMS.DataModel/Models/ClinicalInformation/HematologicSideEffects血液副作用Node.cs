using CTMS.Share.Helpers;
using System.Drawing;

namespace CTMS.DataModel.Models.ClinicalInformation
{
    public class HematologicSideEffects血液副作用Node
    {
        public VisitCodeModel VisitCode { get; set; } = new();
        public List<HematologicSideEffectsItem> WhiteBloodCell白血球 { get; set; } = new();
        public List<HematologicSideEffectsItem> NeutrophilCount絕對嗜中性白血球數 { get; set; } = new();
        public List<HematologicSideEffectsItem> HemoglobinHb血色素 { get; set; } = new();
        public List<HematologicSideEffectsItem> PlateletCount血小板 { get; set; } = new();
    }

    public class HematologicSideEffectsItem
    {
        public string Title { get; set; }
        public GradeItem Grade1 { get; set; } = new();
        public GradeItem Grade2 { get; set; } = new();
        public GradeItem Grade3 { get; set; } = new();
        public GradeItem Grade4 { get; set; } = new();
        public GradeItem Grade5 { get; set; } = new();
    }

    public class GradeItem
    {
        public string GradeValue1 { get; set; } = string.Empty;
        public string GradeValue2 { get; set; } = string.Empty;
        public string RetriveValue { get; set; } = string.Empty;
        public string ApplyCssClass { get; set; } = MagicObjectHelper.NotFoundClass;
    }
}
