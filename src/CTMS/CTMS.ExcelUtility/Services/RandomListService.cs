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
                    MagicObjectHelper.prefix成大醫院, MagicObjectHelper.RandomEarly);
                ReadSheet(workbook, MagicObjectHelper.Sheet成大Advance,
                    MagicObjectHelper.prefix成大醫院, MagicObjectHelper.RandomAdvance);
                ReadSheet(workbook, MagicObjectHelper.Sheet奇美Early,
                    MagicObjectHelper.prefix奇美醫院, MagicObjectHelper.RandomEarly);
                ReadSheet(workbook, MagicObjectHelper.Sheet奇美Advance,
                    MagicObjectHelper.prefix奇美醫院, MagicObjectHelper.RandomAdvance);
                ReadSheet(workbook, MagicObjectHelper.Sheet郭綜合Early,
                    MagicObjectHelper.prefix郭綜合醫院, MagicObjectHelper.RandomEarly);
                ReadSheet(workbook, MagicObjectHelper.Sheet郭綜合Advance,
                    MagicObjectHelper.prefix郭綜合醫院, MagicObjectHelper.RandomAdvance);
            }
        }
    }

    private void ReadSheet(IWorkbook workbook, string sheetName,
        string hospital, string EarlyOrAdvance)
    {
        RandomListItem randomListItem1 = new();
        RandomListItem randomListItem2 = new();
        IWorksheet worksheet = workbook.Worksheets[sheetName];

        for (int i = 2; i < 2000; i++)
        {
            randomListItem1 = new();
            randomListItem1.Hospital = hospital;
            randomListItem1.ECorOC = MagicObjectHelper.EC;
            randomListItem1.EarlyOrAdvance = EarlyOrAdvance;
            randomListItem1.Id = worksheet.Range[$"A{i}"].DisplayText;
            randomListItem1.BlockId = worksheet.Range[$"B{i}"].DisplayText;
            randomListItem1.BlockSize = worksheet.Range[$"C{i}"].DisplayText;
            randomListItem1.Treatment = worksheet.Range[$"D{i}"].DisplayText;
            randomListItem1.SubjectNo = worksheet.Range[$"E{i}"].DisplayText;

            randomListItem2 = randomListItem1.Clone();
            randomListItem2.ECorOC = MagicObjectHelper.OC;

            if (string.IsNullOrEmpty(randomListItem1.Id))
                continue;

            RandomList.Items.AddRange(randomListItem1, randomListItem2);
        }
    }

    #endregion

    #region 維護 Subject No
    public async Task<string> AssignRandomToStudyCodeAsync(RandomParameterMode randomParameterModeBefore,
           RandomParameterMode randomParameterModeAfter)
    {
        await RandomList.ReadAsync();
        string result = MagicObjectHelper.NA;

        if(randomParameterModeAfter.EarlyOrAdvance == "" ||
            randomParameterModeAfter.ECorOC == "")
            return result;

        if (randomParameterModeBefore.EarlyOrAdvance != 
            randomParameterModeAfter.EarlyOrAdvance)
        {
            var foundOldItem = RandomList.Items
                .FirstOrDefault(x => 
                x.SubjectNo == randomParameterModeBefore.SubjectNo);
            if (foundOldItem != null)
            {
                // 有被用過的亂數碼，標記為已使用，不會重覆再使用
                foundOldItem.SubjectNo = $"**{foundOldItem.SubjectNo}";
            }

            var foundNewItem = RandomList.Items
                .Where(x => 
                x.Hospital == randomParameterModeAfter.Hospital &&
                x.ECorOC == randomParameterModeAfter.ECorOC &&
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
