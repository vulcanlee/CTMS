# 副作用與 CTCAE 分級

## 1. 目的與業務意圖

呈現受試者各次回診的副作用情形，分為兩部分：

1. **血液副作用（Adverse Event）**：依 CTCAE 標準，將抽血檢驗的白血球（WBC）、絕對嗜中性白血球數（ANC）、血色素（Hb）、血小板（Platelet）等四項數值，自動對應到 Grade 1–5 等級並於分級表上高亮顯示命中等級。
2. **其他副作用（問卷型）**：以自填問卷形式收集化療相關副作用。

目的是讓研究人員快速辨識血液毒性等級，輔助劑量調整與不良事件通報。

## 2. 使用情境與角色

- 角色：具備 `ROLE副作用` 權限者；`SideEffectPage` 於 `OnInitializedAsync` 以 `CheckAccessPage(MagicObjectHelper.ROLE副作用)` 驗證，無權限顯示「你沒有權限存取此頁面」。
- 情境：選定 Visit Code 後檢視該次抽血數值落在哪個 CTCAE 等級；或登錄問卷型副作用。

## 3. 關鍵頁面（Razor）

- [SideEffectPage.razor](../../src/CTMS/CTMS/Components/Pages/ClinicalInformation/SideEffectPage.razor) — 路由 `/SideEffectPage/{code}`，`EmptyLayout`。三個 `MudTabPanel`：
  - 「血液副作用 (Adverse event)」→ [HematologicSideEffectsDemoView](../../src/CTMS/CTMS/Components/Views/ClinicalInformation/HematologicSideEffectsDemoView.razor)（目前啟用；`HematologicSideEffectsView` 已註解停用）
  - 「其他副作用1」→ [Survey1SideEffectsView](../../src/CTMS/CTMS/Components/Views/ClinicalInformation/Survey1SideEffectsView.razor)
  - 「其他副作用2」→ [Survey2SideEffectsView](../../src/CTMS/CTMS/Components/Views/ClinicalInformation/Survey2SideEffectsView.razor)

## 4. 關鍵服務與方法

[SideEffectsService.cs](../../src/CTMS/CTMS.Business/Services/ClinicalInformation/SideEffectsService.cs)

| 方法 | 用途 |
| --- | --- |
| `InitAll` | 一次初始化四項 CTCAE 分級門檻（`Init血液副作用...`）。 |
| `Init血液副作用WhiteBloodCell白血球` / `...NeutrophilCount絕對嗜中性白血球數` / `...HemoglobinHb血色素` / `...PlateletCount血小板` | 設定各項 Grade1–5 的 `Title` 與門檻值 `GradeValue1`（下界）/`GradeValue2`（上界）。 |
| `Update副作用All` / `Update副作用...` | 由抽血數值更新各項 `RetriveValue` 並呼叫 `ComputeGrade`。 |
| `ComputeGrade` | 依數值落在哪個 `[GradeValue1, GradeValue2)` 區間，將該 Grade 的 CSS 設為命中（`ResetCssClassFound`）。 |
| `ComputeGradeByDemo` | 依 SubjectNo（含 `-C`）展示用的固定高亮。 |
| `FindItemByName` | 以 `BloodTestItemNameUnitHelper.NormalizeItemName` 比對標準項目名稱。 |

> 標準項目名稱常數來自 `MagicObjectHelper`：`WhiteBloodCell白血球WBC`、`NeutrophilCount絕對嗜中性白血球數Seg`、`NeutrophilCount帶狀性中性球Band`、`HemoglobinHb血色素Hb`、`PlateletCount血小板Plt`。

## 5. 畫面商業邏輯（核心）

### 5.1 血液副作用分級表（HematologicSideEffectsDemoView）

- 欄位：先選 Visit Code（`SfDropDownList`，連動 `OnVisitCodeChanged`），表格固定 5 列（grade 1–5）× 4 欄（白血球 / 絕對嗜中性白血球數 / 血色素 / 血小板）。
- 每格顯示 `GradeItem.Title`，並以 `class="@...GradeX.ApplyCssClass"` 套用高亮；命中等級為 `FoundClass`，未命中為 `NotFoundClass`。
- 此 Demo View 僅顯示分級表（不含輸入欄），數值來源為抽血檢驗模組。

### 5.2 CTCAE 分級門檻與判定機制

判定邏輯（`ComputeGrade`）：對每個項目，命中條件為
`GradeValueN1 <= 受測值 < GradeValueN2`（左閉右開）。命中即把該 Grade 標記為 Found。

> 單位換算：WBC 與 Platelet 的受測值與 LLN 皆乘以 1000（如 WBC `4.6 (10^3/μL)` → `4600/μL`）；Hb 不乘。
> Grade1 的上界 `GradeValue2` 在 `Update副作用...` 中被動態改寫為該項目「參考區間開始（LLN）」（WBC/Platelet ×1000，Hb 不乘）。下表為 `Init` 階段的靜態預設值。

實際數值門檻（取自 `SideEffectsService` 各 `Init` 方法；GradeValue1=下界、GradeValue2=上界）：

#### 白血球 WBC（單位 /mm³）

| Grade | Title | 下界 | 上界 |
| --- | --- | --- | --- |
| 1 | <LLN - 3000/mm3 | 3000 | 0（執行期改為 LLN×1000） |
| 2 | <3000 - 2000/mm3 | 2000 | 3000 |
| 3 | <2000 - 1000/mm3 | 1000 | 2000 |
| 4 | <1000 | 0 | 1000 |
| 5 | - | 0 | 0 |

#### 絕對嗜中性白血球數 ANC（單位 /mm³）

| Grade | Title | 下界 | 上界 |
| --- | --- | --- | --- |
| 1 | <LLN - 1500/mm3 | 1500 | 0 |
| 2 | <1500 - 1000/mm3 | 1000 | 1500 |
| 3 | <1000 - 500/mm3 | 500 | 1000 |
| 4 | <500 | 0 | 500 |
| 5 | - | 0 | 0 |

> ANC 計算公式（`Update副作用NeutrophilCount...`）：`ANC = WBC × 1000 × (Seg% + Band%)`，其中 Seg/Band 取自血液檢驗項目；若無 Band，Band% 視為 0。

#### 血色素 Hb（單位 g/dL，不 ×1000）

| Grade | Title | 下界 | 上界 |
| --- | --- | --- | --- |
| 1 | <LLN - 10.0 g/dL | 10.0 | 0（執行期改為 LLN） |
| 2 | Hgb <10.0 - 8.0 g/dL | 8.0 | 10.0 |
| 3 | Hgb <8.0 g/dL | 0 | 8.0 |
| 4 | - | 0 | 0 |
| 5 | Death | 0 | 0 |

#### 血小板 Platelet（單位 /mm³）

| Grade | Title | 下界 | 上界 |
| --- | --- | --- | --- |
| 1 | <LLN - 75,000/mm3 | 75000 | 0 |
| 2 | <75,000 - 50,000/mm3 | 50000 | 75000 |
| 3 | <50,000 - 25,000/mm3 | 25000 | 50000 |
| 4 | <25,000/mm3 | 0 | 25000 |
| 5 | - | 0 | 0 |

> 註：Grade 上界為 0 者（如各項 Grade5、Hb Grade4）因「下界 <= 值 < 0」永不成立，故不會被命中——屬於僅供顯示的占位等級（Hb 的 Death、其餘的 `-`）。

### 5.3 其他副作用（問卷型）

「其他副作用1/2」（Survey1/Survey2SideEffectsView）以問卷卡片形式呈現，邏輯與[問卷收集](問卷收集.md)模組一致（選項點選、`IsVisible` 條件顯示、儲存寫回病患 JSON）。化療副作用之自填問卷主要走 `/Survey` 路由，詳見問卷收集文件。

### 5.4 儲存 / 送出

血液副作用 Demo View 為唯讀分級展示，未提供儲存；問卷型副作用以各 View 的「儲存」按鈕寫回 `PatientService.UpdateAsync`。

## 6. 資料模型與欄位

詳見 [領域資料模型](../03-資料模型/領域資料模型.md)。核心型別：

- [HematologicSideEffects血液副作用.cs](../../src/CTMS/CTMS.DataModel/Models/ClinicalInformation/HematologicSideEffects血液副作用.cs) / [Node](../../src/CTMS/CTMS.DataModel/Models/ClinicalInformation/HematologicSideEffects血液副作用Node.cs)：每 Visit Code 一節點，含四個 `GradeItemSideEffectsItem`。
- `GradeItemSideEffectsItem`：`RetriveValue` + `Grade1..Grade5`（`GradeItem`：`Title`、`GradeValue1/2`、`ApplyCssClass`）。
- 問卷型：[Survey化療副作用.cs](../../src/CTMS/CTMS.DataModel/Models/ClinicalInformation/Survey化療副作用.cs)、[Survey1SideEffects副作用.cs](../../src/CTMS/CTMS.DataModel/Models/ClinicalInformation/Survey1SideEffects副作用.cs)、[Survey2SideEffects副作用.cs](../../src/CTMS/CTMS.DataModel/Models/ClinicalInformation/Survey2SideEffects副作用.cs)。

## 7. 資料流程

1. 載入 `PatientData` → 取 `臨床資料.HematologicSideEffects血液副作用` 首筆節點。
2. 切換 Visit Code → 取對應節點。
3. 分級展示：以 `Init` 設定門檻、`Update副作用...` 由抽血數值計算 `RetriveValue`、`ComputeGrade` 標記命中等級。
4. 問卷型副作用：選項變更即時更新並可儲存。

## 8. 與其他模組的關聯

- 抽血檢驗：CTCAE 受測值（WBC/Seg/Band/Hb/Platelet）來自 `main臨床資料.抽血檢驗血液`，依 Visit Code 比對（見[抽血檢驗](抽血檢驗.md)）。
- 問卷收集：問卷型副作用共用 `SurveyService` 與 `Question`／`VisibilityCondition` 機制（見[問卷收集](問卷收集.md)）。
- Visit Code：跨模組共用回診碼。

## 9. 擴充與變更影響點

- 調整 CTCAE 門檻：修改對應 `Init血液副作用...` 方法中的 `GradeValueN1/N2` 與 `Title`（並注意 WBC/Platelet 的 ×1000 與 Hb 不乘的單位差異）。
- 新增分級項目：於 `HematologicSideEffects血液副作用Node` 增欄、`InitAll`/`Update副作用All` 增對應方法、Demo View 表格增欄、補 `MagicObjectHelper` 標準名稱。
- ANC 計算調整：集中於 `Update副作用NeutrophilCount絕對嗜中性白血球數`。

## 10. 業務規則與注意事項

- 命中判定為左閉右開 `[下界, 上界)`；門檻值以字串存放，計算時 `ToDouble()` 轉換。
- WBC/Platelet 受測值與 LLN 一律 ×1000；Hb 不乘——調整門檻時務必對齊單位。
- Grade1 上界於執行期被改為「參考區間開始（LLN）」，故與 `Init` 預設的靜態上界不同。
- 上界為 0 的等級（`-`、Death）僅供顯示，不會被命中。
- 目前頁面採用 `HematologicSideEffectsDemoView`（展示版），舊版 `HematologicSideEffectsView` 已停用。
