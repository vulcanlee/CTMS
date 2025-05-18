using CTMS.Share.Extensions;
using CTMS.Share.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTMS.DataModel.Models.ClinicalInformation
{
    public class Survey
    {
        public string Title { get; set; }
        public List<Question> Questions { get; set; }=new();
    }

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
