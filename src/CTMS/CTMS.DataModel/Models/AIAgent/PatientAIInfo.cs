using CTMS.DataModel.Models.ClinicalInformation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTMS.DataModel.Models.AIAgent;

public class PatientAIInfo
{
    public string KeyName { get; set; }
    public string Code { get; set; }
    public string SubjectCode { get; set; }
    public string Height { get; set; }
    public string Weight { get; set; }
    public string Age { get; set; }
    public string Gender { get; set; }
    public string 癌別 { get; set; }
    public string DicomFilename { get; set; }
    public string DestionatioDicomFilename { get; set; }
    public string DestionatioPatientJSONFilename { get; set; }
    public string ManualAnnotation { get; set; }

    public void InitKeyName()
    {
        var now = DateTime.Now;

        // 1 tick = 100ns => 10 ticks = 1µs，取當前秒內的微秒，再取前兩位
        int microseconds = (int)((now.Ticks % TimeSpan.TicksPerSecond) / 1000);

        KeyName = $"{now:yyyyMMddHHmmss}{microseconds:0000}";
    }
    public string ToJson()
    {
        var options = new System.Text.Json.JsonSerializerOptions
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All)
        };
        return System.Text.Json.JsonSerializer.Serialize(this, options);
    }

    public PatientAIInfo FromJson(string json)
    {
        var data = System.Text.Json.JsonSerializer.Deserialize<PatientAIInfo>(json);
        return data;
    }

}
