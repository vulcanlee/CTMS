using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTMS.DataModel.Models.ClinicalInformation
{
    public class PatientData
    {
        public BasicClinicalPresentation_臨床資訊 臨床資訊 { get; set; } = new();
        public 臨床資料 臨床資料 { get; set; } = new();

        public string ToJson()
        {
            var options = new System.Text.Json.JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All)
            };
            return System.Text.Json.JsonSerializer.Serialize(this, options);
        }

        public void FromJson(string json)
        {
            var data = System.Text.Json.JsonSerializer.Deserialize<PatientData>(json);
            if (data != null)
            {
                this.臨床資訊 = data.臨床資訊;
                this.臨床資料 = data.臨床資料;
            }
            SyncData();
        }

        public void SyncData()
        {
            this.臨床資料.臨床資料手術.SubjectNo = this.臨床資訊.SubjectNo;
        }
    }
}
