using CTMS.Business.Services.ClinicalInformation;
using CTMS.DataModel.Models;
using CTMS.DataModel.Models.ClinicalInformation;
using CTMS.DataModel.Models.Systems;
using CTMS.Helper;
using System.Reflection.PortableExecutable;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CTMS.Share.Helpers;

public class SurveyTabDisplayHelper
{
    private readonly SurveyService SurveyService;

    public SurveyTabDisplayHelper(SurveyService SurveyService)
    {
        this.SurveyService = SurveyService;
    }

    public SurveyTabDisplayModel Build()
    {
        SurveyTabDisplayModel result = new();

        SurveyTabDisplayItemModel item;

        item = new()
        {
            SurveyName = MagicObjectHelper.SurveyName化療副作用,
            ViewName = "ChemotherapySideEffectsSurveyView",
            CircleIcon = string.Empty,
            CompletionPercent = string.Empty,
        };
        result.Items.Add(item);

        item = new()
        {
            SurveyName = MagicObjectHelper.SurveyName標靶副作用,
            ViewName = "TargetedTherapySideEffectsView",
            CircleIcon = string.Empty,
            CompletionPercent = string.Empty,
        };
        result.Items.Add(item);

        item = new()
        {
            SurveyName = MagicObjectHelper.SurveyName放療副作用,
            ViewName = "RadiotherapySideEffectsView",
            CircleIcon = string.Empty,
            CompletionPercent = string.Empty,
        };
        result.Items.Add(item);

        item = new()
        {
            SurveyName = MagicObjectHelper.SurveyNameWHOQOL問卷,
            ViewName = "WhoqolQuestionnaireView",
            CircleIcon = string.Empty,
            CompletionPercent = string.Empty,
        };
        result.Items.Add(item);

        item = new()
        {
            SurveyName = MagicObjectHelper.SurveyName個人史,
            ViewName = "PersonalHistorySurveyView",
            CircleIcon = string.Empty,
            CompletionPercent = string.Empty,
        };
        result.Items.Add(item);

        item = new()
        {
            SurveyName = MagicObjectHelper.SurveyName家族史,
            ViewName = "FamilyHistorySurveyView",
            CircleIcon = string.Empty,
            CompletionPercent = string.Empty,
        };
        result.Items.Add(item);

        item = new()
        {
            SurveyName = MagicObjectHelper.SurveyName生活品質,
            ViewName = "QualityOfLifeSurveyView",
            CircleIcon = string.Empty,
            CompletionPercent = string.Empty,
        };
        result.Items.Add(item);

        item = new()
        {
            SurveyName = MagicObjectHelper.SurveyName健康,
            ViewName = "HealthSurveyView",
            CircleIcon = string.Empty,
            CompletionPercent = string.Empty,
        };
        result.Items.Add(item);

        return result;
    }

    public void RefreshColor(SurveyTabDisplayModel surveyTabDisplayModel)
    {
        foreach (var item in surveyTabDisplayModel.Items)
        {
            item.CircleIcon = Icons.SetCircleColor(item.CompletionPercent);
        }
    }

    public void Get(PatientData patientData, DropDownListDataModel SelectVisitCode, SurveyTabDisplayModel surveyTabDisplayModel)
    {
        if(SelectVisitCode == null)
        {
            return;
        }
        Survey化療副作用Node dataSurvey化療副作用Node;
        Survey標靶副作用Node dataSurvey標靶副作用Node;
        Survey放療副作用 dataSurvey放療副作用;
        SurveyWhooqol問卷Node dataSurveyWhooqol問卷Node;
        Survey個人史問卷Node dataSurvey個人史問卷Node;
        Survey家族史問卷Node dataSurvey家族史問卷Node;
        Survey生活品質問卷Node dataSurvey生活品質問卷Node;
        Survey健康問卷Node dataSurvey健康問卷Node;

        var Survey化療副作用 = patientData.臨床資料.Survey化療副作用;
        var Survey標靶副作用 = patientData.臨床資料.Survey標靶副作用;
        var Survey放療副作用 = patientData.臨床資料.Survey放療副作用;
        var SurveyWhooqol問卷 = patientData.臨床資料.SurveyWhooqol問卷;
        var Survey個人史問卷 = patientData.臨床資料.Survey個人史問卷;
        var Survey家族史問卷 = patientData.臨床資料.Survey家族史問卷;
        var Survey生活品質問卷 = patientData.臨床資料.Survey生活品質問卷;
        var Survey健康問卷 = patientData.臨床資料.Survey健康問卷;

        #region 開始計算問卷進度

        int totalQuestion = 0;
        int totalAnswer = 0;
        SurveyTabDisplayItemModel SurveyTabDisplayItemModel;

        #region 化療副作用
        SurveyTabDisplayItemModel = surveyTabDisplayModel.Items.FirstOrDefault(x => x.SurveyName == MagicObjectHelper.SurveyName化療副作用);

        Survey化療副作用Node survey化療副作用Node = Survey化療副作用.Items.FirstOrDefault(x => x.VisitCode.Id == SelectVisitCode.Key);
    
        SurveyTabDisplayItemModel.CompletionPercent = "";
        SurveyTabDisplayItemModel.CircleIcon = Icons.SetCircleColor(SurveyTabDisplayItemModel.CompletionPercent);
        if (survey化療副作用Node != null)
        {
        }

        if (survey化療副作用Node != null)
        {
            dataSurvey化療副作用Node = survey化療副作用Node;
            SurveyService.Reset(dataSurvey化療副作用Node.Questions);
            foreach (var item in dataSurvey化療副作用Node.Questions)
            {
                SurveyService.RefreshByQuestionChanged(dataSurvey化療副作用Node.Questions, item);
            }

            totalQuestion = dataSurvey化療副作用Node.Questions.Count(x => x.IsVisible == true);
            totalAnswer = dataSurvey化療副作用Node.Questions.Count(x => x.IsVisible == true && string.IsNullOrEmpty(x.Answer) == false);

            float percent = 100.0f * totalAnswer / totalQuestion;
            SurveyTabDisplayItemModel.CompletionPercent = percent.ToString("0.##");

        }
        else
        {
            SurveyTabDisplayItemModel.CompletionPercent = "";
        }
        SurveyTabDisplayItemModel.CircleIcon = Icons.SetCircleColor(SurveyTabDisplayItemModel.CompletionPercent);
        #endregion

        #region 標靶副作用
        SurveyTabDisplayItemModel = surveyTabDisplayModel.Items.FirstOrDefault(x => x.SurveyName == MagicObjectHelper.SurveyName標靶副作用);
        Survey標靶副作用Node survey標靶副作用Node = Survey標靶副作用.Items.FirstOrDefault(x => x.VisitCode.Id == SelectVisitCode.Key);

        if (survey標靶副作用Node != null)
        {
            dataSurvey標靶副作用Node = survey標靶副作用Node;
            SurveyService.Reset(dataSurvey標靶副作用Node.Questions);
            foreach (var item in dataSurvey標靶副作用Node.Questions)
            {
                SurveyService.RefreshByQuestionChanged(dataSurvey標靶副作用Node.Questions, item);
            }
            totalQuestion = dataSurvey標靶副作用Node.Questions.Count(x => x.IsVisible == true);
            totalAnswer = dataSurvey標靶副作用Node.Questions.Count(x => x.IsVisible == true && string.IsNullOrEmpty(x.Answer) == false);
            float percent = 100.0f * totalAnswer / totalQuestion;
            SurveyTabDisplayItemModel.CompletionPercent = percent.ToString("0.##");
        }
        else
        {
            SurveyTabDisplayItemModel.CompletionPercent = "";
        }

        SurveyTabDisplayItemModel.CircleIcon = Icons.SetCircleColor(SurveyTabDisplayItemModel.CompletionPercent);
        #endregion

        #region 放療副作用
        SurveyTabDisplayItemModel = surveyTabDisplayModel.Items.FirstOrDefault(x => x.SurveyName == MagicObjectHelper.SurveyName放療副作用);
        Survey放療副作用Node survey放療副作用Node = Survey放療副作用.Items.FirstOrDefault(x => x.VisitCode.Id == SelectVisitCode.Key);
        if (survey放療副作用Node != null)
        {
            dataSurvey放療副作用 = Survey放療副作用;
            SurveyService.Reset(dataSurvey放療副作用.Items.SelectMany(x => x.Questions).ToList());
            foreach (var node in dataSurvey放療副作用.Items)
            {
                foreach (var item in node.Questions)
                {
                    SurveyService.RefreshByQuestionChanged(node.Questions, item);
                }
            }
            totalQuestion = dataSurvey放療副作用.Items.SelectMany(x => x.Questions).Count(x => x.IsVisible == true);
            totalAnswer = dataSurvey放療副作用.Items.SelectMany(x => x.Questions).Count(x => x.IsVisible == true && string.IsNullOrEmpty(x.Answer) == false);
            float percent = 100.0f * totalAnswer / totalQuestion;
            SurveyTabDisplayItemModel.CompletionPercent = percent.ToString("0.##");
        }
        else
        {
            SurveyTabDisplayItemModel.CompletionPercent = "";
        }

        SurveyTabDisplayItemModel.CircleIcon = Icons.SetCircleColor(SurveyTabDisplayItemModel.CompletionPercent);
        #endregion

        #region WHOQOL問卷
        SurveyTabDisplayItemModel = surveyTabDisplayModel.Items.FirstOrDefault(x => x.SurveyName == MagicObjectHelper.SurveyNameWHOQOL問卷);
        SurveyWhooqol問卷Node surveyWHOQOL問卷Node = SurveyWhooqol問卷.Items.FirstOrDefault(x => x.VisitCode.Id == SelectVisitCode.Key);
        if (surveyWHOQOL問卷Node != null)
        {
            dataSurveyWhooqol問卷Node = surveyWHOQOL問卷Node;
            SurveyService.Reset(dataSurveyWhooqol問卷Node.Questions);
            foreach (var item in dataSurveyWhooqol問卷Node.Questions)
            {
                SurveyService.RefreshByQuestionChanged(dataSurveyWhooqol問卷Node.Questions, item);
            }
            totalQuestion = dataSurveyWhooqol問卷Node.Questions.Count(x => x.IsVisible == true);
            totalAnswer = dataSurveyWhooqol問卷Node.Questions.Count(x => x.IsVisible == true && string.IsNullOrEmpty(x.Answer) == false);
            float percent = 100.0f * totalAnswer / totalQuestion;
            SurveyTabDisplayItemModel.CompletionPercent = percent.ToString("0.##");
        }
        else
        {
            SurveyTabDisplayItemModel.CompletionPercent = "";
        }

        SurveyTabDisplayItemModel.CircleIcon = Icons.SetCircleColor(SurveyTabDisplayItemModel.CompletionPercent);
        #endregion

        #region 個人史
        SurveyTabDisplayItemModel = surveyTabDisplayModel.Items.FirstOrDefault(x => x.SurveyName == MagicObjectHelper.SurveyName個人史);
        Survey個人史問卷Node survey個人史問卷Node = Survey個人史問卷.Items.FirstOrDefault(x => x.VisitCode.Id == SelectVisitCode.Key);
        if (survey個人史問卷Node != null)
        {
            dataSurvey個人史問卷Node = survey個人史問卷Node;
            SurveyService.Reset(dataSurvey個人史問卷Node.Questions);
            foreach (var item in dataSurvey個人史問卷Node.Questions)
            {
                SurveyService.RefreshByQuestionChanged(dataSurvey個人史問卷Node.Questions, item);
            }
            totalQuestion = dataSurvey個人史問卷Node.Questions.Count(x => x.IsVisible == true);
            totalAnswer = dataSurvey個人史問卷Node.Questions.Count(x => x.IsVisible == true && string.IsNullOrEmpty(x.Answer) == false);
            float percent = 100.0f * totalAnswer / totalQuestion;
            SurveyTabDisplayItemModel.CompletionPercent = percent.ToString("0.##");
        }
        else
        {
            SurveyTabDisplayItemModel.CompletionPercent = "";
        }
        SurveyTabDisplayItemModel.CircleIcon = Icons.SetCircleColor(SurveyTabDisplayItemModel.CompletionPercent);
        #endregion

        #region 家族史
        SurveyTabDisplayItemModel = surveyTabDisplayModel.Items.FirstOrDefault(x => x.SurveyName == MagicObjectHelper.SurveyName家族史);
        Survey家族史問卷Node survey家族史問卷Node = Survey家族史問卷.Items.FirstOrDefault(x => x.VisitCode.Id == SelectVisitCode.Key);
        if (survey家族史問卷Node != null)
        {
            dataSurvey家族史問卷Node = survey家族史問卷Node;
            SurveyService.Reset(dataSurvey家族史問卷Node.Questions);
            foreach (var item in dataSurvey家族史問卷Node.Questions)
            {
                SurveyService.RefreshByQuestionChanged(dataSurvey家族史問卷Node.Questions, item);
            }
            totalQuestion = dataSurvey家族史問卷Node.Questions.Count(x => x.IsVisible == true);
            totalAnswer = dataSurvey家族史問卷Node.Questions.Count(x => x.IsVisible == true && string.IsNullOrEmpty(x.Answer) == false);
            float percent = 100.0f * totalAnswer / totalQuestion;
            SurveyTabDisplayItemModel.CompletionPercent = percent.ToString("0.##");
        }
        else
        {
            SurveyTabDisplayItemModel.CompletionPercent = "";
        }

        SurveyTabDisplayItemModel.CircleIcon = Icons.SetCircleColor(SurveyTabDisplayItemModel.CompletionPercent);
        #endregion

        #region 生活品質
        SurveyTabDisplayItemModel = surveyTabDisplayModel.Items.FirstOrDefault(x => x.SurveyName == MagicObjectHelper.SurveyName生活品質);
        Survey生活品質問卷Node survey生活品質問卷Node = Survey生活品質問卷.Items.FirstOrDefault(x => x.VisitCode.Id == SelectVisitCode.Key);

        if (survey生活品質問卷Node != null)
        {
            dataSurvey生活品質問卷Node = survey生活品質問卷Node;
            SurveyService.Reset(dataSurvey生活品質問卷Node.Questions);
            foreach (var item in dataSurvey生活品質問卷Node.Questions)
            {
                SurveyService.RefreshByQuestionChanged(dataSurvey生活品質問卷Node.Questions, item);
            }
            totalQuestion = dataSurvey生活品質問卷Node.Questions.Count(x => x.IsVisible == true);
            totalAnswer = dataSurvey生活品質問卷Node.Questions.Count(x => x.IsVisible == true && string.IsNullOrEmpty(x.Answer) == false);
            float percent = 100.0f * totalAnswer / totalQuestion;
            SurveyTabDisplayItemModel.CompletionPercent = percent.ToString("0.##");
        }
        else
        {
            SurveyTabDisplayItemModel.CompletionPercent = "";
        }

        SurveyTabDisplayItemModel.CircleIcon = Icons.SetCircleColor(SurveyTabDisplayItemModel.CompletionPercent);
        #endregion

        #region 健康
        SurveyTabDisplayItemModel = surveyTabDisplayModel.Items.FirstOrDefault(x => x.SurveyName == MagicObjectHelper.SurveyName健康);
        Survey健康問卷Node survey健康問卷Node = Survey健康問卷.Items.FirstOrDefault(x => x.VisitCode.Id == SelectVisitCode.Key);
        if (survey健康問卷Node != null)
        {
            dataSurvey健康問卷Node = survey健康問卷Node;
            SurveyService.Reset(dataSurvey健康問卷Node.Questions);
            foreach (var item in dataSurvey健康問卷Node.Questions)
            {
                SurveyService.RefreshByQuestionChanged(dataSurvey健康問卷Node.Questions, item);
            }
            totalQuestion = dataSurvey健康問卷Node.Questions.Count(x => x.IsVisible == true);
            totalAnswer = dataSurvey健康問卷Node.Questions.Count(x => x.IsVisible == true && string.IsNullOrEmpty(x.Answer) == false);
            float percent = 100.0f * totalAnswer / totalQuestion;
            SurveyTabDisplayItemModel.CompletionPercent = percent.ToString("0.##");
        }
        else
        {
            SurveyTabDisplayItemModel.CompletionPercent = "";
        }

        SurveyTabDisplayItemModel.CircleIcon = Icons.SetCircleColor(SurveyTabDisplayItemModel.CompletionPercent);
        #endregion

        #endregion

    }
}
