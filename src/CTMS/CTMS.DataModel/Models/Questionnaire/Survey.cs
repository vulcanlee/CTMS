using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CTMS.DataModel.Models.Questionnaire;

public class Survey
{
    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("questions")]
    public List<Question> Questions { get; set; }
}

public class Question
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("text")]
    public string Text { get; set; }

    [JsonPropertyName("options")]
    public List<Option>? Options { get; set; }

    [JsonPropertyName("visibilityCondition")]
    public VisibilityCondition? VisibilityCondition { get; set; }
}

public class Option
{
    [JsonPropertyName("value")]
    public int Value { get; set; }

    [JsonPropertyName("label")]
    public string Label { get; set; }
}

public class VisibilityCondition
{
    [JsonPropertyName("questionId")]
    public string QuestionId { get; set; }

    [JsonPropertyName("anyOf")]
    public List<int> AnyOf { get; set; }
}