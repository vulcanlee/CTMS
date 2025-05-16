using CTMS.Share.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTMS.DataModel.Models.ClinicalInformation
{
    public class 檢驗項目
    {
        public string 項目名稱 { get; set; }
        public string 參考區間 { get; set; }
        public string 檢驗數值 { get; set; }
        public DateTime? SamplingDate { get; set; }

    }
}
