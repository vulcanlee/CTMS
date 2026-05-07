using CTMS.DataModel.Models.ClinicalInformation;
using CTMS.Share.Helpers;
using System.Text;

namespace CTMS.Business.Services.ClinicalInformation;

/// <summary>
/// 匯出臨床資訊與風險評估的單列 CSV（每位病人一列）。
/// 包含臨床資訊、風險評估、手術、病理報告、合併用藥、病史紀錄。
/// </summary>
public class PatientSingleRowCsvExportService
{
    private record ColumnDef(string Key, string Header);

    public byte[] Export(List<PatientData> patients)
    {
        int max合併用藥 = 1;
        foreach (var p in patients)
        {
            var med = p.臨床資料.臨床資料合併用藥.Items.FirstOrDefault();
            if (med != null)
                max合併用藥 = Math.Max(max合併用藥, med.Items.Count);
        }

        var columns = BuildColumns(max合併用藥);
        var sb = new StringBuilder();

        sb.AppendLine(string.Join(",", columns.Select(c => EscapeCsv(c.Header))));

        foreach (var p in patients)
        {
            var 臨床資訊 = p.臨床資訊;
            var m = p.臨床資料;
            var risk = 臨床資訊.RiskAssessmentResult ?? new RiskAssessmentResult();

            var dict = new Dictionary<string, string>(StringComparer.Ordinal);

            // ── 臨床資訊欄位 ────────────────────────────────────────
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

            // ── 風險評估欄位 ────────────────────────────────────────
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

            // ── 手術 ────────────────────────────────────────────────
            var op = m.臨床資料手術.Items.FirstOrDefault();
            if (op != null)
            {
                dict["OP_手術日期"] = op.手術日期.ToString("yyyy-MM-dd");
                dict["OP_術式"] = op.術式 ?? "";
                dict["OP_OPOutcome"] = op.OPOutcome ?? "";
                dict["OP_Ascites"] = op.Ascites ?? "";
                dict["OP_Uterus"] = op.Uterus ?? "";
                dict["OP_UterusSite"] = op.UterusSite ?? "";
                dict["OP_UterusTumorNumber"] = op.UterusTumorNumber ?? "";
                dict["OP_UterusTumorSize"] = op.UterusTumorSize ?? "";
                dict["OP_Cervix"] = op.Cervix ?? "";
                dict["OP_CervixSite"] = op.CervixSite ?? "";
                dict["OP_CervixTumorNumber"] = op.CervixTumorNumber ?? "";
                dict["OP_Endometrium"] = op.Endometrium ?? "";
                dict["OP_Myometrium"] = op.Myometrium ?? "";
                dict["OP_CulDeSac"] = op.CulDeSac ?? "";
                dict["OP_OvarianSurfaceRuptureRight"] = op.OvarianSurfaceRuptureOrNotRightOvary ?? "";
                dict["OP_OvarianSurfaceRuptureLeft"] = op.OvarianSurfaceRuptureOrNotLeftOvary ?? "";
                dict["OP_LeftAdnexa"] = op.LeftAdnexa ?? "";
                dict["OP_LeftAdnexaTumorNumber"] = op.LeftAdnexaTumorNumber ?? "";
                dict["OP_LeftAdnexaTumorSize"] = op.LeftAdnexaTumorSize ?? "";
                dict["OP_RightAdnexa"] = op.RightAdnexa ?? "";
                dict["OP_RightAdnexaTumorNumber"] = op.RightAdnexaTumorNumber ?? "";
                dict["OP_RightAdnexaTumorSize"] = op.RightAdnexaTumorSize ?? "";
                dict["OP_PelvicPeritonealCavity"] = op.PelvicPeritonealCavity ?? "";
                dict["OP_PelvicPeritonealCavityTumorSize"] = op.PelvicPeritonealCavityTumorSize ?? "";
                dict["OP_ExtrapelvicPeritonealCavity"] = op.ExtrapelvicPeritonealCavity ?? "";
                dict["OP_ExtrapelvicPeritonealCavityOtherFinding"] = op.ExtrapelvicPeritonealCavityOtherFinding ?? "";
                dict["OP_OtherOrganInvolvementGrossLooking"] = op.OtherOrganInvolvementGrossLooking ?? "";
                dict["OP_Optimal"] = op.Optimal ?? "";
                dict["OP_ResidualTumor"] = op.ResidualTumor ?? "";
            }

            // ── 病理報告 ────────────────────────────────────────────
            var path = m.臨床資料病理報告.Items.FirstOrDefault();
            if (path != null)
            {
                dict["Path_切片日期"] = path.切片日期.ToString("yyyy-MM-dd");
                dict["Path_Histology"] = path.Histology ?? "";
                dict["Path_TnmStage"] = path.TnmStage ?? "";
                dict["Path_Myometrium"] = path.Myometrium ?? "";
                dict["Path_UterineSerosaInvolvement"] = path.UterineSerosaInvolvement ?? "";
                dict["Path_BloodLymphaticVesselInvasion"] = path.BloodLymphaticVesselInvasion ?? "";
                dict["Path_Cervix"] = path.Cervix ?? "";
                dict["Path_Parametrium"] = path.Parametrium ?? "";
                dict["Path_OvaryRight"] = path.OvaryRight ?? "";
                dict["Path_OvaryLeft"] = path.OvaryLeft ?? "";
                dict["Path_FallopianTubeRight"] = path.FallopianTubeRight ?? "";
                dict["Path_FallopianTubeLeft"] = path.FallopianTubeLeft ?? "";
                dict["Path_Vagina"] = path.Vagina ?? "";
                dict["Path_RegionalLymphNodes"] = path.RegionalLymphNodes ?? "";
                dict["Path_IsolatedTumorCells"] = path.IsolatedTumorCells ?? "";
                dict["Path_AdditionalPathologicalFindings"] = path.AdditionalPathologicalFindings ?? "";
                dict["Path_ImmunohistochemicalTest"] = path.ImmunohistochemicalTest ?? "";
            }

            // ── 合併用藥 ────────────────────────────────────────────
            var med = m.臨床資料合併用藥.Items.FirstOrDefault();
            if (med != null)
            {
                for (int i = 0; i < med.Items.Count && i < max合併用藥; i++)
                {
                    var item = med.Items[i];
                    int slot = i + 1;
                    dict[$"Med_{slot}_TreatmentDate"] = item.TreatmentDate.ToString("yyyy-MM-dd");
                    dict[$"Med_{slot}_Drug"] = item.Durg ?? "";
                    dict[$"Med_{slot}_Dose"] = item.Dose ?? "";
                    dict[$"Med_{slot}_RouteCode"] = item.RouteCode ?? "";
                    dict[$"Med_{slot}_UnitCode"] = item.UnitCode ?? "";
                }
            }

            // ── 病史紀錄 ────────────────────────────────────────────
            var mh = m.BaselineMedicalHistoryForm.Items.FirstOrDefault();
            if (mh != null)
            {
                dict["MH_CardiovascularIncludeHtn"] = mh.CardiovascularIncludeHtn ?? "";
                dict["MH_PeripheralVascular"] = mh.PeripheralVascular ?? "";
                dict["MH_Respiratory"] = mh.Respiratory ?? "";
                dict["MH_Gastrointestinal"] = mh.Gastrointestinal ?? "";
                dict["MH_Renal"] = mh.Renal ?? "";
                dict["MH_Genitourinary"] = mh.Genitourinary ?? "";
                dict["MH_EndocrineMetabolicIncludeDiabetes"] = mh.EndocrineMetabolicIncludeDiabetes ?? "";
                dict["MH_HematologicLymphatic"] = mh.HematologicLymphatic ?? "";
                dict["MH_Musculoskeletal"] = mh.Musculoskeletal ?? "";
                dict["MH_Dermatologic"] = mh.Dermatologic ?? "";
                dict["MH_DrugAbuse"] = mh.DrugAbuse ?? "";
                dict["MH_Tobacco"] = mh.Tobacco ?? "";
                dict["MH_Neurologic"] = mh.Neurologic ?? "";
                dict["MH_Psychiatric"] = mh.Psychiatric ?? "";
                dict["MH_Allergies"] = mh.Allergies ?? "";
                dict["MH_Neoplasia"] = mh.Neoplasia ?? "";
                dict["MH_AlcoholUse"] = mh.AlcoholUse ?? "";
                dict["MH_ImmunityIncludeHiv"] = mh.ImmunityIncludeHiv ?? "";
                dict["MH_HepatobiliaryIncludeHbvHcv"] = mh.HepatobiliaryIncludeHbvHcv ?? "";
                dict["MH_OtherSpecify"] = mh.OtherSpecify ?? "";
            }

            var row = columns.Select(c => dict.TryGetValue(c.Key, out var v) ? v : "").ToList();
            sb.AppendLine(string.Join(",", row.Select(EscapeCsv)));
        }

        return Encoding.UTF8.GetPreamble()
            .Concat(Encoding.UTF8.GetBytes(sb.ToString()))
            .ToArray();
    }

    private static List<ColumnDef> BuildColumns(int max合併用藥)
    {
        var cols = new List<ColumnDef>
        {
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
        };

        // ── 手術 ────────────────────────────────────────────────────
        cols.AddRange(PairCols("OP_", "手術_", new (string, string)[]
        {
            ("手術日期",                              "手術日期"),
            ("術式",                                  "術式"),
            ("OPOutcome",                             "OP outcome"),
            ("Ascites",                               "Ascites"),
            ("Uterus",                                "Uterus"),
            ("UterusSite",                            "Uterus site"),
            ("UterusTumorNumber",                     "Uterus Tumor number"),
            ("UterusTumorSize",                       "Uterus Tumor size"),
            ("Cervix",                                "Cervix"),
            ("CervixSite",                            "Cervix site"),
            ("CervixTumorNumber",                     "Cervix Tumor number"),
            ("Endometrium",                           "Endometrium"),
            ("Myometrium",                            "Myometrium"),
            ("CulDeSac",                              "Cul de sac"),
            ("OvarianSurfaceRuptureRight",            "Ovarian surface rupture-Right ovary"),
            ("OvarianSurfaceRuptureLeft",             "Ovarian surface rupture-Left ovary"),
            ("LeftAdnexa",                            "Left adnexa"),
            ("LeftAdnexaTumorNumber",                 "Left adnexa Tumor number"),
            ("LeftAdnexaTumorSize",                   "Left adnexa Tumor size"),
            ("RightAdnexa",                           "Right adnexa"),
            ("RightAdnexaTumorNumber",                "Right adnexa Tumor number"),
            ("RightAdnexaTumorSize",                  "Right adnexa Tumor size"),
            ("PelvicPeritonealCavity",                "Pelvic peritoneal cavity"),
            ("PelvicPeritonealCavityTumorSize",       "Pelvic peritoneal cavity Tumor size"),
            ("ExtrapelvicPeritonealCavity",           "Extrapelvic peritoneal cavity"),
            ("ExtrapelvicPeritonealCavityOtherFinding","Extrapelvic peritoneal cavity Other findings"),
            ("OtherOrganInvolvementGrossLooking",     "Other organ involvement (gross looking)"),
            ("Optimal",                               "Optimal"),
            ("ResidualTumor",                         "Residual tumor"),
        }));

        // ── 病理報告 ────────────────────────────────────────────────
        cols.AddRange(PairCols("Path_", "病理報告_", new (string, string)[]
        {
            ("切片日期",                      "切片日期"),
            ("Histology",                     "Histology"),
            ("TnmStage",                      "TNM Stage"),
            ("Myometrium",                    "Myometrium"),
            ("UterineSerosaInvolvement",      "Uterine Serosa Involvement"),
            ("BloodLymphaticVesselInvasion",  "Blood/lymphatic vessel invasion"),
            ("Cervix",                        "Cervix"),
            ("Parametrium",                   "Parametrium"),
            ("OvaryRight",                    "Ovary-Right"),
            ("OvaryLeft",                     "Ovary-Left"),
            ("FallopianTubeRight",            "Fallopian tube-Right"),
            ("FallopianTubeLeft",             "Fallopian tube-Left"),
            ("Vagina",                        "Vagina"),
            ("RegionalLymphNodes",            "Regional Lymph Nodes"),
            ("IsolatedTumorCells",            "Isolated tumor cells"),
            ("AdditionalPathologicalFindings","Additional pathological findings"),
            ("ImmunohistochemicalTest",       "Immunohistochemical test"),
        }));

        // ── 合併用藥（1:N slot）────────────────────────────────────
        for (int i = 1; i <= max合併用藥; i++)
            cols.AddRange(PairCols($"Med_{i}_", $"合併用藥_{i}_", new (string, string)[]
            {
                ("TreatmentDate", "治療日期"),
                ("Drug",          "Drug"),
                ("Dose",          "Dose"),
                ("RouteCode",     "Route Code"),
                ("UnitCode",      "Unit Code"),
            }));

        // ── 病史紀錄 ────────────────────────────────────────────────
        cols.AddRange(PairCols("MH_", "病史記錄_", new (string, string)[]
        {
            ("CardiovascularIncludeHtn",           "Cardiovascular (include HTN)"),
            ("PeripheralVascular",                 "Peripheral Vascular"),
            ("Respiratory",                        "Respiratory"),
            ("Gastrointestinal",                   "Gastrointestinal"),
            ("Renal",                              "Renal"),
            ("Genitourinary",                      "Genitourinary"),
            ("EndocrineMetabolicIncludeDiabetes",  "Metabolic (Include Diabetes)"),
            ("HematologicLymphatic",               "Hematologic-Lymphatic"),
            ("Musculoskeletal",                    "Musculoskeletal"),
            ("Dermatologic",                       "Dermatologic"),
            ("DrugAbuse",                          "Drug Abuse"),
            ("Tobacco",                            "Tobacco"),
            ("Neurologic",                         "Neurologic"),
            ("Psychiatric",                        "Psychiatric"),
            ("Allergies",                          "Allergies"),
            ("Neoplasia",                          "Neoplasia"),
            ("AlcoholUse",                         "Alcohol Use"),
            ("ImmunityIncludeHiv",                 "immunity (Include HIV)"),
            ("HepatobiliaryIncludeHbvHcv",         "Hepatobiliary (Include HBV/HCV)"),
            ("OtherSpecify",                       "other (specify)"),
        }));

        return cols;
    }

    private static IEnumerable<ColumnDef> PairCols(string keyPrefix, string headerPrefix, IEnumerable<(string key, string header)> items)
        => items.Select(x => new ColumnDef($"{keyPrefix}{x.key}", $"{headerPrefix}{x.header}"));

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
