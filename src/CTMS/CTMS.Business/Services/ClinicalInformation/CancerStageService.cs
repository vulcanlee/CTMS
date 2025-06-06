using CTMS.DataModel.Models;
using CTMS.DataModel.Models.ClinicalInformation;
using CTMS.ExcelUtility.Extensions;
using CTMS.Share.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTMS.Business.Services.ClinicalInformation;

public class CancerStageService
{
    public CancerStage ReadEndometrial()
    {
        var cancerStage = ReadFile("EndometrialCancerStage .json");
        return cancerStage;
    }

    public CancerStage ReadFile(string filename)
    {
        string path = Path.Combine("Data", filename);
        string content = File.ReadAllText(path);
        var bloodTest = Newtonsoft.Json.JsonConvert.DeserializeObject<CancerStage>(content);
        return bloodTest;
    }
}
