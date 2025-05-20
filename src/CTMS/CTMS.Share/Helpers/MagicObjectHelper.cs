using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTMS.Share.Helpers
{
    public class MagicObjectHelper
    {
        public const string CT身體組成WorkSheetName = "CT身體組成";
        public const string BIA身體組成WorkSheetName = "BIA身體組成(初版)";
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
        public const string FilenameAthletesExamine = "AthletesExamine.json";
        public const string NoUpdateSymbol = "NA";

        #region 第二階段的需求修正
        public const string 主頁20241212WorkSheetName = "主頁";
        public const string 動作分析能力20241212WorkSheetName = "動作分析能力";
        public const string 心肺功能20241212心肺功能 = "心肺功能";
        public const string 身體組成_肌肉質量20241212WorkSheetName = "身體組成-肌肉質量";
        public const string 身體組成_肌肉品質20241212WorkSheetName = "身體組成-肌肉品質";
        public const string 身體組成_脂肪分析20241212WorkSheetName = "身體組成-脂肪分析";
        public const string 基因體分析20241212WorkSheetName = "基因體分析";
        public const string 基因體細項20241212WorkSheetName = "基因體細項";
        public const string 代謝體分析20241212WorkSheetName = "代謝體分析";
        public const string 抽血檢驗_血液20241212WorkSheetName = "抽血檢驗(血液)";
        public const string 抽血檢驗_生化20241212WorkSheetName = "抽血檢驗(生化)";
        public const string 抽血檢驗_特殊20241212WorkSheetName = "抽血檢驗(特殊)";
        public const string 綜合評估建議20241212WorkSheetName = "綜合評估建議";
        public const string CT身體組成原始擋20241212WorkSheetName = "CT身體組成原始擋";
        public const string BIA身體組成原始擋20241212WorkSheetName = "BIA身體組成原始擋";
        public const string 心肺功能原始擋20241212WorkSheetName = "心肺功能原始擋";
        public const string 心理韌性原始擋20241212WorkSheetName = "心理韌性原始檔";
        public const string 代謝物含量壓力情緒指標原始檔20241212WorkSheetName = "代謝物含量-壓力情緒指標原始檔";
        public const string 基因體分析定義20241212WorkSheetName = "基因體分析";
        public const string 基因體分析原始擋20241212WorkSheetName = "基因體分析原始擋";
        public const string 代謝体分析原始擋20241212WorkSheetName = "代謝体分析原始擋";
        public const string 抽血檢驗_血液原始檔20241212WorkSheetName = "抽血檢驗(血液)原始檔";
        public const string 抽血檢驗_生化原始檔20241212WorkSheetName = "抽血檢驗(生化)原始檔";
        public const string 抽血檢驗_特殊原始檔20241212WorkSheetName = "抽血檢驗(特殊)原始檔";

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
        public const string 預設檢驗時間 = "2025-01-01";
        public const string 你沒有權限存取此頁面 = "你沒有權限存取此頁面";
        public const string ROLE主頁 = "主頁";
        public const string ROLE瀏覽 = "瀏覽";
        public const string ROLE動作能力 = "動作能力";
        public const string ROLE心肺功能 = "心肺功能";
        public const string ROLE心理韌性 = "心理韌性";
        public const string ROLE身體組成 = "身體組成";
        public const string ROLE基因體分析 = "基因體分析";
        public const string ROLE代謝體分析 = "代謝體分析";
        public const string ROLE抽血檢驗 = "抽血檢驗";
        public const string ROLE綜合評估建議 = "綜合評估建議";
        public const string ROLE綜合評估建議編輯 = "綜合評估建議編輯";
        public const string ROLE報告摘要輸入 = "報告摘要輸入";
        public const string ROLE下載PDF = "下載PDF";
        public const string ROLE上傳資料 = "上傳資料";
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

        public static readonly string 角色明細之專案清單管理功能名稱 = "角色明細之專案清單";
        public const string CheckBoxIcon = "mdi-checkbox-marked-outline";
        public const string CheckBoxBlankIcon = "mdi-checkbox-blank-outline";
        public const string VisitCodeA = "A";
        public const string VisitCodeB = "B";
        public const string VisitCodeC = "C";

        public const string TimelineBaseline = "Baseline";
        public const string TimelineSurgery = "Surgery";
        public const string TimelineChemotherapyDate = "Chemotherapy Date";
        public const string TimelinePostChemotherapy = "Post Chemotherapy";

        public const string NotFoundClass = "not-found";
        public const string FoundClass = "has-found";
    }
}