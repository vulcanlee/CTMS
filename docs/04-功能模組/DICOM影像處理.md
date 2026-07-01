# DICOM 影像處理

## 1. 目的與業務意圖

本模組負責「App 內」的 DICOM 影像轉檔：將上傳的 DICOM（`.dicm`）檔轉為 PNG，供兩種用途使用：

1. **網頁顯示**：在臨床資料頁與風險評估頁以 `<img>` 直接呈現 CT 影像（瀏覽器無法原生顯示 DICOM）。
2. **AI 輸入前置**：上傳後保留原始 DICOM，於送 AI 推論時複製進上傳暫存區作為推論輸入。

> 注意：完整的 AI 推論流水線（DICOM 上傳 → Phase1/2/3 各階段處理）另行記載於 [05-AI推論 overview](../05-AI推論/overview.md) 與 [flow-1-dicom-upload](../05-AI推論/flow-1-dicom-upload.md)；本文僅涵蓋 App 內的轉檔服務與上傳對話框，不重複描述推論管線。

## 2. 使用情境與角色

| 角色 | 權限 | 動作 |
| --- | --- | --- |
| AI 操作人員 | `ROLEAI操作` | 在臨床資料頁開啟「上傳 DICOM」對話框並上傳檔案 |

上傳完成後系統自動轉 PNG，無需額外操作。

## 3. 關鍵頁面（Razor）

| 元件 | 路徑 | 說明 |
| --- | --- | --- |
| UploadDicomDialog | [UploadDicomDialog.razor.cs](../../src/CTMS/CTMS/Components/Views/Commons/UploadDicomDialog.razor.cs) | DICOM 上傳對話框，內含上傳事件 `UploaderChange`，於上傳後呼叫轉檔 |
| BasicClinical2View | [BasicClinical2View.razor.cs](../../src/CTMS/CTMS/Components/Views/ClinicalInformation/BasicClinical2View.razor.cs) | `OnUploadBtn`（檢查 `ROLEAI操作`）開啟對話框；`OnUploadDicomAsync` 接收回呼後更新影像版本 |

## 4. 關鍵服務與方法

| 服務 / 方法 | 路徑 | 職責 |
| --- | --- | --- |
| `DicomService.ConvertSingleFile(dicomPath, pngPath)` | [DicomService.cs](../../src/CTMS/CTMS.Business/Services/Dicoms/DicomService.cs) | 把單一 DICOM 檔渲染並輸出為 PNG |
| `DicomService`（建構式 `InitializeDicom`） | 同上 | 啟動時以 `DicomSetupBuilder` 註冊 fo-dicom 服務 |
| `UploadDicomDialog.UploaderChange` | UploadDicomDialog.razor.cs | 接收上傳串流、存成 `.dicm`、再呼叫 `ConvertSingleFile` |

## 5. 畫面商業邏輯（核心）

### 5.1 上傳與存檔流程（UploadDicomDialog.UploaderChange）

逐一處理上傳檔案（取第一個後 `break`）：

1. 以 `file.OpenReadStream(20 * 1024 * 1024)` 開啟串流（**上傳大小上限 20 MB**）。
2. 來源檔名固定為 `{SubjectNo}.dicm`（`SubjectNo` 由父頁帶入）。
3. 存到 `UploadFinalPath`（= `UploadFiles`）目錄下，`FileMode.Create`（覆蓋同名）。
4. 轉 PNG：目標檔名 `{SubjectNo}.png`，路徑同在 `UploadFiles` 下，呼叫 `DicomService.ConvertSingleFile(dicomFilePath, pngFilePath)`。
5. 關閉對話框並 `OnConfirmCallback.InvokeAsync(sourceDicomPath)` 回傳 DICOM 路徑給父頁。

### 5.2 轉檔邏輯（DicomService.ConvertSingleFile）

1. 若目標 PNG 已存在則先 `File.Delete`（確保覆蓋為最新）。
2. `DicomFile.Open(dicomPath)` 開檔 → `new DicomImage(dataset)`。
3. `dicomImage.RenderImage()` 渲染 → `.AsSharpImage()` 轉為 ImageSharp 影像。
4. 以 `FileStream(FileMode.Create)` + `PngEncoder()` 存成 PNG。
5. 任一例外皆 `logger.LogError` 後 `throw`（**不靜默吞錯**）。

### 5.3 fo-dicom 初始化（建構式）

`InitializeDicom` 以 `DicomSetupBuilder` 註冊：

- `AddFellowOakDicom()`
- `AddTranscoderManager<NativeTranscoderManager>()`（支援壓縮 transfer syntax 的解碼）
- `AddImageManager<ImageSharpImageManager>()`（以 ImageSharp 作為影像後端）
- `.SkipValidation()` 後 `Build()`

初始化失敗會記錄錯誤並 `throw`。此為服務建構式邏輯，非畫面控制項。

### 5.4 上傳後的頁面更新（BasicClinical2View.OnUploadDicomAsync）

- 將 `臨床資訊.Image = SubjectNo`、寫回病患 JSON、更新 `imageVersion = DateTime.Now.Ticks`（破快取）。
- 記錄操作日誌（`OperationCategory上傳影像` / `完成影像上傳`）。
- 關閉對話框、`GetDataAsync` 重載、`StateHasChanged`。

### 5.5 影像在頁面上的引用

- 風險評估頁左圖：`UploadFiles/{ImagePng}?v={imageVersion}`。
- 臨床資料頁：`image = Path.Combine(UploadFinalPath, 臨床資訊.ImagePng)`。
- AI 標註影像（推論後）：`{KeyName}/Phase2Result/{KeyName}_muscle5.png`（由 AI 流水線產出，非本服務轉檔）。

### 5.6 驗證 / 限制

- 上傳串流上限 20 MB（`OpenReadStream` 參數）。
- 不檢核副檔名 / DICOM 合法性；非法 DICOM 會在 `ConvertSingleFile` 拋例外（fo-dicom 啟用 `SkipValidation`，但渲染失敗仍會 throw）。
- 檔名以 `SubjectNo` 為鍵，**同一受試者重複上傳會覆蓋**先前影像與 PNG。

## 6. 資料模型與欄位

詳見 [領域資料模型](../03-資料模型/領域資料模型.md)。本模組相關：

- `RiskAssessmentResult`（[原始碼](../../src/CTMS/CTMS.DataModel/Models/ClinicalInformation/RiskAssessmentResult.cs)）：`Image`、`ImageExtension`、`ImageDicom`（`{Image}.dicm`）、`ImagePng`。
- `臨床資訊.Image` / `ImagePng` / `ImageDicom`：影像檔名鍵（`BasicClinicalPresentation_臨床資訊`）。
- `PatientAIInfo.DicomFilename`：送 AI 時的 DICOM 來源檔名（[原始碼](../../src/CTMS/CTMS.DataModel/Models/AIAgent/PatientAIInfo.cs)）。

## 7. 資料流程

1. 使用者於臨床資料頁按上傳 → `UploadDicomDialog` 開啟。
2. 上傳檔案 → 存為 `UploadFiles/{SubjectNo}.dicm`。
3. `ConvertSingleFile` → 產生 `UploadFiles/{SubjectNo}.png`（供網頁顯示）。
4. 回呼 `OnUploadDicomAsync` → 更新病患影像欄位、刷新畫面。
5. 送 AI 推論時：`PushToAI` 將 `UploadFiles/{ImageDicom}` 複製為 `UploadTemp/{KeyName}.dcm` 作為推論輸入（見 [AI風險評估與骨骼肌分析.md](AI風險評估與骨骼肌分析.md) 與 [flow-1-dicom-upload](../05-AI推論/flow-1-dicom-upload.md)）。

## 8. 與其他模組的關聯

- AI 推論輸入與 Phase1/2/3 處理：[overview](../05-AI推論/overview.md)、[flow-1-dicom-upload](../05-AI推論/flow-1-dicom-upload.md)。
- 風險評估結果呈現：[AI風險評估與骨骼肌分析.md](AI風險評估與骨骼肌分析.md)。

## 9. 擴充與變更影響點

- **上傳大小上限**：改 `UploaderChange` 的 `OpenReadStream(20 * 1024 * 1024)`。
- **影像後端 / Transfer Syntax 支援**：改 `InitializeDicom` 的 `DicomSetupBuilder` 設定（如新增 Codec）。
- **檔名規則**：目前以 `SubjectNo` 命名 `.dicm` / `.png`，若改為多影像需調整命名避免覆蓋。
- **輸出格式**：`PngEncoder()` 可替換為其他 ImageSharp encoder。

## 10. 業務規則與注意事項

- DICOM 不能直接在瀏覽器顯示，故所有頁面顯示用的都是轉出的 PNG。
- 轉檔在伺服器端同步執行（上傳即轉），大檔需留意回應時間。
- 轉檔失敗會 throw，呼叫端應預期例外（目前 `UploaderChange` 的 try/catch 為空，實務上例外會冒泡到框架）。
- 原始 `.dicm` 會保留，AI 推論使用的是原始 DICOM 而非 PNG。
