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

        public string ToJson()
        {
            return System.Text.Json.JsonSerializer.Serialize(this);
        }

        public void FromJson(string json)
        {
            var data = System.Text.Json.JsonSerializer.Deserialize<PatientData>(json);
            if (data != null)
            {
                this.臨床資訊 = data.臨床資訊;
            }
        }
    }
}
