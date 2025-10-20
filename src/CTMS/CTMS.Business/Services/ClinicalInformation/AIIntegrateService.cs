using AIAgent.Models;
using AIAgent.Services;
using CTMS.DataModel.Models.AIAgent;
using CTMS.DataModel.Models.ClinicalInformation;
using CTMS.EntityModel.Models;
using CTMS.Share.Extensions;
using CTMS.Share.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SyncExcel.Services;

namespace CTMS.Business.Services.ClinicalInformation;

public class AIIntegrateService
{
    private readonly ILogger<AIIntegrateService> logger;
    private readonly AgentService agentService;
    private readonly DirectoryHelperService directoryHelperService;
    private readonly InputCsvService inputCsvService;
    private readonly RiskAssessmentExcelService riskAssessmentExcelService;
    private readonly Agentsetting agentsetting;

    public AIIntegrateService(ILogger<AIIntegrateService> logger,
        AgentService agentService,
        DirectoryHelperService directoryHelperService,
        IOptions<Agentsetting> agentsettingOptions,
        InputCsvService inputCsvService,
        RiskAssessmentExcelService riskAssessmentExcelService)
    {
        this.logger = logger;
        this.agentService = agentService;
        this.directoryHelperService = directoryHelperService;
        this.inputCsvService = inputCsvService;
        this.riskAssessmentExcelService = riskAssessmentExcelService;
        this.agentsetting = agentsettingOptions.Value;
    }

    public async Task<PatientAIInfo> PushToAI(Patient patient,
        PatientData patientData, string dicomImage)
    {
        PatientAIInfo patientAIInfo = new()
        {
            Age = patientData.臨床資訊.Age,
            Code = patient.Code,
            Gender = "F",
            Height = patientData.臨床資訊.Height,
            Weight = patientData.臨床資訊.Weight,
            SubjectCode = patientData.臨床資訊.SubjectNo,
            癌別 = patientData.臨床資訊.ECorOC,
            DicomFilename = dicomImage,
            DestionatioDicomFilename = "",
            DestionatioPatientJSONFilename = ""
        };
        patientAIInfo.InitKeyName();

        var currentRootPath = Directory.GetCurrentDirectory();
        var dicmTempRootPath = Path.Combine(currentRootPath, MagicObjectHelper.UploadTempPath);
        var sourceDicomFilePath = Path.GetDirectoryName(dicomImage);
        var destinationDicomFileName = Path.Combine(dicmTempRootPath, $"{patientAIInfo.KeyName}.dcm");
        File.Copy(dicomImage, destinationDicomFileName, true);

        agentService.CreateInBound(patientAIInfo, agentsetting);

        return patientAIInfo;
    }

    public async Task<string> PushToAICheck(Patient patient,
        PatientData patientData, string dicomImage)
    {
        string result = "";
        if (string.IsNullOrEmpty(patientData.臨床資訊.Age) ||
            patientData.臨床資訊.Age.ToInt() == 0)
        {
            result = "年齡未填";
            return result;
        }

        if (string.IsNullOrEmpty(patientData.臨床資訊.Height) ||
            patientData.臨床資訊.Height.ToInt() == 0)
        {
            result = "身高未填";
            return result;
        }

        if (string.IsNullOrEmpty(patientData.臨床資訊.Weight) ||
            patientData.臨床資訊.Weight.ToInt() == 0)
        {
            result = "體重未填";
            return result;
        }

        if (string.IsNullOrEmpty(patientData.臨床資訊.ECorOC))
        {
            result = "癌別未填";
            return result;
        }

        if (!File.Exists(dicomImage))
        {
            result = "影像檔案不存在";
            return result;
        }
        return result;
    }

    public async Task<bool> CheckAIProcess(string KeyName)
    {
        bool result = false;
        var completionRootPath = agentsetting.GetCompleteQueuePath();
        var completionKeyNamePath = Path.Combine(completionRootPath, KeyName);
        var completionPhase3ResultPath = Path.Combine(completionKeyNamePath, "Phase3Result");
        var uploadFilesPath = MagicObjectHelper.UploadFinalPath;
        var destinationKeyNamePath = Path.Combine(uploadFilesPath, KeyName);

        if (!(Directory.Exists(completionPhase3ResultPath)))
            return result;

        var files = Directory.GetFiles(completionPhase3ResultPath, "*");
        if (files.Length == 0)
            return result;

        var file = files.FirstOrDefault(x => x.Contains("output.csv"));
        if (file == null)
            return result;

        if (!Directory.Exists(destinationKeyNamePath))
        {
            Directory.CreateDirectory(destinationKeyNamePath);
        }

        directoryHelperService.CopyDirectory(completionKeyNamePath, destinationKeyNamePath, true);
        result = true;

        return result;
    }

    public async Task<InputCsvModel> GetInputCsv(string KeyName)
    {
        InputCsvModel result = new();
        var uploadFilesPath = MagicObjectHelper.UploadFinalPath;
        var destinationKeyNamePath = Path.Combine(uploadFilesPath, KeyName, "Phase3Result", "input.csv");

        if (!(File.Exists(destinationKeyNamePath)))
            return result;

        var foo = inputCsvService.Read(destinationKeyNamePath);

        if (foo != null)
            result = foo;
        return result;
    }

    public async Task<RiskAssessmentAIResult> Get腰圍ACAsync(string KeyName)
    {
        RiskAssessmentAIResult riskAssessmentAIResult = null;
        string result = "";
        var uploadFilesPath = MagicObjectHelper.UploadFinalPath;
        var destinationKeyNamePath = Path.Combine(uploadFilesPath, KeyName, "Phase2Result", $"{KeyName}.csv");

        if ((File.Exists(destinationKeyNamePath)))
            riskAssessmentAIResult = riskAssessmentExcelService.ReadExcel(destinationKeyNamePath);
        return riskAssessmentAIResult;

    }

    public async Task<(string, string)> GetOnputCsv(string KeyName)
    {
        var risk = "";
        var reducedRisk = "";
        var uploadFilesPath = MagicObjectHelper.UploadFinalPath;
        var destinationKeyNamePath = Path.Combine(uploadFilesPath, KeyName, "Phase3Result", "output.csv");

        if (!(File.Exists(destinationKeyNamePath)))
            return ("", "");

        string output = File.ReadAllText(destinationKeyNamePath).ToLower();
        if (output.Contains("a grade III AE".ToLower()))
        {
            risk = "高風險";
            reducedRisk = "需要";
        }
        else
        {
            risk = "低風險";
            reducedRisk = "不需要";
        }
        return (risk, reducedRisk);
    }
}
