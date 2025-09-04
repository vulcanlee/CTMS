using CTMS.Share.Extensions;
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
        public string SheetName { get; set; } = string.Empty;
        public string RandomId { get; set; } = string.Empty;
        public string Treatment { get; set; } = string.Empty;
    }
}
