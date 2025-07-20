using CTMS.DataModel.Models.ClinicalInformation;
using Microsoft.Extensions.Logging;

namespace CTMS.Business.Services.ClinicalInformation;

public class SurveyService
{
    private readonly ILogger<SurveyService> logger;

    public SurveyService(ILogger<SurveyService> logger)
    {
        this.logger = logger;
    }
    public void Read(Survey問卷 survey)
    {
        survey.化療副作用 = ReadFile("化療副作用自填問卷.json");
        survey.放療副作用 = ReadFile("放療副作用.json");
        survey.標靶副作用 = ReadFile("標靶副作用.json");
        survey.whooqol問卷 = ReadFile("whooqol問卷.json");
        survey.個人史問卷 = ReadFile("個人史問卷.json");
        survey.家族史問卷 = ReadFile("家族史問卷.json");
    }

    public Survey ReadFile(string filename)
    {
        string path = Path.Combine("Data", filename);
        string content = File.ReadAllText(path);
        var surveryItems = Newtonsoft.Json.JsonConvert.DeserializeObject<Survey>(content);
        return surveryItems;
    }

    public void Reset(List<Question> questions)
    {
        var hasAnyOfQuestions = questions
            .Where(q => q.VisibilityCondition != null && 
            q.VisibilityCondition.AnyOf!=null && q.VisibilityCondition.AnyOf.Count > 0)
            .ToList();
        foreach (var q in hasAnyOfQuestions)
        {
            q.IsVisible = false;
        }
    }

    public void RefreshByQuestionChanged(List<Question> survey, Question question)
    {
        if (survey == null || question == null)
            return;

        // 找出所有 VisibilityCondition.QuestionId 等於 question.Id 的題目
        var relatedQuestions = survey
            .Where(q => q.VisibilityCondition != null && q.VisibilityCondition.QuestionId == question.Id)
            .ToList();

        // 將 answer 轉為 int，若無法轉換則設為 null
        bool parsed = int.TryParse(question.Answer, out int answerValue);

        foreach (var q in relatedQuestions)
        {
            if (parsed && q.VisibilityCondition.AnyOf != null)
            {
                // 若 answer 出現在 AnyOf，則 IsVisible = true，否則 false
                q.IsVisible = q.VisibilityCondition.AnyOf.Contains(answerValue);
            }
            else
            {
                // 若無法比對，預設為顯示
                q.IsVisible = false;
            }
        }
    }
}
