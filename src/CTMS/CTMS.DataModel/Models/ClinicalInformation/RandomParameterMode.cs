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
        /// <summary>
        /// 癌症分期(2023 FIGO)
        /// </summary>
        public string FIGO { get; set; } = string.Empty;
        public string RandomId { get; set; } = string.Empty;
        public string Treatment { get; set; } = string.Empty;
        /// <summary>
        /// 早期(1,2期) / 晚期(3,4期)
        /// </summary>
        public string EarlyOrAdvance { get; set; } = string.Empty;
        /// <summary>
        /// 醫院
        /// </summary>
        public string Hospital { get; set; } = string.Empty;
        /// <summary>
        /// 癌別
        /// </summary>
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

            // 依 prefix 反查為「院區群組層級」：柳營奇美視為奇美，抽奇美的隨機表。
            var owner = HospitalRegistry.GetOwnerBySubjectNo(SubjectNo);
            if (owner != null)
            {
                Hospital = owner.Prefix;
            }
        }
    }
}
