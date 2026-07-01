# AIAgent 專案說明

## 專案定位

`AIAgent` 是一個以 .NET Worker 形式執行的背景 Console 程式，本身不提供 UI，也不直接與使用者互動。它的主要責任是持續監看指定資料夾與佇列，接手 CTMS 建立好的病患個案與 DICOM 影像，將資料推進三階段 AI 推論流程，最後產出風險評估結果供 CTMS 顯示與確認。

從整體角度來看，CTMS Web 負責建立個案、收集臨床資訊並觸發 AI 推論；`AIAgent` 負責搬運資料、生成推論指令、等待外部 AI 結果、呼叫風險模型並整理輸出；CTMS 再將完成結果回填到畫面與病患資料中。

## 整體架構與角色

- `CTMS Web`
  - 由使用者在臨床資訊頁面觸發 AI 推論。
  - 建立 `PatientAIInfo`、檢查必要欄位、複製 DICOM，並將個案投遞到 `Inbound`。
  - 定期檢查 AI 是否完成，完成後將結果顯示在風險評估頁面。
- `AIAgent`
  - 常駐執行的背景輪詢程式。
  - 維護 `Inbound`、`Phase1`、`Phase1Waiting`、`Phase2`、`Phase2Waiting`、`Phase3`、`Phase3Waiting`、`OutBound`、`Complete` 等資料夾式佇列。
  - 依固定順序推進個案，並負責階段間的檔案搬移與輸入產生。
- `AI 推論服務`
  - 根據 `InferencePath` 下的 JSON 指令執行 Phase 1 與 Phase 2。
  - 使用 `tmp_folder` 回填標註生成與定量分析結果。
- `R 風險模型`
  - 在 Phase 3 接收 `input.csv`。
  - 依病患 `癌別` 選擇 EC 或 OC 模型，輸出 `output.csv`。

若對照架構圖來看，`AIAgent` 位在 CTMS 與 AI 模型之間，前端由 CTMS 將要評估的資料放進 `InBound`，中間經過 `Phase1`、`Phase2`、`Phase3` 三段處理與等待佇列，最後結果整理到 `OutBound` 與 `Complete`，再由 CTMS 取回並更新畫面。

## 端到端流程

1. 使用者在 CTMS 臨床資訊頁面觸發 AI 推論。
2. CTMS 先檢查年齡、身高、體重、癌別與 DICOM 是否齊備，接著建立 `PatientAIInfo`，產生唯一的 `KeyName`，並透過 `AgentService.CreateInBound()` 將個案放進 `Inbound`。
3. `AIAgent` 啟動後持續輪詢各佇列，依序將個案推進到 `Phase1`、`Phase2`、`Phase3`。
4. 外部 AI 推論服務讀取 `InferencePath` 下的 JSON 指令，根據其中的 DICOM 路徑、標註 JSON 路徑與 `tmp_folder`，輸出對應的分析結果。
5. `AIAgent` 在 Phase 3 讀取 Phase 2 的定量分析 CSV，組出 `input.csv`，再呼叫 `Rscript` 執行風險模型，產生 `output.csv`。
6. CTMS 檢查 `Complete/<KeyName>/Phase3Result/output.csv` 是否存在；若存在，便將結果複製到 `UploadFiles/<KeyName>`，更新 `AI處理` 狀態，並在風險評估頁面顯示圖片、數值與醫師確認資訊。

## AIAgent 背景執行邏輯

### 啟動方式

`Program.cs` 會完成以下工作：

- 檢查是否已有同名 `AIAgent` 行程存在，若有則本次直接結束，避免同時有多個背景代理程式並行處理同一批資料。
- 初始化 NLog。
- 建立 Host，註冊 `AIAgentWorker`、`AgentService`、`PatientAIInfoService`、`Phase1Phase2Service`、`DirectoryHelperService`、`RiskAssessmentExcelService`。
- 將 `appsettings.json` 中的 `AgentSetting` 綁定到強型別 `Agentsetting`。

### 背景輪詢

`AIAgentWorker` 是實際執行背景工作的 `BackgroundService`。啟動後會先呼叫 `PrepareQueueDirectoryAsync()` 建立所需資料夾，然後在未取消前持續執行：

- 每次循環約隔 `500ms`。
- 每輪呼叫一次 `agentService.RunAsync()`。
- `RunAsync()` 內部在各階段之間再加上短暫延遲，避免過度頻繁 I/O。

`RunAsync()` 的固定處理順序如下：

1. `ProceeInBoundAsync`
2. `ProceePhase1Async`
3. `ProceePhase1WaitingAsync`
4. `ProceePhase2Async`
5. `ProceePhase2WaitingAsync`
6. `ProceePhase3Async`
7. `ProceePhase3WaitingAsync`
8. `ProceeCompleteAsync`

這個設計屬於典型的「輪詢式」處理，而不是事件驅動式。也就是說，`AIAgent` 不會在檔案建立當下被通知，而是靠固定頻率掃描各資料夾判斷是否有新工作或已完成的結果。

## 三階段 AI 推論說明

### Phase 1 標註生成

Phase 1 的主要目標是根據 DICOM 與病患基本資料產生標註內容。

- 輸入
  - DICOM 路徑
  - 病患年齡、性別、身高、體重
- `AIAgent` 行為
  - 將 `Inbound` 的病患資料夾搬到 `Phase1`
  - 讀取 `PatientData.json`
  - 將 DICOM 複製到 `DicomFolderPath`
  - 產生 `Phase1LabelGeneration` JSON 到 `InferencePath`
  - 將個案移到 `Phase1Waiting`
  - 等待 `Phase1TmpFolder/<KeyName>` 產生結果
- 完成判斷
  - `Phase1TmpFolder/<KeyName>` 內檔案數量至少 2 個
- 主要產物
  - 標註 JSON
  - 彩色標註影像 PNG
  - `Phase1Result/<KeyName>.json`

若對照流程圖，這一段對應「CanWellBeing 標註分析」，由 L3 DICOM 與病患基本資料生成標註內容與影像。

### Phase 2 定量分析

Phase 2 會利用 DICOM 與 Phase 1 的標註結果進行定量分析。

- 輸入
  - DICOM
  - Phase 1 產生的標註 JSON
- `AIAgent` 行為
  - 將個案從 `Phase2` 推到 `Phase2Waiting`
  - 產生 `Phase2QuantitativeAnalysis` JSON 到 `InferencePath`
  - 將 `Phase1Result/<KeyName>.json` 路徑寫入 `jsons`
  - 等待 `Phase2TmpFolder/<KeyName>` 產生結果
- 完成判斷
  - `Phase2TmpFolder/<KeyName>` 內檔案數量至少 23 個
- 主要產物
  - 更多分析圖片 PNG
  - `Phase2Result/<KeyName>.csv`

這一段對應流程圖中的「CanWellBeing 定量分析」，使用第一階段的標註內容進一步產出面積、密度等定量數值。

### Phase 3 風險評估

Phase 3 會讀取第二階段的定量分析結果，轉成風險模型所需格式後進行風險評估。

- 輸入
  - `Phase2Result/<KeyName>.csv`
- `AIAgent` 行為
  - 將個案從 `Phase3` 推到 `Phase3Waiting`
  - 複製個案資料到 `OutBound`
  - 讀取定量分析 CSV
  - 依 `癌別` 生成 `Phase3Result/input.csv`
  - EC 使用 `風險評估模型`
  - OC 使用 `風險評估模型OC`
  - 透過 `Rscript` 執行模型，輸出 `Phase3Result/output.csv`
- 主要產物
  - `Phase3Result/input.csv`
  - `Phase3Result/output.csv`

對照第二張圖，這一段相當於把「CanWellBeing 定量分析結果」轉成 `Input.csv`，再交給風險評估模型處理，最後得到 `output.csv`。

## 資料夾與檔案流向

### 主要設定路徑

| 設定名稱 | 用途 |
| --- | --- |
| `DicomFolderPath` | 集中放置 AIAgent 處理中的 DICOM 檔案。 |
| `QueueFolderPath` | 所有資料夾式佇列的根目錄。 |
| `QueueFolderPath\\InBound` | CTMS 新投遞進來的個案入口。 |
| `QueueFolderPath\\Phase1` | 準備進入 Phase 1 的個案。 |
| `QueueFolderPath\\Phase1Waiting` | 已送出 Phase 1 指令、等待標註結果的個案。 |
| `Phase1TmpFolder` | 外部 AI 回填 Phase 1 結果的暫存位置。 |
| `QueueFolderPath\\Phase2` | 準備進入 Phase 2 的個案。 |
| `QueueFolderPath\\Phase2Waiting` | 已送出 Phase 2 指令、等待定量分析結果的個案。 |
| `Phase2TmpFolder` | 外部 AI 回填 Phase 2 結果的暫存位置。 |
| `QueueFolderPath\\Phase3` | 準備進入 Phase 3 的個案。 |
| `QueueFolderPath\\Phase3Waiting` | 等待產生 Phase 3 風險評估輸入與輸出的個案。 |
| `QueueFolderPath\\OutBound` | 已整理完成、等待 CTMS 取回的個案資料夾。 |
| `QueueFolderPath\\Complete` | 已確認完成的個案資料夾。 |
| `InferencePath` | 提供外部 AI 服務讀取的推論指令 JSON 存放位置。 |

### 個案資料夾範例

下列為單一病患個案在完成後的大致結構：

```text
<KeyName>/
|-- PatientData.json
|-- <KeyName>.dcm
|-- Phase1Result/
|   |-- <KeyName>.json
|   `-- <KeyName>.png
|-- Phase2Result/
|   |-- <KeyName>.csv
|   `-- <KeyName>_muscle5.png
`-- Phase3Result/
    |-- input.csv
    `-- output.csv
```

實際檔案數量可能依外部 AI 產物而增加，但 `PatientData.json`、DICOM、Phase1/2/3 結果資料夾是整體流程的核心結構。

## 關鍵資料模型與產物

### `PatientAIInfo`

`PatientAIInfo` 是 CTMS 與 AIAgent 共用的病患 AI 工作資料模型，主要欄位包括：

- `KeyName`
  - 每次送 AI 時產生的唯一工作識別碼。
- `Code`
  - CTMS 病患代碼。
- `SubjectCode`
  - 受試者編號。
- `Height`、`Weight`、`Age`、`Gender`
  - 推論與風險評估所需基本資料。
- `癌別`
  - 目前實作用於切換 EC 或 OC 風險模型。
- `DicomFilename`
  - DICOM 檔案路徑。

### `Phase1LabelGeneration`

Phase 1 指令 JSON 的核心欄位如下：

- `files`
  - 需要分析的 DICOM 檔案路徑。
- `optional`
  - 年齡、性別、身高、體重等附加資訊。
- `tmp_folder`
  - 外部 AI 應寫回結果的資料夾。

### `Phase2QuantitativeAnalysis`

Phase 2 的資料結構繼承自 Phase 1，另外增加：

- `jsons`
  - Phase 1 標註 JSON 的路徑清單。

### `input.csv`

`input.csv` 是 Phase 3 風險模型的輸入檔，實作上依 `癌別` 使用不同欄位格式：

- EC
  - 包含 `Age`、`Tumor.Grade`、`body.height.cm`、`body.weight.kg`、`Vertebral.Body.Area.cm2`、`Total.SMD`、`Total.ImatA`、`Total.LamaA`、`Total.NamaA`、`VatA`、`SatA`。
- OC
  - 包含 `Body.Height.cm`、`Body.Weight.kg`、`SMA`、`SMD`、`ImatA`、`LamaA`、`NamaA`、`MyosteatosisA`、`VatA`、`SatA`。

### `output.csv`

`output.csv` 由 R 模型輸出。CTMS 目前不是解析完整結構，而是直接讀檔內容：

- 若內容包含 `a grade III AE`，則視為高風險，並判定「需要」降 15% 劑量。
- 否則視為低風險，並判定「不需要」降 15% 劑量。

## CTMS 端如何整合結果

### 建立 AI 工作

CTMS 在 `AIIntegrateService.PushToAI()` 中建立工作，主要步驟如下：

- 從 `Patient` 與 `PatientData` 組出 `PatientAIInfo`
- 將前端上傳的 DICOM 複製到暫存位置
- 呼叫 `agentService.CreateInBound()` 建立 `Inbound/<KeyName>` 個案資料夾
- 寫入 DICOM 與 `PatientData.json`

在臨床資訊頁面 `BasicClinical2View` 中，送出成功後會：

- 將 `patientData.臨床資訊.KeyName` 設成此次 AI 工作編號
- 把 `AI處理` 設為 `處理中`
- 把 `AI評估` 設為 `NA`
- 寫入操作歷程

### 檢查 AI 是否完成

CTMS 透過 `AIIntegrateService.CheckAIProcess()` 檢查：

- `Complete/<KeyName>/Phase3Result/output.csv` 是否存在

若檔案存在，代表整個 AI 流程已完成。接著 CTMS 會把完整結果從 `Complete/<KeyName>` 複製到：

- `UploadFiles/<KeyName>`

### 回填畫面與病患資料

`BasicClinical2View` 偵測到完成後，會呼叫 `ShowAICompletionAsync()`，做以下處理：

- 讀取 Phase 2 的結果 CSV，回填腰圍等數值
- 讀取 `Phase3Result/input.csv`，計算並寫入：
  - `SMD骨骼肌密度`
  - `IMAT肌間肌肉脂肪組織`
  - `LAMA低密度肌肉區域`
  - `NAMA正常密度肌肉區域`
  - `SMA骨骼肌面積`
  - `SMI骨骼肌指標`
  - `Myosteatosis肌肉脂肪變性`
- 讀取 `output.csv`，判斷風險程度與是否需降劑量
- 將 `AI處理` 從 `處理中` 更新成 `已完成`

在 `RiskAssessmentView` 中，CTMS 會顯示：

- AI 產生的分析圖片
- 風險評估數值
- 婦產科醫師與放射科醫師確認狀態

## 目前實作特性與注意事項

- 同一時間僅允許一個 `AIAgent` 行程運行。
- 整體流程依賴資料夾式佇列，而不是 Message Queue 或資料庫工作佇列。
- `AIAgent` 以輪詢方式處理工作，不是事件驅動。
- Phase 1 是否完成，是以 `Phase1TmpFolder/<KeyName>` 內至少 2 個檔案作為判斷。
- Phase 2 是否完成，是以 `Phase2TmpFolder/<KeyName>` 內至少 23 個檔案作為判斷。
- Phase 3 依 `癌別` 切換兩套 R 模型：
  - EC 使用 `風險評估模型`
  - OC 使用 `風險評估模型OC`
- 失敗處理目前以 logging 為主，沒有明確的重試佇列、補償機制或告警流程。
- 各階段大量依賴資料夾搬移與複製，若目錄名稱衝突、權限不足或外部程序占用檔案，流程可能中斷。
- `Complete` 是 CTMS 取回結果的來源，而 `UploadFiles/<KeyName>` 則是 CTMS 最終顯示與保存 AI 結果的位置。

## 實作細節（AIAgent 與 AIAgent.Business）

本節以實際程式碼為依據，補充 `AIAgent`（Worker Service 主機）與 `AIAgent.Business`（佇列處理邏輯）兩個專案的實作細節。相關原始碼：

- [Program.cs](../../src/CTMS/AIAgent/Program.cs)
- [AIAgentWorker.cs](../../src/CTMS/AIAgent/AIAgentWorker.cs)
- [AgentService.cs](../../src/CTMS/AIAgent.Business/Services/AgentService.cs)
- [Phase1Phase2Service.cs](../../src/CTMS/AIAgent.Business/Services/Phase1Phase2Service.cs)
- [PatientAIInfoService.cs](../../src/CTMS/AIAgent.Business/Services/PatientAIInfoService.cs)
- [DirectoryHelperService.cs](../../src/CTMS/AIAgent.Business/Services/DirectoryHelperService.cs)
- [Agentsetting.cs](../../src/CTMS/AIAgent.Business/Models/Agentsetting.cs)

延伸閱讀：[總覽](./overview.md)、[流程一：DICOM 上傳](./flow-1-dicom-upload.md)、[流程二：手動標註](./flow-2-manual-annotation.md)、部署設定請見 [HIS 串接](../06-部署與維護/HIS串接.md)。

### 1. 服務型態與啟動

`AIAgent` 是一個 .NET 9 的 **Worker Service**（泛型主機）。`Program.cs` 的 `Main` 依序完成：

1. **單一實例檢查**：以 `Process.GetProcesses()` 取得所有行程，過濾出 `ProcessName` 含 `aiagent`（不分大小寫）者。若清單中存在「Id 與目前行程不同」的同名行程，便印出 `Process {Id} will be killed!` 並直接 `return`，使本次啟動立即結束。
   - 注意：實作上是「**新啟動者主動退出**」，而非真的終止舊行程（訊息文字雖為 will be killed，但程式並未呼叫 `Kill()`）。此判斷僅依行程名稱，未使用 Mutex 或具名鎖。
2. **NLog 初始化**：`NLog.LogManager.Setup().LoadConfigurationFromAppSettings()` 載入設定，組態檔為 [nlog.config](../../src/CTMS/AIAgent/nlog.config)，輸出到 `c:\temp\AIAgent-${shortdate}.log`，最低層級 `Information`。
3. **建立泛型主機並註冊服務**（皆為 `AddTransient`，背景服務為 `AddHostedService`）：
   - `AIAgentWorker`（HostedService）
   - `AgentService`、`PatientAIInfoService`、`Phase1Phase2Service`、`DirectoryHelperService`、`RiskAssessmentExcelService`
4. **設定強型別繫結**：`builder.Services.Configure<Agentsetting>(...GetSection(MagicObjectHelper.Agentsetting))`。
   - `MagicObjectHelper.Agentsetting` 常數值為字串 `"Agentsetting"`，而 `appsettings.json` 中的區段名稱為 `AgentSetting`。.NET 設定鍵不分大小寫，故仍能正確繫結。
5. 例外時記錄到 NLog 並重新拋出；`finally` 區呼叫 `NLog.LogManager.Shutdown()` 確保 flush。

### 2. Worker 主迴圈

`AIAgentWorker` 繼承 `BackgroundService`，`ExecuteAsync` 行為：

1. 啟動時先呼叫 `agentService.PrepareQueueDirectoryAsync()`，建立所有佇列與資料夾（不存在才建立）。
2. 進入 `while (!stoppingToken.IsCancellationRequested)` 迴圈：
   - 每輪呼叫一次 `await agentService.RunAsync()`。
   - 每輪結尾 `await Task.Delay(500, stoppingToken)`，即**約 500ms 輪詢一次**。
3. `OperationCanceledException`（取消時）會被吞掉視為正常結束；其他例外則記錄後重新拋出，使服務停止。

`RunAsync()`（[AgentService.cs](../../src/CTMS/AIAgent.Business/Services/AgentService.cs)）為單次輪詢，固定依序執行下列八個階段，**每個階段之間以 `Task.Delay(150)` 節流**：

1. `ProceeInBoundAsync`
2. `ProceePhase1Async`
3. `ProceePhase1WaitingAsync`
4. `ProceePhase2Async`
5. `ProceePhase2WaitingAsync`
6. `ProceePhase3Async`
7. `ProceePhase3WaitingAsync`
8. `ProceeCompleteAsync`

整體為**輪詢式（polling）非事件驅動**：不監聽檔案系統事件，而是每輪重新 `Directory.GetDirectories(...)` 掃描各佇列判斷有無新工作或已就緒結果。

> 程式碼中保有 `Sample()` 與被註解掉的測試投遞區塊（`CreateInBound`），僅供開發測試用，正式流程不會執行。

### 3. 佇列與資料夾

所有路徑由 `Agentsetting` 強型別與其 `Get*Path()` 輔助方法組出（多數為 `Path.Combine(QueueFolderPath, *QueueName)`）。`PrepareQueueDirectoryAsync()` 啟動時會建立下列每一個目錄。

| 設定 / 方法 | 預設值（AIAgent appsettings） | 用途 |
| --- | --- | --- |
| `DicomFolderPath` | `C:\temp\Dicom` | 集中存放處理中的 DICOM（以 `<KeyName>.dcm` 命名），供推論指令的 `files` 引用。 |
| `QueueFolderPath` | `C:\temp\CTMS\Queue` | 所有資料夾式佇列的根目錄。 |
| `InBound`（`InBoundQueueName`） | `InBound` | CTMS 投遞新個案的入口。`ProceeInBoundAsync` 將其搬到 `Phase1`。 |
| `Phase1`（`Phase1QueueName`） | `Phase1` | 待進入 Phase 1 標註生成的個案。 |
| `Phase1Waiting`（`Phase1WaitingQueueName`） | `Phase1Waiting` | 已送出 Phase 1 指令、等待外部標註結果回填的個案。 |
| `Phase1TmpFolder` | `C:\CanWellBeing\temp1` | 外部 AI（CanWellBeing）回填 Phase 1 結果的暫存根目錄，內含 `<KeyName>/`。**不在 `QueueFolderPath` 底下**。 |
| `Phase2`（`Phase2QueueName`） | `Phase2` | 待進入 Phase 2 定量分析的個案。 |
| `Phase2Waiting`（`Phase2WaitingQueueName`） | `Phase2Waiting` | 已送出 Phase 2 指令、等待定量分析結果的個案。 |
| `Phase2TmpFolder` | `C:\CanWellBeing\temp2` | 外部 AI 回填 Phase 2 結果的暫存根目錄。同樣獨立於 `QueueFolderPath`。 |
| `Phase3`（`Phase3QueueName`） | `Phase3` | 待進入 Phase 3 風險評估的個案。 |
| `Phase3Waiting`（`Phase3WaitingQueueName`） | `Phase3Waiting` | 等待產生風險評估輸入並執行 R 模型的個案。 |
| `OutBound`（`OutBoundQueueName`） | `OutBound` | 由 Phase3Waiting 複製過來、承載 `input.csv`/`output.csv` 的個案資料夾。 |
| `Complete`（`CompleteQueueName`） | `Complete` | 已完成（已產出 `output.csv`）的個案，供 CTMS 取回。 |
| `InferencePath` | `C:\CanWellBeing\inference` | 提供外部 AI 服務讀取的推論指令 JSON（`<KeyName>.json`）存放位置。 |
| `風險評估模型` | `C:\EndometrioidCancer` | EC（子宮內膜癌）R 模型工作目錄。 |
| `風險評估模型OC` | `C:\EndometrioidCancer\OvarianCancer` | OC（卵巢癌）R 模型工作目錄。 |

> `Phase1TmpFolder`/`Phase2TmpFolder` 取得路徑時是 `Path.Combine(Phase1TmpFolder)`（單一參數，等同原值），與其它「`QueueFolderPath` + 名稱」的佇列不同，代表暫存資料夾路徑可獨立於佇列根目錄設定。

### 4. 多階段處理管線 Phase1/2/3

每個階段方法都先 `Directory.GetDirectories(...)` 掃描來源佇列，並以「**JSON 檔名前綴 `PatientData`**」（`MagicObjectHelper.PrefixPatientData`）找出 `PatientData.json`，再用 `PatientAIInfoService.ReadAsync` 還原 `PatientAIInfo`。

| 階段方法 | 來源佇列 | 行為摘要 | 去向 |
| --- | --- | --- | --- |
| `ProceeInBoundAsync` | `InBound` | 對每個資料夾 `Directory.Move` 到 `Phase1`（含 1 秒模擬延遲；目的端同名會擲例外）。 | `Phase1` |
| `ProceePhase1Async` | `Phase1` | 若 `Phase1Waiting` 已有同名先刪除；讀 `PatientData.json`；`CopyDicomAsync` 複製 DICOM 到 `DicomFolderPath`；`BuildPhase1標註生成Json` + `SavePhase1標註生成Json` 產生指令 JSON 到 `InferencePath` 並建立 `Phase1TmpFolder/<KeyName>`；`MoveToPhase1WaitingAsync`。 | `Phase1Waiting` |
| `ProceePhase1WaitingAsync` | `Phase1Waiting` × `Phase1TmpFolder` | 以同名比對暫存資料夾；當 `Phase1TmpFolder/<KeyName>` 內**檔案數 ≥ 2** 視為就緒，`CopyDirectory` 回填到個案的 `Phase1Result/`，再 `MoveToPhase2Async`。 | `Phase2` |
| `ProceePhase2Async` | `Phase2` | 若 `Phase2Waiting` 已有同名先刪除；`MoveToPhase2WaitingAsync`；`BuildPhase2標註生成Json`（含 Phase1 結果 JSON 路徑 `jsons`）+ `SavePhase1定量分析Json` 寫指令 JSON 到 `InferencePath`、建立 `Phase2TmpFolder/<KeyName>`。 | `Phase2Waiting` |
| `ProceePhase2WaitingAsync` | `Phase2Waiting` × `Phase2TmpFolder` | 當 `Phase2TmpFolder/<KeyName>` 內**檔案數 ≥ 23** 視為就緒，回填到 `Phase2Result/`，再 `MoveToPhase3Async`。 | `Phase3` |
| `ProceePhase3Async` | `Phase3` | 若 `Phase3Waiting` 已有同名先刪除；`MoveToPhase3WaitingAsync`。 | `Phase3Waiting` |
| `ProceePhase3WaitingAsync` | `Phase3Waiting` | `CopyToOutBoundAsync` 複製到 `OutBound`；讀 `Phase2Result/<KeyName>.csv` 產生 `Phase3Result/input.csv`（依癌別）；刪除 `Phase3Waiting` 來源；依癌別呼叫 `Rscript` 產生 `Phase3Result/output.csv`。 | `OutBound`（產出輸出檔） |
| `ProceeCompleteAsync` | `OutBound` | 檢查 `OutBound/<KeyName>/Phase3Result/output.csv` 是否存在；存在才 `MoveToCompletionWaitingAsync` 複製到 `Complete` 並刪除 `OutBound` 來源。 | `Complete` |

搬移/複製細節由 [Phase1Phase2Service.cs](../../src/CTMS/AIAgent.Business/Services/Phase1Phase2Service.cs) 與 [DirectoryHelperService.cs](../../src/CTMS/AIAgent.Business/Services/DirectoryHelperService.cs) 提供：

- `MoveDirectoryRecursive`：同磁碟且目的不存在時用最快的 `Directory.Move`，否則遞迴 `File.Copy` 後刪除來源（支援跨磁碟/合併）。
- `CopyDirectory`：純遞迴複製，不刪除來源（用於 Phase2/3 Waiting、OutBound、Complete 的搬移其實是「複製 + 另行 `Directory.Delete`」）。

與主站的銜接：CTMS Web 透過 `AgentService.CreateInBound`（同檔內）建立 `InBound/<KeyName>`、複製 DICOM 並寫入 `PatientData.json`；CTMS 取回則是輪詢 `Complete/<KeyName>/Phase3Result/output.csv`。詳見 [流程一：DICOM 上傳](./flow-1-dicom-upload.md)。

### 5. AIAgent.Business 關鍵類別

- [AgentService.cs](../../src/CTMS/AIAgent.Business/Services/AgentService.cs)：佇列總管，包含 `RunAsync`、八個 `Procee*Async` 階段方法、`PrepareQueueDirectoryAsync` 與 `CreateInBound`。Phase 3 的 `input.csv` 組裝與 `Rscript` 呼叫亦在此。
- [Phase1Phase2Service.cs](../../src/CTMS/AIAgent.Business/Services/Phase1Phase2Service.cs)：DICOM 複製（`CopyDicomAsync`）、各階段資料夾搬移（`MoveToPhase*`/`CopyToOutBound`/`MoveToCompletionWaiting`）、推論指令 JSON 的建立與寫出（`BuildPhase1標註生成Json`、`BuildPhase2標註生成Json`、`SavePhase1標註生成Json`、`SavePhase1定量分析Json`）。
- [PatientAIInfoService.cs](../../src/CTMS/AIAgent.Business/Services/PatientAIInfoService.cs)：`ReadAsync` 從 `PatientData.json` 反序列化為 [PatientAIInfo](../../src/CTMS/CTMS.DataModel/Models/AIAgent/PatientAIInfo.cs)。`PatientAIInfo.InitKeyName()` 以 `yyyyMMddHHmmss` + 微秒前四碼產生唯一 `KeyName`。
- [DirectoryHelperService.cs](../../src/CTMS/AIAgent.Business/Services/DirectoryHelperService.cs)：通用的 `MoveDirectoryRecursive`/`CopyDirectory`/`CreateDirectoryIfNotExists`。
- 風險評估 CSV 讀取由 `RiskAssessmentExcelService.ReadExcel`（[RiskAssessmentExcelService.cs](../../src/CTMS/CTMS.ExcelUtility/Services/RiskAssessmentExcelService.cs)）負責，讀 `Phase2Result/<KeyName>.csv` 取得 SMD/IMAT/LAMA/NAMA/VAT/SAT 等定量數值。
- **OC 卵巢癌風險模型目錄**：由 `Agentsetting.風險評估模型OC`（預設 `C:\EndometrioidCancer\OvarianCancer`）指定；EC 則用 `風險評估模型`（`C:\EndometrioidCancer`）。Phase 3 依 `patientAIInfo.癌別`（`"EC"` / 其它）切換工作目錄與 `Rscript` 參數。

### 6. DICOM→PNG 轉換 與 推論輸出（骨骼肌 CSV）回寫

- **DICOM 端**：`AIAgent` 本身不做 DICOM→PNG 轉換，而是把 DICOM 路徑寫入推論指令 JSON 的 `files`，交由外部 CanWellBeing 推論服務處理。標註影像 PNG 由外部服務寫入 `Phase1TmpFolder/<KeyName>`，再由 `ProceePhase1WaitingAsync` 回填到 `Phase1Result/`。
- **骨骼肌定量 CSV**：Phase 2 外部服務在 `Phase2TmpFolder/<KeyName>` 產出多個 PNG 與 `<KeyName>.csv`（達 23 檔門檻後回填到 `Phase2Result/`）。Phase 3 的 `ProceePhase3WaitingAsync` 讀此 CSV，依癌別組出 `input.csv`：
  - **EC** 表頭：`ID,Age,Tumor.Grade,body.height.cm,body.weight.kg,Vertebral.Body.Area.cm2,Total.SMD,Total.ImatA,Total.LamaA,Total.NamaA,VatA,SatA,Total.SMG`
  - **OC** 表頭：`ID,Body.Height.cm,Body.Weight.kg,SMA,SMD,ImatA,LamaA,NamaA,MyosteatosisA,VatA,SatA,Total.SMG`（身高換算為公分 ×100、`SMA=LamaA+NamaA`、`MyosteatosisA=ImatA+LamaA`）
  - 兩種格式皆含 `Total.SMG`（骨骼肌綜合指標 Skeletal Muscle Gauge）。
- 以 `Encoding.UTF8` 寫出 `input.csv`，再 `Rscript` 執行模型輸出 `output.csv`。CTMS 端回寫畫面/病患資料的完整流程，請見 [流程一：DICOM 上傳](./flow-1-dicom-upload.md) 與 [流程二：手動標註](./flow-2-manual-annotation.md)，本文不重複展開。

Rscript 命令（依癌別）：

- EC：`Rscript Run_Endometrioid_Model.R -m Endometrioid_Analysis_20250610_Model_data.RData --varname CaseIn_SMA_Imat_BMI -c 0.5 -i <input.csv> -o <output.csv>`（WorkingDirectory = `風險評估模型`）
- OC：`Rscript Run_Ovarian_Model.R -m Ovarian_Analysis_20250908_Model_data.RData -v Case_SMI.BH2_Imat_BMI -d 3 -c 0.5 -i <input.csv> -o <output.csv>`（WorkingDirectory = `風險評估模型OC`）

以 `ProcessStartInfo`（`UseShellExecute=false`、`CreateNoWindow=true`、重導 stdout/stderr）啟動；目前 stdout/stderr 讀出後未進一步處理，例外僅 `logger.LogError`。

### 7. 兩份 appsettings.json 的 AgentSetting 對照

- 主站：[CTMS/appsettings.json](../../src/CTMS/CTMS/appsettings.json)
- 背景服務：[AIAgent/appsettings.json](../../src/CTMS/AIAgent/appsettings.json)

| 鍵 | 主站 CTMS | AIAgent |
| --- | --- | --- |
| `DicomFolderPath` | `C:\temp\Dicom` | `C:\temp\Dicom` |
| `QueueFolderPath` | `C:\temp\CTMS\Queue` | `C:\temp\CTMS\Queue` |
| `InBoundQueueName` | `InBound` | `InBound` |
| `Phase1QueueName` | `Phase1` | `Phase1` |
| `Phase1WaitingQueueName` | `Phase1Waiting` | `Phase1Waiting` |
| `Phase1TmpFolder` | `C:\CanWellBeing\temp1` | `C:\CanWellBeing\temp1` |
| `Phase2QueueName` | `Phase2` | `Phase2` |
| `Phase2WaitingQueueName` | `Phase2Waiting` | `Phase2Waiting` |
| `Phase2TmpFolder` | `C:\CanWellBeing\temp2` | `C:\CanWellBeing\temp2` |
| `Phase3QueueName` | `Phase3` | `Phase3` |
| `Phase3WaitingQueueName` | `Phase3Waiting` | `Phase3Waiting` |
| `OutBoundQueueName` | `OutBound` | `OutBound` |
| `CompleteQueueName` | `Complete` | `Complete` |
| `InferencePath` | `C:\CanWellBeing\inference` | `C:\CanWellBeing\inference` |
| `風險評估模型` | `C:\EndometrioidCancer` | `C:\EndometrioidCancer` |
| `風險評估模型OC` | （未設定） | `C:\EndometrioidCancer\OvarianCancer` |

兩份設定除 **`風險評估模型OC` 僅存在於 AIAgent**（主站未列）外其餘一致。佇列為共享資料夾，CTMS 用此設定投遞 `InBound`、取回 `Complete`；`AIAgent` 用此設定執行完整管線與 R 模型。兩者都帶有被註解的 `C:\temp\...` 替代路徑可供本機測試切換。

> 注意：主站 `appsettings.json` 未提供 `風險評估模型OC`，但主站僅負責投遞與取回，不執行 R 模型，故缺此鍵不影響運作；實際跑 OC 模型的是 `AIAgent`。

## 總結

`AIAgent` 是 CTMS 與 AI 模型之間的橋接器。它的核心價值不在 UI，而在於穩定地把病患影像與基本資料推進標註生成、定量分析與風險評估三個步驟，並將分散的 JSON、PNG、CSV 結果整理回 CTMS 可讀取的結構，讓臨床端能在原本的工作流程中查看、確認並使用 AI 推論結果。
