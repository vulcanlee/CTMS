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
    public class RandomParameterMode
    {
        public string SubjectNo { get; set; } = string.Empty;
        public string FIGO { get; set; } = string.Empty;
        public string RandomId { get; set; } = string.Empty;
        public string Treatment { get; set; } = string.Empty;
        public string EarlyOrAdvance { get; set; } = string.Empty;
        public string Hospital { get; set; } = string.Empty;
        public string ECorOC { get; set; } = string.Empty;

        public void Parse()
        {
            if (string.IsNullOrEmpty(FIGO) == false)
            {
                if (FIGO.StartsWith("IV") || FIGO.StartsWith("III"))
                    EarlyOrAdvance = "Advance";
                else if (FIGO.StartsWith("II") || FIGO.StartsWith("I"))
                    EarlyOrAdvance = "Early";
            }

            if (SubjectNo.Contains(MagicObjectHelper.prefix奇美醫院))
            {
                Hospital = MagicObjectHelper.prefix奇美醫院;
            }
            else if (SubjectNo.Contains(MagicObjectHelper.prefix郭綜合醫院))
            {
                Hospital = MagicObjectHelper.prefix郭綜合醫院;
            }
            else if (SubjectNo.Contains(MagicObjectHelper.prefix成大醫院))
            {
                Hospital = MagicObjectHelper.prefix成大醫院;
            }
        }
    }
}
