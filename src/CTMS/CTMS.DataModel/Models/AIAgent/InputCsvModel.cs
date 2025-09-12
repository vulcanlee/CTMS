using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTMS.DataModel.Models.AIAgent;


public class InputCsvModel
{
    //依據此 CSV 建立起該類別屬性
    //ID,Age,Tumor.Grade,body.height.cm,body.weight.kg,Vertebral.Body.Area.cm2,Total.SMD,Total.ImatA,Total.LamaA,Total.NamaA,VatA,SatA
    public string ID { get; set; }
    public string Age { get; set; }
    public string Tumor_Grade { get; set; }
    public string body_height_cm { get; set; }
    public string body_weight_kg { get; set; }
    public string Vertebral_Body_Area_cm2 { get; set; }
    public string Total_SMD { get; set; }
    public string Total_ImatA { get; set; }
    public string Total_LamaA { get; set; }
    public string Total_NamaA { get; set; }
    public string VatA { get; set; }
    public string SatA { get; set; }


    public string ToJson()
    {
        var options = new System.Text.Json.JsonSerializerOptions
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All)
        };
        return System.Text.Json.JsonSerializer.Serialize(this, options);
    }

    public InputCsvModel FromJson(string json)
    {
        var data = System.Text.Json.JsonSerializer.Deserialize<InputCsvModel>(json);
        return data;
    }

}
