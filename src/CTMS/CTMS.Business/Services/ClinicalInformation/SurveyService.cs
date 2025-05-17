using CTMS.DataModel.Models.ClinicalInformation;

namespace CTMS.Business.Services.ClinicalInformation;

public class SurveyService
{
    public void Read(Survey問卷 survey)
    {
        survey.化療副作用 = ReadFile("化療副作用自填問卷.json");
        survey.放療副作用 = ReadFile("放療副作用.json");
        survey.標靶副作用 = ReadFile("標靶副作用.json");
        survey.whooqol問卷 = ReadFile("whooqol問卷.json");
        //bloodTest.抽血檢驗生化2 = ReadFile("抽血檢驗血液.json");
    }

    public Survey ReadFile(string filename)
    {
        string path = Path.Combine("Data", filename);
        string content = File.ReadAllText(path);
        var surveryItems = Newtonsoft.Json.JsonConvert.DeserializeObject<Survey>(content);
        return surveryItems;
    }
}
