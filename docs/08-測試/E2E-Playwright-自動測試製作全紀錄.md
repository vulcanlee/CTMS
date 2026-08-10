# E2E Playwright 自動測試製作全紀錄（新增三家醫院驗證）

- 文件用途：這套 Playwright E2E 測試從無到有的製作方法、困難與解法
- 主要讀者：測試人員與開發者
- 對應系統版本：未核對（沿用建立時內容）
- 最後核對日期：未核對
- 編碼：UTF-8（繁體中文，含 BOM）

---

> 本文件彙整「如何用 Playwright 做到這套自動化 UI 驗證」的**所有處置動作**，
> 供日後產出對外報告之用。內容包含背景、環境、逐步製作方法、測試案例與需求對應、
> 遇到的困難與解法、執行方式與產出、以及本次驗證結論。

---

## 1. 背景與目標

### 1.1 為什麼做
近期三個 commit 為系統新增三家醫院，並將醫院清單集中化到 `HospitalRegistry`：

- 柳營奇美醫院（沿用奇美 prefix `CHIMEIH`，`IsPrefixOwner=false`）
- 高雄榮民總醫院（prefix `KSVGH`）
- 嘉義長庚紀念醫院（prefix `CYCGMH`）

系統原本**沒有任何自動化測試**。本任務要以 Playwright 建立可**親眼看到瀏覽器操作過程（headed 模式）**、並**產出測試報告**的端對端（E2E）驗證，確認三家醫院在 UI 上確實正常運作。

### 1.2 驗收標準
| 醫院 | Prefix | 院別下拉 | Dashboard | /Random 分頁 | SubjectNo |
|---|---|---|---|---|---|
| 高雄榮民總醫院 | `KSVGH` | 應出現 | 應成一欄 | 4 個分頁 | `KSVGH####` |
| 嘉義長庚紀念醫院 | `CYCGMH` | 應出現 | 應成一欄 | 4 個分頁 | `CYCGMH####` |
| 柳營奇美醫院 | `CHIMEIH`（共用） | 應出現 | 併入奇美 | 無獨立分頁 | `CHIMEIH####`（接奇美計數） |

### 1.3 技術選型：為何選 Node `@playwright/test`
- 專案本身為純 .NET（Blazor Server），無 Node、無測試專案。
- 需求重點是「看得到操作過程」＋「產出報告」。Node 版 `@playwright/test` 內建最完整的 **HTML 報告、headed 模式、trace viewer、操作影片**，勝過 Playwright for .NET（後者報告需另接測試框架）。
- 放在獨立的 `tests/e2e/` 資料夾，不污染 `CTMS.sln`。

---

## 2. 環境與前置

### 2.1 待測系統
- **框架**：ASP.NET Core Blazor Server（.NET 9）。
- **啟動**：於 `src\CTMS\CTMS` 執行 `dotnet run --launch-profile http`，服務於 `http://localhost:5272`。
- **登入**：預設管理員帳號 `support` / 密碼 `support`（種子在啟動時建立，並將其角色設為全部權限，因此可執行新增病患）。
- **需登入的頁面**：`/Dashboard`、`/Random`、`/Browser` 皆需登入且需 admin。

### 2.2 安裝步驟
```powershell
cd tests\e2e
npm install                     # 安裝 @playwright/test
npx playwright install chromium # 安裝瀏覽器
```
本次使用版本：Node v24、npm 11、Playwright 1.61.1、Chromium。

### 2.3 專案結構
```
tests/e2e/
  package.json
  playwright.config.ts
  .gitignore
  tests/
    helpers.ts               # waitForBlazor / selectHospital
    auth.setup.ts            # 登入一次並存 storageState
    dashboard.spec.ts
    random.spec.ts
    enrollment-dropdown.spec.ts
    create-patient.spec.ts
    enrollment-progress.spec.ts
scripts/Run-E2E.ps1          # 備份/啟停/還原編排腳本
```

---

## 3. 製作方法與步驟（逐步）

### 3.1 playwright.config.ts 設定重點
- `baseURL: http://localhost:5272`。
- `headless: process.env.CTMS_HEADLESS === '1'` → **預設 headed**（讓操作可見）；開發迭代時設 `CTMS_HEADLESS=1` 走無頭較快。
- `launchOptions.slowMo = 400` → 放慢操作，過程看得清楚。
- `trace: 'retain-on-failure'`、`video: 'retain-on-failure'`、`screenshot: 'only-on-failure'`。
- `reporter: [['html'], ['list']]` → 產出 HTML 報告。
- `fullyParallel: false, workers: 1` → 因建立病患會寫入共用資料檔，序列化避免競態。
- `projects`：先跑 `setup`（登入），`chromium` 專案 `dependencies: ['setup']` 並套用 `storageState`。

### 3.2 登入（auth.setup.ts）
登入頁 `Login.razor` 是**靜態 SSR 表單 POST**（`[ExcludeFromInteractiveRouting]` + `HttpContext.SignInAsync`），不需等 SignalR，因此登入最單純可靠：
1. `goto('/Auths/Login')`。
2. 填 `input[placeholder="請輸入帳號"]` = support、`input[placeholder="請輸入密碼"]` = support。
3. 按「登入」→ 成功後導向 `/`，經 `SplashView` 約 0.5 秒自動轉址 `/Dashboard`，故 `waitForURL('**/Dashboard')`。
4. `context.storageState({ path: '.auth/state.json' })` 存登入狀態供其餘測試重用。

### 3.3 各頁如何分析並挑選 selector（關鍵：三種 UI 套件並存）
| 頁面/元件 | UI 套件 | selector 策略 |
|---|---|---|
| 登入表單 | Blazor 原生 EditForm | `input[placeholder=...]`、文字按鈕「登入」 |
| Dashboard 統計卡 | 自訂 HTML | `.stat-card`（以 hasText「合作醫院」定位）→ `.stat-value` / `.stat-subtitle` |
| /Random 分頁 | MudBlazor MudTabs | `.mud-tab`（hasText「高榮 EC Early」…）；作用中分頁類別 `mud-tab-active` |
| 新增病患對話框 | Syncfusion | 對話框 `.e-dialog`（hasText「新增病患資料」）；下拉點第一個 `.e-input-group`；選項 `getByRole('option', { exact })` |
| 個案清單表格 | AntDesign | 列具 `role="row"`，用 `getByRole('row', { name: /prefix\d{4}/ })` |
| 清單搜尋框 | 原生 input | `input[placeholder="搜尋受測者編號"]` |

Dashboard 的醫院長條圖是 `<canvas>`（Chart.js 繪製），高榮/嘉長不是 DOM 文字，因此改以「合作醫院」統計卡的**數字（5）**與**醫院清單文字**（`GetHospitalList()` 以「、」join 短名）作為可靠斷言點。

### 3.4 Run-E2E.ps1 編排邏輯
腳本以「備份 → 測試 → 還原」保護開發資料，並保證正確順序：
1. 備份 `BackendDB.db`、`Data\SubjectNoGenerator.json`、`Data\RandomListRuntime.json` 到 `tests\e2e\.data-backup`。
2. 背景啟動 `dotnet run --launch-profile http`，輪詢 `/Auths/Login` 直到 200。
3. 於 `tests\e2e` 執行 `npx playwright test`（`-Headed` 時加 `--headed`）。
4. **先 `taskkill` 關閉 dotnet（含子行程）** 釋放 SQLite 檔鎖，**再**還原三個檔案（`finally` 區塊確保無論成敗都執行）。
5. `npx playwright show-report` 開啟報告。

---

## 4. 測試案例 ↔ 需求對應表

| # | 測試檔 | 驗證內容 | 對應需求 |
|---|---|---|---|
| 1 | `auth.setup.ts` | 以 support 登入並存登入狀態 | 前置 |
| 2 | `dashboard.spec.ts` | 「合作醫院」數為 **5**，清單含高榮、嘉長（且成大/奇美/郭綜合仍在） | 高榮/嘉長成欄、柳營奇美併入奇美 |
| 3 | `random.spec.ts` | 高榮、嘉長各 4 個分頁共 8 個皆存在；點入高榮分頁可正常渲染、無 Blazor 錯誤 | 高榮/嘉長隨機表分頁與載入 |
| 4 | `enrollment-dropdown.spec.ts` | 收案表單院別下拉列出全部 **6 家**（含三家新醫院） | 院別下拉含三家新醫院 |
| 5 | `create-patient.spec.ts` | 建立高榮→`KSVGH####`、嘉長→`CYCGMH####`、柳營奇美→`CHIMEIH####`（共用奇美計數）且醫院名稱正確顯示「柳營奇美醫院」 | SubjectNo 產號、柳營奇美共用 prefix |
| 6 | `enrollment-progress.spec.ts` | 表頭僅成大/郭綜合/奇美，**不含**高榮/嘉長（記錄現況缺口） | 已知缺口 |

---

## 5. 遇到的困難與解法（處置動作紀錄）

1. **Blazor Server 互動需等 SignalR circuit**
   - 問題：MudTabs、Syncfusion 對話框等互動事件要等 circuit 連線後才回應，過早點擊會失效。
   - 解法：`helpers.ts` 的 `waitForBlazor()` 等 `window.Blazor` 就緒後再加緩衝時間；並以 web-first assertion 自動重試，不用 `networkidle`（WebSocket 常駐）。

2. **登入後的自動轉址**
   - 現象：登入成功導向 `/`，`SplashView` 延遲約 0.5 秒才 `NavigateTo('/Dashboard')`。
   - 解法：登入後 `waitForURL('**/Dashboard')` 而非停在 `/`。

3. **Syncfusion 下拉不是原生 `<select>`**
   - 現象：`SfDropDownList` 渲染成 input + 一個 append 到 `<body>` 的 popup 清單。
   - 解法：`selectHospital()` 先點對話框內第一個 `.e-input-group` 開 popup，再以 `getByRole('option', { name, exact: true })` 精確點選（`exact` 避免「奇美醫院」誤中「柳營奇美醫院」）。

4. **關鍵字搜尋實際比對欄位與 placeholder 不符**（本次最關鍵的一坑）
   - 現象：搜尋框 placeholder 寫「搜尋受測者編號」，但 `PatientService` 實際只比對 `醫院` 與 `癌別` 欄位，**不含 SubjectNo**。以 `KSVGH` 搜尋回傳「共找到 0 位」。
   - 解法：改以**醫院名稱**搜尋（例如「高雄榮民總醫院」），再於結果列驗證病人編號符合 `prefix + 4 碼`。此為既有程式行為，非本次測試要修正的範圍，僅於測試中繞過。

5. **Blazor `@bind` 的 change 事件未觸發**
   - 現象：Playwright `fill()` 只送 `input` 事件；搜尋框 `@bind="Keyword"` 綁在 `onchange`，未 blur 時 `Keyword` 不會更新，導致搜尋用到空字串。
   - 解法：`fill()` 後呼叫 `blur()` 提交 change，再點搜尋圖示 `span.search-icon`。

6. **隨機分組不在收案當下發生**
   - 現象：收案僅產生 SubjectNo，`組別=NA`；隨機分配（回填 Treatment、寫入 `RandomListRuntime.json`）要進入更深的臨床資料編輯頁設定 EC/OC 與早期/晚期才觸發，流程深且高風險。
   - 解法：本次不納入隨機取碼回填，改由 `/Random` 高榮/嘉長分頁可正常載入來佐證隨機表可用。

7. **共用資料檔無法改指測試資料庫**
   - 現象：連線字串 `Data Source=BackendDB.db` 與 `Data\` 路徑寫死；建立病患會永久遞增 `SubjectNoGenerator.json` 並寫入 `BackendDB.db`。
   - 解法：`Run-E2E.ps1` 採「備份 → 測試 → 還原」，且**先關 App 再還原**避免 SQLite 檔鎖。

8. **Windows PowerShell 5.1 讀 .ps1 的編碼地雷**
   - 現象：`Run-E2E.ps1` 若含中文但無 UTF-8 BOM，PS 5.1 以 ANSI 解讀而變亂碼、解析失敗。
   - 解法：腳本訊息/註解改為 ASCII（英文），中文說明集中於本文件；避免未來編輯再次踩雷。

---

## 6. 執行方式與產出

### 6.1 執行
```powershell
# 推薦：一鍵執行（自動備份/啟停/還原/開報告），headed 可看到操作過程
pwsh scripts\Run-E2E.ps1

# 無頭快速執行
pwsh scripts\Run-E2E.ps1 -Headed:$false
```
若已自行啟動 App，也可直接於 `tests\e2e` 執行 `npx playwright test`（但此時無備份/還原保護）。

### 6.2 產出位置
- **HTML 測試報告**：`tests/e2e/playwright-report/index.html`（每支測試通過/失敗、步驟、失敗時截圖、trace、影片）。以 `npx playwright show-report` 開啟。
- **失敗附件**：`tests/e2e/test-results/**`（`trace.zip`、`video.webm`、`test-failed-*.png`）。
- **登入狀態**：`tests/e2e/.auth/state.json`。
- 以上執行期產物皆由 `.gitignore` 排除，不進版控。

### 6.3 對外驗收報告（給維運/客戶）
另提供一份**專業、可列印成 PDF** 的對外驗收報告產生器：`tests/e2e/generate-report.mjs`。它會實際登入操作系統、逐項驗證並**截取真實畫面**，將截圖以 base64 內嵌成**單一自包含 HTML**：`docs/08-測試/驗收報告-新增三家醫院.html`。
```powershell
# 需先啟動 App（或透過 Run-E2E.ps1 的備份/還原保護後執行）
cd tests\e2e; node generate-report.mjs
```
產生後以瀏覽器開啟，即可「列印 → 另存 PDF」交付。此報告同樣會建立測試病患，請在備份/還原保護下執行。

### 6.4 資料還原確認
腳本結束後，`BackendDB.db`、`SubjectNoGenerator.json`、`RandomListRuntime.json` 會還原到執行前狀態；`git status` 對這三個檔案應為 clean，不留測試病患。

---

## 7. 本次驗證結論

- **9 個測試全數通過**（1 個登入 setup + 8 個驗證）。
- **高榮 / 嘉長**：院別下拉可選、Dashboard 合作醫院數為 5 且清單含兩家、`/Random` 各有 4 個分頁且可載入、建立病患得 `KSVGH0001` / `CYCGMH0001`。
- **柳營奇美**：院別下拉可選、建立病患得 `CHIMEIH####`（接續奇美計數），且個案清單「醫院名稱」正確顯示「柳營奇美醫院」。
- **已知缺口**：`/EnrollmentProgress` 收案進度統計頁仍硬編碼只有 3 家（成大/郭綜合/奇美），高榮/嘉長不會顯示。測試已斷言此現況並在此標記，**建議後續**將 `EnrollmentProgressService` 改用 `HospitalRegistry`（registry 化）以納入新醫院。

> 結論：除 `/EnrollmentProgress` 的既有缺口外，三家新醫院在院別下拉、儀表板、隨機表、收案編號各面向均正常運作。
