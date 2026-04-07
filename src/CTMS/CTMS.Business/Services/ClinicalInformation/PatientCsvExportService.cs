using CTMS.DataModel.Models.ClinicalInformation;
using CTMS.Share.Helpers;
using System.Text;

namespace CTMS.Business.Services.ClinicalInformation;

/// <summary>
/// 將所有病人資料（每位病人 × 每個 VisitCode = 一列）匯出為 CSV 大表。
/// 病人層級資料（臨床資訊、風險評估）複寫到每一列。
/// 1:N 頁籤（化學治療、合併用藥、其他治療藥物）以 slot 展開（_1..._N）。
/// 抽血資料以三院模板聯集建欄。
/// 問卷欄位以 {問卷名}_{QuestionId} 為穩定 key。
/// </summary>
public class PatientCsvExportService
{
    // ── 問卷名稱常數 ───────────────────────────────────────────
    private const string SV_化療 = "化療副作用";
    private const string SV_標靶 = "標靶副作用";
    private const string SV_放療 = "放療副作用";
    private const string SV_WHOQOL = "WHOQOL";
    private const string SV_個人史 = "個人史";
    private const string SV_家族史 = "家族史";
    private const string SV_生活品質 = "生活品質";
    private const string SV_健康 = "健康";

    // ── 血液/生化模板檔案（三院）──────────────────────────────
    private static readonly string[] 血液模板檔 = { "抽血檢驗血液.json", "抽血檢驗血液1.json", "抽血檢驗血液2.json" };
    private static readonly string[] 生化模板檔 = { "抽血檢驗生化.json", "抽血檢驗生化1.json", "抽血檢驗生化2.json" };

    public byte[] Export(List<PatientData> patients)
    {
        // ── Phase 1：掃描全資料集，決定 schema ─────────────────

        // 1-A. 1:N slot 最大筆數
        int max化學治療 = 1;
        int max合併用藥 = 1;
        int max其他治療藥物 = 1;

        // 1-B. 抽血欄位（三院聯集）
        var 血液欄位 = LoadBloodItemNames(血液模板檔);
        var 生化欄位 = LoadBloodItemNames(生化模板檔);

        // 1-C. 問卷題目（從模板檔讀取一次）
        var 問卷題目 = LoadSurveyQuestionIds();

        // 掃描病人資料
        foreach (var p in patients)
        {
            var m = p.臨床資料;

            // slot 掃描
            foreach (var node in m.臨床資料化學治療.Items)
                max化學治療 = Math.Max(max化學治療, node.Items.Count);
            foreach (var node in m.臨床資料合併用藥.Items)
                max合併用藥 = Math.Max(max合併用藥, node.Items.Count);
            foreach (var node in m.其他治療藥物.Items)
                max其他治療藥物 = Math.Max(max其他治療藥物, node.Items.Count);

            // 抽血欄位（合併病人實際出現的項目）
            foreach (var node in m.抽血檢驗血液.Items)
                foreach (var item in node.抽血檢驗血液)
                    血液欄位.Add(NormalizeItemName(item.項目名稱));
            foreach (var node in m.抽血檢驗生化.Items)
                foreach (var item in node.抽血檢驗生化)
                    生化欄位.Add(NormalizeItemName(item.項目名稱));
        }

        // ── Phase 2：建立固定欄位清單 ──────────────────────────
        var columns = BuildColumnCatalog(
            max化學治療, max合併用藥, max其他治療藥物,
            血液欄位.ToList(), 生化欄位.ToList(),
            問卷題目);

        // ── Phase 3：逐病人 × 逐 VisitCode 產生列 ──────────────
        var sb = new StringBuilder();

        // 標頭
        sb.AppendLine(string.Join(",", columns.Select(EscapeCsv)));

        foreach (var p in patients)
        {
            var 臨床資訊 = p.臨床資訊;
            var m = p.臨床資料;

            // 收集所有 VisitCode
            var visitCodes = CollectAllVisitCodes(m);

            if (visitCodes.Count == 0)
            {
                // 無 VisitCode：輸出 1 列，基本資料填入，Visit 相關留空
                var row = BuildRow(columns, 臨床資訊, m, null,
                    max化學治療, max合併用藥, max其他治療藥物,
                    血液欄位, 生化欄位, 問卷題目, fillBasicInfo: true);
                sb.AppendLine(string.Join(",", row.Select(EscapeCsv)));
            }
            else
            {
                bool isFirstRow = true;
                foreach (var vc in visitCodes)
                {
                    // 基本資料只在第一列填寫，後續列留空
                    var row = BuildRow(columns, 臨床資訊, m, vc,
                        max化學治療, max合併用藥, max其他治療藥物,
                        血液欄位, 生化欄位, 問卷題目, fillBasicInfo: isFirstRow);
                    sb.AppendLine(string.Join(",", row.Select(EscapeCsv)));
                    isFirstRow = false;
                }
            }
        }

        // UTF-8 BOM（Excel 開啟中文不亂碼）
        return Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray();
    }

    // ════════════════════════════════════════════════════════════
    // 建立欄位目錄
    // ════════════════════════════════════════════════════════════
    private List<string> BuildColumnCatalog(
        int max化學治療, int max合併用藥, int max其他治療藥物,
        List<string> 血液欄位, List<string> 生化欄位,
        Dictionary<string, List<string>> 問卷題目)
    {
        var cols = new List<string>();

        // 病人識別
        cols.AddRange(new[] { "PatientCode", "SubjectNo", "Hospital", "CancerType", "FIGOStaging",
            "AJCCClinicalStage", "AJCCClinicalStageT", "AJCCClinicalStageN", "AJCCClinicalStageM",
            "AJCCPathologicalStage", "AJCCPathologicalStageT", "AJCCPathologicalStageN", "AJCCPathologicalStageM",
            "HistologicalType", "HistologicalTypeDetail", "MMRProtein", "P53",
            "HormonStatus", "HormonStatusER", "HormonStatusERPercent", "HormonStatusPR", "HormonStatusPRPercent",
            "Age", "MenstrualStatus", "Height", "Weight", "BMI", "BSA", "AbdominalCircumference",
            "PerformanceStatus", "收案日期" });

        // Visit 識別
        cols.AddRange(new[] { "VisitKey", "VisitAssessmentDate", "VisitTimeline", "VisitCycleMonth" });

        // ── 臨床資料：手術（1:1）──────────────────────────────
        cols.AddRange(PrefixCols("OP_", new[] {
            "手術日期","術式","OPOutcome","Ascites",
            "Uterus","UterusSite","UterusTumorNumber","UterusTumorSize",
            "Cervix","CervixSite","CervixTumorNumber","Endometrium","Myometrium",
            "CulDeSac","OvarianSurfaceRuptureRight","OvarianSurfaceRuptureLeft",
            "LeftAdnexa","LeftAdnexaTumorNumber","LeftAdnexaTumorSize",
            "RightAdnexa","RightAdnexaTumorNumber","RightAdnexaTumorSize",
            "PelvicPeritonealCavity","PelvicPeritonealCavityTumorSize",
            "ExtrapelvicPeritonealCavity","ExtrapelvicPeritonealCavityOtherFinding",
            "OtherOrganInvolvementGrossLooking","Optimal","ResidualTumor"
        }));

        // ── 臨床資料：病理報告（1:1）─────────────────────────
        cols.AddRange(PrefixCols("Path_", new[] {
            "切片日期","Histology","TnmStage","Myometrium","UterineSerosaInvolvement",
            "BloodLymphaticVesselInvasion","Cervix","Parametrium","OvaryRight","OvaryLeft",
            "FallopianTubeRight","FallopianTubeLeft","Vagina","RegionalLymphNodes",
            "IsolatedTumorCells","AdditionalPathologicalFindings","ImmunohistochemicalTest"
        }));

        // ── 臨床資料：化學治療（1:N slot）────────────────────
        for (int i = 1; i <= max化學治療; i++)
            cols.AddRange(PrefixCols($"Chemo_{i}_", new[] {
                "TreatmentDate","BSA","RegimenPaclitaxel","RegimenCarboplatin","Reduction","Bevacizumab"
            }));

        // ── 臨床資料：合併用藥（1:N slot）────────────────────
        for (int i = 1; i <= max合併用藥; i++)
            cols.AddRange(PrefixCols($"Med_{i}_", new[] {
                "TreatmentDate","Drug","Dose","RouteCode","UnitCode"
            }));

        // ── 臨床資料：病史記錄（1:1）─────────────────────────
        cols.AddRange(PrefixCols("MH_", new[] {
            "CardiovascularIncludeHtn","PeripheralVascular","Respiratory","Gastrointestinal",
            "Renal","Genitourinary","EndocrineMetabolicIncludeDiabetes","HematologicLymphatic",
            "Musculoskeletal","Dermatologic","DrugAbuse","Tobacco","Neurologic","Psychiatric",
            "Allergies","Neoplasia","AlcoholUse","ImmunityIncludeHiv","HepatobiliaryIncludeHbvHcv","OtherSpecify"
        }));

        // ── 抽血資料：血液（三院聯集，每項 value + SamplingDate）
        foreach (var name in 血液欄位)
        {
            cols.Add($"Blood_{name}_Value");
            cols.Add($"Blood_{name}_SamplingDate");
        }

        // ── 抽血資料：生化（三院聯集）────────────────────────
        foreach (var name in 生化欄位)
        {
            cols.Add($"Biochem_{name}_Value");
            cols.Add($"Biochem_{name}_SamplingDate");
        }

        // ── 副作用：血液副作用（1:1）─────────────────────────
        foreach (var symptom in new[] { "白血球", "絕對嗜中性白血球數", "血色素", "血小板" })
            AddGradeCols(cols, $"SE_Blood_{symptom}");

        // ── 副作用：其他副作用1（1:1）────────────────────────
        foreach (var symptom in new[] { "噁心", "嘔吐", "口腔炎", "拉肚子", "便秘", "食慾不振" })
            AddGradeCols(cols, $"SE1_{symptom}");

        // ── 副作用：其他副作用2（1:1）────────────────────────
        foreach (var symptom in new[] { "周邊感覺神經異常", "疲倦", "紅疹", "手足症候群", "掉髮" })
            AddGradeCols(cols, $"SE2_{symptom}");

        // ── 問卷（8 份，各以 {問卷名}_{QId} 為欄位）────────
        foreach (var kv in 問卷題目)
            foreach (var qId in kv.Value)
                cols.Add($"{kv.Key}_{qId}");

        // ── 追蹤資料：其他治療（固定 3 組）──────────────────
        foreach (var group in new[] { "Admission", "ER", "Clinics" })
            cols.AddRange(PrefixCols($"OT_{group}_", new[] { "Visit", "Treatment", "Lab", "Image", "Others" }));

        // ── 追蹤資料：其他治療藥物（1:N slot）───────────────
        for (int i = 1; i <= max其他治療藥物; i++)
            cols.AddRange(PrefixCols($"OTMed_{i}_", new[] {
                "Drug","TreatmentDate","Dose","RouteCode","UnitCode"
            }));

        // ── 追蹤資料：其他治療影像（1:1）─────────────────────
        cols.AddRange(PrefixCols("OTImg_", new[] {
            "ChestXRay","ChestXRayDate","LeadEKG12","LeadEKG12Date",
            "ChestCT","ChestCTDate","AbdCT","AbdCTDate",
            "BrainMRI","BrainMRIDate","BoneScan","BoneScanDate"
        }));

        // ── 風險評估（病人層級，複寫到每列）─────────────────
        cols.AddRange(PrefixCols("Risk_", new[] {
            "ExperimentalControl","風險程度","是否需要降15Percent劑量",
            "SMA骨骼肌面積","SMI骨骼肌指標","SMD骨骼肌密度",
            "IMAT肌間肌肉脂肪組織","LAMA低密度肌肉區域","NAMA正常密度肌肉區域","Myosteatosis肌肉脂肪變性"
        }));

        return cols;
    }

    // ════════════════════════════════════════════════════════════
    // 產生單一列的值
    // ════════════════════════════════════════════════════════════
    private List<string> BuildRow(
        List<string> columns,
        BasicClinicalPresentation_臨床資訊 臨床資訊,
        Main臨床資料 m,
        VisitCodeModel? vc,
        int max化學治療, int max合併用藥, int max其他治療藥物,
        HashSet<string> 血液欄位, HashSet<string> 生化欄位,
        Dictionary<string, List<string>> 問卷題目,
        bool fillBasicInfo = true)
    {
        var dict = new Dictionary<string, string>(StringComparer.Ordinal);

        // ── 病人識別（只在第一列填寫）────────────────────────
        if (fillBasicInfo)
        {
            dict["PatientCode"] = 臨床資訊.SubjectNo ?? "";
            dict["SubjectNo"] = 臨床資訊.SubjectNo ?? "";
            dict["Hospital"] = ResolveHospital(臨床資訊.SubjectNo);
            dict["CancerType"] = 臨床資訊.CancerType ?? "";
            dict["FIGOStaging"] = 臨床資訊.FIGOStaging ?? "";
            dict["AJCCClinicalStage"] = 臨床資訊.AJCCClinicalStage ?? "";
            dict["AJCCClinicalStageT"] = 臨床資訊.AJCCClinicalStageT ?? "";
            dict["AJCCClinicalStageN"] = 臨床資訊.AJCCClinicalStageN ?? "";
            dict["AJCCClinicalStageM"] = 臨床資訊.AJCCClinicalStageM ?? "";
            dict["AJCCPathologicalStage"] = 臨床資訊.AJCCPathologicalStage ?? "";
            dict["AJCCPathologicalStageT"] = 臨床資訊.AJCCPathologicalStageT ?? "";
            dict["AJCCPathologicalStageN"] = 臨床資訊.AJCCPathologicalStageN ?? "";
            dict["AJCCPathologicalStageM"] = 臨床資訊.AJCCPathologicalStageM ?? "";
            dict["HistologicalType"] = 臨床資訊.HistologicalType ?? "";
            dict["HistologicalTypeDetail"] = 臨床資訊.HistologicalTypeDetail ?? "";
            dict["MMRProtein"] = 臨床資訊.MMRProtein ?? "";
            dict["P53"] = 臨床資訊.P53 ?? "";
            dict["HormonStatus"] = 臨床資訊.HormonStatus ?? "";
            dict["HormonStatusER"] = 臨床資訊.HormonStatusER ?? "";
            dict["HormonStatusERPercent"] = 臨床資訊.HormonStatusERPercent ?? "";
            dict["HormonStatusPR"] = 臨床資訊.HormonStatusPR ?? "";
            dict["HormonStatusPRPercent"] = 臨床資訊.HormonStatusPRPercent ?? "";
            dict["Age"] = 臨床資訊.Age ?? "";
            dict["MenstrualStatus"] = 臨床資訊.MenstrualStatus ?? "";
            dict["Height"] = 臨床資訊.Height ?? "";
            dict["Weight"] = 臨床資訊.Weight ?? "";
            dict["BMI"] = 臨床資訊.BMI ?? "";
            dict["BSA"] = 臨床資訊.BSA ?? "";
            dict["AbdominalCircumference"] = 臨床資訊.AbdominalCircumference ?? "";
            dict["PerformanceStatus"] = 臨床資訊.PerformanceStatus ?? "";
            dict["收案日期"] = 臨床資訊.收案日期.ToString("yyyy-MM-dd");
        }

        // ── Visit 識別 ─────────────────────────────────────────
        if (vc != null)
        {
            dict["VisitKey"] = vc.GetVisitCodeTitle();
            dict["VisitAssessmentDate"] = vc.AssessmentDate?.ToString("yyyy-MM-dd") ?? "";
            dict["VisitTimeline"] = vc.Timeline ?? "";
            dict["VisitCycleMonth"] = vc.CycleMonth.ToString();
        }

        // ── 手術（1:1）────────────────────────────────────────
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

        // ── 病理報告（1:1）────────────────────────────────────
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

        // ── 化學治療（1:N slot）───────────────────────────────
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

        // ── 合併用藥（1:N slot）───────────────────────────────
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

        // ── 病史記錄（1:1）────────────────────────────────────
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

        // ── 抽血：血液（三院聯集）────────────────────────────
        if (vc != null)
        {
            var blood = m.抽血檢驗血液.Items.FirstOrDefault(x => x.VisitCode.CompareTo(vc));
            if (blood != null)
            {
                foreach (var item in blood.抽血檢驗血液)
                {
                    var key = NormalizeItemName(item.項目名稱);
                    dict[$"Blood_{key}_Value"] = item.檢驗數值 ?? "";
                    dict[$"Blood_{key}_SamplingDate"] = item.SamplingDate?.ToString("yyyy-MM-dd") ?? "";
                }
            }
        }

        // ── 抽血：生化（三院聯集）────────────────────────────
        if (vc != null)
        {
            var biochem = m.抽血檢驗生化.Items.FirstOrDefault(x => x.VisitCode.CompareTo(vc));
            if (biochem != null)
            {
                foreach (var item in biochem.抽血檢驗生化)
                {
                    var key = NormalizeItemName(item.項目名稱);
                    dict[$"Biochem_{key}_Value"] = item.檢驗數值 ?? "";
                    dict[$"Biochem_{key}_SamplingDate"] = item.SamplingDate?.ToString("yyyy-MM-dd") ?? "";
                }
            }
        }

        // ── 副作用：血液副作用（1:1）─────────────────────────
        if (vc != null)
        {
            var se = m.HematologicSideEffects血液副作用.Items.FirstOrDefault(x => x.VisitCode.CompareTo(vc));
            if (se != null)
            {
                FillGradeItem(dict, $"SE_Blood_白血球", se.WhiteBloodCell白血球);
                FillGradeItem(dict, $"SE_Blood_絕對嗜中性白血球數", se.NeutrophilCount絕對嗜中性白血球數);
                FillGradeItem(dict, $"SE_Blood_血色素", se.HemoglobinHb血色素);
                FillGradeItem(dict, $"SE_Blood_血小板", se.PlateletCount血小板);
            }
        }

        // ── 副作用：其他副作用1（1:1）────────────────────────
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

        // ── 副作用：其他副作用2（1:1）────────────────────────
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

        // ── 問卷（8 份）──────────────────────────────────────
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

        // ── 追蹤資料：其他治療（固定 3 組）──────────────────
        if (vc != null)
        {
            var ot = m.其他治療.Items.FirstOrDefault(x => x.VisitCode.CompareTo(vc));
            if (ot != null)
            {
                foreach (var item in ot.Items)
                {
                    string g = item.First; // Admission / ER / Clinics
                    dict[$"OT_{g}_Visit"] = item.Visit?.ToString("yyyy-MM-dd") ?? "";
                    dict[$"OT_{g}_Treatment"] = item.Treatment ?? "";
                    dict[$"OT_{g}_Lab"] = item.Lab ?? "";
                    dict[$"OT_{g}_Image"] = item.Image ?? "";
                    dict[$"OT_{g}_Others"] = item.Others ?? "";
                }
            }
        }

        // ── 追蹤資料：其他治療藥物（1:N slot）───────────────
        if (vc != null)
        {
            var otMed = m.其他治療藥物.Items.FirstOrDefault(x => x.VisitCode.CompareTo(vc));
            if (otMed != null)
            {
                for (int i = 0; i < otMed.Items.Count && i < max其他治療藥物; i++)
                {
                    var item = otMed.Items[i];
                    int slot = i + 1;
                    dict[$"OTMed_{slot}_Drug"] = item.Drug ?? "";
                    dict[$"OTMed_{slot}_TreatmentDate"] = item.TreatmentDate?.ToString("yyyy-MM-dd") ?? "";
                    dict[$"OTMed_{slot}_Dose"] = item.dose ?? "";
                    dict[$"OTMed_{slot}_RouteCode"] = item.RouteCode ?? "";
                    dict[$"OTMed_{slot}_UnitCode"] = item.UnitCode ?? "";
                }
            }
        }

        // ── 追蹤資料：其他治療影像（1:1）─────────────────────
        if (vc != null)
        {
            var otImg = m.其他治療影像.Items.FirstOrDefault(x => x.VisitCode.CompareTo(vc));
            if (otImg != null)
            {
                var img = otImg.Item;
                dict["OTImg_ChestXRay"] = img.ChestXRay ?? "";
                dict["OTImg_ChestXRayDate"] = img.ChestXRayDate?.ToString("yyyy-MM-dd") ?? "";
                dict["OTImg_LeadEKG12"] = img.LeadEKG12 ?? "";
                dict["OTImg_LeadEKG12Date"] = img.LeadEKG12Date?.ToString("yyyy-MM-dd") ?? "";
                dict["OTImg_ChestCT"] = img.ChestCT ?? "";
                dict["OTImg_ChestCTDate"] = img.ChestCTDate?.ToString("yyyy-MM-dd") ?? "";
                dict["OTImg_AbdCT"] = img.AbdCT ?? "";
                dict["OTImg_AbdCTDate"] = img.AbdCTDate?.ToString("yyyy-MM-dd") ?? "";
                dict["OTImg_BrainMRI"] = img.BrainMRI ?? "";
                dict["OTImg_BrainMRIDate"] = img.BrainMRIDate?.ToString("yyyy-MM-dd") ?? "";
                dict["OTImg_BoneScan"] = img.BoneScan ?? "";
                dict["OTImg_BoneScanDate"] = img.BoneScanDate?.ToString("yyyy-MM-dd") ?? "";
            }
        }

        // ── 風險評估（病人層級，只在第一列填寫）─────────────
        if (fillBasicInfo)
        {
            var risk = 臨床資訊.RiskAssessmentResult;
            if (risk != null)
            {
                dict["Risk_ExperimentalControl"] = risk.ExperimentalControl ?? "";
                dict["Risk_風險程度"] = risk.風險程度 ?? "";
                dict["Risk_是否需要降15Percent劑量"] = risk.是否需要降15Percent劑量 ?? "";
                dict["Risk_SMA骨骼肌面積"] = risk.SMA骨骼肌面積 ?? "";
                dict["Risk_SMI骨骼肌指標"] = risk.SMI骨骼肌指標 ?? "";
                dict["Risk_SMD骨骼肌密度"] = risk.SMD骨骼肌密度 ?? "";
                dict["Risk_IMAT肌間肌肉脂肪組織"] = risk.IMAT肌間肌肉脂肪組織 ?? "";
                dict["Risk_LAMA低密度肌肉區域"] = risk.LAMA低密度肌肉區域 ?? "";
                dict["Risk_NAMA正常密度肌肉區域"] = risk.NAMA正常密度肌肉區域 ?? "";
                dict["Risk_Myosteatosis肌肉脂肪變性"] = risk.Myosteatosis肌肉脂肪變性 ?? "";
            }
        }

        // ── 依欄位順序組成列 ──────────────────────────────────
        return columns.Select(col => dict.TryGetValue(col, out var v) ? v : "").ToList();
    }

    // ════════════════════════════════════════════════════════════
    // 工具方法
    // ════════════════════════════════════════════════════════════

    private List<VisitCodeModel> CollectAllVisitCodes(Main臨床資料 m)
    {
        var result = new List<VisitCodeModel>();
        void Add(VisitCodeModel? vc)
        {
            if (vc == null) return;
            if (!result.Any(x => x.CompareTo(vc)))
                result.Add(vc);
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

    private HashSet<string> LoadBloodItemNames(string[] filenames)
    {
        var result = new HashSet<string>(StringComparer.Ordinal);
        foreach (var filename in filenames)
        {
            string path = Path.Combine("Data", filename);
            if (!File.Exists(path)) continue;
            string content = File.ReadAllText(path);
            var items = Newtonsoft.Json.JsonConvert.DeserializeObject<List<TestItem檢驗項目>>(content);
            if (items == null) continue;
            foreach (var item in items)
                result.Add(NormalizeItemName(item.項目名稱));
        }
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

            // 問卷 JSON 格式：{ "title": "...", "questions": [...] }
            var survey = Newtonsoft.Json.JsonConvert.DeserializeObject<SurveyTemplate>(content);
            result[kv.Key] = survey?.Questions?.Select(q => q.Id).ToList() ?? new();
        }
        return result;
    }

    private static void FillGradeItem(Dictionary<string, string> dict, string prefix, GradeItemSideEffectsItem item)
    {
        dict[$"{prefix}_RetriveValue"] = item.RetriveValue ?? "";
        for (int g = 1; g <= 5; g++)
        {
            var grade = g switch
            {
                1 => item.Grade1,
                2 => item.Grade2,
                3 => item.Grade3,
                4 => item.Grade4,
                5 => item.Grade5,
                _ => null
            };
            dict[$"{prefix}_Grade{g}_V1"] = grade?.GradeValue1 ?? "";
            dict[$"{prefix}_Grade{g}_V2"] = grade?.GradeValue2 ?? "";
        }
    }

    private static void AddGradeCols(List<string> cols, string prefix)
    {
        cols.Add($"{prefix}_RetriveValue");
        for (int g = 1; g <= 5; g++)
        {
            cols.Add($"{prefix}_Grade{g}_V1");
            cols.Add($"{prefix}_Grade{g}_V2");
        }
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
        // 移除括號、空格、特殊字元，保留中英文數字
        return System.Text.RegularExpressions.Regex.Replace(name.Trim(), @"[\s\(\)\[\]\/\.\^\³μ]", "_").TrimEnd('_');
    }

    private static string ResolveHospital(string? subjectNo)
    {
        if (subjectNo == null) return "";
        if (subjectNo.Contains(MagicObjectHelper.prefix奇美醫院)) return "奇美醫院";
        if (subjectNo.Contains(MagicObjectHelper.prefix郭綜合醫院)) return "郭綜合醫院";
        if (subjectNo.Contains(MagicObjectHelper.prefix成大醫院)) return "成大醫院";
        return "";
    }

    private static IEnumerable<string> PrefixCols(string prefix, IEnumerable<string> names)
        => names.Select(n => $"{prefix}{n}");

    public static string EscapeCsv(string? value)
    {
        if (string.IsNullOrEmpty(value)) return string.Empty;
        if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
            return $"\"{value.Replace("\"", "\"\"")}\"";
        return value;
    }

    // 用於讀取問卷模板 JSON 的內部 DTO
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
