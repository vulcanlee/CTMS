using System.ComponentModel.DataAnnotations;

namespace CTMS.DataModel.Models.Systems;

public class SurveyTabDisplayModel
{
    public List<SurveyTabDisplayItemModel> Items { get; set; } = new();
}

public class SurveyTabDisplayItemModel
{
    public string SurveyName { get; set; } = string.Empty;
    public string ViewName { get; set; } = string.Empty;
    public string CircleIcon { get; set; } = string.Empty;
    public string CompletionPercent { get; set; } = string.Empty;

}
