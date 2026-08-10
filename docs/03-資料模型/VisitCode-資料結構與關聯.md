# Visit Code：資料結構與關聯

- 文件用途：Visit Code（回診次別）的資料結構、標題組成規則，以及它與各類臨床資料的關聯
- 主要讀者：開發者與 AI 助手
- 對應系統版本：1.1.230
- 最後核對日期：2026/08/10
- 編碼：UTF-8（繁體中文，含 BOM）

---

> Visit Code 是 CTMS 最核心的概念：**幾乎所有臨床資料都掛在某一次回診之下**。不理解它就寫不出正確的查詢或匯出。物件樹全貌見 [領域資料模型](../03-資料模型/領域資料模型.md)。

## 一、資料結構

`VisitCodeModel`（`CTMS.DataModel/Models/ClinicalInformation`）：

| 欄位 | 型別 | 說明 |
| --- | --- | --- |
| `Id` | `string` | ⭐ **關聯鍵**，預設值是 `Guid.NewGuid().ToString()`——每次 `new` 都會產生新值 |
| `AssessmentDate` | `DateTime?` | 評估／回診日期，**可為 null** |
| `Timeline` | `string` | 時間點標籤，例如 `Baseline`、`FU1(3M)`（可選值見下表） |
| `CycleMonth` | `int` | 週期數 |

### `Timeline` 的可選值

來自 `DropDownListDataService.GetVisitCode()`，共 9 項：

`Baseline`、`FU1(3M)`、`FU2(6M)`、`FU3(9M)`、`FU4(12M)`、`FU5(15M)`、`FU6(18M)`、`FU7(21M)`、`FU8(24M)`

## 二、畫面上看到的標題怎麼組出來

`GetVisitCodeTitle()` 的組法：

```
A: {AssessmentDate:yyyy-MM-dd} B: {Timeline} C: Cycle {CycleMonth}
```

- 沒有評估日期時，前段變成 `A: `（**冒號後面是空的**，不是省略整段）。
- 各頁籤的 Visit Code 下拉顯示的就是這個字串，排序由 `VisitCodeHelper.Sort` 決定。

⚠️ **標題是每次即時組出來的，不會存起來。** 改了 `Timeline` 或 `AssessmentDate`，所有畫面上的顯示會一起變。

## 三、比對規則（`CompareTo`）

判斷兩個 Visit Code 是否「相同」時：

| 情況 | 比對依據 |
| --- | --- |
| 兩邊都有 `AssessmentDate` | 日期（只比日期部分）＋ `Timeline` ＋ `CycleMonth` |
| 任一邊沒有日期 | 只比 `Timeline` ＋ `CycleMonth` |

🔴 **`CompareTo` 完全不看 `Id`。** 所以「內容相同但 `Id` 不同」的兩筆會被判定為相同，而畫面上的資料關聯**卻是用 `Id` 找的**（`header.Items.FirstOrDefault(x => x.VisitCode.Id == 選中的 Key)`）。這兩套判定不一致，是重複回診資料最可能的來源。

## 四、與其他資料的關聯

每一類臨床資料都是「一個 Node 清單」，每個 Node 帶一個 `VisitCode`：

```
Patient.JsonData
└─ PatientData
   ├─ 臨床資訊（單筆，不掛 Visit Code）
   ├─ 臨床資料
   │  ├─ 臨床資料手術.Items[]        → 每筆一個 VisitCode
   │  ├─ 臨床資料病理報告.Items[]     → 每筆一個 VisitCode
   │  ├─ 臨床資料化學治療.Items[]     → 每筆一個 VisitCode，底下再有多列治療紀錄
   │  ├─ 臨床資料合併用藥.Items[]     → 同上
   │  └─ BaselineMedicalHistoryForm  → 每筆一個 VisitCode
   ├─ 抽血檢驗血液 / 抽血檢驗生化      → 每筆一個 VisitCode，底下是 43 個檢驗項目
   ├─ 副作用                        → 每筆一個 VisitCode
   ├─ 問卷（8 份）                   → 每筆一個 VisitCode
   ├─ 追蹤（其他治療／藥物／影像）      → 每筆一個 VisitCode
   └─ RiskAssessmentResult（風險評估）
```

關聯方式是 **Node 內嵌一份 `VisitCodeModel` 的複本**，不是外鍵。同一次回診在不同類別各有一份複本，靠 `Id` 對應。

🔴 **後果：改一個 Visit Code 的日期，只會改到那一類資料裡的那一份複本。** 其他類別的同一次回診仍是舊值——除非程式明確做了同步（`PatientData.SyncData()`）。

## 五、哪些畫面可以新增 Visit Code

| 畫面 | 可否新增 |
| --- | --- |
| [基本臨床資料](../12-畫面欄位參考/基本臨床資料.md) | ✅ 「Visit Code」按鈕（僅檢視模式） |
| 副作用（正式版 `HematologicSideEffectsView`） | ✅ 新增／修改／刪除三個按鈕——🔴 **但頁面實際掛的是 Demo 版，這些按鈕看不到** |
| [臨床資訊](../12-畫面欄位參考/臨床資訊.md) 五個頁籤 | ❌ 觸發程式碼從未被呼叫 |
| [抽血檢驗](../12-畫面欄位參考/抽血檢驗.md)、[追蹤資料](../12-畫面欄位參考/追蹤資料.md) | ❌ 只能選 |

⚠️ **沒有任何 Visit Code 時，各頁籤的表格區塊整個不顯示**（`@if (data != null)`）。使用者會看到一個空白畫面而不是提示。

## 六、⚠️ 已知風險

| # | 風險 | 說明 |
| --- | --- | --- |
| 1 | 🔴 `CompareTo` 不看 `Id`，畫面關聯卻用 `Id` | 兩套判定不一致 |
| 2 | 🔴 Visit Code 以複本形式散在各類資料中 | 改一處不會全部同步 |
| 3 | ⚠️ 選錯 Visit Code 不會有任何警告 | 資料會安靜地存到別次回診 |
| 4 | ⚠️ `AssessmentDate` 可為 null | 標題會出現空的 `A: `，排序與比對行為不同 |
