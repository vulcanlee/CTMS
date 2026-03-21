# CTMS AI 推論流程二：上傳人工標註檔後重跑推論

## 文件目的

本文件描述 CTMS 中另一條特殊 AI 推論路徑，也就是使用者上傳人工標註後的 json 檔，讓系統跳過 DICOM 自動標註階段，直接從後段流程重新推論。

這條流程的重點不在「重新分析 DICOM」，而在「把既有個案結果重組成 AIAgent 可接受的 Phase2 工作」。

## 適用情境

適用於以下情境：

- 個案已經做過一次 AI 推論。
- 使用者已經人工修正或人工標註完成。
- 希望保留既有 `KeyName` 與個案上下文，重新執行後段分析。

不適用於以下情境：

- 個案從未做過 AI 推論。
- 系統尚未產生既有 `Phase1Result`。
- 沒有可用的 `PatientData.json` 與既有 DICOM 對應資訊。

## 入口元件與方法

此流程主要由以下檔案組成：

- `src/CTMS/CTMS/Components/Views/ClinicalInformation/BasicClinical2View.razor`
- `src/CTMS/CTMS/Components/Views/ClinicalInformation/BasicClinical2View.razor.cs`
- `src/CTMS/CTMS/Components/Views/Commons/UploadManualAnnotationView.razor.cs`
- `src/CTMS/CTMS.Business/Services/ClinicalInformation/AIIntegrateService.cs`
- `src/CTMS/AIAgent.Business/Services/AgentService.cs`

前端入口方法如下：

- `OnUploadEditAnnotationsBtn()`
- `OnUploadShowUploadManualAnnotationDialogAsync(string filename)`
- `OnAfterRenderAsync(bool firstRender)`
- `OnConfirmAIBtn()`
- `ShowAICompletionAsync()`

## 這條流程和第一條流程最大的差異

最重要的差異只有一句話：

這條流程不重新從 DICOM 產生 Phase1 標註，而是利用既有個案資料與人工標註檔，直接建立 `Phase2Queue` 可用的工作資料夾。

因此它的本質不是「重做整條 pipeline」，而是「接續既有個案的中後段 pipeline」。

## 前置條件

此流程必須依賴已存在的舊 AI 結果。從程式來看，至少需要以下條件：

- `UploadFinalPath/<KeyName>/Phase1Result` 必須存在
- `Phase1Result` 內必須至少找得到一個 `.json` 檔
- `UploadFinalPath/<KeyName>` 根目錄下必須有 `PatientData.json`
- `PatientData.json` 內必須能讀到既有的 `DicomFilename`
- 當前病患資料上必須已有 `patientData.臨床資訊.KeyName`

換句話說，這條流程不是給全新個案用的，而是建立在「已有上一輪 AI 工作成果」之上。

## 階段一：上傳人工標註檔

### 入口

使用者在 `BasicClinical2View` 點擊 `上傳修改標註`，會觸發：

- `OnUploadEditAnnotationsBtn()`

若目前使用者有 `ROLEAI操作` 權限，就把 `ShowUploadManualAnnotationDialog` 設為 `true`，開啟 `UploadManualAnnotationView`。

### 實際上傳

`UploadManualAnnotationView.UploaderChange()` 的行為相對單純：

1. 讀取使用者上傳檔案流
2. 以原始檔名存到 `MagicObjectHelper.UploadTempPath`
3. 呼叫 callback，把暫存檔路徑回傳給 `BasicClinical2View`

這裡有一個重要特徵：

- 上傳元件只負責把人工標註 json 暫存到 `UploadTempPath`
- 這一步尚未建立 `<KeyName>` 工作資料夾
- 也尚未直接呼叫 `PushToAI`

## 階段二：確認既有結果是否足夠重跑

### 入口方法

`BasicClinical2View.OnUploadShowUploadManualAnnotationDialogAsync(string filename)` 收到 callback 後，會開始執行整個重建流程。

### 組出既有與暫存路徑

方法一開始會組出以下幾組路徑：

- `prepareRootPath = UploadTempPath/<KeyName>`
- `prepareAIResult1Path = UploadTempPath/<KeyName>/Phase1Result`
- `hasAIResultRootPath = UploadFinalPath/<KeyName>`
- `hasAIResult1Path = UploadFinalPath/<KeyName>/Phase1Result`
- `hasAIResult2Path = UploadFinalPath/<KeyName>/Phase2Result`
- `hasAIResu3t1Path = UploadFinalPath/<KeyName>/Phase3Result`

這些名稱直接顯示出此流程的設計目標：不是建立全新工作，而是用既有 `<KeyName>` 的結果做重建。

### 驗證 `Phase1Result`

接著程式會先檢查：

1. `hasAIResult1Path` 是否存在
2. 該資料夾內是否至少有一個 `.json` 檔

若任何條件不成立，方法會直接：

- `Logger.LogWarning(...)`
- `return`

因此，只要既有 `Phase1Result` 不完整，這條流程就無法繼續。

## 階段三：清理舊完成結果並把病患狀態改回處理中

### 清除舊 Complete 結果

在正式重建前，程式會呼叫：

- `AIIntegrateService.ManualAnnotationPreprocess(patientData.臨床資訊.KeyName)`

此方法的實作非常直接：

- 找出 `Complete/<KeyName>`
- 直接刪除整個完成資料夾

這代表人工標註重跑會先把既有完成結果視為失效，避免前端讀到舊資料。

### 更新病患狀態

回到 `OnUploadShowUploadManualAnnotationDialogAsync()`，接著會：

1. 重新抓最新 `PatientAdapterModel`
2. 將 `AI處理` 設為 `處理中`
3. 呼叫 `PatientService.UpdateAsync`

這樣前端的自動輪詢才會重新啟動完成檢查。

## 階段四：在 `UploadTemp/<KeyName>` 重建工作資料夾

這是第二條流程最核心的部分。

### 建立新的暫時工作根目錄

程式會先確保：

- `UploadTemp/<KeyName>/Phase1Result` 是乾淨的

具體作法是：

1. 若 `prepareAIResult1Path` 已存在，先刪除
2. 再重新建立 `prepareAIResult1Path`

### 複製既有 `Phase1Result`

接著會把 `hasAIResult1Path` 內所有檔案複製到：

- `UploadTemp/<KeyName>/Phase1Result`

這一步的意義是把「原本上一輪完成後放在 UploadFinalPath 的 Phase1 結果」重新整理進新的工作資料夾。

### 複製既有根目錄檔案

程式接著還會把 `UploadFinalPath/<KeyName>` 根目錄下的所有檔案複製到：

- `UploadTemp/<KeyName>`

這通常包含：

- 舊版 `PatientData.json`
- 其他根目錄層級檔案

### 重寫 `PatientData.json`

之後程式會讀取既有的：

- `UploadFinalPath/<KeyName>/PatientData.json`

並反序列化成 `PatientAIInfo fromFile`，從中取出既有：

- `DicomFilename`

接著再根據目前病患畫面資料，重新組一份新的 `PatientAIInfo`：

- `Age`
- `Code`
- `Gender`
- `Height`
- `Weight`
- `SubjectCode`
- `癌別`
- `DicomFilename = fromFile.DicomFilename`
- `KeyName = patientData.臨床資訊.KeyName`

最後把這份新內容寫回：

- `UploadTemp/<KeyName>/PatientData.json`

### 這一步的真正目的

這代表人工標註流程不是單純把使用者上傳的 json 交給 AIAgent，而是：

1. 沿用既有工作識別碼 `KeyName`
2. 沿用既有 DICOM 檔路徑
3. 把既有 Phase1 結果放回工作資料夾
4. 再讓 AIAgent 從後段 queue 接手

也就是說，這條流程本質上是在「重建工作上下文」。

## 階段五：直接送到 `Phase2Queue`

### `ManualAnnotationProcess`

當 `UploadTemp/<KeyName>` 重建完成後，程式會呼叫：

- `AIIntegrateService.ManualAnnotationProcess(prepareRootPath)`

此方法的實作如下：

1. 找到 `agentsetting.GetPhase2QueuePath()`
2. 取出來源資料夾名稱，也就是 `<KeyName>`
3. 把 `UploadTemp/<KeyName>` 整個資料夾複製到 `Phase2Queue/<KeyName>`

### 這條流程為什麼會跳過 Phase1

因為送件入口不是：

- `PushToAI()`
- `CreateInBound()`

而是直接：

- 複製到 `Phase2Queue`

因此 AIAgent 收到的已經不是需要從 DICOM 開始的新工作，而是「已有 `PatientData.json` 與 `Phase1Result` 的工作資料夾」。

這就是第二條流程與第一條流程最關鍵的分流點。

## 階段六：AIAgent 接續後段流程

當工作進入 `Phase2Queue` 後，後續就與第一條流程後半段趨於一致。

### Phase 2

`AgentService.ProceePhase2Async()` 與 `ProceePhase2WaitingAsync()` 會：

1. 讀取 `PatientData.json`
2. 產生 `Phase2QuantitativeAnalysis` JSON
3. 使用 `Phase1Result` 中的標註 json 當作輸入
4. 等待外部定量分析結果
5. 把結果寫回 `Phase2Result`
6. 把個案推進 `Phase3`

### Phase 3

`AgentService.ProceePhase3Async()` 與 `ProceePhase3WaitingAsync()` 會：

1. 建立 `OutBound/<KeyName>`
2. 從 `Phase2Result/<KeyName>.csv` 讀值
3. 產生 `Phase3Result/input.csv`
4. 呼叫 `Rscript`
5. 產生 `Phase3Result/output.csv`

### 完成

`ProceeCompleteAsync()` 會檢查 `output.csv` 是否存在，存在才把工作移到：

- `Complete/<KeyName>`

因此第二條流程雖然跳過了 Phase1，但後半段仍然會經過：

- `Phase2`
- `Phase2Waiting`
- `Phase3`
- `Phase3Waiting`
- `OutBound`
- `Complete`

## 階段七：前端完成檢查與回填

這一段和第一條流程幾乎相同。

### 自動輪詢

`BasicClinical2View.OnAfterRenderAsync()` 在病患 `AI處理` 狀態為 `處理中` 時，每秒會呼叫一次：

- `AIIntegrateService.CheckAIProcess(patientData.臨床資訊.KeyName)`

### `CheckAIProcess` 完成時

若 `Complete/<KeyName>/Phase3Result/output.csv` 已存在，`CheckAIProcess()` 會：

1. 把 `Complete/<KeyName>` 整份複製到 `UploadFinalPath/<KeyName>`
2. 回傳 `true`

### `ShowAICompletionAsync()`

前端接著會像第一條流程一樣：

- 回填腰圍
- 回填骨骼肌與脂肪分析欄位
- 解析風險評估輸出
- 更新 `RiskAssessmentResult`
- 更新病患資料與畫面

因此，對畫面最終呈現來說，兩條流程的出口是相同的；差異只發生在送件與前段 queue。

## 完成判斷機制

本節專門說明第二種作法在程式中如何判定人工標註重跑已完成，以及為什麼正常情況下不會提早顯示完成訊息。

### 完成判斷入口

這條流程的完成判斷入口與第一種作法相同，仍然有兩個：

- 自動輪詢：`BasicClinical2View.OnAfterRenderAsync(bool firstRender)`
- 手動檢查：`BasicClinical2View.OnConfirmAIBtn()`

兩者最後仍然共用：

- `AIIntegrateService.CheckAIProcess(KeyName)`

完成訊息也仍然只會在：

- `BasicClinical2View.ShowAICompletionAsync()`

被顯示。

### 自動輪詢判斷

人工標註流程開始時，`OnUploadShowUploadManualAnnotationDialogAsync(string filename)` 會先做兩件和完成判斷直接相關的事情：

1. 呼叫 `AIIntegrateService.ManualAnnotationPreprocess(patientData.臨床資訊.KeyName)` 刪除舊的 `Complete/<KeyName>`。
2. 把 `taskPatientAdapterModel.AI處理` 設回 `處理中`。

因此 `OnAfterRenderAsync()` 內的 `checkTask` 之後會重新開始檢查，但它檢查的是「新的人工標註重跑結果」，不是上一次 DICOM 推論留下的完成狀態。

`checkTask` 的判斷條件依然是：

1. 每秒輪詢一次。
2. 若 `AI處理 != 處理中`，不檢查。
3. 若 `AI處理 == 處理中`，才呼叫 `CheckAIProcess(KeyName)`。

### 手動檢查判斷

使用者按下 `推論狀態呈現` 時，`OnConfirmAIBtn()` 仍然會直接呼叫：

- `CheckAIProcess(patientData.臨床資訊.KeyName)`

因此在人工標註流程裡，手動檢查也不會繞過既有判斷邏輯。只要新的完成條件尚未成立，就會和自動輪詢一樣得到 `false`，並顯示「AI處理 尚未 完成」。

### 完成訊息的觸發條件

這條流程雖然是人工標註重跑，但 `CheckAIProcess()` 的完成條件並沒有改變，仍然是：

- `Complete/<KeyName>/Phase3Result` 存在
- 該目錄內已有檔案
- 其中找得到 `output.csv`

只有當新的人工標註重跑真的產出新的 `Complete/<KeyName>/Phase3Result/output.csv`，`CheckAIProcess()` 才會回傳 `true`，然後再由 `ShowAICompletionAsync()` 顯示：

- `"AI處理  已經  完成，可以在 風險評估 頁籤看到推論結果"`

因此，完成訊息的唯一觸發點仍然是 `ShowAICompletionAsync()`，而不是人工標註檔案剛上傳完成的當下。

### 本流程是否可能提早顯示完成

正常情況下，不會。

原因如下：

- 人工標註重跑開始時，會先刪除舊的 `Complete/<KeyName>`。
- 同時把病患 `AI處理` 狀態設回 `處理中`。
- `CheckAIProcess()` 只認 `Complete/<KeyName>/Phase3Result/output.csv`，不會因為 `UploadFinalPath` 內還有舊檔案就直接判定完成。

所以即使這個個案之前已經完成過第一種 DICOM 推論，只要新的人工標註重跑尚未產出新的 `output.csv`，就不會提前顯示：

- `"AI處理  已經  完成，可以在 風險評估 頁籤看到推論結果"`

### 例外與風險

這條流程仍有一個必須明確記錄的風險：

- 若 `ManualAnnotationPreprocess()` 沒有成功刪除舊的 `Complete/<KeyName>`，則 `CheckAIProcess()` 仍可能因為舊的 `output.csv` 殘留而回傳 `true`。

在這種異常情況下，系統就可能提早顯示完成訊息，讓人誤以為新的人工標註推論已經完成。

也就是說，這條流程的「不會提早顯示完成」成立前提是：

- `ManualAnnotationPreprocess()` 有成功清掉舊的完成結果。

### 維護注意事項

- 若有人修改 `ManualAnnotationPreprocess()` 的刪除邏輯，必須重新確認人工標註流程是否仍然能避免舊完成結果干擾。
- 若有人改成讓人工標註流程產生新的 `KeyName`，則目前文件中「沿用同一個 `KeyName` 重跑」的完成判斷說明也必須一起調整。
- 若未來排查「人工標註重跑為什麼一開始就顯示已完成」，優先檢查：`Complete/<KeyName>` 是否真的被刪除、`AI處理` 是否已設回 `處理中`、`CheckAIProcess()` 是否讀到舊 `output.csv`。

## 關鍵限制與維護重點

### 1. 這不是全新送件流程

第二條流程不會建立新的 Inbound 工作，也不會呼叫 `PushToAI()`。

### 2. 這不是重新分析 DICOM 取得標註

它的設計目的是重用人工標註結果，直接跳過自動標註生成。

### 3. 它依賴既有 `Phase1Result`

若既有個案沒有 Phase1 結果，或者該結果中沒有 json 標註檔，流程會直接中止。

### 4. 它保留既有 `KeyName`

這點非常重要。此流程是用同一個 `<KeyName>` 覆寫既有結果，而不是另開一個全新工作編號。

### 5. 它會先刪除舊完成結果

`ManualAnnotationPreprocess()` 會先刪掉 `Complete/<KeyName>`，避免舊 `output.csv` 被前端誤判成新一輪已完成。

### 6. 它實際上重建的是「AIAgent 工作資料夾」

使用者雖然只上傳了一個人工標註 json，但系統真正送進 queue 的內容不只那個 json，還包含：

- 重建後的 `PatientData.json`
- 既有 `Phase1Result`
- 根目錄下既有必要檔案

這是後續維護此流程時最重要的理解點。

## 和第一條流程的對照結論

若要用一句話對比兩條流程：

- 第一條流程是「從 DICOM 建立全新 AI 工作，完整跑完所有階段」。
- 第二條流程是「沿用既有個案與人工標註成果，重建工作上下文後直接從 Phase2 接續重跑」。

## 一句話總結

第二條流程的本質不是把 json 直接丟給模型，而是利用既有 `KeyName`、既有 `PatientData.json`、既有 `Phase1Result` 與人工標註成果，重建出一份可被 AIAgent 後段流程接受的工作資料夾，再從 `Phase2Queue` 開始續跑。
