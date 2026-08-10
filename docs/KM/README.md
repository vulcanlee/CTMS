# 使用者知識庫（KM）

本資料夾收錄**面向使用者**的操作說明，供知識庫問答查詢使用。每份文件對應一個系統畫面，說明這個畫面做什麼、每個欄位怎麼填、每個按鈕按下去會怎樣、畫面會跳出哪些字、以及常見問答。

> 內容以系統實際畫面為準，用白話撰寫，不含程式碼與技術名詞。**畫面上會出現的文字一律逐字照抄程式碼**，這樣你才能把看到的字直接貼進來搜尋。

## 先看這兩份

| 文件 | 什麼時候看 |
| --- | --- |
| [新手入門：從收案到匯出](新手入門-從收案到匯出.md) | 第一次用這套系統。用一位受試者的完整流程把所有畫面串起來，從頭讀到尾 |
| [共通訊息與錯誤訊息](共通訊息與錯誤訊息.md) | 畫面跳出看不懂的錯誤字。⭐ 所有畫面共用的儲存／刪除失敗訊息集中在這裡 |

## 畫面覆蓋矩陣

這張表證明**沒有孤兒畫面**：系統的每一個路由都找得到歸屬。新增路由時，這裡多一列就代表手冊要多一份。

### 個案與總覽

| 畫面 | 路由 | 使用手冊 | 主要程式來源 |
| --- | --- | --- | --- |
| 受試者清單 | `/Browser` | [受試者清單](受試者清單-Browser.md) | [BrowserPage.razor](../../src/CTMS/CTMS/Components/Pages/ClinicalInformation/BrowserPage.razor) |
| 儀表板 | `/Dashboard` | [儀表板](儀表板-Dashboard.md) | [DashboardPage.razor](../../src/CTMS/CTMS/Components/Pages/ClinicalInformation/DashboardPage.razor) |
| 收案進度 | `/EnrollmentProgress` | [收案進度](收案進度-EnrollmentProgress.md) | [EnrollmentProgressPage.razor](../../src/CTMS/CTMS/Components/Pages/ClinicalInformation/EnrollmentProgressPage.razor) |

### 臨床資料

| 畫面 | 路由 | 使用手冊 | 主要程式來源 |
| --- | --- | --- | --- |
| 基本臨床資料 | `/BasicClinical/{code}` | [基本臨床資料](基本臨床資料-BasicClinical.md) | [BasicClinicalPage2.razor](../../src/CTMS/CTMS/Components/Pages/ClinicalInformation/BasicClinicalPage2.razor) |
| 臨床資訊 | `/ClinicalInformation/{code}` | [臨床資訊](臨床資訊-ClinicalInformation.md) | [ClinicalInformationPage.razor](../../src/CTMS/CTMS/Components/Pages/ClinicalInformation/ClinicalInformationPage.razor) |
| 抽血檢驗 | `/BloodTest/{code}` | [抽血檢驗](抽血檢驗-BloodTest.md) | [BloodTestPage.razor](../../src/CTMS/CTMS/Components/Pages/ClinicalInformation/BloodTestPage.razor) |
| 副作用 | `/SideEffectPage/{code}` | [副作用](副作用-SideEffect.md) | [SideEffectPage.razor](../../src/CTMS/CTMS/Components/Pages/ClinicalInformation/SideEffectPage.razor) |
| AI 風險評估 | `/RiskAssessment/{code}` | [風險評估](風險評估-RiskAssessment.md) | [RiskAssessmentPage.razor](../../src/CTMS/CTMS/Components/Pages/ClinicalInformation/RiskAssessmentPage.razor) |
| 追蹤資料與影像 | `/TrackingData/{code}` | [追蹤資料](追蹤資料-TrackingData.md) | [TrackingDataPage.razor](../../src/CTMS/CTMS/Components/Pages/ClinicalInformation/TrackingDataPage.razor) |

### 問卷

| 畫面 | 路由 | 使用手冊 | 主要程式來源 |
| --- | --- | --- | --- |
| 問卷調查 | `/Survey/{code}` | [問卷調查](問卷調查-Survey.md) | [SurveyPage.razor](../../src/CTMS/CTMS/Components/Pages/ClinicalInformation/SurveyPage.razor) |
| 手機問卷 | `/SurveyMobile/{code}` | [手機問卷](手機問卷-SurveyMobile.md) | [SurveyMobilePage.razor](../../src/CTMS/CTMS/Components/Pages/ClinicalInformation/SurveyMobilePage.razor) |
| 問卷（僅標題頁） | `/Questionnaire/{code}` | [問卷](問卷-Questionnaire.md) | [QuestionnairePage.razor](../../src/CTMS/CTMS/Components/Pages/ClinicalInformation/QuestionnairePage.razor) |

### 匯出、稽核與筆記

| 畫面 | 路由 | 使用手冊 | 主要程式來源 |
| --- | --- | --- | --- |
| 隨機表 | `/Random` | [隨機表](隨機表-Random.md) | [RandomPage.razor](../../src/CTMS/CTMS/Components/Pages/AdminTools/RandomPage.razor) |
| 資料匯出 | `/Export` | [資料匯出](資料匯出-Export.md) | [ExportPage.razor](../../src/CTMS/CTMS/Components/Pages/AdminTools/ExportPage.razor) |
| 操作歷程 | `/HistoryTrace` | [操作歷程](操作歷程-HistoryTrace.md) | [OperationHistoryTracePage.razor](../../src/CTMS/CTMS/Components/Pages/AdminTools/OperationHistoryTracePage.razor) |
| 我的筆記 | `/MyNote` | [我的筆記](我的筆記-MyNote.md) | [MyNotePage.razor](../../src/CTMS/CTMS/Components/Pages/Ants/MyNotePage.razor) |

### 帳號與權限

| 畫面 | 路由 | 使用手冊 | 主要程式來源 |
| --- | --- | --- | --- |
| 登入 | `/Auths/Login` | [登入](登入-Login.md) | [Login.razor](../../src/CTMS/CTMS/Components/Auths/Login.razor) |
| 變更密碼 | `/ChangePassword` | [變更密碼](變更密碼-ChangePassword.md) | [ChangePasswordPage.razor](../../src/CTMS/CTMS/Components/Pages/AdminTools/ChangePasswordPage.razor) |
| 使用者管理 | `/UserManagement` | [使用者管理](使用者管理-UserManagement.md) | [MyUserPage.razor](../../src/CTMS/CTMS/Components/Pages/Datas/MyUserPage.razor) |
| 角色權限 | `/RoleView` | [角色權限](角色權限-RoleView.md) | [RoleViewPage.razor](../../src/CTMS/CTMS/Components/Pages/Datas/RoleViewPage.razor) |
| 建立帳號 | `/SignUp` | [建立帳號](建立帳號-SignUp.md) | [SignUpAccountPage.razor](../../src/CTMS/CTMS/Components/Pages/AdminTools/SignUpAccountPage.razor) |
| 註冊 | `/Register` | [註冊](註冊-Register.md) | [RegisterPage.razor](../../src/CTMS/CTMS/Components/Pages/AdminTools/RegisterPage.razor) |
| 註冊審核 | `/ApprovalRegistration` | [註冊審核](註冊審核-ApprovalRegistration.md) | [ApprovalRegistrationPage.razor](../../src/CTMS/CTMS/Components/Pages/AdminTools/ApprovalRegistrationPage.razor) |

### 系統管理

| 畫面 | 路由 | 使用手冊 | 主要程式來源 |
| --- | --- | --- | --- |
| 專案管理 | `/Project` | [專案管理](專案管理-Project.md) | [ProjectPage.razor](../../src/CTMS/CTMS/Components/Pages/Datas/ProjectPage.razor) |
| 系統維護 | `/SystemMaintain` | [系統維護](系統維護-SystemMaintain.md) | [SystemMaintainPage.razor](../../src/CTMS/CTMS/Components/Pages/AdminTools/SystemMaintainPage.razor) |
| 病患對應 | `/PatientKeyName` | [病患對應](病患對應-PatientKeyName.md) | [PatientKeyNamePage.razor](../../src/CTMS/CTMS/Components/Pages/AdminTools/PatientKeyNamePage.razor) |

### 刻意沒有使用手冊的路由

| 路由 | 為什麼不需要手冊 |
| --- | --- |
| `/` | 首頁轉址，沒有使用者可操作的內容 |
| `/Auths/Logout` | 登出處理頁，按下登出後自動導向，沒有畫面 |
| `/Error` | 系統錯誤頁，由系統自行顯示 |
| `/Sample` | 開發用的範例頁，不是正式功能 |

⭐ **以上 26 份手冊 ＋ 4 個免手冊路由 ＝ 系統全部路由。** 這張矩陣是「畫面與路由清單」的唯一出處，其他文件不要另抄一份。

## 每份畫面手冊的固定章節

新增或改寫畫面手冊時，依下列順序寫。⚠️ 前四項與最後一項是**必備**，`scripts/Test-Docs.ps1` 會檢查；中間三項**視畫面實際情況決定要不要寫**。

| 順序 | 章節 | 必備 | 說明 |
| --- | --- | --- | --- |
| 1 | 這個畫面是做什麼的？ | ✅ | 用途 |
| 2 | 怎麼進到這個畫面？ | ✅ | 路由與所需權限 |
| 3 | 畫面欄位說明 | ❌ | 沒有可填欄位的畫面（例如資料匯出）就不寫 |
| 4 | 按鈕與操作說明 | ❌ | 唯讀畫面（例如收案進度）就不寫 |
| 5 | ✍️ 命名與填寫建議 | ❌ | **只有真的存在命名或格式規則時才寫**，硬湊等於編造 |
| 6 | 狀態與訊息 | ✅ | 🔴 **畫面文字必須逐字照抄程式碼，不可潤飾**——使用者是拿看到的字回來搜尋，改一個字就搜不到 |
| 7 | 常見操作步驟 | ❌ | 有多步驟流程時才寫 |
| 8 | 常見問答（FAQ） | ✅ | |

## 名詞小辭典

| 名詞 | 意思 |
| --- | --- |
| 受試者 / 受測者 | 參加本臨床試驗的病人。畫面上的「新增受測者資料」即新增一位病人。 |
| 病人編號 / Subject No | 系統為每位受試者自動產生的唯一編號（例如 `NCKUH0001`），開頭英文代表收案醫院。 |
| 組別 | 隨機分組結果：實驗組（AI）、對照組（Dr）或 NA（尚未分組）。 |
| 收案 / 退出 | 受試者目前的狀態。只有「收案」中的人會被納入儀表板統計。 |
| 癌別（EC / OC） | EC＝子宮內膜癌、OC＝卵巢癌。 |
| 分期（Early / Advance） | 癌症分期：第 I、II 期屬 Early（早期）；第 III、IV 期屬 Advance（晚期）。 |
| Visit Code | 回診次別。幾乎所有臨床資料都掛在某一次回診之下，同一個人會有多筆。 |

> **現況提醒**：以下畫面目前仍在建置或功能未完成，對應文件已如實說明並提供替代做法——
> 「資料匯出（`/Export`）」按鈕尚未生效、「問卷（`/Questionnaire`）」僅顯示標題無法填寫、「建立帳號（`/SignUp`）」送出尚未實作。
