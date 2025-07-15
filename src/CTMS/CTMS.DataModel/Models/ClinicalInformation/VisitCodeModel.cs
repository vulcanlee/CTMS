using CTMS.DataModel.Dtos;
using CTMS.Share.Extensions;
using CTMS.Share.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CTMS.DataModel.Models.ClinicalInformation
{
    public class VisitCodeModel :ICloneable
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public DateTime? AssessmentDate { get; set; }
        public string Timeline { get; set; }
        public int CycleMonth { get; set; }

        public VisitCodeModel Clone()
        {
            return ((ICloneable)this).Clone() as VisitCodeModel;
        }
        object ICloneable.Clone()
        {
            return this.MemberwiseClone();
        }

        public bool CompareTo(VisitCodeModel other)
        {
            if (other == null) return false;
            if (this.AssessmentDate.HasValue && other.AssessmentDate.HasValue)
            {
                return this.AssessmentDate.Value.Date == other.AssessmentDate.Value.Date &&
                       this.Timeline == other.Timeline &&
                       this.CycleMonth == other.CycleMonth;
            }
            else
            {
                return this.Timeline == other.Timeline &&
                       this.CycleMonth == other.CycleMonth;
            }
        }

        public string VisitCodeTitle
        {
            get { return GetVisitCodeTitle(); }
        }

        public string GetVisitCodeTitle()
        {
            // 這個病人來的visit code 
            // Visit code 最後會顯示 ABC 比如說這個病人是2025 / -5 - 17 來: A: 5 - 17 - 2025, B: baseline C cycle 0
            string result = string.Empty;
            if (AssessmentDate.HasValue)
                result = "A: " + AssessmentDate.Value.ToString("yyyy-MM-dd") + " ";
            else
                result = "A: ";
            result = result + $"B: {Timeline} C: Cycle {CycleMonth}";
            return result;
        }

        public string GetTimelineCheckboxIcon(string usingTimeline)
        {
            if (usingTimeline == Timeline)
            {
                return MagicObjectHelper.CheckBoxIcon;
            }
            else
            {
                return MagicObjectHelper.CheckBoxBlankIcon;
            }
        }
    }
}
