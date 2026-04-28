using CTMS.DataModel.Models.ClinicalInformation;
using CTMS.Share.Helpers;
using System.Text;

namespace CTMS.Business.Services.ClinicalInformation;

/// <summary>
/// 匯出臨床資訊與風險評估的單列 CSV（每位病人一列）。
/// 不含 VisitCode、臨床資料、抽血資料、CTCAE 5.0、問卷、追蹤資料。
/// </summary>
public class PatientSingleRowCsvExportService
{
    public byte[] Export(List<PatientData> patients)
    {
        var columns = BuildColumns();
        var sb = new StringBuilder();

        // 標頭：使用中文名稱
        sb.AppendLine(string.Join(",", columns.Select(c => EscapeCsv(c.Header))));

        foreach (var p in patients)
        {
            var 臨床資訊 = p.臨床資訊;
            var risk = 臨床資訊.RiskAssessmentResult ?? new RiskAssessmentResult();

            var dict = new Dictionary<string, string>(StringComparer.Ordinal);

            // ── 臨床資訊欄位（排除 VisitCode）────────────────────
            dict["SubjectNo"]                   = 臨床資訊.SubjectNo ?? "";
            dict["Hospital"]                    = ResolveHospital(臨床資訊.SubjectNo);
            dict["收案日期"]                    = 臨床資訊.收案日期.ToString("yyyy-MM-dd");
            dict["ECorOC"]                      = 臨床資訊.ECorOC ?? "";
            dict["CancerType"]                  = 臨床資訊.CancerType ?? "";
            dict["Age"]                         = 臨床資訊.Age ?? "";
            dict["MenstrualStatus"]             = 臨床資訊.MenstrualStatus ?? "";
            dict["Height"]                      = 臨床資訊.Height ?? "";
            dict["Weight"]                      = 臨床資訊.Weight ?? "";
            dict["BMI"]                         = 臨床資訊.BMI ?? "";
            dict["BSA"]                         = 臨床資訊.BSA ?? "";
            dict["AbdominalCircumference"]      = 臨床資訊.AbdominalCircumference ?? "";
            dict["PerformanceStatus"]           = 臨床資訊.PerformanceStatus ?? "";
            dict["FIGOStaging"]                 = 臨床資訊.FIGOStaging ?? "";
            dict["AJCCClinicalStage"]           = 臨床資訊.AJCCClinicalStage ?? "";
            dict["AJCCClinicalStageT"]          = 臨床資訊.AJCCClinicalStageT ?? "";
            dict["AJCCClinicalStageN"]          = 臨床資訊.AJCCClinicalStageN ?? "";
            dict["AJCCClinicalStageM"]          = 臨床資訊.AJCCClinicalStageM ?? "";
            dict["AJCCPathologicalStage"]       = 臨床資訊.AJCCPathologicalStage ?? "";
            dict["AJCCPathologicalStageT"]      = 臨床資訊.AJCCPathologicalStageT ?? "";
            dict["AJCCPathologicalStageN"]      = 臨床資訊.AJCCPathologicalStageN ?? "";
            dict["AJCCPathologicalStageM"]      = 臨床資訊.AJCCPathologicalStageM ?? "";
            dict["HistologicalType"]            = 臨床資訊.HistologicalType ?? "";
            dict["HistologicalTypeDetail"]      = 臨床資訊.HistologicalTypeDetail ?? "";
            dict["MMRProtein"]                  = 臨床資訊.MMRProtein ?? "";
            dict["P53"]                         = 臨床資訊.P53 ?? "";
            dict["HormonStatus"]                = 臨床資訊.HormonStatus ?? "";
            dict["HormonStatusER"]              = 臨床資訊.HormonStatusER ?? "";
            dict["HormonStatusERPercent"]       = 臨床資訊.HormonStatusERPercent ?? "";
            dict["HormonStatusPR"]              = 臨床資訊.HormonStatusPR ?? "";
            dict["HormonStatusPRPercent"]       = 臨床資訊.HormonStatusPRPercent ?? "";

            // ── 風險評估欄位 ──────────────────────────────────────
            dict["Risk_ExperimentalControl"]    = risk.ExperimentalControl ?? "";
            dict["Risk_風險程度"]                = risk.風險程度 ?? "";
            dict["Risk_是否需要降15Percent劑量"] = risk.是否需要降15Percent劑量 ?? "";
            dict["Risk_SMA"]                    = risk.SMA骨骼肌面積 ?? "";
            dict["Risk_SMI"]                    = risk.SMI骨骼肌指標 ?? "";
            dict["Risk_SMD"]                    = risk.SMD骨骼肌密度 ?? "";
            dict["Risk_IMAT"]                   = risk.IMAT肌間肌肉脂肪組織 ?? "";
            dict["Risk_LAMA"]                   = risk.LAMA低密度肌肉區域 ?? "";
            dict["Risk_NAMA"]                   = risk.NAMA正常密度肌肉區域 ?? "";
            dict["Risk_Myosteatosis"]           = risk.Myosteatosis肌肉脂肪變性 ?? "";

            var row = columns.Select(c => dict.TryGetValue(c.Key, out var v) ? v : "").ToList();
            sb.AppendLine(string.Join(",", row.Select(EscapeCsv)));
        }

        // UTF-8 BOM（Excel 開啟中文不亂碼）
        return Encoding.UTF8.GetPreamble()
            .Concat(Encoding.UTF8.GetBytes(sb.ToString()))
            .ToArray();
    }

    private record ColumnDef(string Key, string Header);

    private static List<ColumnDef> BuildColumns() =>
    [
        // ── 臨床資訊 ────────────────────────────────────────────
        new("SubjectNo",              "臨床資訊_病人編號"),
        new("Hospital",               "臨床資訊_醫院"),
        new("收案日期",               "臨床資訊_收案日期"),
        new("ECorOC",                 "臨床資訊_癌別"),
        new("CancerType",             "臨床資訊_癌症類型"),
        new("Age",                    "臨床資訊_年齡"),
        new("MenstrualStatus",        "臨床資訊_月經狀態"),
        new("Height",                 "臨床資訊_身高(cm)"),
        new("Weight",                 "臨床資訊_體重(kg)"),
        new("BMI",                    "臨床資訊_BMI"),
        new("BSA",                    "臨床資訊_體表面積(m²)"),
        new("AbdominalCircumference", "臨床資訊_腰圍(cm)"),
        new("PerformanceStatus",      "臨床資訊_日常體能狀態(PS)"),
        new("FIGOStaging",            "臨床資訊_癌症分期(2023 FIGO)"),
        new("AJCCClinicalStage",      "臨床資訊_AJCC c stage"),
        new("AJCCClinicalStageT",     "臨床資訊_AJCC c stage T"),
        new("AJCCClinicalStageN",     "臨床資訊_AJCC c stage N"),
        new("AJCCClinicalStageM",     "臨床資訊_AJCC c stage M"),
        new("AJCCPathologicalStage",  "臨床資訊_AJCC p stage"),
        new("AJCCPathologicalStageT", "臨床資訊_AJCC p stage T"),
        new("AJCCPathologicalStageN", "臨床資訊_AJCC p stage N"),
        new("AJCCPathologicalStageM", "臨床資訊_AJCC p stage M"),
        new("HistologicalType",       "臨床資訊_組織型態"),
        new("HistologicalTypeDetail", "臨床資訊_組織型態細節"),
        new("MMRProtein",             "臨床資訊_MMR protein"),
        new("P53",                    "臨床資訊_p53"),
        new("HormonStatus",           "臨床資訊_Hormon status"),
        new("HormonStatusER",         "臨床資訊_Hormon status ER"),
        new("HormonStatusERPercent",  "臨床資訊_Hormon status ER%"),
        new("HormonStatusPR",         "臨床資訊_Hormon status PR"),
        new("HormonStatusPRPercent",  "臨床資訊_Hormon status PR%"),
        // ── 風險評估 ────────────────────────────────────────────
        new("Risk_ExperimentalControl",    "風險評估_組別"),
        new("Risk_風險程度",               "風險評估_風險程度"),
        new("Risk_是否需要降15Percent劑量", "風險評估_是否需要降15%劑量"),
        new("Risk_SMA",                    "風險評估_骨骼肌面積(SMA)"),
        new("Risk_SMI",                    "風險評估_骨骼肌指標(SMI)"),
        new("Risk_SMD",                    "風險評估_骨骼肌密度(SMD)"),
        new("Risk_IMAT",                   "風險評估_肌間肌肉脂肪組織(IMAT)"),
        new("Risk_LAMA",                   "風險評估_低密度肌肉區域(LAMA)"),
        new("Risk_NAMA",                   "風險評估_正常密度肌肉區域(NAMA)"),
        new("Risk_Myosteatosis",           "風險評估_肌肉脂肪變性(Myosteatosis)"),
    ];

    private static string ResolveHospital(string? subjectNo)
    {
        if (subjectNo == null) return "";
        if (subjectNo.Contains(MagicObjectHelper.prefix奇美醫院))   return "奇美醫院";
        if (subjectNo.Contains(MagicObjectHelper.prefix郭綜合醫院)) return "郭綜合醫院";
        if (subjectNo.Contains(MagicObjectHelper.prefix成大醫院))   return "成大醫院";
        return "";
    }

    public static string EscapeCsv(string? value)
    {
        if (string.IsNullOrEmpty(value)) return string.Empty;
        if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
            return $"\"{value.Replace("\"", "\"\"")}\"";
        return value;
    }
}
