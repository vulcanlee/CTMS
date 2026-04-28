# 單一比 Excel 匯出 Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 在 BrowseView 新增「單一比excel匯出」按鈕，每位病人匯出一列，僅包含臨床資訊與風險評估欄位（不含 VisitCode、臨床資料、抽血資料、CTCAE 5.0、問卷、追蹤資料）。

**Architecture:** 新增獨立 service `PatientSingleRowCsvExportService`，與現有 `PatientCsvExportService` 完全隔離，每位病人輸出一列 CSV（UTF-8 BOM）。BrowseView 注入新 service，點擊按鈕時呼叫並透過 JS 下載檔案。

**Tech Stack:** C# / Blazor Server, `System.Text.StringBuilder` (CSV), `Microsoft.JSInterop` (下載), Dependency Injection (AddTransient)

---

## File Map

| 動作 | 檔案 |
|------|------|
| Create | `src/CTMS/CTMS.Business/Services/ClinicalInformation/PatientSingleRowCsvExportService.cs` |
| Modify | `src/CTMS/CTMS/Program.cs` (line ~143, 注冊服務) |
| Modify | `src/CTMS/CTMS/Components/Views/Commons/BrowseView.razor` (新增按鈕) |
| Modify | `src/CTMS/CTMS/Components/Views/Commons/BrowseView.razor.cs` (注入 + handler) |

---

### Task 1: 建立 PatientSingleRowCsvExportService

**Files:**
- Create: `src/CTMS/CTMS.Business/Services/ClinicalInformation/PatientSingleRowCsvExportService.cs`

- [ ] **Step 1: 建立 service 檔案**

在 `src/CTMS/CTMS.Business/Services/ClinicalInformation/` 新增檔案 `PatientSingleRowCsvExportService.cs`，內容如下：

```csharp
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

        // 標頭
        sb.AppendLine(string.Join(",", columns.Select(EscapeCsv)));

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
            dict["Risk_ExperimentalControl"]          = risk.ExperimentalControl ?? "";
            dict["Risk_風險程度"]                      = risk.風險程度 ?? "";
            dict["Risk_是否需要降15Percent劑量"]        = risk.是否需要降15Percent劑量 ?? "";
            dict["Risk_SMA骨骼肌面積"]                 = risk.SMA骨骼肌面積 ?? "";
            dict["Risk_SMI骨骼肌指標"]                 = risk.SMI骨骼肌指標 ?? "";
            dict["Risk_SMD骨骼肌密度"]                 = risk.SMD骨骼肌密度 ?? "";
            dict["Risk_IMAT肌間肌肉脂肪組織"]           = risk.IMAT肌間肌肉脂肪組織 ?? "";
            dict["Risk_LAMA低密度肌肉區域"]             = risk.LAMA低密度肌肉區域 ?? "";
            dict["Risk_NAMA正常密度肌肉區域"]           = risk.NAMA正常密度肌肉區域 ?? "";
            dict["Risk_Myosteatosis肌肉脂肪變性"]       = risk.Myosteatosis肌肉脂肪變性 ?? "";

            var row = columns.Select(col => dict.TryGetValue(col, out var v) ? v : "").ToList();
            sb.AppendLine(string.Join(",", row.Select(EscapeCsv)));
        }

        // UTF-8 BOM（Excel 開啟中文不亂碼）
        return Encoding.UTF8.GetPreamble()
            .Concat(Encoding.UTF8.GetBytes(sb.ToString()))
            .ToArray();
    }

    private static List<string> BuildColumns() =>
    [
        // 臨床資訊
        "SubjectNo", "Hospital", "收案日期", "ECorOC", "CancerType",
        "Age", "MenstrualStatus", "Height", "Weight", "BMI", "BSA",
        "AbdominalCircumference", "PerformanceStatus",
        "FIGOStaging",
        "AJCCClinicalStage", "AJCCClinicalStageT", "AJCCClinicalStageN", "AJCCClinicalStageM",
        "AJCCPathologicalStage", "AJCCPathologicalStageT", "AJCCPathologicalStageN", "AJCCPathologicalStageM",
        "HistologicalType", "HistologicalTypeDetail",
        "MMRProtein", "P53",
        "HormonStatus", "HormonStatusER", "HormonStatusERPercent",
        "HormonStatusPR", "HormonStatusPRPercent",
        // 風險評估
        "Risk_ExperimentalControl", "Risk_風險程度", "Risk_是否需要降15Percent劑量",
        "Risk_SMA骨骼肌面積", "Risk_SMI骨骼肌指標", "Risk_SMD骨骼肌密度",
        "Risk_IMAT肌間肌肉脂肪組織", "Risk_LAMA低密度肌肉區域",
        "Risk_NAMA正常密度肌肉區域", "Risk_Myosteatosis肌肉脂肪變性"
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
```

- [ ] **Step 2: Commit**

```bash
git add src/CTMS/CTMS.Business/Services/ClinicalInformation/PatientSingleRowCsvExportService.cs
git commit -m "feat: add PatientSingleRowCsvExportService for clinical info + risk assessment export"
```

---

### Task 2: 在 Program.cs 注冊服務

**Files:**
- Modify: `src/CTMS/CTMS/Program.cs` (line ~143)

- [ ] **Step 1: 在 AddTransient<PatientCsvExportService>() 下方新增一行**

找到以下程式碼（約第 143 行）：
```csharp
builder.Services.AddTransient<PatientCsvExportService>();
```
在其**正下方**新增：
```csharp
builder.Services.AddTransient<PatientSingleRowCsvExportService>();
```

- [ ] **Step 2: Commit**

```bash
git add src/CTMS/CTMS/Program.cs
git commit -m "feat: register PatientSingleRowCsvExportService in DI"
```

---

### Task 3: 新增按鈕到 BrowseView.razor

**Files:**
- Modify: `src/CTMS/CTMS/Components/Views/Commons/BrowseView.razor`

- [ ] **Step 1: 在「匯出 Excel」按鈕後新增「單一比excel匯出」按鈕**

找到以下程式碼（第 41-43 行）：
```razor
        <button class="btn btn-export" @onclick="@(async () => await On匯出ExcelAsync())">
            <span class="icon-refresh">📥</span> 匯出 Excel
        </button>
```
在其**正下方**新增：
```razor
        <button class="btn btn-export" @onclick="@(async () => await On單一比ExcelAsync())">
            <span class="icon-refresh">📊</span> 單一比excel匯出
        </button>
```

- [ ] **Step 2: Commit**

```bash
git add src/CTMS/CTMS/Components/Views/Commons/BrowseView.razor
git commit -m "feat: add 單一比excel匯出 button to BrowseView"
```

---

### Task 4: 在 BrowseView.razor.cs 注入 service 並實作 handler

**Files:**
- Modify: `src/CTMS/CTMS/Components/Views/Commons/BrowseView.razor.cs`

- [ ] **Step 1: 在 PatientCsvExportService 注入宣告下方新增注入**

找到以下程式碼（約第 322 行）：
```csharp
    [Inject] PatientCsvExportService PatientCsvExportService { get; set; } = default!;
```
在其**正下方**新增：
```csharp
    [Inject] PatientSingleRowCsvExportService PatientSingleRowCsvExportService { get; set; } = default!;
```

- [ ] **Step 2: 在 On匯出ExcelAsync() 方法下方新增 handler**

找到以下程式碼（約第 346 行）：
```csharp
        await JS.InvokeVoidAsync("downloadFileFromStream", fileName, streamRef);
    }
```
在其**正下方**新增：
```csharp
    async Task On單一比ExcelAsync()
    {
        // 取得全部病人（不分頁）
        var allPatients = await PatientService.GetAsync();

        // 反序列化每位病人的 JsonData
        var patientDataList = new List<PatientData>();
        foreach (var item in allPatients)
        {
            var pd = new PatientData();
            pd.FromJson(item.JsonData);
            patientDataList.Add(pd);
        }

        // 呼叫匯出服務
        var csvBytes = PatientSingleRowCsvExportService.Export(patientDataList);

        var stream = new MemoryStream(csvBytes);
        using var streamRef = new DotNetStreamReference(stream: stream);
        var fileName = $"CTMS單一比_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
        await JS.InvokeVoidAsync("downloadFileFromStream", fileName, streamRef);
    }
```

- [ ] **Step 3: Commit**

```bash
git add src/CTMS/CTMS/Components/Views/Commons/BrowseView.razor.cs
git commit -m "feat: wire up 單一比excel匯出 button handler in BrowseView"
```

---

## 驗收標準

1. 點擊「單一比excel匯出」下載 `CTMS單一比_YYYYMMDD_HHmmss.csv`
2. CSV 每位病人**一列**
3. 標頭包含臨床資訊欄位（不含 VisitCode）與 `Risk_` 前綴的風險評估欄位
4. 不含 臨床資料/抽血資料/CTCAE 5.0/問卷/追蹤資料 相關欄位
5. 以 Excel 開啟中文正常顯示（UTF-8 BOM）
6. 現有「匯出 Excel」大表功能不受影響
