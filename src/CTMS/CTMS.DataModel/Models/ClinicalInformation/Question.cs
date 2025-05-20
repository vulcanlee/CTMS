using CTMS.Share.Helpers;

namespace CTMS.DataModel.Models.ClinicalInformation
{
    public class Question
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Text { get; set; }
        public string Answer { get; set; }=string.Empty;
        public List<Option> Options { get; set; }= new();
        public VisibilityCondition? VisibilityCondition { get; set; } = new();
        public bool IsVisible { get; set; } = true;
    }
    public class Option
    {
        public int Value { get; set; }
        public string Label { get; set; }
        public bool HasCheck { get; set; } = false;
        public string CheckBoxIcon { get; set; } = MagicObjectHelper.CheckBoxBlankIcon;
    }

    public class VisibilityCondition
    {
        public string QuestionId { get; set; }
        public List<int> AnyOf { get; set; }
    }
}
