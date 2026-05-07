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
    private static readonly string[] 血液模板檔 = { "抽血檢驗血液.json", "抽血檢驗血液1.json", "抽血檢驗血液2.json" };
    private static readonly string[] 生化模板檔 = { "抽血檢驗生化.json", "抽血檢驗生化1.json", "抽血檢驗生化2.json" };

    private record ColumnDef(string Key, string Header);

    public byte[] Export(List<PatientData> patients)
    {
        int max化學治療 = 1;
        int max其他治療藥物 = 1;

        var 血液欄位 = LoadBloodItemColumns(血液模板檔);
        var 生化欄位 = LoadBloodItemColumns(生化模板檔);

        foreach (var p in patients)
        {
            var m = p.臨床資料;
            foreach (var node in m.臨床資料化學治療.Items)
                max化學治療 = Math.Max(max化學治療, node.Items.Count);
            foreach (var node in m.其他治療藥物.Items)
                max其他治療藥物 = Math.Max(max其他治療藥物, node.Items.Count);
            foreach (var node in m.抽血檢驗血液.Items)
                foreach (var item in node.抽血檢驗血液)
                    血液欄位.TryAdd(NormalizeItemName(item.項目名稱), ExtractChineseName(item.項目名稱));
            foreach (var node in m.抽血檢驗生化.Items)
                foreach (var item in node.抽血檢驗生化)
                    生化欄位.TryAdd(NormalizeItemName(item.項目名稱), ExtractChineseName(item.項目名稱));
        }

        // 去除同名重複欄位，避免同一檢驗項目被排到後方形成大片空白欄。
        血液欄位 = DeduplicateColumnsByDisplayName(血液欄位);
        生化欄位 = DeduplicateColumnsByDisplayName(生化欄位);

        var columns = BuildColumnCatalog(
            max化學治療, max其他治療藥物,
            血液欄位, 生化欄位);

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
                    max化學治療, max其他治療藥物,
                    血液欄位, 生化欄位);
                sb.AppendLine(string.Join(",", row.Select(EscapeCsv)));
            }
            else
            {
                foreach (var vc in visitCodes)
                {
                    var row = BuildRow(columns, 臨床資訊, m, vc,
                        max化學治療, max其他治療藥物,
                        血液欄位, 生化欄位);
                    sb.AppendLine(string.Join(",", row.Select(EscapeCsv)));
                }
            }
        }

        return Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray();
    }

    private List<ColumnDef> BuildColumnCatalog(
        int max化學治療, int max其他治療藥物,
        Dictionary<string, string> 血液欄位, Dictionary<string, string> 生化欄位)
    {
        var cols = new List<ColumnDef>();

        // 病人識別（對照欄）
        cols.Add(new("SubjectNo", "病人編號"));

        // 就診資訊
        cols.Add(new("VisitKey",            "就診_代碼"));
        cols.Add(new("VisitAssessmentDate", "就診_評估日期"));
        cols.Add(new("VisitTimeline",       "就診_Timeline"));
        cols.Add(new("VisitCycleMonth",     "就診_Cycle/Month"));

        // 化學治療（1:N slot）
        for (int i = 1; i <= max化學治療; i++)
            cols.AddRange(PairCols($"Chemo_{i}_", $"臨床資料_化學治療_{i}_", new (string, string)[]
            {
                ("TreatmentDate",      "治療日期"),
                ("BSA",                "BSA"),
                ("RegimenPaclitaxel",  "Regimen Paclitaxel"),
                ("RegimenCarboplatin", "Regimen Carboplatin"),
                ("Reduction",          "Reduction"),
                ("Bevacizumab",        "Bevacizumab"),
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
        int max化學治療, int max其他治療藥物,
        Dictionary<string, string> 血液欄位, Dictionary<string, string> 生化欄位)
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
            var bloodItems = m.抽血檢驗血液.Items
                .Where(x => x.VisitCode.CompareTo(vc))
                .SelectMany(x => x.抽血檢驗血液);
            foreach (var item in bloodItems)
            {
                var key = ResolveColumnKey(血液欄位, item.項目名稱);
                var value = item.檢驗數值 ?? "";
                var unit = ResolveItemUnit(item);
                // 同一次 visit 可能拆成多筆資料，這裡合併後優先保留有值的那筆。
                if (!string.IsNullOrEmpty(value) || !dict.ContainsKey($"Blood_{key}_Value"))
                {
                    dict[$"Blood_{key}_Unit"] = unit;
                    dict[$"Blood_{key}_Value"] = value;
                    dict[$"Blood_{key}_SamplingDate"] = item.SamplingDate?.ToString("yyyy-MM-dd") ?? "";
                }
            }
        }

        if (vc != null)
        {
            var biochemItems = m.抽血檢驗生化.Items
                .Where(x => x.VisitCode.CompareTo(vc))
                .SelectMany(x => x.抽血檢驗生化);
            foreach (var item in biochemItems)
            {
                var key = ResolveColumnKey(生化欄位, item.項目名稱);
                var value = item.檢驗數值 ?? "";
                var unit = ResolveItemUnit(item);
                // 同一次 visit 可能拆成多筆資料，這裡合併後優先保留有值的那筆。
                if (!string.IsNullOrEmpty(value) || !dict.ContainsKey($"Biochem_{key}_Value"))
                {
                    dict[$"Biochem_{key}_Unit"] = unit;
                    dict[$"Biochem_{key}_Value"] = value;
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
        foreach (var x in m.臨床資料化學治療.Items) Add(x.VisitCode);
        foreach (var x in m.抽血檢驗血液.Items) Add(x.VisitCode);
        foreach (var x in m.抽血檢驗生化.Items) Add(x.VisitCode);
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

    private static Dictionary<string, string> DeduplicateColumnsByDisplayName(Dictionary<string, string> columns)
    {
        var result = new Dictionary<string, string>(StringComparer.Ordinal);
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var (key, display) in columns)
        {
            var normalizedDisplay = NormalizeDisplayName(display);
            if (seen.Add(normalizedDisplay))
                result[key] = display;
        }

        return result;
    }

    private static string ResolveColumnKey(Dictionary<string, string> columns, string itemName)
    {
        var normalizedKey = NormalizeItemName(itemName);
        if (columns.ContainsKey(normalizedKey)) return normalizedKey;

        var display = ExtractChineseName(itemName);
        var normalizedDisplay = NormalizeDisplayName(display);

        foreach (var (key, value) in columns)
        {
            if (NormalizeDisplayName(value) == normalizedDisplay)
                return key;
        }

        return normalizedKey;
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

    private static string NormalizeItemName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return "Unknown";
        // 先移除括號及其內容（通常包含單位），避免相同項目產生不同的key
        var cleaned = System.Text.RegularExpressions.Regex.Replace(name.Trim(), @"\s*[\(\[].*?[\)\]]", "");
        // 移除其他特殊字符
        return System.Text.RegularExpressions.Regex.Replace(cleaned, @"[\s\(\)\[\]\/\.\^\³μ]", "_").TrimEnd('_');
    }

    private static string ExtractUnit(string? itemName)
    {
        if (string.IsNullOrWhiteSpace(itemName)) return "";
        var match = System.Text.RegularExpressions.Regex.Match(itemName, @"\(([^)]+)\)\s*$");
        return match.Success ? match.Groups[1].Value : "";
    }

    private static string ResolveItemUnit(TestItem檢驗項目 item)
    {
        if (!string.IsNullOrWhiteSpace(item.單位))
            return item.單位.Trim();
        return ExtractUnit(item.項目名稱);
    }

    private static string ExtractChineseName(string? itemName)
    {
        if (string.IsNullOrWhiteSpace(itemName)) return "Unknown";
        var match = System.Text.RegularExpressions.Regex.Match(itemName.Trim(), @"^[\u4e00-\u9fff\u3400-\u4dbf]+");
        return match.Success ? match.Value : itemName.Trim().Split(' ')[0];
    }

    private static string NormalizeDisplayName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name)) return "Unknown";
        var cleaned = System.Text.RegularExpressions.Regex.Replace(name.Trim(), @"\s*[\(\[].*?[\)\]]", "");
        return System.Text.RegularExpressions.Regex.Replace(cleaned, @"[\s\-_/\.\(\)\[\]，,]+", "").ToUpperInvariant();
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
