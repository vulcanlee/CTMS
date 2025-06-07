using CTMS.DataModel.Models.ClinicalInformation;

namespace CTMS.Business.Services.ClinicalInformation;

public class CancerStageService
{
    public CancerStage CancerStageEC { get; set; } = new();
    public CancerStage CancerStageOC { get; set; } = new();

    public void ReadAll()
    {        
        // 讀取子宮內膜癌和卵巢癌的分期資料
        CancerStageEC = ReadEndometrial();
        CancerStageOC = ReadOvarian();
    }
    public CancerStage ReadEndometrial()
    {
        // 子宮內膜癌
        var CancerStageEC = ReadFile("EndometrialCancerStage.json");
        return CancerStageEC;
    }

    public CancerStage ReadOvarian()
    {
        // 卵巢癌
        var CancerStageOC = ReadFile("OvarianCancerStage.json");
        return CancerStageOC;
    }

    public CancerStage ReadFile(string filename)
    {
        string path = Path.Combine("Data", filename);
        string content = File.ReadAllText(path);
        var fooCancerStage = Newtonsoft.Json.JsonConvert.DeserializeObject<CancerStage>(content);
        return fooCancerStage;
    }

    public string GetStageNames(string ECorOC, string stageCode, string t, string n, string m)
    {
        List<StageNode> stages = new();
        if (ECorOC == "EC")
        {
            stages = CancerStageEC.Stage
            .Where(s => s.Children.Any(c => c.Name == t || c.Name == "T*") &&
            s.Children.Any(c => c.Name == n || c.Name == "N*") &&
            s.Children.Any(c => c.Name == m)).ToList();
        }
        else if (ECorOC == "OC")
        {
            stages = CancerStageOC.Stage
            .Where(s => s.Children.Any(c => c.Name == t || c.Name == "T*") &&
            s.Children.Any(c => c.Name == n || c.Name == "N*") &&
            s.Children.Any(c => c.Name == m)).ToList();
        }
        string result = string.Empty;
        string foundStage = stages.FirstOrDefault()?.Name ?? string.Empty;
        result = $"{foundStage}({stageCode}{t}{n}{m})".Trim();
        return result;
    }
}
