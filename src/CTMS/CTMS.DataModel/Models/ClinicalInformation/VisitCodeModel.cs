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
    public class VisitCodeModel
    {
        public string VisitCode { get; set; } = string.Empty;
        public DateTime? VisitCodeDate { get; set; }
        public string Timeline { get; set; }
        public int CycleMonth { get; set; }
        public DateTime AssessmentDate { get; set; }
        public string VisitCodeIconA { get; set; } = MagicObjectHelper.CheckBoxBlankIcon;
        public string VisitCodeIconB { get; set; } = MagicObjectHelper.CheckBoxBlankIcon;
        public string VisitCodeIconC { get; set; } = MagicObjectHelper.CheckBoxBlankIcon;

        public void UpdateIcon()
        {
            VisitCodeIconA = MagicObjectHelper.CheckBoxBlankIcon;
            VisitCodeIconB = MagicObjectHelper.CheckBoxBlankIcon;
            VisitCodeIconC = MagicObjectHelper.CheckBoxBlankIcon;
            if (VisitCode == "A") VisitCodeIconA = MagicObjectHelper.CheckBoxIcon;
            else if (VisitCode == "B") VisitCodeIconB = MagicObjectHelper.CheckBoxIcon;
            else if (VisitCode == "C") VisitCodeIconC = MagicObjectHelper.CheckBoxIcon;
        }
    }
}
