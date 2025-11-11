namespace CTMS.Share.Helpers
{
    public class MagicObjectHelper
    {
        public const string DefaultSQLiteConnectionString = "SQLiteDefaultConnection";
        public const string CTMSSettings = "CTMSSettings";
        public const string PdfFilesPath = "PdfFiles";
        public const string UploadTempPath = "UploadTemp";
        public const string UploadFinalPath = "UploadFiles";
        public const string DownloadPath = "DownloadFiles";
        public const string DecompressPath = "DecompressFiles";
        public const string DbTablePath = "DbTable";
        public const string FilenameProject = "Project.json";
        public const string FilenameRoleView = "RoleView.json";
        public const string FilenameMyUser = "MyUser.json";
        public const string FilenameRegisterModel = "RegisterModels.json";
        public const string FilenameAthletesExamine = "AthletesExamine.json";
        public const string NoUpdateSymbol = "NA";

        #region 第二階段的需求修正

        public const string 參考區間類型_區間 = "參考區間類型_區間";
        public const string 參考區間類型_小於 = "參考區間類型_小於";
        public const string 參考區間類型_小於等於 = "參考區間類型_小於等於";
        public const string 參考區間類型_大於 = "參考區間類型_大於";
        public const string 參考區間類型_大於等於 = "參考區間類型_大於等於";
        public const string 正常檢驗數計類別 = "blood-black";
        public const string 異常檢驗數計類別 = "blood-red";
        #endregion

        #region System
        public const string CookieScheme = "CookieAuthenticationScheme";
        public const string 開發者帳號 = "support";
        public const string 預設專案 = "Default Project";
        public const string 預設角色 = "Default Role";
        public const string 預設新建帳號角色 = "預設新建帳號角色";
        public const string 你沒有權限存取此頁面 = "你沒有權限存取此頁面";
        public const string 你沒有權限操作此功能 = "你沒有權限操作此功能";
        public const string ROLE瀏覽 = "瀏覽";
        public const string ROLE新增病患 = "新增病患";
        public const string ROLE臨床資訊 = "臨床資訊";
        public const string ROLE臨床資料 = "臨床資料";
        public const string ROLE抽血資料 = "抽血資料";
        public const string ROLE副作用 = "副作用";
        public const string ROLE問卷 = "問卷";
        public const string ROLE追蹤資料 = "追蹤資料";
        public const string ROLE風險評估 = "風險評估";
        public const string ROLE通知放射科醫師 = "通知放射科醫師";
        public const string ROLE風險評估影像確認 = "風險評估影像確認";
        public const string ROLE風險評估結果確認 = "風險評估結果確認";
        public const string ROLE風險評估確認歷程 = "風險評估確認歷程";
        public const string ROLEAI操作 = "AI操作";
        public const string ROLE備份還原 = "備份還原";
        public static readonly int NeedDelayRefresh = 200;
        #endregion

        #region DataGrid 的欄位設定
        public static readonly string DataGrid狀態寬度 = "30";
        public static readonly string DataGrid順序寬度 = "60";
        public static readonly string DataGrid簽核寬度 = "110";
        public static readonly string DataGrid4個命令寬度 = "180";
        public static readonly string DataGrid3個命令寬度 = "140";
        public static readonly string DataGrid2個命令寬度 = "100";
        public static readonly string DataGrid圖示大小 = "mdi-18px";
        public static readonly int GridPageSize = 12;
        #endregion

        #region AI 臨床試驗管理平臺
        public static readonly string 角色明細之專案清單管理功能名稱 = "角色明細之專案清單";
        public const string CheckBoxIcon = "mdi-checkbox-marked-outline";
        public const string CheckBoxBlankIcon = "mdi-checkbox-blank-outline";
        public const string CheckBoxUnknownIcon = "mdi-checkbox-blank";
        public const string VisitCodeA = "A";
        public const string VisitCodeB = "B";
        public const string VisitCodeC = "C";

        public const string TimelineBaseline = "Baseline";
        public const string TimelineSurgery = "Surgery";
        public const string TimelineChemotherapyDate = "Chemotherapy Date";
        public const string TimelinePostChemotherapy = "Post Chemotherapy";

        public const string NotFoundClass = "not-found";
        public const string FoundClass = "has-found";
        public const string FoundLowClass = "grade-level-low";
        public const string FoundHeighClass = "grade-level-heigh";

        public const string ProteinLoss = "Loss";
        public const string ProteinPreserve = "Preserve";
        public const string ProteinUnknown = "Unknown";
        public const string Protein_dMMR = "dMMR";
        public const string Protein_pMMR = "pMMR";

        public const string 實驗組 = "實驗組";
        public const string 實驗組Message = "需";
        public const string 對照組 = "對照組";
        public const string 對照組Message = "不需";

        public const string StageC = "c";
        public const string StageP = "p";

        public const string 國立成功大學醫學院附設醫院 = "國立成功大學醫學院附設醫院";
        public const string 耀瑄科技股份有限公司 = "耀瑄科技股份有限公司";
        public const string 癌別 = "癌別";
        public const string 院別 = "院別";

        public const string Blood抽血檢驗血液 = "抽血檢驗血液";
        public const string Blood抽血檢驗生化 = "抽血檢驗生化";

        public const string 成醫抽血檢驗血液File = "抽血檢驗血液.json";
        public const string 成醫抽血檢驗生化File = "抽血檢驗生化.json";
        public const string 奇美抽血檢驗血液File = "抽血檢驗血液1.json";
        public const string 奇美抽血檢驗生化File = "抽血檢驗生化1.json";
        public const string 郭綜合抽血檢驗血液File = "抽血檢驗血液2.json";
        public const string 郭綜合抽血檢驗生化File = "抽血檢驗生化2.json";
        public const string RandomListDefaultFile = "RandomList.xlsx";
        public const string RandomListRuntimeFile = "RandomListRuntime.xlsx";
        public const string RandomListRuntimeJsonFile = "RandomListRuntime.json";
        public const string SubjectNoGeneratorJsonFile = "SubjectNoGenerator.json";
        public const string TreatmentDr = "Dr";
        public const string TreatmentAI = "AI";
        public const string Sheet成大Early = "成大Early";
        public const string Sheet成大Advance = "成大Advance";
        public const string Sheet奇美Early = "奇美Early";
        public const string Sheet奇美Advance = "奇美Advance";
        public const string Sheet郭綜合Early = "郭綜合Early";
        public const string Sheet郭綜合Advance = "郭綜合Advance";
        public const string Sheet預計收案人數 = "預計收案人數";

        public const string RandomEarly = "Early";
        public const string RandomAdvance = "Advance";
        public const string prefix成大醫院 = "NCKUH";
        public const string prefix奇美醫院 = "CHIMEIH";
        public const string prefix郭綜合醫院 = "KGH";
        public const string PrefixSheetName成大醫院 = "成大";
        public const string PrefixSheetName奇美醫院 = "奇美";
        public const string PrefixSheetName郭綜合醫院 = "郭綜合";
        public const string OC = "OC";
        public const string EC = "EC";
        public const string NA = "NA";
        public const string 組別對照組英文 = "Dr";
        public const string 組別實驗組英文 = "AI";
        public const string 組別對照組中文 = 對照組;
        public const string 組別實驗組中文 = 實驗組;
        public const string AI處理處理中 = "處理中";
        public const string AI處理已完成 = "已完成";
        public const string AI評估完成 = "已完成";
        public const string NeedChangePassword = "123456";

        public const string TypeI = "Type I";
        public const string TypeII = "Type II";
        #endregion

        #region AIAgent 專用
        public const string Agentsetting = "Agentsetting";
        public const string PrefixPatientData = "PatientData";
        public const string Phase1ResultPath = "Phase1Result";
        public const string Phase2ResultPath = "Phase2Result";
        public const string Phase3ResultPath = "Phase3Result";
        public const string 風險評估輸入csv = "input.csv";
        public const string 風險評估輸出csv = "output.csv";

        #endregion

        #region 操作歷程
        public const string OperationMessage使用者登入操作 = "使用者登入操作";
        public const string OperationMessage建立一筆受測者資料 = "建立一筆受測者資料";

        public const string OperationCategory登入 = "登入";
        public const string OperationCategory建立紀錄 = "新增";

        public const string Prompt原始資料Instruction = "原始資料";
        public const string Prompt編輯後Instruction = "編輯後";
        public const string PromptOperationHistoryInstruction =
            @"這裡需要紀錄 操作軌跡並要寫入到資料庫，整理出一句話的摘要與詳細說明資料異動狀況 ，可儲存為操作紀錄訊息
使用底下 json 結構回應，不需要其他額外文字內容 { ""summary"": ""一句話的摘要"", ""details"": ""一句話詳細說明資料異動狀況 ，可儲存為操作紀錄訊息"" isChanged: true}";

        #endregion
    }
}