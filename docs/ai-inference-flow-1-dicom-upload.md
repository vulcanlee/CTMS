# CTMS AI 推論流程一：上傳 DICOM 後直接推論

## 文件目的

本文件描述 CTMS 中最標準的 AI 推論路徑，也就是使用者先上傳 DICOM 影像，再由畫面觸發 `AIAgent` 進行完整三階段推論。

這條流程會完整經過 AIAgent 的標準 queue pipeline，因此是理解整體系統行為時最重要的一條主路徑。

## 適用情境

適用於以下情境：

- 個案剛建立，尚未有 AI 推論結果。
- 使用者希望從原始 DICOM 開始執行完整流程。
- 系統需要自動產生 Phase1 標註、Phase2 定量分析與 Phase3 風險評估。

不適用於以下情境：

- 使用者已有人工作業後的標註 json，且只想重跑後段流程。
- 個案已有既有 `KeyName` 與 `Phase1Result`，希望直接跳過標註生成。

## 入口元件與方法

此流程主要由以下檔案組成：

- `src/CTMS/CTMS/Components/Views/ClinicalInformation/BasicClinical2View.razor`
- `src/CTMS/CTMS/Components/Views/ClinicalInformation/BasicClinical2View.razor.cs`
- `src/CTMS/CTMS/Components/Views/Commons/UploadDicomDialog.razor.cs`
- `src/CTMS/CTMS.Business/Services/ClinicalInformation/AIIntegrateService.cs`
- `src/CTMS/AIAgent.Business/Services/AgentService.cs`

前端入口方法如下：

- `OnUploadBtn()`
- `OnUploadDicomAsync(string filename)`
- `OnPushToAIBtn()`
- `OnAfterRenderAsync(bool firstRender)`
- `OnConfirmAIBtn()`
- `ShowAICompletionAsync()`

## 流程總覽

這條流程可以拆成 6 個階段：

1. 使用者上傳 DICOM
2. CTMS 前端檢查送件條件
3. Web 端建立 AI 工作並投遞到 `Inbound`
4. `AIAgent` 推進 Phase1、Phase2、Phase3
5. 前端輪詢完成狀態
6. 完成後回填推論結果到病患資料

## 階段一：上傳 DICOM

### 入口

使用者在 `BasicClinical2View` 點擊 `上傳影像`，會觸發：

- `OnUploadBtn()`

此方法只做一件事：若目前使用者有 `ROLEAI操作` 權限，就把 `ShowUploadDicomDialog` 設為 `true`，開啟 `UploadDicomDialog`。

### 實際上傳

`UploadDicomDialog.UploaderChange()` 會處理實際檔案上傳。程式行為如下：

1. 讀取使用者上傳檔案流。
2. 以 `<SubjectNo>.dicm` 為檔名寫入 `MagicObjectHelper.UploadFinalPath`。
3. 立刻把該 DICOM 轉成 `<SubjectNo>.png`，同樣寫入 `UploadFinalPath`。
4. 呼叫 `OnConfirmCallback`，把儲存後的 DICOM 路徑回傳給 `BasicClinical2View`。

### 上傳後處理

`BasicClinical2View.OnUploadDicomAsync(string filename)` 在收到 callback 後，會：

1. 更新 `patientData.臨床資訊.Image`
2. 將 `patientData` 寫回 `patientAdapterModel.JsonData`
3. 呼叫 `PatientService.UpdateAsync`
4. 更新畫面版本戳記 `imageVersion`
5. 關閉上傳視窗並重新載入畫面資料
6. 記錄操作日誌

目前實作中，這一步是把 DICOM 與 PNG 準備到 CTMS 自己的上傳區，但尚未建立 AIAgent queue 工作。

## 階段二：送件前檢查

### 入口

使用者點擊 `AI推論`，會執行：

- `OnPushToAIBtn()`

### 權限檢查

`OnPushToAIBtn()` 第一個檢查是：

- 若沒有 `ROLEAI操作` 權限，直接 return。

### 組合 DICOM 路徑

方法內會先重新呼叫 `GetDataAsync()` 取得最新病患資料，接著組出：

- `dicomImage = Path.Combine(currentRootPath, MagicObjectHelper.UploadFinalPath, patientData.臨床資訊.ImageDicom)`

這表示送件時真正使用的是病患資料中的 `ImageDicom` 欄位所對應檔案，而不是純看畫面目前顯示哪張圖。

### `PushToAICheck`

正式送件前，`OnPushToAIBtn()` 會呼叫：

- `AIIntegrateService.PushToAICheck(patient, patientData, dicomImage)`

此方法會檢查以下條件：

- `Age` 不可為空或 0
- `Height` 不可為空或 0
- `Weight` 不可為空或 0
- `ECorOC` 不可為空
- `dicomImage` 檔案必須存在

若任一條件不成立，會回傳錯誤訊息並由畫面顯示 MessageBox，中止送件。

## 階段三：建立新 AI 工作並投遞到 Inbound

### `PushToAI`

檢查通過後，`OnPushToAIBtn()` 會呼叫：

- `AIIntegrateService.PushToAI(patient, patientData, dicomImage)`

這一步是第一條流程的核心橋接點。

### `PatientAIInfo` 建立

`PushToAI()` 會組出一份新的 `PatientAIInfo`，其中包含：

- `Age`
- `Code`
- `Gender`
- `Height`
- `Weight`
- `SubjectCode`
- `癌別`
- `DicomFilename`
- `DestionatioDicomFilename`
- `DestionatioPatientJSONFilename`

建立後會呼叫：

- `patientAIInfo.InitKeyName()`

也就是說，這條流程通常會產生一個新的 `KeyName`，作為本次 AI 工作識別碼。

### 暫存 DICOM 並建立 Inbound 工作

`PushToAI()` 接著會：

1. 取得目前工作目錄
2. 組出 `UploadTempPath`
3. 把原始 DICOM 複製成 `UploadTempPath/<KeyName>.dcm`
4. 呼叫 `agentService.CreateInBound(patientAIInfo, agentsetting)`

到這一步為止，Web 端的責任已完成。接下來工作會交給 `AIAgent` 透過 queue 推進。

### 更新病患狀態

回到 `OnPushToAIBtn()` 之後，前端還會做以下更新：

1. 清空 `ObstetricianGynecologistConfirmation`
2. 清空 `RadiologistConfirmation`
3. 把新的 `KeyName` 寫回 `patientData.臨床資訊.KeyName`
4. 把病患 JSON 寫回資料庫
5. 將 `patientAdapterModel.AI處理` 設為 `處理中`
6. 將 `patientAdapterModel.AI評估` 設為 `NA`
7. 記錄操作日誌
8. 顯示「已經將此紀錄送至 AI 推論中」的提示訊息

這個 `AI處理 = 處理中` 狀態，也是後續前端輪詢是否要繼續檢查的重要依據。

## 階段四：AIAgent 完整三階段推論

### 背景輪詢進入點

`AIAgentWorker.ExecuteAsync()` 啟動後，會持續呼叫：

- `agentService.RunAsync()`

`RunAsync()` 每輪固定依序執行：

1. `ProceeInBoundAsync()`
2. `ProceePhase1Async()`
3. `ProceePhase1WaitingAsync()`
4. `ProceePhase2Async()`
5. `ProceePhase2WaitingAsync()`
6. `ProceePhase3Async()`
7. `ProceePhase3WaitingAsync()`
8. `ProceeCompleteAsync()`

### Phase 1：標註生成

此流程在 Phase 1 會做以下事情：

1. 把 `Inbound/<KeyName>` 搬到 `Phase1/<KeyName>`
2. 讀取病患 `PatientData.json`
3. 複製 DICOM 到 AIAgent 使用的位置
4. 產生 `Phase1LabelGeneration` JSON
5. 將個案移到 `Phase1Waiting`
6. 等待外部標註結果寫回 `Phase1TmpFolder/<KeyName>`
7. 若暫存結果檔案數量達門檻，複製回 `Phase1Result`
8. 把個案推進 `Phase2`

這一段代表系統會從 DICOM 自動生成標註，所以第一條流程一定經過這一步。

### Phase 2：定量分析

Phase 2 會利用 Phase1 產物進行定量分析：

1. 把個案從 `Phase2` 移到 `Phase2Waiting`
2. 讀取 `PatientData.json`
3. 產生 `Phase2QuantitativeAnalysis` JSON
4. 把 `Phase1Result` 的標註 json 當作輸入
5. 等待外部定量分析結果寫回 `Phase2TmpFolder/<KeyName>`
6. 若檔案數量達門檻，複製回 `Phase2Result`
7. 把個案推進 `Phase3`

### Phase 3：風險評估

Phase 3 不是再呼叫前述 AI 標註模型，而是轉成風險評估輸入並執行 Rscript：

1. 把個案從 `Phase3` 移到 `Phase3Waiting`
2. 複製整個工作資料夾到 `OutBound/<KeyName>`
3. 從 `Phase2Result/<KeyName>.csv` 讀值
4. 依癌別產生 `Phase3Result/input.csv`
5. 若癌別為 EC，使用 EC 模型參數
6. 若癌別為 OC，使用 OC 模型參數
7. 呼叫 `Rscript` 產生 `Phase3Result/output.csv`

### 完成條件

最後由 `ProceeCompleteAsync()` 檢查：

- `OutBound/<KeyName>/Phase3Result/output.csv` 是否存在

若存在，才會將個案移轉到：

- `Complete/<KeyName>`

這是前端完成判斷的主要依據。

## 階段五：前端輪詢完成狀態

### 自動輪詢

`BasicClinical2View.OnAfterRenderAsync(bool firstRender)` 在第一次 render 後，會啟動背景檢查 Task：

1. 每秒重新讀取病患資料
2. 若頁面正在編輯模式，則跳過本輪
3. 若 `taskPatientAdapterModel.AI處理 != 處理中`，則跳過檢查
4. 否則呼叫 `AIIntegrateService.CheckAIProcess(taskPatientData.臨床資訊.KeyName)`

### `CheckAIProcess` 的行為

`CheckAIProcess` 會檢查：

- `Complete/<KeyName>/Phase3Result` 是否存在
- 該目錄下是否有檔案
- 是否能找到 `output.csv`

若未完成，直接回傳 `false`。

若已完成：

1. 確保 `UploadFinalPath/<KeyName>` 存在
2. 將 `Complete/<KeyName>` 整個資料夾複製到 `UploadFinalPath/<KeyName>`
3. 回傳 `true`

這表示前端畫面最終讀的是 CTMS 自己的 `UploadFinalPath`，不是直接讀 queue 內的 `Complete`。

### 手動確認入口

使用者點擊 `推論狀態呈現` 時，會觸發 `OnConfirmAIBtn()`，它本質上也是呼叫 `CheckAIProcess`，只是改成由使用者主動發起。

## 階段六：完成後回填資料到病患欄位

當 `CheckAIProcess` 成功後，前端會執行：

- `ShowAICompletionAsync()`

### 回填內容

此方法會把 AI 結果轉成 CTMS 病患資料中的欄位：

- 從 `Get腰圍ACAsync()` 讀取 Phase2 CSV 內的腰圍
- 從 `GetInputCsv()` 讀取 `Phase3Result/input.csv`
- 從 `GetOnputCsv()` 讀取 `Phase3Result/output.csv`
- 組出 `RiskAssessmentResult.ImagePng`
- 計算或填入：
  - `SMD骨骼肌密度`
  - `IMAT肌間肌肉脂肪組織`
  - `LAMA低密度肌肉區域`
  - `NAMA正常密度肌肉區域`
  - `SMA骨骼肌面積`
  - `SMI骨骼肌指標`
  - `Myosteatosis肌肉脂肪變性`
  - `風險程度`
  - `是否需要降15Percent劑量`

同時也會根據 `patientAdapterModel.組別` 補上 `ExperimentalControl`，最後再更新病患 JSON 與顯示完成訊息。

## 完成判斷機制

本節專門說明第一種作法在程式中如何判定 AI 已完成，以及完成訊息是在哪個點被顯示。

### 完成判斷入口

這條流程有兩個完成判斷入口，兩者共用同一套判斷核心：

- 自動輪詢：`BasicClinical2View.OnAfterRenderAsync(bool firstRender)`
- 手動檢查：`BasicClinical2View.OnConfirmAIBtn()`

兩條路徑最後都會呼叫：

- `AIIntegrateService.CheckAIProcess(KeyName)`

若 `CheckAIProcess()` 回傳 `true`，才會進一步進入：

- `BasicClinical2View.ShowAICompletionAsync()`

### 自動輪詢判斷

`OnAfterRenderAsync()` 在首次 render 後會建立 `checkTask`，其行為如下：

1. 每秒重新抓一次最新病患資料。
2. 若頁面仍在編輯模式，跳過本輪。
3. 若 `taskPatientAdapterModel.AI處理 != 處理中`，跳過本輪。
4. 只有在 `AI處理 == 處理中` 時，才呼叫 `CheckAIProcess(taskPatientData.臨床資訊.KeyName)`。

也就是說，這裡不是只要有 AI 結果資料夾就算完成，而是病患狀態必須仍在「處理中」，系統才會進入完成檢查。

### 手動檢查判斷

使用者按下 `推論狀態呈現` 按鈕時，會執行 `OnConfirmAIBtn()`。

這條路徑也會：

1. 重新取得最新病患資料。
2. 呼叫 `CheckAIProcess(patientData.臨床資訊.KeyName)`。
3. 若回傳 `false`，顯示「AI處理 尚未 完成，請稍後再確認」。
4. 若回傳 `true`，直接呼叫 `ShowAICompletionAsync()`。

因此，手動檢查和自動輪詢的差異只在於觸發時機不同，不在於完成判斷邏輯不同。

### 完成訊息的觸發條件

`CheckAIProcess(KeyName)` 的完成條件非常明確，依據如下：

- `Complete/<KeyName>/Phase3Result` 目錄必須存在。
- 該目錄內必須有檔案。
- 檔案中必須找得到 `output.csv`。

只有滿足以上條件，`CheckAIProcess()` 才會：

1. 把 `Complete/<KeyName>` 整份複製到 `UploadFinalPath/<KeyName>`。
2. 回傳 `true`。

而完成訊息本身：

- `"AI處理  已經  完成，可以在 風險評估 頁籤看到推論結果"`

並不是在 `CheckAIProcess()` 內顯示，而是只會在 `ShowAICompletionAsync()` 內顯示。

因此，完成訊息的唯一觸發點是：

- `CheckAIProcess()` 判定完成
- 接著進入 `ShowAICompletionAsync()`
- 最後由 `MessageBox.Show(...)` 顯示完成訊息

### 本流程是否可能提早顯示完成

在目前設計下，這條流程不會因為只有 Phase1 或 Phase2 結果出現，就提早顯示完成訊息。

原因是：

- `CheckAIProcess()` 不檢查 `Phase1Result` 是否存在
- 也不檢查 `Phase2Result` 是否存在
- 它只承認 `Complete/<KeyName>/Phase3Result/output.csv` 為完成依據

所以，只要 Phase3 的最終輸出尚未就緒，畫面就不會顯示「AI處理 已經 完成」。

### 維護注意事項

- 若未來有人修改 `CheckAIProcess()` 的判斷條件，必須同步更新自動輪詢與手動檢查文件，因為兩者共用同一個核心方法。
- 若未來把完成訊息移出 `ShowAICompletionAsync()`，文件中「完成訊息唯一觸發點」的描述也必須一起修正。
- 若排查「為什麼畫面沒顯示完成」，應依序檢查：`AI處理` 狀態、`Complete/<KeyName>/Phase3Result/output.csv`、`CheckAIProcess()` 是否成功複製結果回 `UploadFinalPath/<KeyName>`。

## 關鍵限制與維護重點

### 1. 這條流程一定需要可用的 DICOM 檔案

若 `PushToAICheck()` 找不到 DICOM 或必要欄位缺漏，就無法送件。

### 2. 這條流程會新建 `KeyName`

第一條流程不是重用既有 AI 工作，而是建立新的 `PatientAIInfo` 與新的 queue 工作資料夾。

### 3. 完成判斷不是只看 Phase1 或 Phase2

前端的完成條件是 `Phase3Result/output.csv` 已存在並成功複製回 `UploadFinalPath/<KeyName>`，不是只要標註圖出來就算完成。

### 4. 結果顯示依賴回填

即使 queue 內已有完成結果，若 `CheckAIProcess()` 尚未把結果從 `Complete` 複製回 CTMS 的 `UploadFinalPath`，前端仍可能看不到完整結果。

### 5. Phase3 其實經過 `OutBound`

雖然 UI 層通常只描述到 `Complete`，但實際上 `ProceePhase3WaitingAsync()` 先把工作複製到 `OutBound`，再由 `ProceeCompleteAsync()` 檢查 `output.csv` 後移到 `Complete`。

## 一句話總結

第一條流程是「從原始 DICOM 建立全新 AI 工作，完整走過標註生成、定量分析與風險評估，最後再由前端輪詢把完成結果回填到病患資料」。
