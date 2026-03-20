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

## 總結

`AIAgent` 是 CTMS 與 AI 模型之間的橋接器。它的核心價值不在 UI，而在於穩定地把病患影像與基本資料推進標註生成、定量分析與風險評估三個步驟，並將分散的 JSON、PNG、CSV 結果整理回 CTMS 可讀取的結構，讓臨床端能在原本的工作流程中查看、確認並使用 AI 推論結果。
