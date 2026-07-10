# AI 自動化測試 — 隨機表缺口修復記錄

**日期：** 2026/07/09　·　**系統：** CTMS v1.1.226　·　**關聯：** 《AI自動化測試-成果報告（GM）》第九節、《驗收報告-新增三家醫院》更正補述、《新增三家醫院-重新檢查報告》§4.1–4.2

---

## 1. 現象

`/Random` 隨機表頁中，**高榮、嘉長**兩院的分頁（EC/OC × Early/Advance 共 8 個）存在，但**點入後表格為空**（無任何資料列）。

## 2. 根因

- 隨機表資料來源為 `src/CTMS/CTMS/Data/RandomList.xlsx`。`RandomListService.ReadSheetIfExists()` 對**不存在的分頁會略過並記警告**（commit `RandomListService 缺分頁改為略過並記警告`）。
- 該 xlsx **從未建立** `高榮Early / 高榮Advance / 嘉長Early / 嘉長Advance` 四個分頁，故兩院隨機清單被靜默略過，執行期快取 `RandomListRuntime.json` 亦無 `KSVGH` / `CYCGMH` 任何項目。
- 分頁（tab）由 `HospitalRegistry.PrefixOwners` 驅動而生成，與 xlsx 是否有資料無關 → 形成「有分頁、無內容」。

> 此缺口先前已於《新增三家醫院-重新檢查報告》§4.1、§4.2 記錄為未解，但成果／驗收報告初版仍以「隨機分組表通過」呈現——因為當時的自動化測試**只驗證分頁存在與點入不報錯，未驗證內容**，屬**測試涵蓋不足的假陽性**。

## 3. 修復內容

### 3.1 補齊隨機分派清單（比照奇美完全複製）

> ⚠️ **2026-07-10 更正**：本節原記載以 `mulberry32` 亂數種子為高榮／嘉長產生「各自獨立、與奇美不同」的序列。**此做法有誤**——隨機表（治療組別分派清單）由試驗統計專家提供，用於把病患分派到對照／實驗組以進行有效統計分析，**系統不得自行以亂數產生**。經決策改為：**高榮、嘉長的隨機表完全複製奇美（順序、內容皆相同）**；日後若統計師提供各院正式獨立清單再行替換。以下已更新為現行做法（原 mulberry32 種子表已作廢移除）。

依決策「高榮、嘉長比照奇美隨機表，內容與奇美完全相同（同序同內容）；柳營與奇美共用」辦理：

- **設計**：沿用奇美的置換區塊（permuted block）結構，**block size = 4、每區塊 2 Dr : 2 AI**。Early = 20 區塊 / 80 列；Advance = 80 區塊 / 320 列。
- **內容比照奇美**：高榮／嘉長四個分頁為奇美對應分頁（`奇美Early`／`奇美Advance`）的**完整複本**——id／區塊／區塊大小／treatment 各欄皆與奇美逐格相同（**不是**自行產生的獨立序列）。
- **落地**：以 OpenXML 直接把 `奇美Early`／`奇美Advance` 的工作表內容複製覆蓋至 `高榮Early`／`高榮Advance`／`嘉長Early`／`嘉長Advance`，寫入 `RandomList.xlsx`。SubjectNo 欄本就不存在於 xlsx（配號僅存於 `RandomListRuntime.json`），兩院各自從零配號（KSVGH… / CYCGMH…）。
- **既有系統覆蓋**：runtime 已存在舊序列者，自癒補讀**不會**覆蓋；須於 `/SystemMaintain` 按「高榮/嘉長隨機表比照奇美（複製）」一次性重建（見 [受試者編號與隨機分組 §9](../04-功能模組/受試者編號與隨機分組.md)）。
- **柳營奇美**：`IsPrefixOwner = false` 沿用奇美 `CHIMEIH`，程式面已處理，無需另建分頁。
- **EC / OC**：`RandomListService.ReadSheet()` 由單一分頁同時產生 EC 與 OC 兩份項目，故各院 EC 與 OC 序列相同（與奇美既有行為一致）。

### 3.2 強化測試防「空表假陽性」

於 `tests/e2e/tests/random.spec.ts` 新增內容斷言：點入高榮／嘉長各分頁後，斷言 `table.sample-table tbody tr` **實際載入預期列數（Early 80、Advance 320）** 且首列編號可見。空表將使測試失敗。

## 4. 驗證結果

- 執行 `scripts/Run-E2E.ps1 -Headed:$false`：**13 項全數通過**（含新增 4 項內容斷言）。
- 自癒補讀確認：App 啟動時 `RandomListService.InitialAsync()` 偵測 `KSVGH` / `CYCGMH` 為缺漏院別，自 xlsx 補讀並附加至 `RandomListRuntime.json`（不清除既有配號）。
- xlsx 結構複核：高榮／嘉長各分頁區塊大小皆為 4、每區塊 2 Dr : 2 AI、總數 Early 40:40、Advance 160:160；**高榮／嘉長 treatment 序列與奇美逐格相同（為奇美之完整複本）**。

## 5. 已知事項與後續

- **收案進度統計頁**（`EnrollmentProgressService`）仍為舊三院硬編碼，高榮／嘉長未納入（見成果報告「額外發現」與重新檢查報告 §4.3）；屬統計呈現層後續優化，不影響隨機分組運作。
- `Run-E2E.ps1` 的還原步驟在 App 尚未釋放 SQLite 檔案鎖時可能失敗（`BackendDB.db` 被占用）；建議於還原前加大等待或輪詢鎖釋放。本次執行測試資料清理需另行確認。
