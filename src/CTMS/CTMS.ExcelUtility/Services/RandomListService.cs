using CTMS.DataModel.Models.ClinicalInformation;
using CTMS.Share.Helpers;
using Syncfusion.XlsIO;

namespace SyncExcel.Services;

public class RandomListService
{
    public RandomListModel RandomList { get; set; } = new();

    public async Task CheckRandomListXlsx()
    {
        string filenameDefault = Path.Combine("Data", MagicObjectHelper.RandomListDefaultFile);
        string filenameRuntime = Path.Combine("Data", MagicObjectHelper.RandomListRuntimeFile);

        if (File.Exists(filenameRuntime) == false)
        {
            File.Copy(filenameDefault, filenameRuntime, true);
            return;
        }
    }

    #region 讀取 Excel 內的資料

    public void ReadExcel()
    {
        using (ExcelEngine excelEngine = new ExcelEngine())
        {
            //Instantiate the Excel application object
            IApplication application = excelEngine.Excel;

            //Assigns default application version
            application.DefaultVersion = ExcelVersion.Xlsx;

            string filenameRandomList = Path.Combine("Data", MagicObjectHelper.RandomListRuntimeFile);

            using (FileStream sampleFile = new FileStream(filenameRandomList, FileMode.Open))
            {
                IWorkbook workbook = application.Workbooks.Open(sampleFile);

                ReadSheet(workbook, MagicObjectHelper.Sheet成大Early);
                ReadSheet(workbook, MagicObjectHelper.Sheet成大Advance);
                ReadSheet(workbook, MagicObjectHelper.Sheet奇美Early);
                ReadSheet(workbook, MagicObjectHelper.Sheet奇美Advance);
                ReadSheet(workbook, MagicObjectHelper.Sheet郭綜合Early);
                ReadSheet(workbook, MagicObjectHelper.Sheet郭綜合Advance);
            }
        }
    }

    private void ReadSheet(IWorkbook workbook, string sheetName)
    {
        RandomListItem randomListItem = new();
        List<RandomListItem> sheetData = new();
        switch (sheetName)
        {
            case MagicObjectHelper.Sheet成大Early:
                sheetData = RandomList.成大Early;
                break;
            case MagicObjectHelper.Sheet成大Advance:
                sheetData = RandomList.成大Advance;
                break;
            case MagicObjectHelper.Sheet奇美Early:
                sheetData = RandomList.奇美Early;
                break;
            case MagicObjectHelper.Sheet奇美Advance:
                sheetData = RandomList.奇美Advance;
                break;
            case MagicObjectHelper.Sheet郭綜合Early:
                sheetData = RandomList.郭綜合Early;
                break;
            case MagicObjectHelper.Sheet郭綜合Advance:
                sheetData = RandomList.郭綜合Advance;
                break;
        }
        IWorksheet worksheet = workbook.Worksheets[sheetName];

        for (int i = 2; i < 2000; i++)
        {
            randomListItem = new();
            randomListItem.Id = worksheet.Range[$"A{i}"].DisplayText;
            randomListItem.BlockId = worksheet.Range[$"B{i}"].DisplayText;
            randomListItem.BlockSize = worksheet.Range[$"C{i}"].DisplayText;
            randomListItem.Treatment = worksheet.Range[$"D{i}"].DisplayText;
            randomListItem.StudyCode = worksheet.Range[$"E{i}"].DisplayText;

            if (string.IsNullOrEmpty(randomListItem.Id))
                continue;

            sheetData.Add(randomListItem);
        }
    }

    #endregion

    #region 更新 Study Code
    public void UpdateStudyCode(string sheetName, string id, string studyCode)
    {
        RandomListItem randomListItem = new();
        bool isUpdated = false;

        using (ExcelEngine excelEngine = new ExcelEngine())
        {
            //Instantiate the Excel application object
            IApplication application = excelEngine.Excel;

            //Assigns default application version
            application.DefaultVersion = ExcelVersion.Xlsx;

            string filenameRandomList = Path.Combine("Data", MagicObjectHelper.RandomListRuntimeFile);

            // 開啟並更新 Excel 內的資料
            using (FileStream sampleFile = new FileStream(filenameRandomList, FileMode.Open, FileAccess.ReadWrite))
            {
                IWorkbook workbook = application.Workbooks.Open(sampleFile);
                IWorksheet worksheet = workbook.Worksheets[sheetName];

                for (int i = 2; i < 2000; i++)
                {
                    randomListItem = new();
                    randomListItem.Id = worksheet.Range[$"A{i}"].DisplayText;
                    randomListItem.BlockId = worksheet.Range[$"B{i}"].DisplayText;
                    randomListItem.BlockSize = worksheet.Range[$"C{i}"].DisplayText;
                    randomListItem.Treatment = worksheet.Range[$"D{i}"].DisplayText;
                    randomListItem.StudyCode = worksheet.Range[$"E{i}"].DisplayText;

                    if (string.IsNullOrEmpty(randomListItem.Id))
                        continue;

                    if (randomListItem.Id == id)
                    {
                        randomListItem.StudyCode = studyCode;
                        worksheet.Range[$"E{i}"].Text = studyCode; // 更新 Study Code
                        isUpdated = true;
                        break;
                    }
                }

                if (isUpdated)
                {
                    workbook.SaveAs(sampleFile);
                }
            }
        }
    }
    #endregion

    #region 支援方法
    public string AssignRandomToStudyCode(RandomParameterMode randomParameterMode)
    {
        RandomListItem randomListItem = new();
        bool isUpdated = false;

        string sheetName = string.Empty;
        string FIGO = randomParameterMode.FIGO;
        string subjectNo = randomParameterMode.SubjectNo;
        string earlyOrAdvance = string.Empty;
        string hospital = string.Empty;
        if (string.IsNullOrEmpty(FIGO) == false)
        {
            if (FIGO.StartsWith("IIII") || FIGO.StartsWith("III"))
            {
                earlyOrAdvance = "Advance";
            }
            else if (FIGO.StartsWith("II") || FIGO.StartsWith("I"))
            {
                earlyOrAdvance = "Early";
            }
        }
        else
            return sheetName;

        #region 依據院別，產生出 Name
        string subjectNoPrefix = "";
        if (subjectNo.Contains(MagicObjectHelper.prefix奇美醫院))
        {
            hospital = MagicObjectHelper.PrefixSheetName奇美醫院;
        }
        else if (subjectNo.Contains(MagicObjectHelper.prefix郭綜合醫院))
        {
            hospital = MagicObjectHelper.PrefixSheetName郭綜合醫院;
        }
        else
        {
            hospital = MagicObjectHelper.PrefixSheetName成大醫院;
        }
        #endregion

        sheetName = $"{hospital}{earlyOrAdvance}";
        using (ExcelEngine excelEngine = new ExcelEngine())
        {
            //Instantiate the Excel application object
            IApplication application = excelEngine.Excel;

            //Assigns default application version
            application.DefaultVersion = ExcelVersion.Xlsx;

            string filenameRandomList = Path.Combine("Data", MagicObjectHelper.RandomListRuntimeFile);


            using (FileStream sampleFile = new FileStream(filenameRandomList,
                FileMode.Open, FileAccess.ReadWrite))
            {
                IWorkbook workbook = application.Workbooks.Open(sampleFile);
                IWorksheet worksheet = workbook.Worksheets[sheetName];

                for (int i = 2; i < 2000; i++)
                {
                    randomListItem = new();
                    randomListItem.Id = worksheet.Range[$"A{i}"].DisplayText;
                    randomListItem.BlockId = worksheet.Range[$"B{i}"].DisplayText;
                    randomListItem.BlockSize = worksheet.Range[$"C{i}"].DisplayText;
                    randomListItem.Treatment = worksheet.Range[$"D{i}"].DisplayText;
                    randomListItem.StudyCode = worksheet.Range[$"E{i}"].DisplayText;

                    if (string.IsNullOrEmpty(randomListItem.StudyCode))
                    {
                        randomParameterMode.RandomId = randomListItem.Id;
                        worksheet.Range[$"E{i}"].Text = subjectNo; // 更新 Study Code
                        isUpdated = true;
                        break;
                    }

                }

                if (isUpdated)
                {
                    workbook.SaveAs(sampleFile);
                }

            }
        }


        return sheetName;
    }
    #endregion
}
