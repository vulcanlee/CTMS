using CTMS.DataModel.Models.ClinicalInformation;
using CTMS.ExcelUtility.Extensions;
using System.Diagnostics;

namespace CTMS.Business.Services.ClinicalInformation;

public class SideEffectsService
{
    string WhiteBloodCell白血球 = "白血球計數 WBC(10^3/μL)";
    string NeutrophilCount絕對嗜中性白血球數 = "";
    string HemoglobinHb血色素 = "血色素 Hb(g/dL)";
    string PlateletCount血小板 = "血小板 Plt (10^3/μL)";

    public void InitAll(HematologicSideEffects血液副作用Node hematologicSideEffects)
    {
        Init血液副作用WhiteBloodCell白血球(hematologicSideEffects);
        Init血液副作用NeutrophilCount絕對嗜中性白血球數(hematologicSideEffects);
        Init血液副作用HemoglobinHb血色素(hematologicSideEffects);
        Init血液副作用PlateletCount血小板(hematologicSideEffects);
    }

    public void Init血液副作用WhiteBloodCell白血球(HematologicSideEffects血液副作用Node hematologicSideEffects)
    {
        GradeItemSideEffectsItem Item副作用;
        Item副作用 = hematologicSideEffects.WhiteBloodCell白血球;
        Item副作用.Grade1.ResetCssClassNotFound();
        Item副作用.Grade1.Title = $"<LLN - 3000/mm3";
        Item副作用.Grade1.GradeValue1 = "3000";
        Item副作用.Grade1.GradeValue2 = "0";
        Item副作用.Grade2.ResetCssClassNotFound();
        Item副作用.Grade2.Title = $"<3000 - 2000/mm3";
        Item副作用.Grade2.GradeValue1 = "2000";
        Item副作用.Grade2.GradeValue2 = "3000";
        Item副作用.Grade3.ResetCssClassNotFound();
        Item副作用.Grade3.Title = $"<2000 - 1000/mm3";
        Item副作用.Grade3.GradeValue1 = "1000";
        Item副作用.Grade3.GradeValue2 = "2000";
        Item副作用.Grade4.ResetCssClassNotFound();
        Item副作用.Grade4.Title = $"<1000";
        Item副作用.Grade4.GradeValue1 = "0";
        Item副作用.Grade4.GradeValue2 = "1000";
        Item副作用.Grade5.ResetCssClassNotFound();
        Item副作用.Grade5.Title = $"-";
        Item副作用.Grade5.GradeValue1 = "0";
        Item副作用.Grade5.GradeValue2 = "0";
        Item副作用.RetriveValue = string.Empty;

    }

    public void Init血液副作用NeutrophilCount絕對嗜中性白血球數(HematologicSideEffects血液副作用Node hematologicSideEffects)
    {
        GradeItemSideEffectsItem Item副作用;
        Item副作用 = hematologicSideEffects.NeutrophilCount絕對嗜中性白血球數;
        Item副作用.Grade1.ResetCssClassNotFound();
        Item副作用.Grade1.Title = $"<LLN - 1500/mm3";
        Item副作用.Grade1.GradeValue1 = "1500";
        Item副作用.Grade1.GradeValue2 = "0";
        Item副作用.Grade2.ResetCssClassNotFound();
        Item副作用.Grade2.Title = $"<1500 - 1000/mm3";
        Item副作用.Grade2.GradeValue1 = "1000";
        Item副作用.Grade2.GradeValue2 = "1500";
        Item副作用.Grade3.ResetCssClassNotFound();
        Item副作用.Grade3.Title = $"<1000 - 500/mm3";
        Item副作用.Grade3.GradeValue1 = "500";
        Item副作用.Grade3.GradeValue2 = "1000";
        Item副作用.Grade4.ResetCssClassNotFound();
        Item副作用.Grade4.Title = $"<500";
        Item副作用.Grade4.GradeValue1 = "0";
        Item副作用.Grade4.GradeValue2 = "500";
        Item副作用.Grade5.ResetCssClassNotFound();
        Item副作用.Grade5.Title = $"-";
        Item副作用.Grade5.GradeValue1 = "0";
        Item副作用.Grade5.GradeValue2 = "0";
        Item副作用.RetriveValue = string.Empty;

    }

    public void Init血液副作用HemoglobinHb血色素(HematologicSideEffects血液副作用Node hematologicSideEffects)
    {
        GradeItemSideEffectsItem Item副作用;
        Item副作用 = hematologicSideEffects.HemoglobinHb血色素;
        Item副作用.Grade1.ResetCssClassNotFound();
        Item副作用.Grade1.Title = $" <LLN - 10.0 g/dL";
        Item副作用.Grade1.GradeValue1 = "10.0";
        Item副作用.Grade1.GradeValue2 = "0";
        Item副作用.Grade2.ResetCssClassNotFound();
        Item副作用.Grade2.Title = $"Hgb <10.0 - 8.0 g/dL";
        Item副作用.Grade2.GradeValue1 = "8.0";
        Item副作用.Grade2.GradeValue2 = "10.0";
        Item副作用.Grade3.ResetCssClassNotFound();
        Item副作用.Grade3.Title = $"Hgb <8.0 g/dL";
        Item副作用.Grade3.GradeValue1 = "0";
        Item副作用.Grade3.GradeValue2 = "8.0";
        Item副作用.Grade4.ResetCssClassNotFound();
        Item副作用.Grade4.Title = $"-";
        Item副作用.Grade4.GradeValue1 = "0";
        Item副作用.Grade4.GradeValue2 = "0";
        Item副作用.Grade5.ResetCssClassNotFound();
        Item副作用.Grade5.Title = $"Death";
        Item副作用.Grade5.GradeValue1 = "0";
        Item副作用.Grade5.GradeValue2 = "0";
        Item副作用.RetriveValue = string.Empty;

    }

    public void Init血液副作用PlateletCount血小板(HematologicSideEffects血液副作用Node hematologicSideEffects)
    {
        GradeItemSideEffectsItem Item副作用;
        Item副作用 = hematologicSideEffects.PlateletCount血小板;
        Item副作用.Grade1.ResetCssClassNotFound();
        Item副作用.Grade1.Title = $"<LLN - 75,000/mm3";
        Item副作用.Grade1.GradeValue1 = "75000";
        Item副作用.Grade1.GradeValue2 = "0";
        Item副作用.Grade2.ResetCssClassNotFound();
        Item副作用.Grade2.Title = $"<75,000 - 50,000/mm3 ";
        Item副作用.Grade2.GradeValue1 = "50000";
        Item副作用.Grade2.GradeValue2 = "75000";
        Item副作用.Grade3.ResetCssClassNotFound();
        Item副作用.Grade3.Title = $"<50,000 - 25,000/mm3 ";
        Item副作用.Grade3.GradeValue1 = "25000";
        Item副作用.Grade3.GradeValue2 = "50000";
        Item副作用.Grade4.ResetCssClassNotFound();
        Item副作用.Grade4.Title = $"<25,000/mm3";
        Item副作用.Grade4.GradeValue1 = "0";
        Item副作用.Grade4.GradeValue2 = "25000";
        Item副作用.Grade5.ResetCssClassNotFound();
        Item副作用.Grade5.Title = $"-";
        Item副作用.Grade5.GradeValue1 = "0";
        Item副作用.Grade5.GradeValue2 = "0";
        Item副作用.RetriveValue = string.Empty;

    }

    public void Update副作用All(Main臨床資料 main臨床資料,
        HematologicSideEffects血液副作用Node hematologicSideEffects)
    {
        Update副作用WhiteBloodCell白血球(main臨床資料, hematologicSideEffects);
        Update副作用HemoglobinHb血色素(main臨床資料, hematologicSideEffects);
        Update副作用PlateletCount血小板(main臨床資料, hematologicSideEffects);
    }

    public void Update副作用WhiteBloodCell白血球(Main臨床資料 main臨床資料,
        HematologicSideEffects血液副作用Node hematologicSideEffects)
    {
        var visitCodeTitle = hematologicSideEffects.VisitCode.VisitCodeTitle;
        BloodTest抽血檢驗血液Node bloodTest = main臨床資料.抽血檢驗血液
            .Items.FirstOrDefault(x => x.VisitCode.VisitCodeTitle == visitCodeTitle);
        if (bloodTest == null)
            return;

        TestItem檢驗項目 testItem = bloodTest.抽血檢驗血液
            .FirstOrDefault(x => x.項目名稱 == WhiteBloodCell白血球);

        GradeItemSideEffectsItem sideEffectsItem =
                hematologicSideEffects.WhiteBloodCell白血球;

        var 參考區間開始 = testItem.參考區間開始 * 1000.0;
        var grade = sideEffectsItem.Grade1;
        grade.GradeValue2 = (參考區間開始).ToString();
        sideEffectsItem.RetriveValue = (testItem.檢驗數值.ToDouble() * 1000.0).ToString();

        ComputeGrade(main臨床資料, sideEffectsItem, bloodTest, testItem);
    }

    public void Update副作用HemoglobinHb血色素(Main臨床資料 main臨床資料,
        HematologicSideEffects血液副作用Node hematologicSideEffects)
    {
        var visitCodeTitle = hematologicSideEffects.VisitCode.VisitCodeTitle;
        BloodTest抽血檢驗血液Node bloodTest = main臨床資料.抽血檢驗血液
            .Items.FirstOrDefault(x => x.VisitCode.VisitCodeTitle == visitCodeTitle);
        if (bloodTest == null)
            return;

        TestItem檢驗項目 testItem = bloodTest.抽血檢驗血液
            .FirstOrDefault(x => x.項目名稱 == HemoglobinHb血色素);

        GradeItemSideEffectsItem sideEffectsItem =
                hematologicSideEffects.HemoglobinHb血色素;

        testItem.參考區間開始 = testItem.參考區間開始;
        var grade = sideEffectsItem.Grade1;
        grade.GradeValue2 = (testItem.參考區間開始).ToString();
        sideEffectsItem.RetriveValue = (testItem.檢驗數值.ToDouble()).ToString();

        ComputeGrade(main臨床資料, sideEffectsItem, bloodTest, testItem);
    }

    public void Update副作用PlateletCount血小板(Main臨床資料 main臨床資料,
        HematologicSideEffects血液副作用Node hematologicSideEffects)
    {
        var visitCodeTitle = hematologicSideEffects.VisitCode.VisitCodeTitle;
        BloodTest抽血檢驗血液Node bloodTest = main臨床資料.抽血檢驗血液
            .Items.FirstOrDefault(x => x.VisitCode.VisitCodeTitle == visitCodeTitle);
        if (bloodTest == null)
            return;

        TestItem檢驗項目 testItem = bloodTest.抽血檢驗血液
            .FirstOrDefault(x => x.項目名稱 == PlateletCount血小板);

        GradeItemSideEffectsItem sideEffectsItem =
                hematologicSideEffects.PlateletCount血小板;

        var 參考區間開始 = testItem.參考區間開始 * 1000.0;
        var grade = sideEffectsItem.Grade1;
        grade.GradeValue2 = (參考區間開始).ToString();
        sideEffectsItem.RetriveValue = (testItem.檢驗數值.ToDouble() * 1000.0).ToString();

        ComputeGrade(main臨床資料, sideEffectsItem, bloodTest, testItem);
    }

    public void ComputeGrade(Main臨床資料 main臨床資料,
        GradeItemSideEffectsItem sideEffectsItem,
        BloodTest抽血檢驗血液Node bloodTest,
        TestItem檢驗項目 testItem)
    {
        GradeItem grade;

        #region 進行副作用檢查
        if (testItem != null)
        {
            //sideEffectsItem.RetriveValue = (testItem.檢驗數值.ToDouble()*1000.0).ToString();

            #region 設定 LLN
            //grade = sideEffectsItem.Grade1;
            //grade.GradeValue2 = (testItem.參考區間開始*1000.0).ToString();
            #endregion

            #region Reset CssClass
            sideEffectsItem.Grade1.ResetCssClassNotFound();
            sideEffectsItem.Grade2.ResetCssClassNotFound();
            sideEffectsItem.Grade3.ResetCssClassNotFound();
            sideEffectsItem.Grade4.ResetCssClassNotFound();
            sideEffectsItem.Grade5.ResetCssClassNotFound();
            #endregion

            #region 判斷副作用等級
            if (sideEffectsItem.Grade1.GradeValue1.ToDouble() <=
                sideEffectsItem.RetriveValue.ToDouble() &&
                sideEffectsItem.RetriveValue.ToDouble() <=
                sideEffectsItem.Grade1.GradeValue2.ToDouble())
                sideEffectsItem.Grade1.ResetCssClassFound();

            if (sideEffectsItem.Grade2.GradeValue1.ToDouble() <=
                sideEffectsItem.RetriveValue.ToDouble() &&
                sideEffectsItem.RetriveValue.ToDouble() <=
                sideEffectsItem.Grade2.GradeValue2.ToDouble())
                sideEffectsItem.Grade2.ResetCssClassFound();

            if (sideEffectsItem.Grade3.GradeValue1.ToDouble() <=
                sideEffectsItem.RetriveValue.ToDouble() &&
                sideEffectsItem.RetriveValue.ToDouble() <=
                sideEffectsItem.Grade3.GradeValue2.ToDouble())
                sideEffectsItem.Grade3.ResetCssClassFound();

            if (sideEffectsItem.Grade4.GradeValue1.ToDouble() <=
                sideEffectsItem.RetriveValue.ToDouble() &&
                sideEffectsItem.RetriveValue.ToDouble() <=
                sideEffectsItem.Grade4.GradeValue2.ToDouble())
                sideEffectsItem.Grade4.ResetCssClassFound();

            if (sideEffectsItem.Grade5.GradeValue1.ToDouble() <=
                sideEffectsItem.RetriveValue.ToDouble() &&
                sideEffectsItem.RetriveValue.ToDouble() <=
                sideEffectsItem.Grade5.GradeValue2.ToDouble())
                sideEffectsItem.Grade5.ResetCssClassFound();

            #endregion
        }
        #endregion
    }
}
