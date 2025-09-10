using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTMS.DataModel.Models.AIAgent;


public class Phase1LabelGeneration
{
    public Phase1LabelGeneration()
    {
        GPU = "";
        optional = new();
        files = new();
        tmp_folder = "";
    }

    public string GPU { get; set; }
    public PhaseOptional optional { get; set; } = new();
    public List<string> files { get; set; }
    public string tmp_folder { get; set; }

    public string ToJson()
    {
        var options = new System.Text.Json.JsonSerializerOptions
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All)
        };
        return System.Text.Json.JsonSerializer.Serialize(this, options);
    }

    public Phase1LabelGeneration FromJson(string json)
    {
        var data = System.Text.Json.JsonSerializer.Deserialize<Phase1LabelGeneration>(json);
        return data;
    }

}

public class PhaseOptional
{
    public List<string> height { get; set; }
    public List<string> weight { get; set; }
    public List<string> gender { get; set; }
    public List<string> age { get; set; }
}

public class Phase2QuantitativeAnalysis : Phase1LabelGeneration
{
    public List<string> jsons { get; set; }


    public string ToJson()
    {
        var options = new System.Text.Json.JsonSerializerOptions
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All)
        };
        return System.Text.Json.JsonSerializer.Serialize(this, options);
    }

    public Phase2QuantitativeAnalysis FromJson(string json)
    {
        var data = System.Text.Json.JsonSerializer.Deserialize<Phase2QuantitativeAnalysis>(json);
        return data;
    }
}