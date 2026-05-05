using CTMS.DataModel.Models.ClinicalInformation;
using CTMS.Share.Helpers;
using System.Text;

namespace CTMS.Business.Services.ClinicalInformation;

/// <summary>
/// 每位病人 × 每個 VisitCode = 一列的 CSV 大表。
/// 病人層級資料（臨床資訊、風險評估）已由 PatientSingleRowCsvExportService 負責，此表不重複。
/// </summary>
public class PatientCsvExportService
{
    private const string SV_化療 = "化療副作用";
    private const string SV_標靶 = "標靶副作用";
    private const string SV_放療 = "放療副作用";
    private const string SV_WHOQOL = "WHOQOL";
    private const string SV_個人史 = "個人史";
    private const string SV_家族史 = "家族史";
    private const string SV_生活品質 = "生活品質";
    private const string SV_健康 = "健康";

    private static readonly string[] 血液模板檔 = { "抽血檢驗血液.json", "抽血檢驗血液1.json", "抽血檢驗血液2.json" };
    private static readonly string[] 生化模板檔 = { "抽血檢驗生化.json", "抽血檢驗生化1.json", "抽血檢驗生化2.json" };

    private record ColumnDef(string Key, string Header);

    public byte[] Export(List<PatientData> patients)
    {
        int max化學治療 = 1;
        int max合併用藥 = 1;
        int max其他治療藥物 = 1;

        var 血液欄位 = LoadBloodItemColumns(血液模板檔);
        var 生化欄位 = LoadBloodItemColumns(生化模板檔);
        var 問卷題目 = LoadSurveyQuestionIds();

        foreach (var p in patients)
        {
            var m = p.臨床資料;
            foreach (var node in m.臨床資料化學治療.Items)
                max化學治療 = Math.Max(max化學治療, node.Items.Count);
            foreach (var node in m.臨床資料合併用藥.Items)
                max合併用藥 = Math.Max(max合併用藥, node.Items.Count);
            foreach (var node in m.其他治療藥物.Items)
                max其他治療藥物 = Math.Max(max其他治療藥物, node.Items.Count);
            foreach (var node in m.抽血檢驗血液.Items)
                foreach (var item in node.抽血檢驗血液)
                    血液欄位.TryAdd(NormalizeItemName(item.項目名稱), ExtractChineseName(item.項目名稱));
            foreach (var node in m.抽血檢驗生化.Items)
                foreach (var item in node.抽血檢驗生化)
                    生化欄位.TryAdd(NormalizeItemName(item.項目名稱), ExtractChineseName(item.項目名稱));
        }

        var columns = BuildColumnCatalog(
            max化學治療, max合併用藥, max其他治療藥物,
            血液欄位, 生化欄位, 問卷題目);

        var sb = new StringBuilder();
        sb.AppendLine(string.Join(",", columns.Select(c => EscapeCsv(c.Header))));

        foreach (var p in patients)
        {
            var 臨床資訊 = p.臨床資訊;
            var m = p.臨床資料;
            var visitCodes = CollectAllVisitCodes(m);

            if (visitCodes.Count == 0)
            {
                var row = BuildRow(columns, 臨床資訊, m, null,
                    max化學治療, max合併用藥, max其他治療藥物,
                    血液欄位, 生化欄位, 問卷題目);
                sb.AppendLine(string.Join(",", row.Select(EscapeCsv)));
            }
            else
            {
                foreach (var vc in visitCodes)
                {
                    var row = BuildRow(columns, 臨床資訊, m, vc,
                        max化學治療, max合併用藥, max其他治療藥物,
                        血液欄位, 生化欄位, 問卷題目);
                    sb.AppendLine(string.Join(",", row.Select(EscapeCsv)));
                }
            }
        }

        return Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray();
    }

    private List<ColumnDef> BuildColumnCatalog(
        int max化學治療, int max合併用藥, int max其他治療藥物,
        Dictionary<string, string> 血液欄位, Dictionary<string, string> 生化欄位,
        Dictionary<string, List<string>> 問卷題目)
    {
        var cols = new List<ColumnDef>();

        // 病人識別（對照欄）
        cols.Add(new("SubjectNo", "病人編號"));

        // 就診資訊
        cols.Add(new("VisitKey",            "就診_代碼"));
        cols.Add(new("VisitAssessmentDate", "就診_評估日期"));
        cols.Add(new("VisitTimeline",       "就診_Timeline"));
        cols.Add(new("VisitCycleMonth",     "就診_Cycle/Month"));

        // 手術
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

        // 病理報告
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

        // 化學治療（1:N slot）
        for (int i = 1; i <= max化學治療; i++)
            cols.AddRange(PairCols($"Chemo_{i}_", $"化學治療_{i}_", new (string, string)[]
            {
                ("TreatmentDate",      "治療日期"),
                ("BSA",                "BSA"),
                ("RegimenPaclitaxel",  "Regimen Paclitaxel"),
                ("RegimenCarboplatin", "Regimen Carboplatin"),
                ("Reduction",          "Reduction"),
                ("Bevacizumab",        "Bevacizumab"),
            }));

        // 合併用藥（1:N slot）
        for (int i = 1; i <= max合併用藥; i++)
            cols.AddRange(PairCols($"Med_{i}_", $"合併用藥_{i}_", new (string, string)[]
            {
                ("TreatmentDate", "治療日期"),
                ("Drug",          "Drug"),
                ("Dose",          "Dose"),
                ("RouteCode",     "Route Code"),
                ("UnitCode",      "Unit Code"),
            }));

        // 病史記錄
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

        // 抽血血液
        foreach (var (key, display) in 血液欄位)
        {
            cols.Add(new($"Blood_{key}_Unit",         $"抽血血液_{display}_單位"));
            cols.Add(new($"Blood_{key}_Value",        $"抽血血液_{display}_數值"));
            cols.Add(new($"Blood_{key}_SamplingDate", $"抽血血液_{display}_採檢日期"));
        }

        // 抽血生化
        foreach (var (key, display) in 生化欄位)
        {
            cols.Add(new($"Biochem_{key}_Unit",         $"抽血生化_{display}_單位"));
            cols.Add(new($"Biochem_{key}_Value",        $"抽血生化_{display}_數值"));
            cols.Add(new($"Biochem_{key}_SamplingDate", $"抽血生化_{display}_採檢日期"));
        }

        // CTCAE 血液副作用
        foreach (var s in new[] { "白血球", "絕對嗜中性白血球數", "血色素", "血小板" })
            AddGradeCols(cols, $"SE_Blood_{s}", $"CTCAE_血液副作用_{s}");

        // CTCAE 其他副作用1
        foreach (var s in new[] { "噁心", "嘔吐", "口腔炎", "拉肚子", "便秘", "食慾不振" })
            AddGradeCols(cols, $"SE1_{s}", $"CTCAE_副作用_{s}");

        // CTCAE 其他副作用2
        foreach (var s in new[] { "周邊感覺神經異常", "疲倦", "紅疹", "手足症候群", "掉髮" })
            AddGradeCols(cols, $"SE2_{s}", $"CTCAE_副作用_{s}");

        // 問卷
        foreach (var kv in 問卷題目)
            foreach (var qId in kv.Value)
                cols.Add(new($"{kv.Key}_{qId}", $"問卷_{kv.Key}_{qId}"));

        // 追蹤_其他治療
        foreach (var group in new[] { "Admission", "ER", "Clinics" })
            cols.AddRange(PairCols($"OT_{group}_", $"追蹤_其他治療_{group}_", new (string, string)[]
            {
                ("Visit",     "就診"),
                ("Treatment", "Treatment"),
                ("Lab",       "Lab"),
                ("Image",     "Image"),
                ("Others",    "Others"),
            }));

        // 追蹤_其他治療藥物（1:N slot）
        for (int i = 1; i <= max其他治療藥物; i++)
            cols.AddRange(PairCols($"OTMed_{i}_", $"追蹤_其他治療藥物_{i}_", new (string, string)[]
            {
                ("Drug",          "Drug"),
                ("TreatmentDate", "治療日期"),
                ("Dose",          "Dose"),
                ("RouteCode",     "Route Code"),
                ("UnitCode",      "Unit Code"),
            }));

        // 追蹤_影像
        cols.AddRange(PairCols("OTImg_", "追蹤_影像_", new (string, string)[]
        {
            ("ChestXRay",     "CXR"),
            ("ChestXRayDate", "CXR 日期"),
            ("LeadEKG12",     "12-lead EKG"),
            ("LeadEKG12Date", "12-lead EKG 日期"),
            ("ChestCT",       "Chest CT"),
            ("ChestCTDate",   "Chest CT 日期"),
            ("AbdCT",         "Abd CT"),
            ("AbdCTDate",     "Abd CT 日期"),
            ("BrainMRI",      "Brain MRI"),
            ("BrainMRIDate",  "Brain MRI 日期"),
            ("BoneScan",      "Bone scan"),
            ("BoneScanDate",  "Bone scan 日期"),
        }));

        return cols;
    }

    private List<string> BuildRow(
        List<ColumnDef> columns,
        BasicClinicalPresentation_臨床資訊 臨床資訊,
        Main臨床資料 m,
        VisitCodeModel? vc,
        int max化學治療, int max合併用藥, int max其他治療藥物,
        Dictionary<string, string> 血液欄位, Dictionary<string, string> 生化欄位,
        Dictionary<string, List<string>> 問卷題目)
    {
        var dict = new Dictionary<string, string>(StringComparer.Ordinal);

        dict["SubjectNo"] = 臨床資訊.SubjectNo ?? "";

        if (vc != null)
        {
            dict["VisitKey"]            = vc.GetVisitCodeTitle();
            dict["VisitAssessmentDate"] = vc.AssessmentDate?.ToString("yyyy-MM-dd") ?? "";
            dict["VisitTimeline"]       = vc.Timeline ?? "";
            dict["VisitCycleMonth"]     = vc.CycleMonth.ToString();
        }

        if (vc != null)
        {
            var op = m.臨床資料手術.Items.FirstOrDefault(x => x.VisitCode.CompareTo(vc));
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
        }

        if (vc != null)
        {
            var path = m.臨床資料病理報告.Items.FirstOrDefault(x => x.VisitCode.CompareTo(vc));
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
        }

        if (vc != null)
        {
            var chemo = m.臨床資料化學治療.Items.FirstOrDefault(x => x.VisitCode.CompareTo(vc));
            if (chemo != null)
            {
                for (int i = 0; i < chemo.Items.Count && i < max化學治療; i++)
                {
                    var item = chemo.Items[i];
                    int slot = i + 1;
                    dict[$"Chemo_{slot}_TreatmentDate"] = item.TreatmentDate.ToString("yyyy-MM-dd");
                    dict[$"Chemo_{slot}_BSA"] = item.BSA ?? "";
                    dict[$"Chemo_{slot}_RegimenPaclitaxel"] = item.RegimenPaclitaxel ?? "";
                    dict[$"Chemo_{slot}_RegimenCarboplatin"] = item.RegimenCarboplatin ?? "";
                    dict[$"Chemo_{slot}_Reduction"] = item.Reduction ?? "";
                    dict[$"Chemo_{slot}_Bevacizumab"] = item.Bevacizumab ?? "";
                }
            }
        }

        if (vc != null)
        {
            var med = m.臨床資料合併用藥.Items.FirstOrDefault(x => x.VisitCode.CompareTo(vc));
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
        }

        if (vc != null)
        {
            var mh = m.BaselineMedicalHistoryForm.Items.FirstOrDefault(x => x.VisitCode.CompareTo(vc));
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
        }

        if (vc != null)
        {
            var blood = m.抽血檢驗血液.Items.FirstOrDefault(x => x.VisitCode.CompareTo(vc));
            if (blood != null)
            {
                foreach (var item in blood.抽血檢驗血液)
                {
                    var key = NormalizeItemName(item.項目名稱);
                    dict[$"Blood_{key}_Unit"] = ExtractUnit(item.項目名稱);
                    dict[$"Blood_{key}_Value"] = item.檢驗數值 ?? "";
                    dict[$"Blood_{key}_SamplingDate"] = item.SamplingDate?.ToString("yyyy-MM-dd") ?? "";
                }
            }
        }

        if (vc != null)
        {
            var biochem = m.抽血檢驗生化.Items.FirstOrDefault(x => x.VisitCode.CompareTo(vc));
            if (biochem != null)
            {
                foreach (var item in biochem.抽血檢驗生化)
                {
                    var key = NormalizeItemName(item.項目名稱);
                    dict[$"Biochem_{key}_Unit"] = ExtractUnit(item.項目名稱);
                    dict[$"Biochem_{key}_Value"] = item.檢驗數值 ?? "";
                    dict[$"Biochem_{key}_SamplingDate"] = item.SamplingDate?.ToString("yyyy-MM-dd") ?? "";
                }
            }
        }

        if (vc != null)
        {
            var se = m.HematologicSideEffects血液副作用.Items.FirstOrDefault(x => x.VisitCode.CompareTo(vc));
            if (se != null)
            {
                FillGradeItem(dict, "SE_Blood_白血球", se.WhiteBloodCell白血球);
                FillGradeItem(dict, "SE_Blood_絕對嗜中性白血球數", se.NeutrophilCount絕對嗜中性白血球數);
                FillGradeItem(dict, "SE_Blood_血色素", se.HemoglobinHb血色素);
                FillGradeItem(dict, "SE_Blood_血小板", se.PlateletCount血小板);
            }
        }

        if (vc != null)
        {
            var se1 = m.SurveySideEffects副作用1.Items.FirstOrDefault(x => x.VisitCode.CompareTo(vc));
            if (se1 != null)
            {
                FillGradeItem(dict, "SE1_噁心", se1.Nausea噁心);
                FillGradeItem(dict, "SE1_嘔吐", se1.Vomiting嘔吐);
                FillGradeItem(dict, "SE1_口腔炎", se1.MucositisOral口腔炎);
                FillGradeItem(dict, "SE1_拉肚子", se1.Diarrhea拉肚子);
                FillGradeItem(dict, "SE1_便秘", se1.Constipation便秘);
                FillGradeItem(dict, "SE1_食慾不振", se1.Anorexia食慾不振);
            }
        }

        if (vc != null)
        {
            var se2 = m.SurveySideEffects副作用2.Items.FirstOrDefault(x => x.VisitCode.CompareTo(vc));
            if (se2 != null)
            {
                FillGradeItem(dict, "SE2_周邊感覺神經異常", se2.PeripheralNeuropathy周邊感覺神經異常);
                FillGradeItem(dict, "SE2_疲倦", se2.Fatigue疲倦);
                FillGradeItem(dict, "SE2_紅疹", se2.SkinRash紅疹);
                FillGradeItem(dict, "SE2_手足症候群", se2.HandFootSyndrome手足症候群);
                FillGradeItem(dict, "SE2_掉髮", se2.Alopecia掉髮);
            }
        }

        if (vc != null)
        {
            FillSurvey(dict, SV_化療,   m.Survey化療副作用.Items.FirstOrDefault(x => x.VisitCode.CompareTo(vc))?.Questions);
            FillSurvey(dict, SV_標靶,   m.Survey標靶副作用.Items.FirstOrDefault(x => x.VisitCode.CompareTo(vc))?.Questions);
            FillSurvey(dict, SV_放療,   m.Survey放療副作用.Items.FirstOrDefault(x => x.VisitCode.CompareTo(vc))?.Questions);
            FillSurvey(dict, SV_WHOQOL, m.SurveyWhooqol問卷.Items.FirstOrDefault(x => x.VisitCode.CompareTo(vc))?.Questions);
            FillSurvey(dict, SV_個人史, m.Survey個人史問卷.Items.FirstOrDefault(x => x.VisitCode.CompareTo(vc))?.Questions);
            FillSurvey(dict, SV_家族史, m.Survey家族史問卷.Items.FirstOrDefault(x => x.VisitCode.CompareTo(vc))?.Questions);
            FillSurvey(dict, SV_生活品質, m.Survey生活品質問卷.Items.FirstOrDefault(x => x.VisitCode.CompareTo(vc))?.Questions);
            FillSurvey(dict, SV_健康,   m.Survey健康問卷.Items.FirstOrDefault(x => x.VisitCode.CompareTo(vc))?.Questions);
        }

        if (vc != null)
        {
            var ot = m.其他治療.Items.FirstOrDefault(x => x.VisitCode.CompareTo(vc));
            if (ot != null)
            {
                foreach (var item in ot.Items)
                {
                    string g = item.First;
                    dict[$"OT_{g}_Visit"]     = item.Visit?.ToString("yyyy-MM-dd") ?? "";
                    dict[$"OT_{g}_Treatment"] = item.Treatment ?? "";
                    dict[$"OT_{g}_Lab"]       = item.Lab ?? "";
                    dict[$"OT_{g}_Image"]     = item.Image ?? "";
                    dict[$"OT_{g}_Others"]    = item.Others ?? "";
                }
            }
        }

        if (vc != null)
        {
            var otMed = m.其他治療藥物.Items.FirstOrDefault(x => x.VisitCode.CompareTo(vc));
            if (otMed != null)
            {
                for (int i = 0; i < otMed.Items.Count && i < max其他治療藥物; i++)
                {
                    var item = otMed.Items[i];
                    int slot = i + 1;
                    dict[$"OTMed_{slot}_Drug"]          = item.Pharmacy_Name ?? "";
                    dict[$"OTMed_{slot}_TreatmentDate"] = FormatDateString(item.Order_Effect_Date);
                    dict[$"OTMed_{slot}_Dose"]          = item.Totally_Dosage_Unit ?? "";
                    dict[$"OTMed_{slot}_RouteCode"]     = item.Usage_Code ?? "";
                    dict[$"OTMed_{slot}_UnitCode"]      = item.Dosage_Unit ?? "";
                }
            }
        }

        if (vc != null)
        {
            var otImg = m.其他治療影像.Items.FirstOrDefault(x => x.VisitCode.CompareTo(vc));
            if (otImg != null)
            {
                PopulateOtherTreatmentImageColumns(dict, otImg.Items);
            }
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
        foreach (var x in m.臨床資料手術.Items) Add(x.VisitCode);
        foreach (var x in m.臨床資料病理報告.Items) Add(x.VisitCode);
        foreach (var x in m.臨床資料化學治療.Items) Add(x.VisitCode);
        foreach (var x in m.臨床資料合併用藥.Items) Add(x.VisitCode);
        foreach (var x in m.BaselineMedicalHistoryForm.Items) Add(x.VisitCode);
        foreach (var x in m.抽血檢驗血液.Items) Add(x.VisitCode);
        foreach (var x in m.抽血檢驗生化.Items) Add(x.VisitCode);
        foreach (var x in m.Survey化療副作用.Items) Add(x.VisitCode);
        foreach (var x in m.Survey標靶副作用.Items) Add(x.VisitCode);
        foreach (var x in m.Survey放療副作用.Items) Add(x.VisitCode);
        foreach (var x in m.SurveyWhooqol問卷.Items) Add(x.VisitCode);
        foreach (var x in m.Survey個人史問卷.Items) Add(x.VisitCode);
        foreach (var x in m.Survey家族史問卷.Items) Add(x.VisitCode);
        foreach (var x in m.Survey生活品質問卷.Items) Add(x.VisitCode);
        foreach (var x in m.Survey健康問卷.Items) Add(x.VisitCode);
        foreach (var x in m.HematologicSideEffects血液副作用.Items) Add(x.VisitCode);
        foreach (var x in m.SurveySideEffects副作用1.Items) Add(x.VisitCode);
        foreach (var x in m.SurveySideEffects副作用2.Items) Add(x.VisitCode);
        foreach (var x in m.其他治療.Items) Add(x.VisitCode);
        foreach (var x in m.其他治療藥物.Items) Add(x.VisitCode);
        foreach (var x in m.其他治療影像.Items) Add(x.VisitCode);
        return result;
    }

    private Dictionary<string, string> LoadBloodItemColumns(string[] filenames)
    {
        var result = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var filename in filenames)
        {
            string path = Path.Combine("Data", filename);
            if (!File.Exists(path)) continue;
            string content = File.ReadAllText(path);
            var items = Newtonsoft.Json.JsonConvert.DeserializeObject<List<TestItem檢驗項目>>(content);
            if (items == null) continue;
            foreach (var item in items)
                result.TryAdd(NormalizeItemName(item.項目名稱), ExtractChineseName(item.項目名稱));
        }
        return result;
    }

    private Dictionary<string, List<string>> LoadSurveyQuestionIds()
    {
        var mapping = new Dictionary<string, string>
        {
            [SV_化療]   = "化療副作用自填問卷.json",
            [SV_標靶]   = "標靶副作用.json",
            [SV_放療]   = "放療副作用.json",
            [SV_WHOQOL] = "whooqol問卷.json",
            [SV_個人史] = "個人史問卷.json",
            [SV_家族史] = "家族史問卷.json",
            [SV_生活品質] = "QOL-CIPN20生活品質問卷.json",
            [SV_健康]   = "EQ-5D-3L健康問卷.json",
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

    private static void FillGradeItem(Dictionary<string, string> dict, string prefix, GradeItemSideEffectsItem item)
    {
        dict[$"{prefix}_RetriveValue"] = item.RetriveValue ?? "";
        dict[$"{prefix}_SelectedGrade"] = GetSelectedGrade(item);
    }

    private static string GetSelectedGrade(GradeItemSideEffectsItem item)
    {
        if (item.Grade1.ApplyCssClass != "not-found") return "1";
        if (item.Grade2.ApplyCssClass != "not-found") return "2";
        if (item.Grade3.ApplyCssClass != "not-found") return "3";
        if (item.Grade4.ApplyCssClass != "not-found") return "4";
        if (item.Grade5.ApplyCssClass != "not-found") return "5";
        return "";
    }

    private static void AddGradeCols(List<ColumnDef> cols, string keyPrefix, string headerPrefix)
    {
        cols.Add(new($"{keyPrefix}_RetriveValue", $"{headerPrefix}_數值"));
        cols.Add(new($"{keyPrefix}_SelectedGrade", $"{headerPrefix}_等級"));
    }

    private static void FillSurvey(Dictionary<string, string> dict, string surveyName, List<Question>? questions)
    {
        if (questions == null) return;
        foreach (var q in questions)
            dict[$"{surveyName}_{q.Id}"] = q.Answer ?? "";
    }

    private static string NormalizeItemName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return "Unknown";
        return System.Text.RegularExpressions.Regex.Replace(name.Trim(), @"[\s\(\)\[\]\/\.\^\³μ]", "_").TrimEnd('_');
    }

    private static string ExtractUnit(string? itemName)
    {
        if (string.IsNullOrWhiteSpace(itemName)) return "";
        var match = System.Text.RegularExpressions.Regex.Match(itemName, @"\(([^)]+)\)\s*$");
        return match.Success ? match.Groups[1].Value : "";
    }

    private static string ExtractChineseName(string? itemName)
    {
        if (string.IsNullOrWhiteSpace(itemName)) return "Unknown";
        var match = System.Text.RegularExpressions.Regex.Match(itemName.Trim(), @"^[\u4e00-\u9fff\u3400-\u4dbf]+");
        return match.Success ? match.Value : itemName.Trim().Split(' ')[0];
    }

    private static IEnumerable<ColumnDef> PairCols(string keyPrefix, string headerPrefix, IEnumerable<(string key, string header)> items)
        => items.Select(x => new ColumnDef($"{keyPrefix}{x.key}", $"{headerPrefix}{x.header}"));

    public static string EscapeCsv(string? value)
    {
        if (string.IsNullOrEmpty(value)) return string.Empty;
        if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
            return $"\"{value.Replace("\"", "\"\"")}\"";
        return value;
    }

    private static string FormatDateString(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        return DateTime.TryParse(value, out var date)
            ? date.ToString("yyyy-MM-dd")
            : value;
    }

    private static void PopulateOtherTreatmentImageColumns(
        Dictionary<string, string> dict,
        List<OtherTreatmentImageItem> items)
    {
        foreach (var item in items)
        {
            var target = ResolveOtherTreatmentImageTarget(item.OrderName);
            if (target == null)
            {
                continue;
            }

            var (valueKey, dateKey) = target.Value;
            if (!dict.ContainsKey(valueKey))
            {
                dict[valueKey] = item.ReportText ?? item.OrderName ?? string.Empty;
            }

            if (!dict.ContainsKey(dateKey))
            {
                dict[dateKey] = FormatDateString(item.ExecuteTime);
            }
        }
    }

    private static (string ValueKey, string DateKey)? ResolveOtherTreatmentImageTarget(string? orderName)
    {
        if (string.IsNullOrWhiteSpace(orderName))
        {
            return null;
        }

        var normalized = orderName.Trim().ToUpperInvariant();

        if (normalized.Contains("CXR") || normalized.Contains("X-RAY") || normalized.Contains("XRAY"))
        {
            return ("OTImg_ChestXRay", "OTImg_ChestXRayDate");
        }

        if (normalized.Contains("EKG") || normalized.Contains("ECG"))
        {
            return ("OTImg_LeadEKG12", "OTImg_LeadEKG12Date");
        }

        if (normalized.Contains("BRAIN") && normalized.Contains("MRI"))
        {
            return ("OTImg_BrainMRI", "OTImg_BrainMRIDate");
        }

        if (normalized.Contains("BONE"))
        {
            return ("OTImg_BoneScan", "OTImg_BoneScanDate");
        }

        if (normalized.Contains("ABD") && normalized.Contains("CT"))
        {
            return ("OTImg_AbdCT", "OTImg_AbdCTDate");
        }

        if (normalized.Contains("CHEST") && normalized.Contains("CT"))
        {
            return ("OTImg_ChestCT", "OTImg_ChestCTDate");
        }

        return null;
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
}
