using CTMS.DataModel.Models.ClinicalInformation;
using CTMS.Share.Helpers;
using Syncfusion.XlsIO;
using System.Threading.Tasks;

namespace SyncExcel.Services;

public class RandomListService
{
    public RandomListModel RandomList { get; set; } = new();

    #region 讀取 Excel 內的資料
    public async Task InitialAsync()
    {
        string filenameDefault = Path.Combine("Data", MagicObjectHelper.RandomListDefaultFile);
        string filenameRuntime = Path.Combine("Data", MagicObjectHelper.RandomListRuntimeJsonFile);

        if (File.Exists(filenameRuntime) == false)
        {
            ReadExcel();
            await RandomList.SaveAsync();
        }
        else
        {
            await RandomList.ReadAsync();
        }
    }

    public void ReadExcel()
    {
        using (ExcelEngine excelEngine = new ExcelEngine())
        {
            //Instantiate the Excel application object
            IApplication application = excelEngine.Excel;

            //Assigns default application version
            application.DefaultVersion = ExcelVersion.Xlsx;

            string filenameRandomList = Path.Combine("Data", MagicObjectHelper.RandomListDefaultFile);

            using (FileStream sampleFile = new FileStream(filenameRandomList, FileMode.Open))
            {
                IWorkbook workbook = application.Workbooks.Open(sampleFile);

                ReadSheet(workbook, MagicObjectHelper.Sheet成大Early,
                    MagicObjectHelper.PrefixSheetName成大醫院, MagicObjectHelper.RandomEarly);
                ReadSheet(workbook, MagicObjectHelper.Sheet成大Advance,
                    MagicObjectHelper.PrefixSheetName成大醫院, MagicObjectHelper.RandomAdvance);
                ReadSheet(workbook, MagicObjectHelper.Sheet奇美Early,
                    MagicObjectHelper.PrefixSheetName奇美醫院, MagicObjectHelper.RandomEarly);
                ReadSheet(workbook, MagicObjectHelper.Sheet奇美Advance,
                    MagicObjectHelper.PrefixSheetName奇美醫院, MagicObjectHelper.RandomAdvance);
                ReadSheet(workbook, MagicObjectHelper.Sheet郭綜合Early,
                    MagicObjectHelper.PrefixSheetName郭綜合醫院, MagicObjectHelper.RandomEarly);
                ReadSheet(workbook, MagicObjectHelper.Sheet郭綜合Advance,
                    MagicObjectHelper.PrefixSheetName郭綜合醫院, MagicObjectHelper.RandomAdvance);
            }
        }
    }

    private void ReadSheet(IWorkbook workbook, string sheetName,
        string hospital, string EarlyOrAdvance)
    {
        RandomListItem randomListItem = new();
        IWorksheet worksheet = workbook.Worksheets[sheetName];

        for (int i = 2; i < 2000; i++)
        {
            randomListItem = new();
            randomListItem.Hospital = hospital;
            randomListItem.EarlyOrAdvance = EarlyOrAdvance;
            randomListItem.Id = worksheet.Range[$"A{i}"].DisplayText;
            randomListItem.BlockId = worksheet.Range[$"B{i}"].DisplayText;
            randomListItem.BlockSize = worksheet.Range[$"C{i}"].DisplayText;
            randomListItem.Treatment = worksheet.Range[$"D{i}"].DisplayText;
            randomListItem.SubjectNo = worksheet.Range[$"E{i}"].DisplayText;

            if (string.IsNullOrEmpty(randomListItem.Id))
                continue;

            RandomList.Items.Add(randomListItem);
        }
    }

    #endregion

    #region 維護 Study Code
    public async Task<string> AssignRandomToStudyCodeAsync(RandomParameterMode randomParameterModeBefore,
           RandomParameterMode randomParameterModeAfter)
    {
        await RandomList.ReadAsync();
        string result = string.Empty;
        if (randomParameterModeBefore.EarlyOrAdvance != randomParameterModeAfter.EarlyOrAdvance)
        {
            var foundOldItem = RandomList.Items
                .FirstOrDefault(x => x.SubjectNo == randomParameterModeBefore.SubjectNo);
            if (foundOldItem != null)
            {
                foundOldItem.SubjectNo = string.Empty;
            }

            var foundNewItem = RandomList.Items
                .Where(x => x.Hospital == randomParameterModeAfter.Hospital &&
                x.EarlyOrAdvance == randomParameterModeAfter.EarlyOrAdvance &&
                x.SubjectNo == "")
                .OrderBy(x => x.Id)
                .FirstOrDefault();
            if (foundNewItem != null)
            {
                foundNewItem.SubjectNo = randomParameterModeAfter.SubjectNo;
                result = foundNewItem.Treatment;
            }
            await RandomList.SaveAsync();
        }
        else
        {
            var foundItem = RandomList.Items
                .FirstOrDefault(x => x.SubjectNo == randomParameterModeAfter.SubjectNo);
            if (foundItem != null)
            {
                result = foundItem.Treatment;
            }
        }
        return result;
    }

    public async Task RemoveAsync(RandomParameterMode randomParameterModeAfter)
    {
        await RandomList.ReadAsync();
        string result = string.Empty;
        if (randomParameterModeAfter.EarlyOrAdvance != "")
        {
            var foundOldItem = RandomList.Items
                .FirstOrDefault(x => x.SubjectNo == randomParameterModeAfter.SubjectNo);
            if (foundOldItem != null)
            {
                foundOldItem.SubjectNo = string.Empty;
                await RandomList.SaveAsync();
            }
        }
        return ;
    }

    #endregion
}
