# AI 風險評估與骨骼肌分析

## 1. 目的與業務意圖

本模組對應「風險評估」頁籤（路由 `/RiskAssessment/{code}`），用於呈現 AI 推論完成後的骨骼肌綜合分析結果，並由放射科與婦產科醫師對結果進行簽核確認。

業務意圖：在化療給藥前，利用 CT 影像上 L3 椎體位置的骨骼肌與脂肪量測，評估個案的肌少症 / 肌肉脂肪變性風險，據以決定「是否需要降 15% 劑量」，作為 AI 輔助決策的臨床建議。實驗組（AI 組）會採用此 AI 輔助建議，對照組（Dr 組）則不採用。

> 注意：本文聚焦於「風險評估『頁面』」如何呈現結果、推論前置檢查（Push-to-AI）、組別判定與風險判定邏輯。完整的 AI 推論流水線（DICOM 上傳 → Phase1/2/3）另見 [05-AI推論 overview](../05-AI推論/overview.md) 與 [flow-1-dicom-upload](../05-AI推論/flow-1-dicom-upload.md)。

## 2. 使用情境與角色

| 角色 | 權限常數 | 動作 |
| --- | --- | --- |
| AI 操作人員 | `ROLEAI操作` | 在臨床資料頁觸發「送 AI 推論」（PushToAI）、確認推論狀態 |
| 一般使用者 | `ROLE風險評估` | 進入風險評估頁檢視結果 |
| 放射科醫師 | `ROLE風險評估影像確認` | 對影像與數據進行簽名確認 |
| 婦產科醫師 | `ROLE風險評估結果確認` | 對影像與數據進行簽名確認 |
| 稽核 | `ROLE風險評估確認歷程` | 檢視歷史簽呈 |

權限常數定義於 [MagicObjectHelper.cs](../../src/CTMS/CTMS.Share/Helpers/MagicObjectHelper.cs)。

## 3. 關鍵頁面（Razor）

| 頁面 / 元件 | 路徑 | 說明 |
| --- | --- | --- |
| RiskAssessmentPage | [RiskAssessmentPage.razor](../../src/CTMS/CTMS/Components/Pages/ClinicalInformation/RiskAssessmentPage.razor) | 路由頁，`OnInitializedAsync` 檢查 `ROLE風險評估` 權限，無權限則顯示提示 |
| RiskAssessmentView | [RiskAssessmentView.razor](../../src/CTMS/CTMS/Components/Views/ClinicalInformation/RiskAssessmentView.razor) / [.razor.cs](../../src/CTMS/CTMS/Components/Views/ClinicalInformation/RiskAssessmentView.razor.cs) | 結果呈現與簽名確認的主畫面 |
| SignHistoryDialog | [SignHistoryDialog.razor](../../src/CTMS/CTMS/Components/Commons/Dialogs/SignHistoryDialog.razor) | 顯示歷史簽呈清單 |
| BasicClinical2View | [BasicClinical2View.razor.cs](../../src/CTMS/CTMS/Components/Views/ClinicalInformation/BasicClinical2View.razor.cs) | 臨床資料頁，承載「送 AI 推論」按鈕與推論完成後寫回 RiskAssessmentResult 的邏輯 |

## 4. 關鍵服務與方法

| 服務 / 方法 | 檔案 | 職責 |
| --- | --- | --- |
| `AIIntegrateService.PushToAICheck` | [AIIntegrateService.cs](../../src/CTMS/CTMS.Business/Services/ClinicalInformation/AIIntegrateService.cs) | 送 AI 前的前置檢查，回傳錯誤訊息字串（空字串代表通過） |
| `AIIntegrateService.PushToAI` | 同上 | 建立 `PatientAIInfo`、產生 `KeyName`、複製 DICOM 到上傳暫存區、`CreateInBound` 入列 |
| `AIIntegrateService.CheckAIProcess` | 同上 | 檢查 Phase3Result 是否產出 `output.csv`，並回拷結果目錄 |
| `AIIntegrateService.GetInputCsv` | 同上 | 讀取 `Phase3Result/input.csv` → `InputCsvModel`（各骨骼肌量測值） |
| `AIIntegrateService.GetOnputCsv` | 同上 | 讀取 `Phase3Result/output.csv`，依內容判定風險程度與是否需降劑量 |
| `AIIntegrateService.Get腰圍ACAsync` | 同上 | 讀取 `Phase2Result/{KeyName}.csv` 取得腰圍 |
| `ShowAICompletionAsync` | BasicClinical2View.razor.cs | 推論完成後計算各指標、組別並寫回 `RiskAssessmentResult` |

## 5. 畫面商業邏輯（核心）

### 5.1 風險評估頁的整體顯示條件

`RiskAssessmentView.InitData()`：

- 從 `patientData.臨床資訊.RiskAssessmentResult` 取得 `data`。
- 只有當 `data != null` 且 `data.風險程度` 非空字串時，`hasRiskAssessment = true`，整個結果區塊（影像、指標表、AI 結果、簽名）才會顯示；否則畫面只剩標題與 SubjectNo。
- 換言之：**尚未完成 AI 推論（無風險程度）時，頁面不顯示任何結果與簽名鈕**。

### 5.2 影像顯示（左 / 右兩張）

- 左圖：`UploadFiles/{ImagePng}?v={imageVersion}`，即 Phase1 的原始 CT PNG。
- 右圖（AI 影像）：`imageAIUrl = UploadFiles/{KeyName}/Phase2Result/{KeyName}_muscle5.png`，為 AI 分割後的肌肉標註影像。
- `imageVersion = DateTime.Now.Ticks`，作為查詢字串避免瀏覽器快取。

### 5.3 骨骼肌指標表（詳細內容）

直接綁定 `RiskAssessmentResult` 欄位，單位由樣板硬編：

| 顯示欄位 | 綁定屬性 | 單位（樣板） |
| --- | --- | --- |
| 骨骼肌面積 (SMA) | `SMA骨骼肌面積` | （無） |
| 骨骼肌指標 (SMI) | `SMI骨骼肌指標` | （無） |
| 骨骼肌密度 (SMD) | `SMD骨骼肌密度` | HU |
| 肌間/肌肉脂肪組織 (IMAT) | `IMAT肌間肌肉脂肪組織` | 平方公分 |
| 低密度肌肉區域 (LAMA) | `LAMA低密度肌肉區域` | 平方公分 |
| 正常密度肌肉區域 (NAMA) | `NAMA正常密度肌肉區域` | 平方公分 |
| 肌肉脂肪變性 (Myosteatosis) | `Myosteatosis肌肉脂肪變性` | 平方公分 |

### 5.4 指標計算（推論完成後寫回）

於 `BasicClinical2View.ShowAICompletionAsync()`，自 `InputCsvModel`（Phase3Result/input.csv）計算後寫回 `RiskAssessmentResult`，皆 `ToString("F2")`：

- `SMD骨骼肌密度` = `Total_SMD`
- `IMAT肌間肌肉脂肪組織` = `Total_ImatA`
- `LAMA低密度肌肉區域` = `Total_LamaA`
- `NAMA正常密度肌肉區域` = `Total_NamaA`
- `SMA骨骼肌面積` = `Total_LamaA + Total_NamaA`（低密度 + 正常密度肌肉面積之和）
- `SMI骨骼肌指標` = `(Total_LamaA + Total_NamaA) / (身高公尺)^2`（身高來自 `臨床資訊.Height`，cm → m）
- `Myosteatosis肌肉脂肪變性` = `Total_ImatA + Total_LamaA`
- 另由 `Get腰圍ACAsync` 取 `Phase2Result/{KeyName}.csv` 的腰圍寫入 `臨床資訊.AbdominalCircumference`

### 5.5 風險程度與是否降 15% 劑量的判定

於 `AIIntegrateService.GetOnputCsv(KeyName)`，讀取 `Phase3Result/output.csv` 全文轉小寫後字串比對：

```
若 output 內容包含 "a grade III AE"（小寫比對）
    → 風險程度 = "高風險"，是否需要降15%劑量 = "需要"
否則
    → 風險程度 = "低風險"，是否需要降15%劑量 = "不需要"
```

亦即：**風險與降劑量為同一條件決定，僅二分（高/低、需要/不需要）**，並非由各骨骼肌指標在前端計算門檻。判定來源完全是 AI 輸出的 `output.csv`。

### 5.6 實驗組 / 對照組的顯示

於 `ShowAICompletionAsync()` 依 `patientAdapterModel.組別` 決定：

- `組別 == "AI"` → `ExperimentalControl = 實驗組`
- 否則 → `ExperimentalControl = 對照組`

樣板「研究組別」面板顯示：
- 標題與本文：`@data.ExperimentalControl`（「實驗組」或「對照組」）。
- `ExperimentalControlMessage`：實驗組 → `"需"`、對照組 → `"不需"`，組成「**需/不需** 使用 AI 輔助決策系統」字句（見 RiskAssessmentResult.ExperimentalControlMessage）。

組別本身由亂數表指派（`BasicClinical2View.OnSave` / `OnRecomputeRandomListAsync` 呼叫 `RandomListService`），值為 `"AI"`（實驗）或 `"Dr"`（對照）。

### 5.7 簽名鈕的顯示與權限前置檢查

- 簽名區塊只在 `hasRiskAssessment == true` 時隨結果一同顯示，兩顆「簽名確認」按鈕（放射科、婦產科）恆常呈現於該區塊。
- 點擊放射科簽名 `On放射科醫師確認SignAsync`：先檢查 `CheckAccessPage(ROLE風險評估影像確認)`，否則跳「你沒有權限操作此功能」並中止；通過後彈出 ConfirmBox 二次確認「確定要以放射科醫師身分確認…」，使用者按否則中止。
- 點擊婦產科簽名 `On婦產科醫師確認SignAsync`：先檢查 `CheckAccessPage(ROLE風險評估結果確認)`，流程同上。
- 簽名成功後：寫入 `SignatureNode`（SignatureId/Name/Date）、加入對應 List、`AI評估 = 已完成`、更新病患並記錄操作日誌。詳見 [簽核確認.md](簽核確認.md)。

### 5.8 送 AI 推論的前置檢查（PushToAICheck，關鍵）

於臨床資料頁 `OnPushToAIBtn`（先檢查 `ROLEAI操作` 權限）呼叫 `PushToAICheck(patient, patientData, dicomImage)`，依序檢查，**任一不通過即回傳錯誤訊息並以 MessageBox 顯示、停止送 AI**：

| 順序 | 條件 | 失敗訊息 |
| --- | --- | --- |
| 1 | `臨床資訊.Age` 為空或 `ToInt() == 0` | 年齡未填 |
| 2 | `臨床資訊.Height` 為空或 `ToInt() == 0` | 身高未填 |
| 3 | `臨床資訊.Weight` 為空或 `ToInt() == 0` | 體重未填 |
| 4 | `臨床資訊.ECorOC`（癌別）為空 | 癌別未填 |
| 5 | DICOM 檔案 `File.Exists(dicomImage)` 為 false | 影像檔案不存在 |

通過後 `PushToAI` 建立 `PatientAIInfo`（`Gender` 固定 `"F"`）、`InitKeyName()` 產生 `KeyName`（`yyyyMMddHHmmss` + 微秒 4 碼）、把 DICOM 複製到 `UploadTemp/{KeyName}.dcm` 並 `CreateInBound` 入列；隨後病患 `AI處理 = 處理中`、`AI評估 = NA`、清空兩位醫師的當前簽名節點。

### 5.9 驗證規則小結

- 送 AI 前：年齡 / 身高 / 體重須為非零數字、癌別必填、DICOM 必須存在。
- 顯示結果前：必須有 `風險程度`（推論完成）。
- 簽名前：需對應角色權限 + 二次確認。

## 6. 資料模型與欄位

詳見 [領域資料模型](../03-資料模型/領域資料模型.md)。本模組相關：

- `RiskAssessmentResult`（[原始碼](../../src/CTMS/CTMS.DataModel/Models/ClinicalInformation/RiskAssessmentResult.cs)）：`ExperimentalControl`、`ExperimentalControlMessage`、`風險程度`、`是否需要降15Percent劑量`、`SMA骨骼肌面積`、`SMI骨骼肌指標`、`SMD骨骼肌密度`、`IMAT肌間肌肉脂肪組織`、`LAMA低密度肌肉區域`、`NAMA正常密度肌肉區域`、`Myosteatosis肌肉脂肪變性`、`ImagePng` 等。
- `InputCsvModel`（[原始碼](../../src/CTMS/CTMS.DataModel/Models/AIAgent/InputCsvModel.cs)）：`Total_SMD`、`Total_SMG`、`Total_ImatA`、`Total_LamaA`、`Total_NamaA`、`VatA`、`SatA`、`Myosteatosis` 等，為 AI 量測原始值。
- `SignatureNode`（[原始碼](../../src/CTMS/CTMS.DataModel/Models/ClinicalInformation/SignatureNode.cs)）：簽名紀錄。
- `PatientAIInfo`（[原始碼](../../src/CTMS/CTMS.DataModel/Models/AIAgent/PatientAIInfo.cs)）：送入 AI 的病患輸入。

掛載點為 `BasicClinicalPresentation_臨床資訊`（`KeyName`、`RiskAssessmentResult`、`RadiologistConfirmation(List)`、`ObstetricianGynecologistConfirmation(List)`）。

## 7. 資料流程

1. 上傳 DICOM → 轉 PNG（見 [DICOM影像處理.md](DICOM影像處理.md)）。
2. 臨床資料頁按「送 AI 推論」→ `PushToAICheck` 通過 → `PushToAI` 入列，`AI處理=處理中`。
3. AI 背景服務跑 Phase1/2/3，產出 `input.csv` / `output.csv` / 標註 PNG（見 [05-AI推論](../05-AI推論/overview.md)）。
4. `CheckAIProcess` 偵測到 `output.csv` → `ShowAICompletionAsync` 計算指標、判定風險/降劑量、依組別設定實驗/對照、寫回 `RiskAssessmentResult`、`AI處理=已完成`。
5. 風險評估頁讀 `RiskAssessmentResult` 呈現 → 放射科 / 婦產科簽名確認。

## 8. 與其他模組的關聯

- AI 推論流水線：[overview](../05-AI推論/overview.md)、[flow-1-dicom-upload](../05-AI推論/flow-1-dicom-upload.md)、[flow-2-manual-annotation](../05-AI推論/flow-2-manual-annotation.md)、[AIAgent背景服務](../05-AI推論/AIAgent背景服務.md)。
- DICOM 轉檔：[DICOM影像處理.md](DICOM影像處理.md)。
- 醫師簽核：[簽核確認.md](簽核確認.md)。
- 組別指派：亂數表服務（`RandomListService`），於臨床資料儲存時決定 `AI` / `Dr`。

## 9. 擴充與變更影響點

- **風險判定規則**：目前以 `output.csv` 是否含 `"a grade III AE"` 二分。若要改為多級風險或以指標門檻計算，需改 `GetOnputCsv`。
- **指標公式**：SMA / SMI / Myosteatosis 的計算集中於 `ShowAICompletionAsync`，變更公式只需改此處。
- **影像路徑**：左圖 `ImagePng`、右圖 `{KeyName}/Phase2Result/{KeyName}_muscle5.png` 為硬編慣例，AI 輸出檔名變動會影響右圖。
- **新增量測欄位**：需同步 `InputCsvModel`、`InputCsvService.Read` 的 header 對應、`RiskAssessmentResult` 與樣板表格。

## 10. 業務規則與注意事項

- 性別在送 AI 時固定為 `"F"`（本研究為婦科個案）。
- 風險程度與是否降劑量為同一布林條件，不可各自獨立設定。
- 實驗 / 對照的顯示僅影響「是否建議採用 AI 輔助決策」的文字，AI 仍會對兩組都產出風險結果。
- 結果區塊與簽名鈕在「風險程度為空」時完全不顯示，避免醫師對未完成的推論誤簽。
- `GetOnputCsv` 以「全文轉小寫 + Contains」比對，對 `output.csv` 格式變動較敏感，務必維持關鍵字一致性。
