using CTMS.Share.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTMS.DataModel.Models.ClinicalInformation
{
    public class TestItem檢驗項目
    {
        public string 項目名稱 { get; set; }
        public string 參考區間 { get; set; }
        public double 參考區間開始 { get; set; }
        public double 參考區間結束 { get; set; }
        public string 參考區間類型 { get; set; }
        public string 檢驗數值 { get; set; }
        public DateTime? SamplingDate { get; set; }
        public string TextClassName { get; set; } = MagicObjectHelper.正常檢驗數計類別;

    }
}
