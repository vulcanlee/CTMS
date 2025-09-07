using CTMS.Business.Helpers;
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

    public List<TestItem檢驗項目> ReadFile(string filename)
    {
        string path = Path.Combine("Data", filename);
        string content = File.ReadAllText(path);
        var bloodTest = Newtonsoft.Json.JsonConvert.DeserializeObject<List<TestItem檢驗項目>>(content);
        return bloodTest;
    }

    public void CheckBloodExame(List<TestItem檢驗項目> bloodExame)
    {
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
