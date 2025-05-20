using CTMS.Share.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTMS.DataModel.Models.ClinicalInformation
{
    public class Main臨床資料
    {
        public 臨床資料手術 臨床資料手術 { get; set; } = new();
        public 臨床資料病理報告 臨床資料病理報告 { get; set; } = new();
        public 臨床資料化學治療 臨床資料化學治療 { get; set; } = new();
        public 臨床資料合併用藥 臨床資料合併用藥 { get; set; } = new();
        public BaselineMedicalHistoryForm BaselineMedicalHistoryForm { get; set; } = new();
        public BloodTest抽血檢驗血液 抽血檢驗血液 { get; set; } = new();
        public BloodTest抽血檢驗生化 抽血檢驗生化 { get; set; } = new();
        public Survey問卷 問卷 { get; set; } = new();
    }
}
