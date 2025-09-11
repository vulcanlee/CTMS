using AIAgent.Models;
using AIAgent.Services;
using CTMS.DataModel.Models.AIAgent;
using CTMS.DataModel.Models.ClinicalInformation;
using CTMS.EntityModel.Models;
using CTMS.Share.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CTMS.Business.Services.ClinicalInformation;

public class AIIntegrateService
{
    private readonly ILogger<AIIntegrateService> logger;
    private readonly AgentService agentService;
    private readonly DirectoryHelperService directoryHelperService;
    private readonly Agentsetting agentsetting;

    public AIIntegrateService(ILogger<AIIntegrateService> logger,
        AgentService agentService,
        DirectoryHelperService directoryHelperService,
        IOptions<Agentsetting> agentsettingOptions)
    {
        this.logger = logger;
        this.agentService = agentService;
        this.directoryHelperService = directoryHelperService;
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

        var file = files.FirstOrDefault(x => x.StartsWith("output.csv"));
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
}
