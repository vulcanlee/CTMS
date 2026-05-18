using CTMS.Business.Helpers;
using CTMS.DataModel.Models.Apis;
using CTMS.DataModel.Models.ClinicalInformation;
using CTMS.Share.Extensions;
using CTMS.Share.Helpers;
namespace CTMS.Business.Services.ClinicalInformation;

public class BloodExameService
{
    private readonly SubjectNoHelper subjectNoHelper;

    public BloodExameService(SubjectNoHelper subjectNoHelper)
    {
        this.subjectNoHelper = subjectNoHelper;
    }
    public void Read血液Node(BloodTest抽血檢驗血液Node bloodTest,
        string subjectNo)
    {
        string filename = subjectNoHelper
            .GetBloodFilename(subjectNo, MagicObjectHelper.Blood抽血檢驗血液);
        bloodTest.抽血檢驗血液 = ReadFile(filename);
    }

    public void Read生化Node(BloodTest抽血檢驗生化Node bloodTest,
        string subjectNo)
    {
        string filename = subjectNoHelper
      .GetBloodFilename(subjectNo, MagicObjectHelper.Blood抽血檢驗生化);
        bloodTest.抽血檢驗生化 = ReadFile(filename);
    }

    public void MatchApiBloodResult(List<TestItem檢驗項目>? bloodExame, List<BloodApiModel> apiResult)
    {
        if (apiResult == null)
        {
            return;
        }

        int apiResultCount = apiResult.Count;
        //for (int i = apiResultCount - 1; i >= 0; i--)
        for (int i = 0; i < apiResultCount; i++)
        {
            BloodApiModel bloodApiModelItem = apiResult[i];
            var itemName = bloodApiModelItem.ItemName;
            TestItem檢驗項目 foundItem = bloodExame?.FirstOrDefault(x => x.項目名稱.ToLower().Contains(itemName.ToLower()));
            if (foundItem != null)
            {
                foundItem.檢驗數值 = bloodApiModelItem.TestValue;
                foundItem.單位 = bloodApiModelItem.Unit;
                foundItem.參考區間 = bloodApiModelItem.RefData;
                DateTime executeTime;
                if (DateTime.TryParse(bloodApiModelItem.ExecuteTime, out executeTime))
                {
                    foundItem.SamplingDate = executeTime;
                }
            }
            else
            {
            }
        }

        CheckBloodExame(bloodExame ?? new List<TestItem檢驗項目>());
    }

    public void MatchApiChemistrydResult(List<TestItem檢驗項目>? bloodExame, List<BloodApiModel> apiResult)
    {
        if (apiResult == null)
        {
            return;
        }

        int apiResultCount = apiResult.Count;
        TestItem檢驗項目 foundItem;
        for (int i = apiResultCount - 1; i >= 0; i--)
        {
            BloodApiModel bloodApiModelItem = apiResult[i];
            var itemName = bloodApiModelItem.ItemName;
            foundItem = null;
            if (itemName.ToLower() == "k".ToLower() || itemName.ToLower() == "na".ToLower())
            {
                foundItem = bloodExame?.FirstOrDefault(x => x.項目名稱.ToLower() == itemName.ToLower());
            }
            else if (itemName.ToLower() == "ca 125".ToLower())
            {
                foundItem = bloodExame?.FirstOrDefault(x => x.項目名稱.ToLower().Contains("CA125".ToLower()));
            }
            else if (itemName.ToLower() == "ca 199".ToLower())
            {
                foundItem = bloodExame?.FirstOrDefault(x => x.項目名稱.ToLower().Contains("CA19-9".ToLower()));
            }
            else if ((bloodApiModelItem.OrderName.ToLower() == "glu random".ToLower()) ||
                (bloodApiModelItem.OrderName.ToLower() == "glu ac".ToLower()) ||
                (bloodApiModelItem.OrderName.ToLower() == "glu pc".ToLower()))
            {
                foundItem = bloodExame?.FirstOrDefault(x => x.項目名稱.ToLower().Contains("glucose".ToLower()));
            }
            else
            {
                if(itemName.ToLower() == "HbA1c".ToLower())
                {
                    var bar = 0;
                }
                foundItem = bloodExame?.FirstOrDefault(x => x.項目名稱.ToLower().Contains(bloodApiModelItem.OrderName.ToLower()));
            }

            if (foundItem != null)
            {
                if(bloodExame.IndexOf(foundItem) ==5)
                {
                    var bar = 0;
                }   
                foundItem.檢驗數值 = bloodApiModelItem.TestValue;
                foundItem.單位 = bloodApiModelItem.Unit;
                foundItem.參考區間 = bloodApiModelItem.RefData;
                DateTime executeTime;
                if (DateTime.TryParse(bloodApiModelItem.ExecuteTime, out executeTime))
                {
                    foundItem.SamplingDate = executeTime;
                }
            }
            else
            {
            }
        }

        CheckBloodExame(bloodExame ?? new List<TestItem檢驗項目>());
    }

    public bool FillUnitsFor血液Node(BloodTest抽血檢驗血液Node bloodTest, string subjectNo)
    {
        return FillUnitsFromSubjectNo(bloodTest.抽血檢驗血液, subjectNo, MagicObjectHelper.Blood抽血檢驗血液);
    }

    public bool FillUnitsFor生化Node(BloodTest抽血檢驗生化Node bloodTest, string subjectNo)
    {
        return FillUnitsFromSubjectNo(bloodTest.抽血檢驗生化, subjectNo, MagicObjectHelper.Blood抽血檢驗生化);
    }

    public List<TestItem檢驗項目> ReadFile(string filename)
    {
        string path = Path.Combine("Data", filename);
        string content = File.ReadAllText(path);
        var bloodTest = Newtonsoft.Json.JsonConvert.DeserializeObject<List<TestItem檢驗項目>>(content);
        NormalizeBloodExameItems(bloodTest);
        return bloodTest;
    }

    public bool NormalizeBloodExameItems(List<TestItem檢驗項目>? bloodExame)
    {
        if (bloodExame == null)
        {
            return false;
        }

        var changed = false;
        foreach (var item in bloodExame)
        {
            var originalName = item.項目名稱 ?? string.Empty;
            var originalUnit = item.單位 ?? string.Empty;
            var (normalizedName, normalizedUnit) = BloodTestItemNameUnitHelper.Parse(originalName);
            if (string.IsNullOrEmpty(normalizedName) == false)
                item.項目名稱 = normalizedName;
            if (string.IsNullOrEmpty(normalizedUnit) == false)
                item.單位 = normalizedUnit;
            changed |= originalName != normalizedName || originalUnit != normalizedUnit;
        }

        return changed;
    }
    public bool FillUnitsFromSubjectNo(List<TestItem檢驗項目>? bloodExame, string subjectNo, string bloodType)
    {
        if (bloodExame == null || string.IsNullOrWhiteSpace(subjectNo))
        {
            return false;
        }

        string filename = subjectNoHelper.GetBloodFilename(subjectNo, bloodType);
        var sourceItems = ReadFile(filename);
        var sourceLookup = sourceItems.ToDictionary(x => x.項目名稱, x => x.單位 ?? string.Empty);

        var changed = false;
        foreach (var item in bloodExame)
        {
            var originalName = item.項目名稱 ?? string.Empty;
            var originalUnit = item.單位 ?? string.Empty;
            var (normalizedName, normalizedUnit) = BloodTestItemNameUnitHelper.Parse(originalName);
            item.項目名稱 = normalizedName;
            item.單位 = normalizedUnit;

            if (sourceLookup.TryGetValue(item.項目名稱, out var sourceUnit) &&
                string.IsNullOrWhiteSpace(item.單位))
            {
                item.單位 = sourceUnit;
            }

            changed |= originalName != item.項目名稱 || originalUnit != (item.單位 ?? string.Empty);
        }

        return changed;
    }

    public void CheckBloodExame(List<TestItem檢驗項目> bloodExame)
    {
        NormalizeBloodExameItems(bloodExame);

        // 3.4-9.5 參考區間類型 = MagicObjectHelper.參考區間類型_區間
        // <60 參考區間類型 = MagicObjectHelper.參考區間類型_小於
        // ≦50 參考區間類型 = MagicObjectHelper.參考區間類型_小於等於
        // >40 參考區間類型 = MagicObjectHelper.參考區間類型_大於
        // ≧50 參考區間類型 = MagicObjectHelper.參考區間類型_大於等於
        foreach (var item in bloodExame)
        {
            double value1 = 0;
            double value2 = 0;
            if (item.參考區間.Contains("<"))
            {
                value1 = item.參考區間.Replace("<", "").ToBloodDouble();
                value2 = item.參考區間.Replace("<", "").ToBloodDouble();
                item.參考區間類型 = MagicObjectHelper.參考區間類型_小於;
                item.參考區間開始 = value1;
                item.參考區間結束 = value2;
            }
            else if (item.參考區間.Contains("≦"))
            {
                value1 = item.參考區間.Replace("≦", "").ToBloodDouble();
                value2 = item.參考區間.Replace("≦", "").ToBloodDouble();
                item.參考區間類型 = MagicObjectHelper.參考區間類型_小於等於;
                item.參考區間開始 = value1;
                item.參考區間結束 = value2;
            }
            else if (item.參考區間.Contains(">"))
            {
                value1 = item.參考區間.Replace(">", "").ToBloodDouble();
                value2 = item.參考區間.Replace(">", "").ToBloodDouble();
                item.參考區間類型 = MagicObjectHelper.參考區間類型_大於;
                item.參考區間開始 = value1;
                item.參考區間結束 = value2;
            }
            else if (item.參考區間.Contains("≧"))
            {
                value1 = item.參考區間.Replace("≧", "").ToBloodDouble();
                value2 = item.參考區間.Replace("≧", "").ToBloodDouble();
                item.參考區間類型 = MagicObjectHelper.參考區間類型_大於等於;
                item.參考區間開始 = value1;
                item.參考區間結束 = value2;
            }
            else if (item.參考區間.Contains("-"))
            {
                value1 = item.參考區間.Split('-')[0].ToBloodDouble();
                value2 = item.參考區間.Split('-')[1].ToBloodDouble();
                item.參考區間類型 = MagicObjectHelper.參考區間類型_區間;
                item.參考區間開始 = value1;
                item.參考區間結束 = value2;
            }
        }

        計算血液檢查的異常(bloodExame);
    }

    void 計算血液檢查的異常(List<TestItem檢驗項目> Items)
    {
        foreach (var item in Items)
        {
            item.TextClassName = MagicObjectHelper.正常檢驗數計類別;
            if (string.IsNullOrEmpty(item.參考區間))
            {
                continue;
            }
            if (item.參考區間類型 == MagicObjectHelper.參考區間類型_區間)
            {
                if (string.IsNullOrEmpty(item.檢驗數值) == false &&
                    (item.檢驗數值.ToBloodDouble() < item.參考區間開始 ||
                    item.檢驗數值.ToDouble() > item.參考區間結束))
                {
                    item.TextClassName = MagicObjectHelper.異常檢驗數計類別;
                }
            }
            else if (item.參考區間類型 == MagicObjectHelper.參考區間類型_大於)
            {
                if (string.IsNullOrEmpty(item.檢驗數值) == false &&
                    (item.檢驗數值.ToBloodDouble() <= item.參考區間結束))
                {
                    item.TextClassName = MagicObjectHelper.異常檢驗數計類別;
                }
            }
            else if (item.參考區間類型 == MagicObjectHelper.參考區間類型_大於等於)
            {
                if (string.IsNullOrEmpty(item.檢驗數值) == false &&
                    (item.檢驗數值.ToBloodDouble() < item.參考區間結束))
                {
                    item.TextClassName = MagicObjectHelper.異常檢驗數計類別;
                }
            }
            else if (item.參考區間類型 == MagicObjectHelper.參考區間類型_小於)
            {
                if (string.IsNullOrEmpty(item.檢驗數值) == false &&
                    (item.檢驗數值.ToBloodDouble() >= item.參考區間開始))
                {
                    item.TextClassName = MagicObjectHelper.異常檢驗數計類別;
                }
            }
            else if (item.參考區間類型 == MagicObjectHelper.參考區間類型_小於等於)
            {
                if (string.IsNullOrEmpty(item.檢驗數值) == false &&
                    (item.檢驗數值.ToBloodDouble() > item.參考區間開始))
                {
                    item.TextClassName = MagicObjectHelper.異常檢驗數計類別;
                }
            }
        }
    }
}
