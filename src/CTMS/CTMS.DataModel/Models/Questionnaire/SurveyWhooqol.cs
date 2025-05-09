using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CTMS.DataModel.Models.Questionnaire;

public class SurveyWhooqol
{
    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("formLabel")]
    public string FormLabel { get; set; }

    [JsonPropertyName("code")]
    public string Code { get; set; }

    [JsonPropertyName("questionLabel")]
    public string QuestionLabel { get; set; }

    [JsonPropertyName("questions")]
    public List<Question> Questions { get; set; }
}

public class QuestionWhooqol
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("text")]
    public string Text { get; set; }

    [JsonPropertyName("options")]
    public List<Option> Options { get; set; }
}

public class OptionWhooqol
{
    [JsonPropertyName("value")]
    public int Value { get; set; }

    [JsonPropertyName("label")]
    public string Label { get; set; }
}