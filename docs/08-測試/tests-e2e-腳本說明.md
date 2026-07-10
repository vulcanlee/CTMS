# tests/e2e 腳本說明

本文件說明 `tests/e2e/` 資料夾（以及搭配的 `scripts/Run-E2E.ps1`）內每一個檔案的用途，
方便維護者快速了解「這套自動化測試由哪些腳本組成、各自做什麼」。

> 為何獨立成 `tests/e2e/`：此為 Node（`@playwright/test`）專案，刻意與 .NET 方案分離，
> **不加入 `CTMS.sln`、不污染主專案**；透過 `.gitignore` 排除 `node_modules/` 等產物。

---

## 目錄結構

```
tests/e2e/
├─ package.json            # Node 專案定義與相依（@playwright/test）
├─ package-lock.json       # 相依鎖定版本
├─ playwright.config.ts    # Playwright 設定（baseURL、headed、報告、trace…）
├─ .gitignore             # 排除 node_modules、報告、截圖、備份等產物
├─ generate-report.mjs     # 產生「對外驗收報告」自包含 HTML（內嵌截圖）
└─ tests/
   ├─ fixtures.ts          # 自訂 fixture：每測試自動擷取「操作前/後」截圖
   ├─ helpers.ts           # 共用工具：等待 Blazor、選 Syncfusion 院別
   ├─ auth.setup.ts        # 前置：登入一次並保存登入狀態
   ├─ dashboard.spec.ts    # 驗證：儀表板顯示 5 家（含高榮、嘉長）
   ├─ random.spec.ts       # 驗證：隨機表出現高榮、嘉長分頁且可載入
   ├─ enrollment-dropdown.spec.ts  # 驗證：院別下拉列出 6 家
   ├─ create-patient.spec.ts       # 驗證：建立三家病患、SubjectNo 產號
   └─ enrollment-progress.spec.ts  # 驗證：收案進度頁現況（已知缺口）

scripts/Run-E2E.ps1        # 一鍵編排：備份→啟動 App→測試→關閉→還原→開報告
```

---

## 設定與基礎檔

### `package.json`
Node 專案定義。相依只有 `@playwright/test`。提供 npm 指令：
- `npm test`：執行測試；`npm run test:headed`：headed 執行；`npm run report`：開報告。

### `playwright.config.ts`
Playwright 全域設定，重點：
- `baseURL = http://localhost:5272`（可用環境變數 `CTMS_BASE_URL` 覆寫）。
- `headless = (CTMS_HEADLESS === '1')`：**預設 headed**（看得到操作）；設 `CTMS_HEADLESS=1` 走無頭。
- `launchOptions.slowMo = 400`：放慢每步，方便觀看。
- `trace / video = retain-on-failure`、`screenshot = only-on-failure`：失敗時保留佐證。
- `workers = 1、fullyParallel = false`：因測試會寫入共用資料庫，序列化避免競態。
- `projects`：先跑 `setup`（登入），再跑 `chromium`（套用登入狀態 `storageState`）。

### `.gitignore`
排除不進版控的產物：`node_modules/`、`playwright-report/`、`test-results/`、
`.auth/`（登入狀態）、`.data-backup/`（資料備份）、`app-run*.log`、`screenshots/`（前後截圖）。

---

## 測試支援檔（tests/）

### `fixtures.ts`
自訂 Playwright `test`，為**每支測試自動擷取「操作前 / 操作後」全螢幕截圖**：
- 操作前：第一次 `page.goto` 頁面載入後擷取。
- 操作後：測試結束、瀏覽器關閉前擷取。
- 截圖同時**存檔到 `tests/e2e/screenshots/<測試名>__before|after.png`**，並 **attach 進 HTML 報告**（可在報告內逐測試看前後對照）。
- 各 spec 皆從此檔 `import { test, expect }`（取代原本的 `@playwright/test`）。

### `helpers.ts`
共用工具函式：
- `waitForBlazor(page)`：等 Blazor Server 的 SignalR circuit 就緒，避免互動事件過早點擊失效。
- `selectHospital(page, dialog, name)`：操作 Syncfusion 院別下拉（非原生 `<select>`：點開 popup → 精確點選選項）。

### `auth.setup.ts`（前置）
以帳號 `support` / 密碼 `support` 登入系統一次，將登入 Cookie 存到 `.auth/state.json`，
供其餘測試重用（不必每支重登）。登入頁為靜態表單 POST，成功後自動轉址 `/Dashboard`。

---

## 驗證測試（tests/*.spec.ts）

| 檔案 | 驗證什麼 | 對應需求 |
|---|---|---|
| `dashboard.spec.ts` | `/Dashboard`「合作醫院」數為 5，清單含高榮、嘉長 | 高榮/嘉長成欄、柳營奇美併入奇美 |
| `random.spec.ts` | `/Random` 高榮、嘉長各 4 個分頁共 8 個，且點入可正常載入 | 新醫院隨機表分頁 |
| `enrollment-dropdown.spec.ts` | 新增病患對話框「院別」下拉列出全部 6 家 | 院別下拉含三家新醫院 |
| `create-patient.spec.ts` | 建立高榮→`KSVGH####`、嘉長→`CYCGMH####`、柳營奇美→`CHIMEIH####`（共用奇美計數）並顯示正確院名 | 收案編號、柳營奇美共用 prefix |
| `enrollment-progress.spec.ts` | `/EnrollmentProgress` 現況僅 3 家（高榮/嘉長尚未納入） | 已知缺口（後續優化） |

---

## 報告產生器

### `generate-report.mjs`
產生**對外驗收報告**（給維運/客戶）：以 Playwright 實際操作系統、逐項驗證並擷取真實畫面，
把截圖以 base64 內嵌成**單一自包含 HTML**：`docs/08-測試/驗收報告-新增三家醫院.html`。
用瀏覽器開啟後可「列印 → 另存 PDF」交付。執行：`cd tests\e2e; node generate-report.mjs`（需先啟動 App）。

---

## 一鍵編排腳本

### `scripts/Run-E2E.ps1`
串起「保護資料 → 跑測試 → 還原」的完整流程：
1. **備份** `BackendDB.db`、`Data\SubjectNoGenerator.json`、`Data\RandomListRuntime.json`（含 SQLite 的 `-wal`/`-shm` 側車檔）。
2. **建置並啟動** App（`dotnet build` 後 `dotnet run --no-build`），輪詢直到就緒。
3. 執行 `npx playwright test`（`-Headed` 時加 `--headed`）。
4. **先關閉 App**（釋放 SQLite 檔鎖）**再還原**資料（`finally` 確保無論成敗都執行）。
5. 開啟 HTML 報告。

參數：
- `-Headed`（預設開）：有頭模式，看得到瀏覽器操作。`-Headed:$false` 走無頭。
- `-ShowReport`（預設開）：結束後開啟 HTML 報告。
- `-KeepData`（預設關）：**跳過還原**，讓測試建立的病患**留在資料庫**（重啟 App 後看得到）。

```powershell
pwsh scripts\Run-E2E.ps1                # 看操作 + 開報告 + 跑完還原資料
pwsh scripts\Run-E2E.ps1 -Headed:$false # 無頭快跑
pwsh scripts\Run-E2E.ps1 -KeepData      # 保留測試病患進資料庫
```

---

## 產出與資料安全

- **測試報告**：`tests/e2e/playwright-report/index.html`（含每測試前後截圖、trace、影片）。
- **前後截圖檔**：`tests/e2e/screenshots/`。
- **驗收報告**：`docs/08-測試/驗收報告-新增三家醫院.html`。
- **資料保護**：測試會寫入共用資料庫，預設跑完自動還原（含刪除 SQLite WAL/SHM，避免「database disk image is malformed」）。
  若要保留測試結果進資料庫，改用 `-KeepData`。

相關文件：`docs/08-測試/E2E-Playwright-自動測試製作全紀錄.md`（完整製作過程與踩雷紀錄）。
