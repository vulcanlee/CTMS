# CTMS AI 推論架構總覽

## 文件目的

本文件提供 CTMS 內兩條 AI 推論流程的共同架構說明，讓工程師或 AI 助手在閱讀細節文件前，先理解系統中哪些元件負責 UI、哪些元件負責送件、哪些元件負責背景輪詢與結果回填。

本文件不重複展開所有步驟細節。若要追單一路徑，請再搭配以下文件閱讀：

- `docs/ai-inference-flow-1-dicom-upload.md`
- `docs/ai-inference-flow-2-manual-annotation.md`

## 核心元件與責任

### 1. 前端頁面入口

AI 推論相關按鈕與對話框入口集中在以下檔案：

- `src/CTMS/CTMS/Components/Views/ClinicalInformation/BasicClinical2View.razor`
- `src/CTMS/CTMS/Components/Views/ClinicalInformation/BasicClinical2View.razor.cs`

在 `BasicClinical2View` 中，使用者可看到 4 個與 AI 相關的按鈕：

- `上傳影像`
- `上傳修改標註`
- `AI推論`
- `推論狀態呈現`

其中：

- `上傳影像` 走的是 DICOM 直接推論流程。
- `上傳修改標註` 走的是人工標註檔重跑流程。
- `AI推論` 主要對應第一條流程的送件入口。
- `推論狀態呈現` 與 `OnAfterRenderAsync` 的輪詢一起負責結果確認與回填。

### 2. 檔案上傳元件

上傳動作分別由兩個共用元件負責：

- `src/CTMS/CTMS/Components/Views/Commons/UploadDicomDialog.razor.cs`
- `src/CTMS/CTMS/Components/Views/Commons/UploadManualAnnotationView.razor.cs`

責任分工如下：

- `UploadDicomDialog` 將上傳的 DICOM 檔寫入 `UploadFinalPath`，並同步轉出 PNG 圖檔供前端顯示。
- `UploadManualAnnotationView` 只把人工標註 json 檔暫存到 `UploadTempPath`，不直接送進 AIAgent。

### 3. Web 與 AIAgent 之間的橋接層

`src/CTMS/CTMS.Business/Services/ClinicalInformation/AIIntegrateService.cs` 是 Web 端和 AIAgent queue 機制之間的橋接服務。

它的主要責任有 4 類：

- 送件前檢查：`PushToAICheck`
- 建立新 AI 工作：`PushToAI`
- 檢查完成並複製結果：`CheckAIProcess`
- 人工標註重跑橋接：`ManualAnnotationPreprocess`、`ManualAnnotationProcess`

這一層不執行 AI 模型本身，而是負責整理病患資料、複製檔案與把資料夾放到正確 queue。

### 4. AIAgent 背景程式

`AIAgent` 是一個常駐輪詢的 .NET Worker，相關核心檔案如下：

- `src/CTMS/AIAgent/Program.cs`
- `src/CTMS/AIAgent/AIAgentWorker.cs`
- `src/CTMS/AIAgent.Business/Services/AgentService.cs`

責任分工如下：

- `Program.cs`
  - 啟動 Host。
  - 註冊背景 worker 與所需 service。
  - 檢查是否已有同名 `AIAgent` 行程，避免同時執行多個 worker。
- `AIAgentWorker.cs`
  - 啟動後先建立 queue 相關目錄。
  - 進入持續輪詢，反覆呼叫 `agentService.RunAsync()`。
- `AgentService.cs`
  - 以固定順序推進 queue。
  - 負責各階段資料夾搬移、指令 JSON 產生、等待外部結果、呼叫 Rscript 產生風險評估輸出。

## 共通資料夾與名詞

### `UploadFinalPath`

CTMS Web 端使用的最終檔案區。完成推論後，前端會從這裡讀取病患對應的 AI 結果資料夾與輸出檔案。

典型用途：

- 上傳後的 DICOM 與 PNG 暫存在這裡。
- `CheckAIProcess` 完成時，會把 `Complete/<KeyName>` 的內容複製到這裡。
- 畫面顯示的 Phase1/Phase2/Phase3 結果，最終都回到這裡。

### `UploadTempPath`

Web 端的暫存區。

典型用途：

- `PushToAI` 會先把 DICOM 複製到這裡再建立 Inbound 工作。
- 人工標註流程會在這裡重建 `<KeyName>` 工作資料夾，之後再送到 `Phase2Queue`。

### `KeyName`

AI 工作的唯一識別碼，也是 queue 中資料夾名稱的核心鍵值。

用途包括：

- 對應 queue 內的病患工作資料夾名稱。
- 對應 `UploadFinalPath/<KeyName>` 的結果資料夾。
- 對應 `Phase1Result`、`Phase2Result`、`Phase3Result` 的主要檔名。

第一條流程通常會新建 `KeyName`。第二條流程則保留既有個案的 `KeyName`，以便覆蓋既有結果並沿用同一份個案資料。

### `PatientData.json`

這裡指的是 AIAgent 使用的病患 AI 工作描述檔，不是 CTMS 資料庫中的病患主資料列本身。

在 AI 工作資料夾中，`PatientData.json` 會保存：

- `KeyName`
- `SubjectCode`
- `Age`
- `Height`
- `Weight`
- `癌別`
- `DicomFilename`

人工標註流程會重寫這個檔案，以確保 Phase2 之後仍能用既有 DICOM 路徑與同一個 `KeyName` 續跑。

### Queue 概念

`AIAgent` 不是用資料庫表格排程，而是使用資料夾作為 queue。核心階段如下：

- `Inbound`
- `Phase1`
- `Phase1Waiting`
- `Phase2`
- `Phase2Waiting`
- `Phase3`
- `Phase3Waiting`
- `OutBound`
- `Complete`

`AgentService.RunAsync()` 每輪都依固定順序掃描這些資料夾並推進工作。

## 兩條流程的共同點

不論是 DICOM 直接推論，還是人工標註重跑，兩者都有以下共同點：

- UI 入口都在 `BasicClinical2View`。
- Web 端都透過 `AIIntegrateService` 對 AIAgent queue 做橋接。
- AIAgent 都以資料夾式 queue 輪詢推進。
- 完成判斷都依賴 `Phase3Result/output.csv` 是否存在。
- 前端最終都由 `ShowAICompletionAsync()` 把結果寫回 `patientData.臨床資訊.RiskAssessmentResult`。

## 兩條流程的關鍵差異

### 第一條流程：DICOM 直接推論

特徵如下：

- 由 `上傳影像` 與 `AI推論` 兩個動作組成。
- 送件前會執行 `PushToAICheck`。
- 會建立新的 `PatientAIInfo` 與新的 Inbound 工作。
- 會完整走完 `Inbound -> Phase1 -> Phase2 -> Phase3 -> Complete`。
- 需要先從 DICOM 產生 Phase1 標註結果。

### 第二條流程：人工標註重跑

特徵如下：

- 由 `上傳修改標註` 啟動。
- 不重新分析 DICOM 來生成初始標註。
- 前提是個案已經有既有 AI 結果，至少要有 `Phase1Result` 與既有 `PatientData.json`。
- Web 端會先把現有結果重建成 AIAgent 可接受的工作資料夾。
- 之後直接送到 `Phase2Queue`，跳過 `Inbound` 與 `Phase1`。

## 前端完成判斷與回填方式

### 自動輪詢

`BasicClinical2View.OnAfterRenderAsync(bool firstRender)` 在首次 render 後會建立背景檢查工作：

- 每秒抓一次最新病患資料。
- 若 `patientAdapterModel.AI處理 != 處理中`，就不繼續檢查。
- 若狀態仍為處理中，則呼叫 `AIIntegrateService.CheckAIProcess(taskPatientData.臨床資訊.KeyName)`。
- 一旦發現完成，就呼叫 `ShowAICompletionAsync()`。

### 手動確認

使用者也可以按下 `推論狀態呈現` 按鈕，對應 `OnConfirmAIBtn()`：

- 手動執行 `CheckAIProcess`
- 若未完成則顯示「AI處理尚未完成」
- 若完成則直接執行 `ShowAICompletionAsync()`

### 結果回填

`ShowAICompletionAsync()` 會把推論產物轉成畫面可用的欄位：

- 從 Phase2 CSV 讀取腰圍
- 從 `Phase3Result/input.csv` 讀取骨骼肌與脂肪相關數值
- 從 `Phase3Result/output.csv` 判斷風險程度與是否需要降劑量
- 設定 `RiskAssessmentResult.ImagePng`
- 更新病患 JSON 與 AI 狀態

## 閱讀建議

若目標是理解整體維護脈絡，建議閱讀順序如下：

1. 先讀本文件，建立共同名詞與責任分工。
2. 再讀 `docs/ai-inference-flow-1-dicom-upload.md`，理解標準送件流程。
3. 最後讀 `docs/ai-inference-flow-2-manual-annotation.md`，理解人工標註重跑如何跳過 Phase1。

若目標是排查特定問題，可依情境切入：

- UI 按鈕沒反應：先看 `BasicClinical2View`
- 送件資料夾建立錯誤：先看 `AIIntegrateService`
- Queue 沒有往下推進：先看 `AIAgentWorker` 與 `AgentService`
- 結果沒有回到畫面：先看 `CheckAIProcess` 與 `ShowAICompletionAsync`
