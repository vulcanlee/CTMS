# CTMS

CTMS（Clinical Trial Management System）是一套以婦癌臨床試驗情境為核心的管理平台，整合受測者收案、臨床資料維護、抽血與副作用追蹤、問卷蒐集、AI 風險評估、隨機分派、帳號權限管理與操作歷程追蹤。

這個 repository 不只是單一 Web 專案，而是一個以 `ASP.NET Core .NET 9` 為基礎的多專案 solution，包含：

- 主系統 Web 應用程式 `CTMS`
- 商業邏輯層 `CTMS.Business`
- 資料模型與實體模型
- SQLite / EF Core 資料庫
- 背景 AI 工作服務 `AIAgent`
- 輔助工具 `EncodeJSON`

本 README 以「開發者導向」為主，但在前段保留使用者可以快速理解的系統概覽，並補上所有已盤點路由頁面的功能、顯示資訊、可執行操作，以及它們在臨床試驗環境中的價值。

## 目錄

- [一、系統概覽](#一系統概覽)
- [二、系統在臨床試驗環境中的價值](#二系統在臨床試驗環境中的價值)
- [三、技術架構總覽](#三技術架構總覽)
- [四、Solution 與專案結構](#四solution-與專案結構)
- [五、主要流程與資料流](#五主要流程與資料流)
- [六、執行環境需求](#六執行環境需求)
- [七、快速開始](#七快速開始)
- [八、設定檔與重要參數](#八設定檔與重要參數)
- [九、資料庫與 EF Core Migration](#九資料庫與-ef-core-migration)
- [十、AI Agent 與檔案處理流程](#十ai-agent-與檔案處理流程)
- [十一、重要目錄說明](#十一重要目錄說明)
- [十二、路由與頁面功能地圖](#十二路由與頁面功能地圖)
- [十三、目前觀察到的限制與注意事項](#十三目前觀察到的限制與注意事項)
- [十四、參考文件](#十四參考文件)

## 一、系統概覽

### 1. 專案定位

此系統的核心目標，是提供臨床試驗團隊一個可以圍繞「單一受測者個案」展開工作的工作平台。系統將資料切成數個臨床工作面向：

- 收案與受測者主檔
- 基本臨床資訊
- 臨床資料與治療紀錄
- 抽血檢驗
- CTCAE / 副作用
- 問卷
- 追蹤資料
- AI 風險評估與影像確認
- 隨機分派與 Subject No 管理
- 帳號、權限、申請與操作歷程

### 2. 主要功能

- 以儀表板呈現合作醫院、病例數、完成率、癌別與分期統計
- 以受測者清單提供搜尋、篩選、新增、編輯與狀態切換
- 以個案頁面整合多個子模組，支援 Visit Code 切換
- 儲存內膜癌 / 卵巢癌研究所需的結構化臨床資料
- 維護血液與生化抽血檢驗資料，並標示參考區間
- 蒐集副作用與多份問卷資料，並提供行動裝置填寫入口
- 管理其他治療、合併用藥、影像追蹤等 longitudinal data
- 整合 AI 推論流程與風險評估結果展示
- 支援放射科醫師與婦產科醫師簽名確認
- 管理帳號申請、使用者、角色、專案、隨機表與操作歷程

### 3. 適用角色

- 臨床研究護理師 / CRC
- 研究主持人與協同醫師
- 放射科醫師
- 婦產科醫師
- 系統管理者
- 維運 / 開發人員

## 二、系統在臨床試驗環境中的價值

### 1. 降低資料分散

臨床試驗常見的資料來源包括紙本表單、Excel、問卷、影像、醫囑與人工追蹤紀錄。此系統把受測者相關資訊集中在同一個平台，讓團隊可以圍繞個案完整追蹤，而不是在多個工具之間切換。

### 2. 提高資料一致性

- 使用固定資料模型與下拉選單，降低自由輸入造成的格式落差
- 利用 Visit Code 分段維護資料，讓時間序列更清楚
- 透過角色權限控制不同職能可見與可編輯範圍

### 3. 支援研究流程治理

- 帳號申請與審核
- 操作歷程追蹤
- 受測者狀態切換
- 醫師簽名確認
- 隨機分派與 Subject No 編號維護

### 4. 加速 AI 輔助決策落地

系統不是只存放 AI 結果，而是把 AI 相關流程嵌入研究現場：

- 上傳影像
- 上傳標註修正
- 送入 AI 推論
- 顯示量化指標與風險評估
- 顯示實驗組 / 對照組資訊
- 醫師簽名與歷程保留

### 5. 提升跨院協作可視性

從 dashboard、隨機表到 Subject No 生成都看得出來，系統是以跨院資料整合為前提設計，特別針對成大、奇美、郭綜合等合作院所的收案與分派情境。

## 三、技術架構總覽

### 1. 核心技術

- `ASP.NET Core 9.0`
- `Razor Components / Blazor Server` 互動式伺服器渲染
- `Entity Framework Core 9 + SQLite`
- `MudBlazor`
- `Syncfusion Blazor`
- `Ant Design Blazor`
- `AutoMapper`
- `NLog`
- `QRCoder`
- `Azure OpenAI / OpenAI Chat client`
- `fo-dicom`

### 2. 架構分層

- `CTMS`
  負責 UI、路由、頁面組裝、DI 註冊、middleware、靜態檔案與 app 啟動
- `CTMS.Business`
  放商業邏輯、AI 整合、檔案處理、儀表板統計、角色權限與領域服務
- `CTMS.DataModel`
  放 DTO、ViewModel、前端交換模型與大量臨床資料 JSON 對應模型
- `CTMS.EntityModel`
  放 EF Core `DbContext`、實體與 migrations
- `CTMS.ExcelUtility`
  放 Excel / CSV 相關服務，包含 random list 與風險評估輸入輸出
- `CTMS.Share`
  放共用 helper、常數、擴充方法
- `AIAgent`
  獨立背景 Worker，輪詢 Queue 目錄執行 AI 流程
- `AIAgent.Business`
  AI Agent 的商業邏輯與資料夾處理服務
- `EncodeJSON`
  小型工具程式

### 3. 應用程式啟動行為

主系統 `CTMS` 在啟動時會做幾件重要事情：

- 設定 NLog
- 註冊 Razor Components / MudBlazor / Syncfusion / Ant Design
- 設定 SQLite `BackendDB.db`
- 執行 `dbContext.Database.Migrate()`
- 建立預設專案 `Default Project`
- 建立預設角色 `Default Role`
- 建立預設新帳號角色 `預設新建帳號角色`
- 建立預設管理者帳號 `support`
- 將沒有狀態的病患預設成 `收案`

這代表第一次啟動時，系統會自動補齊最基本的資料結構。

## 四、Solution 與專案結構

Solution 檔位於：

`src/CTMS/CTMS.sln`

### 1. Solution 內專案

| 專案 | 類型 | 用途 |
| --- | --- | --- |
| `CTMS` | ASP.NET Core Web | 主系統網站、UI、路由、啟動點 |
| `CTMS.Business` | Class Library | 核心商業邏輯與流程服務 |
| `CTMS.DataModel` | Class Library | DTO 與臨床資料模型 |
| `CTMS.EntityModel` | Class Library | EF Core `DbContext`、實體、Migration |
| `CTMS.ExcelUtility` | Class Library | Excel / CSV 讀寫與 random list 支援 |
| `CTMS.Share` | Class Library | 共用常數與 helper |
| `AIAgent` | Worker Service | 背景 AI 佇列處理服務 |
| `AIAgent.Business` | Class Library | AI 流程邏輯 |
| `EncodeJSON` | Console App | JSON 編碼 / 處理工具 |

### 2. 重要的 UI 組織方式

`CTMS` 專案中的畫面大致分成：

- `Components/Pages`
  真正對應路由的頁面
- `Components/Views`
  頁面內主要功能 View
- `Components/Commons`
  共用對話框、提示、picker、上傳元件
- `Components/Layout`
  Layout 與 Header / Footer / 導覽骨架

### 3. 個案頁面的導航模型

系統以 `BottomNavigationView` 串接主要受測者工作頁：

- `/ClinicalInformation/{code}`
- `/BloodTest/{code}`
- `/SideEffectPage/{code}`
- `/Survey/{code}`
- `/TrackingData/{code}`
- `/RiskAssessment/{code}`

其中 `{code}` 對應的是 `Patient.Code`，不是畫面上顯示的 `Subject No`。

## 五、主要流程與資料流

### 1. 使用者登入與工作起點

1. 使用者進入 `/Auths/Login`
2. 登入成功後建立 Cookie Authentication
3. 首頁 `/` 顯示 splash screen，初始化 random list 與 subject number generator
4. 驗證登入狀態後導向 `/Dashboard`
5. 由儀表板進入 `/Browser`，再選擇或新增受測者

### 2. 個案處理流程

1. 於 `/Browser` 找到或建立受測者
2. 進入 `/BasicClinical/{code}` 維護主檔與 AI 相關控制點
3. 從底部導覽切換到臨床資料、抽血、副作用、問卷、追蹤、風險評估等頁面
4. 各頁面依 Visit Code 維護 longitudinal data
5. 操作結果寫回病患 `JsonData`
6. 部分操作會產生操作歷程

### 3. 帳號申請流程

1. 使用者在 `/Register` 送出申請
2. 申請資料被寫入註冊模型清單
3. 管理者在 `/ApprovalRegistration` 審核
4. 管理者核准後建立正式帳號並套用預設角色

### 4. AI 流程

1. 於基本臨床頁上傳影像或標註修正
2. 送入 AI 推論
3. 背景 `AIAgent` 監看 Queue 資料夾
4. 推論結果回寫到個案資料與相關輸出資料夾
5. `/RiskAssessment/{code}` 顯示量化結果與建議
6. 醫師簽名確認，形成研究流程憑證

## 六、執行環境需求

### 1. 建議環境

- Windows
- `.NET SDK 9.x`
- Visual Studio 2022 或以上，或使用 `dotnet CLI`
- 可寫入本機檔案系統的權限

### 2. 為何建議 Windows

從程式碼與套件可見此專案有明顯 Windows 傾向：

- `Syncfusion.HtmlToPdfConverter.Net.Windows`
- 預設檔案路徑為 `C:\...`
- 影像與檔案處理流程以 Windows 路徑為前提

### 3. 資料庫

- SQLite
- 預設連線字串：`Data Source=BackendDB.db`
- 啟動後會自動 migration

### 4. 第三方授權與外部依賴

專案目前直接使用或引用：

- Syncfusion 授權
- Azure OpenAI endpoint / model
- 本機 queue 與 AI inference 目錄

正式環境建議將敏感資訊移出原始碼與 `appsettings.json`，改由安全設定機制管理。

## 七、快速開始

### 1. 還原套件

```powershell
dotnet restore .\src\CTMS\CTMS.sln
```

### 2. 編譯

```powershell
dotnet build .\src\CTMS\CTMS.sln
```

### 3. 啟動主系統

```powershell
dotnet run --project .\src\CTMS\CTMS\CTMS.csproj
```

### 4. 啟動 AI 背景服務

```powershell
dotnet run --project .\src\CTMS\AIAgent\AIAgent.csproj
```

### 5. 初次啟動預期結果

- 自動建立 / 更新 SQLite schema
- 建立預設專案與角色
- 建立預設管理者帳號 `support`
- 預設管理者密碼同帳號名稱，亦即 `support`

> 安全提醒：此預設帳密僅適合本機或開發情境。正式環境必須立即調整。

### 6. 常用進入點

- 登入頁：`/Auths/Login`
- 首頁 splash：`/`
- 儀表板：`/Dashboard`
- 受測者瀏覽：`/Browser`

## 八、設定檔與重要參數

主系統設定檔：

- `src/CTMS/CTMS/appsettings.json`
- `src/CTMS/CTMS/appsettings.Development.json`

AI Agent 設定檔：

- `src/CTMS/AIAgent/appsettings.json`
- `src/CTMS/AIAgent/appsettings.Development.json`

### 1. `CTMSSettings`

主要包含：

- `ConnectionStrings.SQLiteDefaultConnection`
- `SystemInformation.SystemName`
- `SystemInformation.SystemDescription`
- `SystemInformation.SystemVersion`

### 2. `AgentSetting`

目前可見的設定重點：

- `DicomFolderPath`
- `QueueFolderPath`
- `InBoundQueueName`
- `Phase1QueueName`
- `Phase1WaitingQueueName`
- `Phase2QueueName`
- `Phase2WaitingQueueName`
- `Phase3QueueName`
- `Phase3WaitingQueueName`
- `OutBoundQueueName`
- `CompleteQueueName`
- `InferencePath`

### 3. 靜態檔案與輸出資料夾

主系統會把下列資料夾暴露為靜態內容或作為執行期工作目錄：

- `UploadFiles`
- `DownloadFiles`
- `PdfFiles`
- `UploadTemp`
- `DecompressFiles`
- `DawnCache`
- `OperationContent`
- `Data`

## 九、資料庫與 EF Core Migration

### 1. DbContext

- `CTMS.EntityModel/BackendDBContext.cs`

目前主要 DbSet 包含：

- `Patient`
- `MyUser`
- `Project`
- `RoleView`
- `RoleViewProject`
- `OperationHistoryTrace`

### 2. Migration 專案

- Migration 放在 `src/CTMS/CTMS.EntityModel/Migrations`
- 官方啟動專案是 `CTMS`

### 3. 常用指令

新增 migration：

```powershell
dotnet ef migrations add <MigrationName> --project .\src\CTMS\CTMS.EntityModel\CTMS.EntityModel.csproj --startup-project .\src\CTMS\CTMS\CTMS.csproj
```

更新資料庫：

```powershell
dotnet ef database update --project .\src\CTMS\CTMS.EntityModel\CTMS.EntityModel.csproj --startup-project .\src\CTMS\CTMS\CTMS.csproj
```

移除最新 migration：

```powershell
dotnet ef migrations remove --project .\src\CTMS\CTMS.EntityModel\CTMS.EntityModel.csproj --startup-project .\src\CTMS\CTMS\CTMS.csproj
```

### 4. 參考文件

repo 已經有既有說明：

- `docs/EFCore.md`
- `docs/Migration History.md`
- `docs/SQLite.md`

## 十、AI Agent 與檔案處理流程

### 1. `AIAgent` 的角色

`AIAgent` 是獨立的 Worker Service，不直接提供 UI，而是持續巡迴監看 queue 資料夾並執行 AI 流程。

### 2. 啟動行為

- 檢查是否已有同名程序在執行
- 初始化 NLog
- 啟動 `AIAgentWorker`
- 建立 queue 目錄
- 每 500ms 輪詢一次工作

### 3. 可觀察到的 Queue 階段

- `InBound`
- `Phase1`
- `Phase1Waiting`
- `Phase2`
- `Phase2Waiting`
- `Phase3`
- `Phase3Waiting`
- `OutBound`
- `Complete`

### 4. 與主系統的關聯

主系統中的基本臨床頁與風險評估頁，顯示了 AI 流程在 UI 端的落點：

- 影像上傳
- 標註修正上傳
- 推送 AI
- 呈現 AI 風險結果
- 顯示實驗組 / 對照組
- 醫師確認與歷程查詢

## 十一、重要目錄說明

| 路徑 | 用途 |
| --- | --- |
| `src/CTMS/CTMS` | 主系統 Web 專案 |
| `src/CTMS/CTMS.Business` | 商業邏輯與流程服務 |
| `src/CTMS/CTMS.DataModel` | DTO 與臨床 JSON 模型 |
| `src/CTMS/CTMS.EntityModel` | EF Core 實體與 migration |
| `src/CTMS/AIAgent` | 背景 AI 服務 |
| `src/CTMS/AIAgent.Business` | AI 流程邏輯 |
| `src/CTMS/CTMS/Components/Pages` | 路由頁面 |
| `src/CTMS/CTMS/Components/Views` | 頁面功能 View |
| `src/CTMS/CTMS/Data` | 問卷、檢驗、random list 等資料檔 |
| `docs` | 額外文件與資料 |
| `Reference` | 參考資料 |

## 十二、路由與頁面功能地圖

本節依實際程式碼盤點目前已存在的路由。部分頁面已高度整合研究流程，部分頁面則仍偏原型、示範或保留用途，README 會一併標註。

### A. 共用、登入與入口頁

#### `/`

- 頁面用途：系統入口 splash page。
- 主要資訊：顯示啟動畫面與 Logo。
- 主要操作：
  - 初始化 `RandomListService`
  - 初始化 `SubjectNoGeneratorService`
  - 檢查登入狀態
  - 通過驗證後導向 `/Dashboard`
- 前置條件：使用者應先完成登入。
- 臨床試驗價值：
  - 確保進入主流程前先載入關鍵研究資源
  - 讓收案編號與隨機分派資料在使用前先就緒

#### `/Auths/Login`

- 頁面用途：使用者登入頁。
- 主要資訊：
  - 帳號
  - 密碼
  - 錯誤訊息
  - 帳號申請入口
- 主要操作：
  - 送出登入
  - 建立 Cookie Authentication
  - 紀錄登入操作歷程
  - 導向原始 ReturnUrl 或首頁
- 前置條件：帳號已存在且可用。
- 臨床試驗價值：
  - 讓研究資料維持可追蹤的身分認證
  - 登入事件可進入操作歷程，支援稽核需求

#### `/Auths/Logout`

- 頁面用途：登出頁。
- 主要操作：
  - 清除 Cookie
  - 導回登入頁
- 臨床試驗價值：
  - 避免共用工作站上的帳號殘留
  - 降低受試者敏感資料外洩風險

#### `/Register`

- 頁面用途：公開帳號申請頁。
- 主要資訊：
  - 名稱
  - 帳號
  - 密碼
  - Email
- 主要操作：
  - 送出申請
  - 寫入待審核註冊資料
  - 導回首頁
- 實作現況：此頁面已具備基本送件流程，申請狀態會記為 `待審核`。
- 臨床試驗價值：
  - 讓跨院或新加入研究人員可透過正式流程申請帳號
  - 形成治理而非私下建立帳號

#### `/ChangePassword`

- 頁面用途：變更目前使用者密碼。
- 主要資訊：
  - 新密碼
  - 確認密碼
- 主要操作：
  - 驗證密碼一致
  - 呼叫 `MyUserService.ChangePasswordAsync`
  - 成功後自動登出
- 臨床試驗價值：
  - 降低共用預設密碼或初始密碼造成的風險

#### `/Error`

- 頁面用途：標準錯誤頁。
- 主要資訊：
  - 錯誤提示
  - Request ID
  - Development mode 說明
- 臨床試驗價值：
  - 發生例外時保留追查入口
  - 在正式環境中避免直接暴露過多細節

#### `/Sample`

- 頁面用途：樣板 / 測試性頁面。
- 實作現況：偏示範用，非主要研究流程頁。
- 臨床試驗價值：
  - 對正式研究流程價值有限
  - 較適合作為 UI 驗證或開發測試用途

### B. 儀表板與受測者總覽

#### `/Dashboard`

- 頁面用途：研究總覽儀表板。
- 主要資訊：
  - 合作醫院數與院所清單
  - 總病例數
  - 本月新增病例數
  - 完成率與成長幅度
  - 分析報告數量
  - 分期統計圖
  - 卵巢癌 / 內膜癌統計圖
  - 完成 / 未完成統計圖
- 主要操作：
  - 切換主題
  - 前往病人資料瀏覽
- 臨床試驗價值：
  - 讓 PI、CRC 或管理者即時看到研究進度
  - 幫助跨院收案與完成率監控

#### `/Browser`

- 頁面用途：受測者瀏覽、搜尋與工作入口頁。
- 主要資訊：
  - 病人編號
  - 醫院名稱
  - 癌症類型
  - 期別
  - 組別
  - 狀態
- 主要操作：
  - 新增受測者資料
  - 重新整理
  - 開啟 / 收合篩選條件
  - 依醫院、癌別、狀態與關鍵字搜尋
  - 修改個案資料
  - 管理者可切換受測者狀態
- 臨床試驗價值：
  - 作為日常收案與追蹤的主要入口
  - 讓研究團隊快速找到特定族群或個案
  - 將收案、退出等狀態管理結合操作歷程

### C. 個案工作區頁面

這組頁面多數帶有 `{code}` 參數，表示針對特定受測者工作。`{code}` 對應 `Patient.Code`。

#### `/BasicClinical/{code}`

- 頁面用途：個案總控台與基本臨床資訊頁。
- 主要資訊：
  - Subject No
  - 癌別
  - 年齡
  - 月經狀態
  - 身高、體重、BMI、BSA、腰圍
  - 日常體能狀態
  - 組織型態與細節
  - MMR protein 結果
- 主要操作：
  - 編輯與儲存基本臨床資料
  - 開啟 Visit Code 設定
  - 通知放射科醫師
  - 上傳影像
  - 上傳修改標註
  - 送出 AI 推論
  - 確認 AI 結果
  - 導向其他個案模組
- 權限：需要 `ROLE瀏覽`，其中 AI 功能另受 `ROLEAI操作` 等權限控制。
- 臨床試驗價值：
  - 是單一受測者的主工作台
  - 把收案主檔、影像、AI 與後續模組串在一起
  - 讓研究團隊從一頁掌握個案進度與下一步動作

#### `/ClinicalInformation/{code}`

- 頁面用途：臨床資料頁。
- 主要資訊 / Tab：
  - 手術 `SurgeryView`
  - 病理報告 `PathologyReportView`
  - 化學治療 `ChemotherapyView`
  - 合併用藥 `CommonMedicationView`
  - 基線病史 / 病史表單 `BaselineMedicalHistoryFormView`
- 主要操作：
  - 依 tab 維護不同類型的臨床資料
  - 由底部導覽切換到其他模組
- 權限：`ROLE臨床資料`
- 臨床試驗價值：
  - 將治療前後的重要臨床資訊結構化保存
  - 有助於後續研究分析、審查與資料清理

#### `/BloodTest/{code}`

- 頁面用途：抽血檢驗頁。
- 主要資訊 / Tab：
  - 抽血檢驗（血液）
  - 抽血檢驗（生化）
- 主要資訊內容：
  - 檢驗項目
  - 參考區間
  - 檢驗數據
  - Sampling 日期
  - Visit Code
- 主要操作：
  - 進入編輯模式
  - 批次重設所有日期
  - 鎖定編輯，避免覆蓋既有值
  - 儲存與取消
- 權限：`ROLE抽血資料`
- 臨床試驗價值：
  - 讓抽血資料可依時點追蹤
  - 將參考區間與結果放在同一畫面，利於臨床判讀與資料核對

#### `/SideEffectPage/{code}`

- 頁面用途：副作用與 CTCAE 相關頁面。
- 主要資訊 / Tab：
  - 血液副作用
  - Survey 1 副作用
  - Survey 2 副作用
- 主要操作：
  - 維護不同類型副作用紀錄
  - 透過底部導覽切換其他研究模組
- 權限：`ROLE副作用`
- 臨床試驗價值：
  - 讓研究團隊持續追蹤治療毒性與副作用發生情形
  - 對安全性監測與資料分析十分重要

#### `/Survey/{code}`

- 頁面用途：問卷主頁。
- 主要資訊 / Tab：
  - 化療副作用
  - 標靶副作用
  - 放療副作用
  - WHOQOL 問卷
  - 個人史
  - 家族史
  - 生活品質
  - 健康
  - QR Code
- 主要操作：
  - 維護各類問卷
  - 產生 QR Code 供行動裝置填寫
  - 依問卷完成狀態切換圖示
- 權限：`ROLE問卷`
- 臨床試驗價值：
  - 將 PRO（Patient-Reported Outcomes）與副作用問卷正式納入個案流程
  - 方便研究團隊追蹤不同時點的生活品質與症狀變化

#### `/SurveyMobile/{code}`

- 頁面用途：行動裝置版問卷頁。
- 主要資訊 / Tab：
  - 與 `/Survey/{code}` 大致相同，但偏向行動填寫情境
- 主要操作：
  - 以手機或平板填寫問卷
- 權限：`ROLE問卷`
- 臨床試驗價值：
  - 支援受測者或臨床現場用行動裝置直接輸入
  - 降低紙本轉錄負擔

#### `/Questionnaire/{code}`

- 頁面用途：舊版 / 保留中的問卷頁。
- 主要資訊：
  - 目前掛載 `WhooqolView`
- 實作現況：
  - 目前內容相對精簡，明顯比 `/Survey/{code}` 與 `/SurveyMobile/{code}` 更偏早期版本或保留頁。
- 臨床試驗價值：
  - 現階段價值較像歷史兼容入口
  - 正式問卷流程建議以 `/Survey/{code}` 為主

#### `/TrackingData/{code}`

- 頁面用途：追蹤資料頁。
- 主要資訊 / Tab：
  - 其他治療
  - 其他治療-藥物
  - 其他治療-影像
- 主要資訊內容：
  - Visit Code
  - 治療日期、藥物、劑量、路徑、單位
  - CXR、EKG、CT、MRI、Bone scan 等影像時點
- 主要操作：
  - 編輯與儲存 longitudinal 追蹤資料
  - 新增藥物紀錄
- 權限：`ROLE追蹤資料`
- 臨床試驗價值：
  - 支援治療後持續追蹤
  - 將非主要治療、合併療法與影像監測整合到同一資料鏈

#### `/RiskAssessment/{code}`

- 頁面用途：AI 風險評估結果頁。
- 主要資訊：
  - 原始影像
  - AI 結果影像
  - SMA / SMI / SMD / IMAT / LAMA / NAMA / Myosteatosis
  - 實驗組 / 對照組訊息
  - AI 輔助決策結果
  - 風險程度
  - 是否需降 15% 劑量
  - 放射科與婦產科簽名紀錄
- 主要操作：
  - 查看歷史簽呈
  - 放射科醫師簽名確認
  - 婦產科醫師簽名確認
- 權限：`ROLE風險評估`，其中部分確認行為還會受更細緻權限限制。
- 臨床試驗價值：
  - 是 AI 研究價值呈現最直接的頁面
  - 把影像量化結果與臨床決策建議連結起來
  - 透過雙醫師簽名提高研究流程可信度與稽核性

### D. 管理與後台頁面

#### `/ApprovalRegistration`

- 頁面用途：帳號申請審查頁。
- 主要資訊：
  - 名稱
  - 帳號
  - 電子郵件
  - 建立時間
  - 狀態
- 主要操作：
  - 建立正式帳號
  - 刪除申請
- 權限：管理者
- 臨床試驗價值：
  - 讓研究團隊進入系統前有正式審查點
  - 避免帳號任意建立，符合研究治理需求

#### `/UserManagement`

- 頁面用途：使用者管理頁。
- 主要資訊：
  - 帳號
  - 名稱
  - 啟用狀態
  - 是否為管理者
  - 角色
  - Email
- 主要操作：
  - 新增 / 編輯 / 刪除使用者
  - 啟用 / 停用使用者
  - 指派角色
- 權限：管理者
- 臨床試驗價值：
  - 強化帳號治理與職責分工
  - 避免未授權人員接觸敏感研究資料

#### `/Project`

- 頁面用途：專案管理頁。
- 主要資訊：
  - 專案名稱
- 主要操作：
  - 新增 / 編輯 / 刪除專案
- 權限：管理者
- 臨床試驗價值：
  - 支援多研究專案或多資料視角管理
  - 為角色與資料範圍切分提供基礎

#### `/RoleView`

- 頁面用途：角色模板與權限管理頁。
- 主要資訊：
  - 角色名稱
  - 角色細項
  - 各功能是否啟用
- 主要操作：
  - 新增 / 編輯 / 刪除角色
  - 勾選功能權限
  - 查看角色細節
- 權限：管理者
- 臨床試驗價值：
  - 讓不同專業角色只看見必要資訊
  - 降低誤操作與資料外洩風險

#### `/HistoryTrace`

- 頁面用途：操作歷程頁。
- 主要資訊：
  - 使用者
  - 類型
  - Subject No
  - 主題 / 詳細說明
  - 發生時間
- 主要操作：
  - 查閱紀錄
  - 編輯 / Retry / 刪除
  - 以對話框檢視完整內容
- 權限：管理者
- 臨床試驗價值：
  - 研究資料治理的重要基礎
  - 便於追查誰在什麼時候修改了什麼

#### `/Random`

- 頁面用途：隨機分派表與 Subject No 自動編號管理頁。
- 主要資訊 / Tab：
  - 成大 / 奇美 / 郭綜合
  - EC / OC
  - Early / Advance
  - 最後自動編號
- 主要資訊內容：
  - Random list 的 `Id`、`BlockId`、`BlockSize`、`Treatment`、`SubjectNo`
  - 各院的最後自動編號值
- 主要操作：
  - 編輯 random list 對應的 `SubjectNo`
  - 儲存 random list
  - 調整各院 Subject No 起始 / 最後序號
- 權限：管理者
- 臨床試驗價值：
  - 是受測者分派與編號控制的核心頁
  - 對研究公平性、追蹤一致性與跨院協作非常重要

#### `/PatientKeyName`

- 頁面用途：AI 結果 Key Name / Subject 對應管理頁。
- 主要資訊：
  - Key Name 目錄列表
  - Subject Code
  - Key Name
- 主要操作：
  - 點擊 Key Name
  - 變更病歷號 / Subject No
  - 刪除目錄
- 權限：管理者
- 臨床試驗價值：
  - 協助 AI 產出檔案與研究個案之間建立正確對應
  - 降低檔名與主檔脫鉤造成的風險

#### `/Export`

- 頁面用途：資料匯出原型頁。
- 主要操作：
  - `Export Data` 按鈕
- 實作現況：
  - 目前程式碼僅示範讀取單一病患並轉回 `PatientData`
  - 尚未看見完整匯出檔案流程
- 臨床試驗價值：
  - 潛在上可作為資料匯出入口
  - 目前更接近開發中原型

#### `/SignUp`

- 頁面用途：另一個帳號申請 / 註冊頁。
- 主要資訊：
  - 帳號
  - 姓名
  - 信箱
  - 備註
- 實作現況：
  - 表單 UI 已存在
  - `OnSignAsync()` 目前未見實作
- 臨床試驗價值：
  - 目前較偏預留頁或未完成頁

### E. 其他功能頁

#### `/MyNote`

- 頁面用途：內部記事 / 備忘錄頁。
- 主要資訊：
  - 代號
  - 名稱
- 主要操作：
  - 新增
  - 編輯
  - 刪除
- 臨床試驗價值：
  - 可作為研究過程中的內部備註工具
  - 比較偏輔助性功能，不是核心 EDC 流程

## 十三、目前觀察到的限制與注意事項

### 1. 部分頁面仍在開發中或偏原型

從目前程式碼可見，下列頁面尚未完全成形或功能較保留：

- `/Export`
- `/SignUp`
- `/Questionnaire/{code}`
- `/Sample`

### 2. `CTMS.Tests` 目前不在 solution 清單內

雖然 repository 中存在 `src/CTMS/CTMS.Tests` 目錄與編譯輸出痕跡，但目前 `src/CTMS/CTMS.sln` 的專案清單不包含 `CTMS.Tests`。這代表：

- 專案曾經有測試資產或測試輸出
- 目前 solution 層級並未把它作為正式專案管理

### 3. 預設帳號存在安全風險

啟動時會建立：

- 帳號：`support`
- 密碼：`support`

正式環境請務必：

- 第一時間變更密碼
- 限制部署環境
- 補上更完整的帳密與祕密管理策略

### 4. 敏感設定目前有硬編碼跡象

從程式碼可見，AI endpoint、API key、Syncfusion 授權等資料目前並未完全外部化。若要進入正式環境，建議優先改善。

### 5. 平台相依性偏高

由於使用 Windows 專用套件與 Windows 路徑，部署到 Linux 前需要重新檢查：

- PDF 轉換元件
- 檔案路徑
- 影像處理依賴

### 6. `NavMenu.razor` 仍保留預設模板痕跡

專案目前主要導航看起來已轉向客製化 dashboard / header / button flow，但 `NavMenu.razor` 仍保留預設模板項目（如 `Counter`、`Weather`）。這可能表示：

- 該 layout 不是主要入口
- 或仍有待清理的樣板遺留

## 十四、參考文件

### 1. Repo 內既有文件

- `docs/EFCore.md`
- `docs/Migration History.md`
- `docs/SQLite.md`

### 2. 研究相關資料

`docs/臨床試驗相關` 內可見：

- 隨機分派列表
- 預計收案人數
- Lab normal range 參考資料

### 3. 建議後續補強文件

如果這個專案會持續交接與擴充，建議下一步補上：

- 部署文件
- AI 流程操作手冊
- 角色權限矩陣
- 個案資料 JSON 結構說明
- 研究作業 SOP 對應表
