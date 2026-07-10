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
        string filenameRuntime = Path.Combine("Data", MagicObjectHelper.RandomListRuntimeJsonFile);

        if (File.Exists(filenameRuntime) == false)
        {
            ReadExcel();
            await RandomList.SaveAsync();
        }
        else
        {
            await RandomList.ReadAsync();

            // 自癒式補讀：runtime 快取建立後才新增的醫院，只從 xlsx 補讀該院分頁並附加，
            // 不動既有醫院已落地的隨機分配結果（不可刪快取重建，會清空已分配的隨機碼）。
            var missingOwners = HospitalRegistry.PrefixOwners
                .Where(o => RandomList.Items.Any(i => i.Hospital == o.Prefix) == false)
                .ToList();
            if (missingOwners.Count > 0)
            {
                ReadExcel(missingOwners);
                await RandomList.SaveAsync();
            }
        }
    }

    /// <summary>
    /// 將指定 prefix 擁有者的隨機清單以 xlsx 分頁「移除後重建」，其餘醫院不受影響。
    /// 用於維護：高榮/嘉長隨機表比照奇美完全複製後，強制以更新後的 xlsx 覆蓋 runtime
    /// （自癒補讀只補「完全缺少」的院，無法覆蓋已存在的舊資料，故需此方法）。
    /// 注意：會清掉被指定院既有的 SubjectNo 配號，僅適用於尚未配號的院別。
    /// 回傳重建後這些院的項目筆數。
    /// </summary>
    public async Task<int> RebuildOwnersFromExcelAsync(params string[] prefixes)
    {
        await RandomList.ReadAsync();
        var set = new HashSet<string>(prefixes);
        RandomList.Items.RemoveAll(i => set.Contains(i.Hospital));
        var owners = HospitalRegistry.PrefixOwners.Where(o => set.Contains(o.Prefix)).ToList();
        ReadExcel(owners);
        await RandomList.SaveAsync();
        return RandomList.Items.Count(i => set.Contains(i.Hospital));
    }

    public void ReadExcel()
    {
        ReadExcel(HospitalRegistry.PrefixOwners.ToList());
    }

    private void ReadExcel(IReadOnlyCollection<HospitalDefinition> owners)
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

                // 尚未提供隨機表分頁的醫院（已加入 HospitalRegistry 但還沒補上 xlsx 分頁）直接略過，
                // 避免單一缺頁丟例外而中斷整份讀取與 App 啟動；日後補上該分頁即自動生效。
                var existingSheets = new HashSet<string>();
                foreach (IWorksheet sheet in workbook.Worksheets)
                {
                    existingSheets.Add(sheet.Name);
                }

                foreach (var owner in owners)
                {
                    ReadSheetIfExists(workbook, existingSheets, owner.SheetEarly, owner.Prefix, MagicObjectHelper.RandomEarly);
                    ReadSheetIfExists(workbook, existingSheets, owner.SheetAdvance, owner.Prefix, MagicObjectHelper.RandomAdvance);
                }
            }
        }
    }

    private void ReadSheetIfExists(IWorkbook workbook, HashSet<string> existingSheets,
        string sheetName, string hospital, string EarlyOrAdvance)
    {
        if (existingSheets.Contains(sheetName) == false)
        {
            System.Diagnostics.Trace.TraceWarning(
                $"RandomList.xlsx 缺少分頁「{sheetName}」（{hospital}），已略過；補上該分頁後隨機表即自動生效。");
            return;
        }

        ReadSheet(workbook, sheetName, hospital, EarlyOrAdvance);
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

        if (randomParameterModeAfter.EarlyOrAdvance == "" ||
            randomParameterModeAfter.ECorOC == "")
            return result;

        if ((randomParameterModeBefore.EarlyOrAdvance != randomParameterModeAfter.EarlyOrAdvance) ||
            (randomParameterModeBefore.ECorOC != randomParameterModeAfter.ECorOC))
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

    public async Task<string> ReComputeRandomListAsync(RandomParameterMode parameterMode)
    {
        await RandomList.ReadAsync();
        string result = MagicObjectHelper.NA;

        if (parameterMode.EarlyOrAdvance == "" ||
            parameterMode.ECorOC == "")
            return result;

        {
            var foundOldItem = RandomList.Items
                .FirstOrDefault(x =>
                x.SubjectNo == parameterMode.SubjectNo);
            if (foundOldItem != null)
            {
                result = foundOldItem.Treatment;
                return result;
            }

            var foundNewItem = RandomList.Items
                .Where(x =>
                x.Hospital == parameterMode.Hospital &&
                x.ECorOC == parameterMode.ECorOC &&
                x.EarlyOrAdvance == parameterMode.EarlyOrAdvance &&
                x.SubjectNo == "")
                .OrderBy(x => x.Id)
                .FirstOrDefault();
            if (foundNewItem != null)
            {
                foundNewItem.SubjectNo = parameterMode.SubjectNo;
                result = foundNewItem.Treatment;
            }
            await RandomList.SaveAsync();
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
        return;
    }

    public async Task SaveAsync(List<RandomListItem> items)
    {
        await RandomList.ReadAsync();

        foreach (var item in items) {
            var foundItem = RandomList.Items
                .FirstOrDefault(x => x.Id == item.Id &&
                x.Hospital == item.Hospital &&
                x.ECorOC == item.ECorOC &&
                x.EarlyOrAdvance == item.EarlyOrAdvance);
            if (foundItem != null)
            {
                foundItem.SubjectNo = item.SubjectNo;
            }
        }
        await RandomList.SaveAsync();
        return;
    }

    #endregion
}
