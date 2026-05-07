using CTMS.DataModel.Models.ClinicalInformation;
using System.Text;

namespace CTMS.Business.Services.ClinicalInformation;

/// <summary>
/// 匯出問卷資料的 CSV（每位病人 × 每個 VisitCode = 一列）。
/// </summary>
public class PatientSurveyCsvExportService
{
    private const string SV_化療 = "化療副作用";
    private const string SV_標靶 = "標靶副作用";
    private const string SV_放療 = "放療副作用";
    private const string SV_WHOQOL = "WHOQOL";
    private const string SV_個人史 = "個人史";
    private const string SV_家族史 = "家族史";
    private const string SV_生活品質 = "生活品質";
    private const string SV_健康 = "健康";

    private record ColumnDef(string Key, string Header);

    public byte[] Export(List<PatientData> patients)
    {
        var 問卷題目 = LoadSurveyQuestionIds();

        var columns = BuildColumnCatalog(問卷題目);

        var sb = new StringBuilder();
        sb.AppendLine(string.Join(",", columns.Select(c => EscapeCsv(c.Header))));

        foreach (var p in patients)
        {
            var 臨床資訊 = p.臨床資訊;
            var m = p.臨床資料;
            var visitCodes = CollectAllVisitCodes(m);

            if (visitCodes.Count == 0)
            {
                var row = BuildRow(columns, 臨床資訊, m, null, 問卷題目);
                sb.AppendLine(string.Join(",", row.Select(EscapeCsv)));
            }
            else
            {
                foreach (var vc in visitCodes)
                {
                    var row = BuildRow(columns, 臨床資訊, m, vc, 問卷題目);
                    sb.AppendLine(string.Join(",", row.Select(EscapeCsv)));
                }
            }
        }

        return Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray();
    }

    private List<ColumnDef> BuildColumnCatalog(Dictionary<string, List<string>> 問卷題目)
    {
        var cols = new List<ColumnDef>();

        cols.Add(new("SubjectNo", "病人編號"));
        cols.Add(new("VisitKey", "就診_代碼"));
        cols.Add(new("VisitAssessmentDate", "就診_評估日期"));
        cols.Add(new("VisitTimeline", "就診_Timeline"));
        cols.Add(new("VisitCycleMonth", "就診_Cycle/Month"));

        foreach (var kv in 問卷題目)
            foreach (var qId in kv.Value)
                cols.Add(new($"{kv.Key}_{qId}", $"問卷_{kv.Key}_{qId}"));

        return cols;
    }

    private List<string> BuildRow(
        List<ColumnDef> columns,
        BasicClinicalPresentation_臨床資訊 臨床資訊,
        Main臨床資料 m,
        VisitCodeModel? vc,
        Dictionary<string, List<string>> 問卷題目)
    {
        var dict = new Dictionary<string, string>(StringComparer.Ordinal);

        dict["SubjectNo"] = 臨床資訊.SubjectNo ?? "";

        if (vc != null)
        {
            dict["VisitKey"] = vc.GetVisitCodeTitle();
            dict["VisitAssessmentDate"] = vc.AssessmentDate?.ToString("yyyy-MM-dd") ?? "";
            dict["VisitTimeline"] = vc.Timeline ?? "";
            dict["VisitCycleMonth"] = vc.CycleMonth.ToString();
        }

        if (vc != null)
        {
            FillSurvey(dict, SV_化療, m.Survey化療副作用.Items.FirstOrDefault(x => x.VisitCode.CompareTo(vc))?.Questions);
            FillSurvey(dict, SV_標靶, m.Survey標靶副作用.Items.FirstOrDefault(x => x.VisitCode.CompareTo(vc))?.Questions);
            FillSurvey(dict, SV_放療, m.Survey放療副作用.Items.FirstOrDefault(x => x.VisitCode.CompareTo(vc))?.Questions);
            FillSurvey(dict, SV_WHOQOL, m.SurveyWhooqol問卷.Items.FirstOrDefault(x => x.VisitCode.CompareTo(vc))?.Questions);
            FillSurvey(dict, SV_個人史, m.Survey個人史問卷.Items.FirstOrDefault(x => x.VisitCode.CompareTo(vc))?.Questions);
            FillSurvey(dict, SV_家族史, m.Survey家族史問卷.Items.FirstOrDefault(x => x.VisitCode.CompareTo(vc))?.Questions);
            FillSurvey(dict, SV_生活品質, m.Survey生活品質問卷.Items.FirstOrDefault(x => x.VisitCode.CompareTo(vc))?.Questions);
            FillSurvey(dict, SV_健康, m.Survey健康問卷.Items.FirstOrDefault(x => x.VisitCode.CompareTo(vc))?.Questions);
        }

        return columns.Select(c => dict.TryGetValue(c.Key, out var v) ? v : "").ToList();
    }

    private List<VisitCodeModel> CollectAllVisitCodes(Main臨床資料 m)
    {
        var result = new List<VisitCodeModel>();
        void Add(VisitCodeModel? vc)
        {
            if (vc == null) return;
            if (!result.Any(x => x.CompareTo(vc))) result.Add(vc);
        }
        foreach (var x in m.Survey化療副作用.Items) Add(x.VisitCode);
        foreach (var x in m.Survey標靶副作用.Items) Add(x.VisitCode);
        foreach (var x in m.Survey放療副作用.Items) Add(x.VisitCode);
        foreach (var x in m.SurveyWhooqol問卷.Items) Add(x.VisitCode);
        foreach (var x in m.Survey個人史問卷.Items) Add(x.VisitCode);
        foreach (var x in m.Survey家族史問卷.Items) Add(x.VisitCode);
        foreach (var x in m.Survey生活品質問卷.Items) Add(x.VisitCode);
        foreach (var x in m.Survey健康問卷.Items) Add(x.VisitCode);
        return result;
    }

    private Dictionary<string, List<string>> LoadSurveyQuestionIds()
    {
        var mapping = new Dictionary<string, string>
        {
            [SV_化療] = "化療副作用自填問卷.json",
            [SV_標靶] = "標靶副作用.json",
            [SV_放療] = "放療副作用.json",
            [SV_WHOQOL] = "whooqol問卷.json",
            [SV_個人史] = "個人史問卷.json",
            [SV_家族史] = "家族史問卷.json",
            [SV_生活品質] = "QOL-CIPN20生活品質問卷.json",
            [SV_健康] = "EQ-5D-3L健康問卷.json",
        };
        var result = new Dictionary<string, List<string>>();
        foreach (var kv in mapping)
        {
            string path = Path.Combine("Data", kv.Value);
            if (!File.Exists(path)) { result[kv.Key] = new(); continue; }
            string content = File.ReadAllText(path);
            var survey = Newtonsoft.Json.JsonConvert.DeserializeObject<SurveyTemplate>(content);
            result[kv.Key] = survey?.Questions?.Select(q => q.Id).ToList() ?? new();
        }
        return result;
    }

    private static void FillSurvey(Dictionary<string, string> dict, string surveyName, List<Question>? questions)
    {
        if (questions == null) return;
        foreach (var q in questions)
            dict[$"{surveyName}_{q.Id}"] = q.Answer ?? "";
    }

    private class SurveyTemplate
    {
        [Newtonsoft.Json.JsonProperty("questions")]
        public List<SurveyQuestionDto> Questions { get; set; } = new();
    }

    private class SurveyQuestionDto
    {
        [Newtonsoft.Json.JsonProperty("id")]
        public string Id { get; set; } = "";
    }

    public static string EscapeCsv(string? value)
    {
        if (string.IsNullOrEmpty(value)) return string.Empty;
        if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
            return $"\"{value.Replace("\"", "\"\"")}\"";
        return value;
    }
}
