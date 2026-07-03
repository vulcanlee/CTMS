# 新增三家醫院實作計畫（做法 B：HospitalRegistry 集中化）

## Context

CTMS 目前支援三家醫院（成大 `NCKUH`、奇美 `CHIMEIH`、郭綜合 `KGH`），醫院定義以硬編碼散落約 18 處（switch、連續 if、固定清單、Excel 分頁、JSON）。docs 已完成決策文件（`docs/07-擴充指南/新增三家醫院-實施做法.md`、`docs/07-擴充指南/新增醫院.md`），本次實際完成三家新醫院的程式實作。文件已定案、程式尚未動工；`feature/add-3hospital` 分支已存在但落後一個 commit，先 fast-forward 併入 652d88f 再實作。

## 已定案決策（使用者確認）

| 醫院 | Prefix | 短名 | 處理方式 |
| --- | --- | --- | --- |
| 柳營奇美醫院 | `CHIMEIH`（沿用奇美） | —（群組歸奇美） | 特例：只需院別下拉 + 院別→prefix 對應，其餘全繼承奇美 |
| 高雄榮民總醫院 | `KSVGH` | 高榮 | 完整新增 |
| 嘉義長庚紀念醫院 | `CYCGMH` | 嘉長 | 完整新增 |

- **做法 B**：建立集中化 `HospitalRegistry`，散落 switch/if 改讀清單；既有常數字串（NCKUH/CHIMEIH/KGH、成大/奇美/郭綜合）保留不動、只引用。
- **隨機分組表**：高榮、嘉長從奇美 Early/Advance 分頁**複製內容**成獨立新分頁。
- **抽血檢驗檔**：複製奇美 `抽血檢驗血液1/生化1.json` 成 `血液3/生化3`（高榮）、`血液4/生化4`（嘉長）。
- **LastSubjectNoView** 後台流水號欄位要做（高榮、嘉長）；TargetCases（294）不動；起始流水號 0。
- 收尾：appsettings.json `1.1.220` → `1.1.221`、更新 docs（UTF-8 with BOM）。

## 兩個關鍵設計發現（超出 docs 清單的部分）

1. **儀表板靜默漏算陷阱（已核對原始碼）**：`DashboardService.GetHospitalName`（DashboardService.cs:324-347）用 `hospital.Contains(短名)` 比對，但「高雄榮民總醫院」不含「高榮」、「嘉義長庚紀念醫院」不含「嘉長」——照舊模式加 Contains 判斷必定 100% 漏算。Registry 的正規化方法必須**先做 DisplayName 精確比對**，Contains(短名) 只留作容錯 fallback。
2. **`RandomListRuntime.json` 不只是快取**：它保存已落地的隨機分配結果（用過的碼標記 `**`、回填 SubjectNo）。生產環境刪檔重建＝清空已分配隨機碼＝臨床試驗資料事故。改在 `RandomListService.InitialAsync` 加「**自癒式補讀**」：讀入現有 runtime json 後，發現 registry 中有醫院不存在於 Items，就只從 xlsx 補讀該院分頁並附加存檔。生產升版零手動步驟、既有分配原封不動。

## 實作內容

### 1. 常數與 Registry（新增，不改既有）

`src/CTMS/CTMS.Share/Helpers/MagicObjectHelper.cs` 只**新增** 12 個常數：
- `prefix高雄榮總醫院="KSVGH"`、`prefix嘉義長庚醫院="CYCGMH"`
- `PrefixSheetName高雄榮總醫院="高榮"`、`PrefixSheetName嘉義長庚醫院="嘉長"`
- `Sheet高榮Early/Advance`、`Sheet嘉長Early/Advance`
- `高榮抽血檢驗血液File="抽血檢驗血液3.json"`、`高榮抽血檢驗生化File="抽血檢驗生化3.json"`、嘉長用 4

**新檔** `src/CTMS/CTMS.Share/Helpers/HospitalRegistry.cs`（CTMS.Share 是最底層專案，DataModel/ExcelUtility/Business/Web 都能引用）：

- `HospitalDefinition` 欄位：`Prefix`、`DisplayName`（院別全名＝下拉字串＝Patient.醫院）、`ShortName`、`SheetEarly/SheetAdvance`、`BloodHematologyFile/BloodBiochemistryFile`、`CounterKey`（流水號鍵）、`IsPrefixOwner`（柳營奇美 = false，Prefix 填 `CHIMEIH`）。
- 六筆資料：成大（CounterKey `NCKUH成大`）、奇美（`CHIMEIH奇美`）、郭綜合（`KGH郭綜合`）、柳營奇美（IsPrefixOwner=false）、高榮（`KSVGH高榮`）、嘉長（`CYCGMH嘉長`）。既有三筆全部引用 MagicObjectHelper 既有常數。
- 查詢方法：`PrefixOwners`（迭代用，自動排除柳營奇美）、`GetByDisplayName(院別)`、`GetOwnerByPrefix(prefix)`、`GetOwnerBySubjectNo(subjectNo)`（Contains 反查；已驗證五個 prefix 互不為子字串）、`NormalizeToOwnerShortName(hospital)`（**精確 DisplayName 比對優先** → 群組 owner 短名；Contains(短名) 為 fallback）。

### 2. 流水號：SubjectNoGeneratorModel 改 Dictionary

- `SubjectNoGeneratorModel.cs`：三個固定屬性 → `Dictionary<string,int> Counters`。`Deserialize<Dictionary<string,int>>` 與既有 JSON `{"NCKUH成大":18,...}` 形狀完全相容，既有計數不可能流失；`ReadAsync` 後對 `PrefixOwners` 逐一 `TryAdd(CounterKey, 0)` 補缺鍵（高榮/嘉長起始 0 自動生效）。固定屬性消費者只有 SubjectNoGeneratorService 與 LastSubjectNoView，皆在本次範圍。
- `SubjectNoGeneratorService.cs:38-55`：`switch(site)` → `GetOwnerByPrefix(site)`，null 時照舊 log+throw；`Counters[owner.CounterKey]++`，組 `{Prefix}{count:0000}`。柳營奇美 site=`CHIMEIH` 自動共用奇美計數。
- `src/CTMS/CTMS/Data/SubjectNoGenerator.json`：加 `"KSVGH高榮":0,"CYCGMH嘉長":0`。

### 3. prefix 反查點（保留 prefix 語意，**不可**改讀 Patient.醫院）

柳營奇美被視為奇美是決策明訂行為（院區群組層級）：
- `SubjectNoHelper.cs`：`GetHospital` → `GetOwnerBySubjectNo(...)?.Prefix ?? "未知"`；`GetBloodFilename` → 回 owner 的血液/生化檔名，null 時 fallback 成大檔名（=現行行為）。
- `RandomParameterMode.cs:44-56`：連續 if → `GetOwnerBySubjectNo`，找不到維持原值。
- `PatientSingleRowCsvExportService.cs` `ResolveHospital` → `GetOwnerBySubjectNo(...)?.DisplayName ?? ""`（柳營奇美匯出顯示「奇美醫院」是文件明訂現階段預期行為）。
- `SystemMaintainServices.cs:214`（Plan agent 盤點出的文件外引用點）：**不改**，屬一次性歷史資料修正。

### 4. 院別對應、下拉、儀表板

- `PatientService.cs:246-259`：`switch(院別)` → `GetByDisplayName(院別)?.Prefix ?? ""`（柳營奇美→CHIMEIH 自動成立）。
- `DropDownListDataService.cs` `Get院別()` → `foreach (HospitalRegistry.All)` 產六筆（含柳營奇美）。
- `DashboardService.cs:34-39` `HospitalNames` 與 `:70-93` `HospitalStats` → 由 `PrefixOwners` 產生（卡片自動變 5 家）。
- `DashboardService.cs:324-347` `GetHospitalName` → `HospitalRegistry.NormalizeToOwnerShortName(hospital)`（柳營奇美醫院→奇美）。
- `wwwroot/js/dashboard.js:142-144` fallback 陣列補 `'高榮','嘉長'` 與兩個 0（保險用）。

### 5. 畫面

- `RandomPage.razor:31-85`：12 個硬編碼 MudTabPanel → `@foreach (HospitalRegistry.PrefixOwners)` 動態產生（每家 EC/OC × Early/Advance 4 頁，共 21 頁含最後自動編號）。RandomView 參數為單向字串，無繫結複雜度。
- `LastSubjectNoView.razor`：三個硬編碼欄位 → `@foreach (PrefixOwners)` + `Dictionary<string,int> Edit` 複本編輯、儲存時寫回 `Counters` 並 SaveAsync。若 SfNumericTextBox 對索引子繫結異常，退階用 `List<CounterRow>`。

### 6. 資料檔

- `src/CTMS/CTMS/Data/RandomList.xlsx`：以一次性 console 腳本（放 scratchpad，用專案現成的 Syncfusion.XlsIO 與 Program.cs:89 的 license key）`Worksheets.AddCopy` 複製奇美 Early/Advance → 高榮Early/Advance、嘉長Early/Advance，**清空 E2:E2000（SubjectNo 欄）避免殘留分配值**，讀回驗證列數與 E 欄全空。
- `RandomListService.cs:45-56`：六段 ReadSheet → `foreach (PrefixOwners)`；`InitialAsync` 加自癒補讀（見上方設計發現 2）。此步與 xlsx 更新**同批完成**（分頁不存在會擲例外）。
- 抽血檢驗檔：複製 `抽血檢驗血液1.json`→`血液3`、`血液4`；`生化1`→`生化3`、`生化4`（csproj 不需改）。

### 7. 收尾

- `src/CTMS/CTMS/appsettings.json` SystemVersion `1.1.220` → `1.1.221`，commit message 註明版本變更。
- 更新 docs（UTF-8 with BOM，跑 `scripts/Test-DocsEncoding.ps1`）：`新增醫院.md` 改寫為 registry 單點新增指南＋記錄六家清單；`新增三家醫院-實施做法.md` 標註已實施；`04-功能模組/儀表板.md`、`受試者編號與隨機分組.md` 同步。

## 實作順序（分五段，每段有驗證關卡）

前置：`feature/add-3hospital` fast-forward 至 652d88f；備份 `Data/SubjectNoGenerator.json`、`Data/RandomListRuntime.json`。

### 第一段：基礎建設（常數 + Registry）
- MagicObjectHelper 新增 12 個常數（實作內容 §1）
- 新檔 HospitalRegistry.cs（六筆資料 + 查詢方法）
- **驗證**：`dotnet build` 成功（純新增，不影響既有行為）

### 第二段：流水號與 prefix 反查
- SubjectNoGeneratorModel 改 Dictionary + TryAdd 補鍵（§2）
- SubjectNoGeneratorService switch → GetOwnerByPrefix
- `Data/SubjectNoGenerator.json` 加 `"KSVGH高榮":0,"CYCGMH嘉長":0`
- SubjectNoHelper、RandomParameterMode、PatientSingleRowCsvExportService 三個 prefix 反查點改讀 registry（§3）
- **驗證**：build；收成大病患得 `NCKUH0019`、json 既有計數 18/2/1 保留（驗收底線）；既有病患抽血檔名、CSV 院名照舊

### 第三段：院別對應、下拉與隨機表資料（xlsx 與服務同批改）
- PatientService switch(院別) → GetByDisplayName；DropDownListDataService Get院別 → foreach All（§4 前半）
- RandomListService 六段 ReadSheet → foreach PrefixOwners；InitialAsync 加自癒補讀（§6）
- 一次性腳本複製 RandomList.xlsx 奇美分頁 → 高榮/嘉長四分頁（清空 E 欄）；複製抽血檢驗 json 3/4
- **驗證**：下拉六筆；柳營奇美收案得 `CHIMEIH000N`（接續奇美）、清單顯示「柳營奇美醫院」；刪 dev runtime json 重建含 KSVGH/CYCGMH；還原備份 runtime json 重啟驗證自癒補讀且既有分配仍在

### 第四段：儀表板與畫面
- DashboardService 三點（HospitalNames、HospitalStats、GetHospitalName → NormalizeToOwnerShortName）+ dashboard.js fallback（§4 後半）
- RandomPage.razor foreach 動態分頁；LastSubjectNoView.razor Dictionary 迭代（§5）
- **驗證**：`/Dashboard` 五欄、**既有三家數字與改版前完全一致**；收高榮病患後「高榮」欄 +1（抓靜默漏算的關鍵驗證）；`/Random` 21 分頁可編輯；五個流水號欄位存檔同步

### 第五段：端到端驗證與收尾
- 高榮、嘉長各跑一遍 docs`新增醫院.md §5` 驗證清單（收案 `KSVGH0001`/`CYCGMH0001` → 隨機回填 → 抽血檔 3/4 → 儀表板 → CSV 院名全名）
- 柳營奇美特例驗證（編號共用、隨機抽奇美表、儀表板併入奇美、CSV 顯示「奇美醫院」）；既有三家全項回歸
- appsettings.json `1.1.220` → `1.1.221`；docs 更新 + `Test-DocsEncoding.ps1`（§7）

## 驗證方式

- 無測試專案：以 `dotnet build src/CTMS/CTMS.sln` + `dotnet run --project src/CTMS/CTMS` 手動走上述檢查清單。
- 文件編碼：`scripts/Test-DocsEncoding.ps1`。

## 風險與對策

| 風險 | 等級 | 對策 |
| --- | --- | --- |
| 高榮/嘉長全名不含短名 → 儀表板靜默漏算 | 高 | Registry 精確 DisplayName 比對優先；步驟 6 專項驗證 |
| 生產 RandomListRuntime.json 含已落地隨機分配，刪檔即事故 | 高 | 自癒補讀取代刪檔；升版前備份 |
| SubjectNoGenerator.json 既有計數流失 → 重號 | 高 | Dictionary 原鍵直讀 + TryAdd 補 0；步驟 2 驗證 |
| 複製分頁 E 欄殘留 SubjectNo | 中 | 腳本清空 E2:E2000 + 讀回驗證 |
| 分頁名打錯 → 啟動擲例外 | 中 | 腳本以常數字串建分頁，不手打 |
| 重構動到既有邏輯 | 中 | 既有常數零改動、每步 build、既有三家回歸（步驟 8） |
