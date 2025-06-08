using CTMS.Share.Helpers;
using System.Drawing;

namespace CTMS.DataModel.Models.ClinicalInformation
{
    public class HematologicSideEffects血液副作用Node
    {
        public VisitCodeModel VisitCode { get; set; } = new();
        public GradeItemSideEffectsItem WhiteBloodCell白血球 { get; set; } = new();
        public GradeItemSideEffectsItem NeutrophilCount絕對嗜中性白血球數 { get; set; } = new();
        public GradeItemSideEffectsItem HemoglobinHb血色素 { get; set; } = new();
        public GradeItemSideEffectsItem PlateletCount血小板 { get; set; } = new();
    }

    public class GradeItemSideEffectsItem
    {
        public string RetriveValue { get; set; } = string.Empty;
        public GradeItem Grade1 { get; set; } = new();
        public GradeItem Grade2 { get; set; } = new();
        public GradeItem Grade3 { get; set; } = new();
        public GradeItem Grade4 { get; set; } = new();
        public GradeItem Grade5 { get; set; } = new();
    }

    public class GradeItem
    {
        public string Title { get; set; }
        public string GradeValue1 { get; set; } = string.Empty;
        public string GradeValue2 { get; set; } = string.Empty;
        public string ApplyCssClass { get; set; } = MagicObjectHelper.NotFoundClass;

        public void ResetCssClassNotFound()
        {
            ApplyCssClass = MagicObjectHelper.NotFoundClass;
        }

        public void ResetCssClassFound()
        {
            ApplyCssClass = MagicObjectHelper.FoundClass;
        }

        public void ResetCssClassFoundLow()
        {
            ApplyCssClass = MagicObjectHelper.FoundLowClass;
        }

        public void ResetCssClassFoundHeigh()
        {
            ApplyCssClass = MagicObjectHelper.FoundHeighClass;
        }
    }
}
