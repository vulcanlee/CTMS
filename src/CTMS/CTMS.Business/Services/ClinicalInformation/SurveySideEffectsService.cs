using CTMS.DataModel.Models.ClinicalInformation;
using CTMS.ExcelUtility.Extensions;
using System.Diagnostics;

namespace CTMS.Business.Services.ClinicalInformation;

public class SurveySideEffectsService
{
    string 請問您是否感到噁心 = "請問您是否感到噁心";
    string 請問您是否有嘔吐情形 = "請問您是否有嘔吐情形";
    string 請問您是否有發生口腔黏膜炎 = "請問您是否有發生口腔黏膜炎";
    string 請問您是否有腹瀉情形 = "請問您是否有腹瀉情形";
    string 請問您是否有便秘情形 = "請問您是否有便秘情形";
    string 請問您是否有食慾不振 = "請問您是否有食慾不振情形";

    string 請問您是否有食慾請問您是否有發生周邊感覺神經異常情形不振 = "請問您是否有發生周邊感覺神經異常情形";
    string 請問您是否會感到疲倦 = "請問您是否會感到疲倦";
    string 請問您是否有發生紅疹情形 = "請問您是否有發生紅疹情形";
    string 請問您是否有手足症候群情形 = "請問您是否有手足症候群情形";
    string 請問您是否有掉髮情形 = "請問您是否有掉髮情形";

    #region Init 1
    public void Init1All(Survey1SideEffects副作用Node hematologicSideEffects)
    {
        Init副作用Nausea噁心(hematologicSideEffects);
        Init副作用Vomiting嘔吐(hematologicSideEffects);
        Init副作用MucositisOral口腔炎(hematologicSideEffects);
        Init副作用Diarrhea拉肚子(hematologicSideEffects);
        Init副作用Constipation便秘(hematologicSideEffects);
        Init副作用Anorexia食慾不振(hematologicSideEffects);
    }

    public void Init副作用Nausea噁心(Survey1SideEffects副作用Node hematologicSideEffects)
    {
        GradeItemSideEffectsItem Item副作用;
        Item副作用 = hematologicSideEffects.Nausea噁心;
        Item副作用.Grade1.ResetCssClassNotFound();
        Item副作用.Grade1.Title = $"有噁心感，但飲食習慣未改變";
        Item副作用.Grade1.GradeValue1 = "";
        Item副作用.Grade1.GradeValue2 = "";
        Item副作用.Grade2.ResetCssClassNotFound();
        Item副作用.Grade2.Title = $"由於噁心感，飲食量下降，但體重沒有變輕或脫水";
        Item副作用.Grade2.GradeValue1 = "";
        Item副作用.Grade2.GradeValue2 = "";
        Item副作用.Grade3.ResetCssClassNotFound();
        Item副作用.Grade3.Title = $"由於噁心感，導致飲食及飲水量大量減少，被給予靜脈輸液或住院治療";
        Item副作用.Grade3.GradeValue1 = "";
        Item副作用.Grade3.GradeValue2 = "";
        Item副作用.Grade4.ResetCssClassNotFound();
        Item副作用.Grade4.Title = $"";
        Item副作用.Grade4.GradeValue1 = "";
        Item副作用.Grade4.GradeValue2 = "";
        Item副作用.Grade5.ResetCssClassNotFound();
        Item副作用.Grade5.Title = $"";
        Item副作用.Grade5.GradeValue1 = "";
        Item副作用.Grade5.GradeValue2 = "";
        Item副作用.RetriveValue = string.Empty;
    }

    public void Init副作用Vomiting嘔吐(Survey1SideEffects副作用Node hematologicSideEffects)
    {
        GradeItemSideEffectsItem Item副作用;
        Item副作用 = hematologicSideEffects.Vomiting嘔吐;
        Item副作用.Grade1.ResetCssClassNotFound();
        Item副作用.Grade1.Title = $"一天之中發生1~2次嘔吐(每次間隔超過5分鐘)";
        Item副作用.Grade1.GradeValue1 = "";
        Item副作用.Grade1.GradeValue2 = "";
        Item副作用.Grade2.ResetCssClassNotFound();
        Item副作用.Grade2.Title = $"一天之中發生3~5次嘔吐(每次間隔超過5分鐘)";
        Item副作用.Grade2.GradeValue1 = "";
        Item副作用.Grade2.GradeValue2 = "";
        Item副作用.Grade3.ResetCssClassNotFound();
        Item副作用.Grade3.Title = $"一天之中發生6次以上嘔吐(每次間隔超過5分鐘)，被給予靜脈輸液或住院治療";
        Item副作用.Grade3.GradeValue1 = "";
        Item副作用.Grade3.GradeValue2 = "";
        Item副作用.Grade4.ResetCssClassNotFound();
        Item副作用.Grade4.Title = $"因嚴重嘔吐需緊急住院治療";
        Item副作用.Grade4.GradeValue1 = "";
        Item副作用.Grade4.GradeValue2 = "";
        Item副作用.Grade5.ResetCssClassNotFound();
        Item副作用.Grade5.Title = $"";
        Item副作用.Grade5.GradeValue1 = "";
        Item副作用.Grade5.GradeValue2 = "";
        Item副作用.RetriveValue = string.Empty;
    }

    public void Init副作用MucositisOral口腔炎(Survey1SideEffects副作用Node hematologicSideEffects)
    {
        GradeItemSideEffectsItem Item副作用;
        Item副作用 = hematologicSideEffects.MucositisOral口腔炎;
        Item副作用.Grade1.ResetCssClassNotFound();
        Item副作用.Grade1.Title = $"症狀或輕微症狀，無須介入治療";
        Item副作用.Grade1.GradeValue1 = "";
        Item副作用.Grade1.GradeValue2 = "";
        Item副作用.Grade2.ResetCssClassNotFound();
        Item副作用.Grade2.Title = $"中等疼痛，但不影響進食";
        Item副作用.Grade2.GradeValue1 = "";
        Item副作用.Grade2.GradeValue2 = "";
        Item副作用.Grade3.ResetCssClassNotFound();
        Item副作用.Grade3.Title = $"嚴重疼痛，已影響進食";
        Item副作用.Grade3.GradeValue1 = "";
        Item副作用.Grade3.GradeValue2 = "";
        Item副作用.Grade4.ResetCssClassNotFound();
        Item副作用.Grade4.Title = $"因嚴重口腔黏膜炎需緊急住院治療";
        Item副作用.Grade4.GradeValue1 = "";
        Item副作用.Grade4.GradeValue2 = "";
        Item副作用.Grade5.ResetCssClassNotFound();
        Item副作用.Grade5.Title = $"";
        Item副作用.Grade5.GradeValue1 = "";
        Item副作用.Grade5.GradeValue2 = "";
        Item副作用.RetriveValue = string.Empty;
    }

    public void Init副作用Diarrhea拉肚子(Survey1SideEffects副作用Node hematologicSideEffects)
    {
        GradeItemSideEffectsItem Item副作用;
        Item副作用 = hematologicSideEffects.Diarrhea拉肚子;
        Item副作用.Grade1.ResetCssClassNotFound();
        Item副作用.Grade1.Title = $"與平日相比，每天增加次數1-3次／或造口排泄少量增加";
        Item副作用.Grade1.GradeValue1 = "";
        Item副作用.Grade1.GradeValue2 = "";
        Item副作用.Grade2.ResetCssClassNotFound();
        Item副作用.Grade2.Title = $"與平日相比，每天增加次數4-6次／或造口排泄中量增加／已輕度影響日常生活";
        Item副作用.Grade2.GradeValue1 = "";
        Item副作用.Grade2.GradeValue2 = "";
        Item副作用.Grade3.ResetCssClassNotFound();
        Item副作用.Grade3.Title = $"與平日相比，每天增加次數>7次／或造口排泄多量增加／已嚴重影響日常生活或需住院治療";
        Item副作用.Grade3.GradeValue1 = "";
        Item副作用.Grade3.GradeValue2 = "";
        Item副作用.Grade4.ResetCssClassNotFound();
        Item副作用.Grade4.Title = $"因嚴重腹瀉需緊急住院治療";
        Item副作用.Grade4.GradeValue1 = "";
        Item副作用.Grade4.GradeValue2 = "";
        Item副作用.Grade5.ResetCssClassNotFound();
        Item副作用.Grade5.Title = $"";
        Item副作用.Grade5.GradeValue1 = "";
        Item副作用.Grade5.GradeValue2 = "";
        Item副作用.RetriveValue = string.Empty;
    }

    public void Init副作用Constipation便秘(Survey1SideEffects副作用Node hematologicSideEffects)
    {
        GradeItemSideEffectsItem Item副作用;
        Item副作用 = hematologicSideEffects.Constipation便秘;
        Item副作用.Grade1.ResetCssClassNotFound();
        Item副作用.Grade1.Title = $"偶爾有便秘情形，有時會用到軟便藥、改變飲食或灌腸";
        Item副作用.Grade1.GradeValue1 = "";
        Item副作用.Grade1.GradeValue2 = "";
        Item副作用.Grade2.ResetCssClassNotFound();
        Item副作用.Grade2.Title = $"持續有便秘情形，經常使用軟便藥或灌腸，日常生活功能受影響";
        Item副作用.Grade2.GradeValue1 = "";
        Item副作用.Grade2.GradeValue2 = "";
        Item副作用.Grade3.ResetCssClassNotFound();
        Item副作用.Grade3.Title = $"持續有便秘情形，需借助人工挖除";
        Item副作用.Grade3.GradeValue1 = "";
        Item副作用.Grade3.GradeValue2 = "";
        Item副作用.Grade4.ResetCssClassNotFound();
        Item副作用.Grade4.Title = $"因持續嚴重便秘需緊急住院治療";
        Item副作用.Grade4.GradeValue1 = "";
        Item副作用.Grade4.GradeValue2 = "";
        Item副作用.Grade5.ResetCssClassNotFound();
        Item副作用.Grade5.Title = $"";
        Item副作用.Grade5.GradeValue1 = "";
        Item副作用.Grade5.GradeValue2 = "";
        Item副作用.RetriveValue = string.Empty;
    }

    public void Init副作用Anorexia食慾不振(Survey1SideEffects副作用Node hematologicSideEffects)
    {
        GradeItemSideEffectsItem Item副作用;
        Item副作用 = hematologicSideEffects.Anorexia食慾不振;
        Item副作用.Grade1.ResetCssClassNotFound();
        Item副作用.Grade1.Title = $"食慾輕微下降，但飲食習慣沒有改變";
        Item副作用.Grade1.GradeValue1 = "";
        Item副作用.Grade1.GradeValue2 = "";
        Item副作用.Grade2.ResetCssClassNotFound();
        Item副作用.Grade2.Title = $"食慾輕微下降，無顯著體重下降，進而補充營養品";
        Item副作用.Grade2.GradeValue1 = "";
        Item副作用.Grade2.GradeValue2 = "";
        Item副作用.Grade3.ResetCssClassNotFound();
        Item副作用.Grade3.Title = $"食慾嚴重下降，體重明顯減輕／可能同時使用管灌或靜脈輸液";
        Item副作用.Grade3.GradeValue1 = "";
        Item副作用.Grade3.GradeValue2 = "";
        Item副作用.Grade4.ResetCssClassNotFound();
        Item副作用.Grade4.Title = $"食慾不振相當嚴重需緊急住院治療";
        Item副作用.Grade4.GradeValue1 = "";
        Item副作用.Grade4.GradeValue2 = "";
        Item副作用.Grade5.ResetCssClassNotFound();
        Item副作用.Grade5.Title = $"";
        Item副作用.Grade5.GradeValue1 = "";
        Item副作用.Grade5.GradeValue2 = "";
        Item副作用.RetriveValue = string.Empty;
    }

    public void InitEmpty(Survey1SideEffects副作用Node hematologicSideEffects)
    {
        GradeItemSideEffectsItem Item副作用;
        Item副作用 = hematologicSideEffects.Diarrhea拉肚子;
        Item副作用.Grade1.ResetCssClassNotFound();
        Item副作用.Grade1.Title = $"否";
        Item副作用.Grade1.GradeValue1 = "";
        Item副作用.Grade1.GradeValue2 = "";
        Item副作用.Grade2.ResetCssClassNotFound();
        Item副作用.Grade2.Title = $"";
        Item副作用.Grade2.GradeValue1 = "";
        Item副作用.Grade2.GradeValue2 = "";
        Item副作用.Grade3.ResetCssClassNotFound();
        Item副作用.Grade3.Title = $"";
        Item副作用.Grade3.GradeValue1 = "";
        Item副作用.Grade3.GradeValue2 = "";
        Item副作用.Grade4.ResetCssClassNotFound();
        Item副作用.Grade4.Title = $"";
        Item副作用.Grade4.GradeValue1 = "";
        Item副作用.Grade4.GradeValue2 = "";
        Item副作用.Grade5.ResetCssClassNotFound();
        Item副作用.Grade5.Title = $"";
        Item副作用.Grade5.GradeValue1 = "";
        Item副作用.Grade5.GradeValue2 = "";
        Item副作用.RetriveValue = string.Empty;
    }
    #endregion

    #region Update副作用 1
    public void Update副作用1All(Main臨床資料 main臨床資料,
        Survey1SideEffects副作用Node surveySideEffects副作用Node)
    {
        Update副作用Nausea噁心(main臨床資料, surveySideEffects副作用Node);
        Update副作用Vomiting嘔吐(main臨床資料, surveySideEffects副作用Node);
        Update副作用MucositisOral口腔炎(main臨床資料, surveySideEffects副作用Node);
        Update副作用Diarrhea拉肚子(main臨床資料, surveySideEffects副作用Node);
        Update副作用Constipation便秘(main臨床資料, surveySideEffects副作用Node);
        Update副作用Anorexia食慾不振(main臨床資料, surveySideEffects副作用Node);
    }

    public void Update副作用Nausea噁心(Main臨床資料 main臨床資料,
        Survey1SideEffects副作用Node surveySideEffects)
    {
        var visitCodeTitle = surveySideEffects.VisitCode.VisitCodeTitle;
        Survey化療副作用Node survey化療副作用Node = main臨床資料.Survey化療副作用
            .Items.FirstOrDefault(x => x.VisitCode.VisitCodeTitle == visitCodeTitle);
        if (survey化療副作用Node == null) return;

        Question question = survey化療副作用Node.Questions
            .FirstOrDefault(x => x.Text.Contains(請問您是否感到噁心));

        if (question == null) return;

        if (question.Answer == "2")
            surveySideEffects.Nausea噁心.Grade1.ResetCssClassFound();
        else if (question.Answer == "3")
            surveySideEffects.Nausea噁心.Grade2.ResetCssClassFound();
        else if (question.Answer == "4")
            surveySideEffects.Nausea噁心.Grade3.ResetCssClassFound();
    }

    public void Update副作用Vomiting嘔吐(Main臨床資料 main臨床資料,
        Survey1SideEffects副作用Node surveySideEffects)
    {
        var visitCodeTitle = surveySideEffects.VisitCode.VisitCodeTitle;
        Survey化療副作用Node survey化療副作用Node = main臨床資料.Survey化療副作用
            .Items.FirstOrDefault(x => x.VisitCode.VisitCodeTitle == visitCodeTitle);
        if (survey化療副作用Node == null) return;

        Question question = survey化療副作用Node.Questions
            .FirstOrDefault(x => x.Text.Contains(請問您是否有嘔吐情形));

        if (question == null) return;

        if (question.Answer == "2")
            surveySideEffects.Vomiting嘔吐.Grade1.ResetCssClassFound();
        else if (question.Answer == "3")
            surveySideEffects.Vomiting嘔吐.Grade2.ResetCssClassFound();
        else if (question.Answer == "4")
            surveySideEffects.Vomiting嘔吐.Grade3.ResetCssClassFound();
        else if (question.Answer == "5")
            surveySideEffects.Vomiting嘔吐.Grade4.ResetCssClassFound();
    }

    public void Update副作用MucositisOral口腔炎(Main臨床資料 main臨床資料,
        Survey1SideEffects副作用Node surveySideEffects)
    {
        var visitCodeTitle = surveySideEffects.VisitCode.VisitCodeTitle;
        Survey化療副作用Node survey化療副作用Node = main臨床資料.Survey化療副作用
            .Items.FirstOrDefault(x => x.VisitCode.VisitCodeTitle == visitCodeTitle);
        if (survey化療副作用Node == null) return;

        Question question = survey化療副作用Node.Questions
            .FirstOrDefault(x => x.Text.Contains(請問您是否有發生口腔黏膜炎));

        if (question == null) return;

        if (question.Answer == "2")
            surveySideEffects.MucositisOral口腔炎.Grade1.ResetCssClassFound();
        else if (question.Answer == "3")
            surveySideEffects.MucositisOral口腔炎.Grade2.ResetCssClassFound();
        else if (question.Answer == "4")
            surveySideEffects.MucositisOral口腔炎.Grade3.ResetCssClassFound();
        else if (question.Answer == "5")
            surveySideEffects.MucositisOral口腔炎.Grade4.ResetCssClassFound();
    }

    public void Update副作用Diarrhea拉肚子(Main臨床資料 main臨床資料,
        Survey1SideEffects副作用Node surveySideEffects)
    {
        var visitCodeTitle = surveySideEffects.VisitCode.VisitCodeTitle;
        Survey化療副作用Node survey化療副作用Node = main臨床資料.Survey化療副作用
            .Items.FirstOrDefault(x => x.VisitCode.VisitCodeTitle == visitCodeTitle);
        if (survey化療副作用Node == null) return;

        Question question = survey化療副作用Node.Questions
            .FirstOrDefault(x => x.Text.Contains(請問您是否有腹瀉情形));

        if (question == null) return;

        if (question.Answer == "2")
            surveySideEffects.Diarrhea拉肚子.Grade1.ResetCssClassFound();
        else if (question.Answer == "3")
            surveySideEffects.Diarrhea拉肚子.Grade2.ResetCssClassFound();
        else if (question.Answer == "4")
            surveySideEffects.Diarrhea拉肚子.Grade3.ResetCssClassFound();
        else if (question.Answer == "5")
            surveySideEffects.Diarrhea拉肚子.Grade4.ResetCssClassFound();
    }

    public void Update副作用Constipation便秘(Main臨床資料 main臨床資料,
        Survey1SideEffects副作用Node surveySideEffects)
    {
        var visitCodeTitle = surveySideEffects.VisitCode.VisitCodeTitle;
        Survey化療副作用Node survey化療副作用Node = main臨床資料.Survey化療副作用
            .Items.FirstOrDefault(x => x.VisitCode.VisitCodeTitle == visitCodeTitle);
        if (survey化療副作用Node == null) return;

        Question question = survey化療副作用Node.Questions
            .FirstOrDefault(x => x.Text.Contains(請問您是否有便秘情形));

        if (question == null) return;

        if (question.Answer == "2")
            surveySideEffects.Constipation便秘.Grade1.ResetCssClassFound();
        else if (question.Answer == "3")
            surveySideEffects.Constipation便秘.Grade2.ResetCssClassFound();
        else if (question.Answer == "4")
            surveySideEffects.Constipation便秘.Grade3.ResetCssClassFound();
        else if (question.Answer == "5")
            surveySideEffects.Constipation便秘.Grade4.ResetCssClassFound();
    }

    public void Update副作用Anorexia食慾不振(Main臨床資料 main臨床資料,
        Survey1SideEffects副作用Node surveySideEffects)
    {
        var visitCodeTitle = surveySideEffects.VisitCode.VisitCodeTitle;
        Survey化療副作用Node survey化療副作用Node = main臨床資料.Survey化療副作用
            .Items.FirstOrDefault(x => x.VisitCode.VisitCodeTitle == visitCodeTitle);
        if (survey化療副作用Node == null) return;

        Question question = survey化療副作用Node.Questions
            .FirstOrDefault(x => x.Text.Contains(請問您是否有食慾不振));

        if (question == null) return;

        if (question.Answer == "2")
            surveySideEffects.Anorexia食慾不振.Grade1.ResetCssClassFound();
        else if (question.Answer == "3")
            surveySideEffects.Anorexia食慾不振.Grade2.ResetCssClassFound();
        else if (question.Answer == "4")
            surveySideEffects.Anorexia食慾不振.Grade3.ResetCssClassFound();
        else if (question.Answer == "5")
            surveySideEffects.Anorexia食慾不振.Grade4.ResetCssClassFound();
    }

    #endregion

    #region Init 2
    public void Init2All(Survey2SideEffects副作用Node hematologicSideEffects)
    {
        Init副作用PeripheralNeuropathy周邊感覺神經異常(hematologicSideEffects);
        Init副作用Fatigue疲倦(hematologicSideEffects);
        Init副作用SkinRash紅疹(hematologicSideEffects);
        Init副作用HandFootSyndrome手足症候群(hematologicSideEffects);
        Init副作用Alopecia掉髮(hematologicSideEffects);
    }

    public void Init副作用PeripheralNeuropathy周邊感覺神經異常(Survey2SideEffects副作用Node hematologicSideEffects)
    {
        GradeItemSideEffectsItem Item副作用;
        Item副作用 = hematologicSideEffects.PeripheralNeuropathy周邊感覺神經異常;
        Item副作用.Grade1.ResetCssClassNotFound();
        Item副作用.Grade1.Title = $"感覺輕微異常，但沒有影響到日常生活功能";
        Item副作用.Grade1.GradeValue1 = "";
        Item副作用.Grade1.GradeValue2 = "";
        Item副作用.Grade2.ResetCssClassNotFound();
        Item副作用.Grade2.Title = $"因感覺異常，輕微影響日常生活能力(如上街購物、煮飯、做家事等)";
        Item副作用.Grade2.GradeValue1 = "";
        Item副作用.Grade2.GradeValue2 = "";
        Item副作用.Grade3.ResetCssClassNotFound();
        Item副作用.Grade3.Title = $"因感覺異常，嚴重影響日常生活自我照顧能力";
        Item副作用.Grade3.GradeValue1 = "";
        Item副作用.Grade3.GradeValue2 = "";
        Item副作用.Grade4.ResetCssClassNotFound();
        Item副作用.Grade4.Title = $"因周邊神經異常相當嚴重需緊急住院治療";
        Item副作用.Grade4.GradeValue1 = "";
        Item副作用.Grade4.GradeValue2 = "";
        Item副作用.Grade5.ResetCssClassNotFound();
        Item副作用.Grade5.Title = $"";
        Item副作用.Grade5.GradeValue1 = "";
        Item副作用.Grade5.GradeValue2 = "";
        Item副作用.RetriveValue = string.Empty;
    }

    public void Init副作用Fatigue疲倦(Survey2SideEffects副作用Node hematologicSideEffects)
    {
        GradeItemSideEffectsItem Item副作用;
        Item副作用 = hematologicSideEffects.Fatigue疲倦;
        Item副作用.Grade1.ResetCssClassNotFound();
        Item副作用.Grade1.Title = $"有時感到疲倦，但休息後可緩解";
        Item副作用.Grade1.GradeValue1 = "";
        Item副作用.Grade1.GradeValue2 = "";
        Item副作用.Grade2.ResetCssClassNotFound();
        Item副作用.Grade2.Title = $"感到疲倦時，無法透過休息緩解／已影響日常家務活動(穿衣、洗澡等)";
        Item副作用.Grade2.GradeValue1 = "";
        Item副作用.Grade2.GradeValue2 = "";
        Item副作用.Grade3.ResetCssClassNotFound();
        Item副作用.Grade3.Title = $"疲倦程度已經影響生活無法自理";
        Item副作用.Grade3.GradeValue1 = "";
        Item副作用.Grade3.GradeValue2 = "";
        Item副作用.Grade4.ResetCssClassNotFound();
        Item副作用.Grade4.Title = $"";
        Item副作用.Grade4.GradeValue1 = "";
        Item副作用.Grade4.GradeValue2 = "";
        Item副作用.Grade5.ResetCssClassNotFound();
        Item副作用.Grade5.Title = $"";
        Item副作用.Grade5.GradeValue1 = "";
        Item副作用.Grade5.GradeValue2 = "";
        Item副作用.RetriveValue = string.Empty;
    }

    public void Init副作用SkinRash紅疹(Survey2SideEffects副作用Node hematologicSideEffects)
    {
        GradeItemSideEffectsItem Item副作用;
        Item副作用 = hematologicSideEffects.SkinRash紅疹;
        Item副作用.Grade1.ResetCssClassNotFound();
        Item副作用.Grade1.Title = $"皮膚發生些微紅斑／肌膚乾燥／脫屑";
        Item副作用.Grade1.GradeValue1 = "";
        Item副作用.Grade1.GradeValue2 = "";
        Item副作用.Grade2.ResetCssClassNotFound();
        Item副作用.Grade2.Title = $"皮膚發生中度紅疹／皮膚脫屑且腫脹、潮濕";
        Item副作用.Grade2.GradeValue1 = "";
        Item副作用.Grade2.GradeValue2 = "";
        Item副作用.Grade3.ResetCssClassNotFound();
        Item副作用.Grade3.Title = $"除皺褶以外的皮膚皆發生脫屑且腫脹、潮濕／紅疹因擦傷等引起的出血";
        Item副作用.Grade3.GradeValue1 = "";
        Item副作用.Grade3.GradeValue2 = "";
        Item副作用.Grade4.ResetCssClassNotFound();
        Item副作用.Grade4.Title = $"皮膚發生潰瘍或壞死／紅疹發生自發性出血";
        Item副作用.Grade4.GradeValue1 = "";
        Item副作用.Grade4.GradeValue2 = "";
        Item副作用.Grade5.ResetCssClassNotFound();
        Item副作用.Grade5.Title = $"";
        Item副作用.Grade5.GradeValue1 = "";
        Item副作用.Grade5.GradeValue2 = "";
        Item副作用.RetriveValue = string.Empty;
    }

    public void Init副作用HandFootSyndrome手足症候群(Survey2SideEffects副作用Node hematologicSideEffects)
    {
        GradeItemSideEffectsItem Item副作用;
        Item副作用 = hematologicSideEffects.HandFootSyndrome手足症候群;
        Item副作用.Grade1.ResetCssClassNotFound();
        Item副作用.Grade1.Title = $"輕微紅腫、脫皮，不影響日常生活";
        Item副作用.Grade1.GradeValue1 = "";
        Item副作用.Grade1.GradeValue2 = "";
        Item副作用.Grade2.ResetCssClassNotFound();
        Item副作用.Grade2.Title = $"有水泡、破皮，有些微影響日常生活";
        Item副作用.Grade2.GradeValue1 = "";
        Item副作用.Grade2.GradeValue2 = "";
        Item副作用.Grade3.ResetCssClassNotFound();
        Item副作用.Grade3.Title = $"有水泡、破皮、出血伴隨疼痛，嚴重影響日常生活";
        Item副作用.Grade3.GradeValue1 = "";
        Item副作用.Grade3.GradeValue2 = "";
        Item副作用.Grade4.ResetCssClassNotFound();
        Item副作用.Grade4.Title = $"";
        Item副作用.Grade4.GradeValue1 = "";
        Item副作用.Grade4.GradeValue2 = "";
        Item副作用.Grade5.ResetCssClassNotFound();
        Item副作用.Grade5.Title = $"";
        Item副作用.Grade5.GradeValue1 = "";
        Item副作用.Grade5.GradeValue2 = "";
        Item副作用.RetriveValue = string.Empty;
    }

    public void Init副作用Alopecia掉髮(Survey2SideEffects副作用Node hematologicSideEffects)
    {
        GradeItemSideEffectsItem Item副作用;
        Item副作用 = hematologicSideEffects.Alopecia掉髮;
        Item副作用.Grade1.ResetCssClassNotFound();
        Item副作用.Grade1.Title = $"掉髮量少於一半(<50%)，外觀無明顯變化／不需使用假髮或髮片";
        Item副作用.Grade1.GradeValue1 = "";
        Item副作用.Grade1.GradeValue2 = "";
        Item副作用.Grade2.ResetCssClassNotFound();
        Item副作用.Grade2.Title = $"掉髮量大於一半 (≥50%)，外觀有明顯變化／會需使用假髮或髮片";
        Item副作用.Grade2.GradeValue1 = "";
        Item副作用.Grade2.GradeValue2 = "";
        Item副作用.Grade3.ResetCssClassNotFound();
        Item副作用.Grade3.Title = $"";
        Item副作用.Grade3.GradeValue1 = "";
        Item副作用.Grade3.GradeValue2 = "";
        Item副作用.Grade4.ResetCssClassNotFound();
        Item副作用.Grade4.Title = $"";
        Item副作用.Grade4.GradeValue1 = "";
        Item副作用.Grade4.GradeValue2 = "";
        Item副作用.Grade5.ResetCssClassNotFound();
        Item副作用.Grade5.Title = $"";
        Item副作用.Grade5.GradeValue1 = "";
        Item副作用.Grade5.GradeValue2 = "";
        Item副作用.RetriveValue = string.Empty;
    }

    public void InitEmpty(Survey2SideEffects副作用Node hematologicSideEffects)
    {
        GradeItemSideEffectsItem Item副作用;
        Item副作用 = hematologicSideEffects.PeripheralNeuropathy周邊感覺神經異常;
        Item副作用.Grade1.ResetCssClassNotFound();
        Item副作用.Grade1.Title = $"否";
        Item副作用.Grade1.GradeValue1 = "";
        Item副作用.Grade1.GradeValue2 = "";
        Item副作用.Grade2.ResetCssClassNotFound();
        Item副作用.Grade2.Title = $"";
        Item副作用.Grade2.GradeValue1 = "";
        Item副作用.Grade2.GradeValue2 = "";
        Item副作用.Grade3.ResetCssClassNotFound();
        Item副作用.Grade3.Title = $"";
        Item副作用.Grade3.GradeValue1 = "";
        Item副作用.Grade3.GradeValue2 = "";
        Item副作用.Grade4.ResetCssClassNotFound();
        Item副作用.Grade4.Title = $"";
        Item副作用.Grade4.GradeValue1 = "";
        Item副作用.Grade4.GradeValue2 = "";
        Item副作用.Grade5.ResetCssClassNotFound();
        Item副作用.Grade5.Title = $"";
        Item副作用.Grade5.GradeValue1 = "";
        Item副作用.Grade5.GradeValue2 = "";
        Item副作用.RetriveValue = string.Empty;
    }
    #endregion

    #region Update副作用 2
    public void Update副作用2All(Main臨床資料 main臨床資料,
        Survey2SideEffects副作用Node surveySideEffects副作用Node)
    {
        Update副作用PeripheralNeuropathy周邊感覺神經異常(main臨床資料, surveySideEffects副作用Node);
        Update副作用Fatigue疲倦(main臨床資料, surveySideEffects副作用Node);
        Update副作用SkinRash紅疹(main臨床資料, surveySideEffects副作用Node);
        Update副作用HandFootSyndrome手足症候群(main臨床資料, surveySideEffects副作用Node);
        Update副作用Alopecia掉髮(main臨床資料, surveySideEffects副作用Node);
    }

    public void Update副作用PeripheralNeuropathy周邊感覺神經異常(Main臨床資料 main臨床資料,
        Survey2SideEffects副作用Node surveySideEffects)
    {
        var visitCodeTitle = surveySideEffects.VisitCode.VisitCodeTitle;
        Survey化療副作用Node survey化療副作用Node = main臨床資料.Survey化療副作用
            .Items.FirstOrDefault(x => x.VisitCode.VisitCodeTitle == visitCodeTitle);
        if (survey化療副作用Node == null) return;

        Question question = survey化療副作用Node.Questions
            .FirstOrDefault(x => x.Text.Contains(請問您是否有食慾請問您是否有發生周邊感覺神經異常情形不振));

        if (question == null) return;

        if (question.Answer == "2")
            surveySideEffects.PeripheralNeuropathy周邊感覺神經異常.Grade1.ResetCssClassFound();
        else if (question.Answer == "3")
            surveySideEffects.PeripheralNeuropathy周邊感覺神經異常.Grade2.ResetCssClassFound();
        else if (question.Answer == "4")
            surveySideEffects.PeripheralNeuropathy周邊感覺神經異常.Grade3.ResetCssClassFound();
        else if (question.Answer == "5")
            surveySideEffects.PeripheralNeuropathy周邊感覺神經異常.Grade4.ResetCssClassFound();
    }

    public void Update副作用Fatigue疲倦(Main臨床資料 main臨床資料,
        Survey2SideEffects副作用Node surveySideEffects)
    {
        var visitCodeTitle = surveySideEffects.VisitCode.VisitCodeTitle;
        Survey化療副作用Node survey化療副作用Node = main臨床資料.Survey化療副作用
            .Items.FirstOrDefault(x => x.VisitCode.VisitCodeTitle == visitCodeTitle);
        if (survey化療副作用Node == null) return;

        Question question = survey化療副作用Node.Questions
            .FirstOrDefault(x => x.Text.Contains(請問您是否會感到疲倦));

        if (question == null) return;

        if (question.Answer == "2")
            surveySideEffects.Fatigue疲倦.Grade1.ResetCssClassFound();
        else if (question.Answer == "3")
            surveySideEffects.Fatigue疲倦.Grade2.ResetCssClassFound();
        else if (question.Answer == "4")
            surveySideEffects.Fatigue疲倦.Grade3.ResetCssClassFound();
    }

    public void Update副作用SkinRash紅疹(Main臨床資料 main臨床資料,
        Survey2SideEffects副作用Node surveySideEffects)
    {
        var visitCodeTitle = surveySideEffects.VisitCode.VisitCodeTitle;
        Survey化療副作用Node survey化療副作用Node = main臨床資料.Survey化療副作用
            .Items.FirstOrDefault(x => x.VisitCode.VisitCodeTitle == visitCodeTitle);
        if (survey化療副作用Node == null) return;

        Question question = survey化療副作用Node.Questions
            .FirstOrDefault(x => x.Text.Contains(請問您是否有發生紅疹情形));

        if (question == null) return;

        if (question.Answer == "2")
            surveySideEffects.SkinRash紅疹.Grade1.ResetCssClassFound();
        else if (question.Answer == "3")
            surveySideEffects.SkinRash紅疹.Grade2.ResetCssClassFound();
        else if (question.Answer == "4")
            surveySideEffects.SkinRash紅疹.Grade3.ResetCssClassFound();
        else if (question.Answer == "5")
            surveySideEffects.SkinRash紅疹.Grade4.ResetCssClassFound();
    }

    public void Update副作用HandFootSyndrome手足症候群(Main臨床資料 main臨床資料,
        Survey2SideEffects副作用Node surveySideEffects)
    {
        var visitCodeTitle = surveySideEffects.VisitCode.VisitCodeTitle;
        Survey化療副作用Node survey化療副作用Node = main臨床資料.Survey化療副作用
            .Items.FirstOrDefault(x => x.VisitCode.VisitCodeTitle == visitCodeTitle);
        if (survey化療副作用Node == null) return;

        Question question = survey化療副作用Node.Questions
            .FirstOrDefault(x => x.Text.Contains(請問您是否有手足症候群情形));

        if (question == null) return;

        if (question.Answer == "2")
            surveySideEffects.HandFootSyndrome手足症候群.Grade1.ResetCssClassFound();
        else if (question.Answer == "3")
            surveySideEffects.HandFootSyndrome手足症候群.Grade2.ResetCssClassFound();
        else if (question.Answer == "4")
            surveySideEffects.HandFootSyndrome手足症候群.Grade3.ResetCssClassFound();
    }

    public void Update副作用Alopecia掉髮(Main臨床資料 main臨床資料,
        Survey2SideEffects副作用Node surveySideEffects)
    {
        var visitCodeTitle = surveySideEffects.VisitCode.VisitCodeTitle;
        Survey化療副作用Node survey化療副作用Node = main臨床資料.Survey化療副作用
            .Items.FirstOrDefault(x => x.VisitCode.VisitCodeTitle == visitCodeTitle);
        if (survey化療副作用Node == null) return;

        Question question = survey化療副作用Node.Questions
            .FirstOrDefault(x => x.Text.Contains(請問您是否有掉髮情形));

        if (question == null) return;

        if (question.Answer == "2")
            surveySideEffects.Alopecia掉髮.Grade1.ResetCssClassFound();
        else if (question.Answer == "3")
            surveySideEffects.Alopecia掉髮.Grade2.ResetCssClassFound();
    }
    #endregion

}
