using AIAgent.Models;
using AIAgent.Services;
using CTMS.DataModel.Models.AIAgent;
using CTMS.DataModel.Models.ClinicalInformation;
using CTMS.EntityModel.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CTMS.Business.Services.ClinicalInformation;

public class AIIntegrateService
{
    private readonly ILogger<AIIntegrateService> logger;
    private readonly AgentService agentService;
    private readonly Agentsetting agentsetting;

    public AIIntegrateService(ILogger<AIIntegrateService> logger,
        AgentService agentService,
        IOptions<Agentsetting> agentsettingOptions)
    {
        this.logger = logger;
        this.agentService = agentService;
        this.agentsetting = agentsettingOptions.Value;
    }

    public async Task PushToAI(Patient patient,
        PatientData patientData)
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
            DicomFilename = "",
            DestionatioDicomFilename = "",
            DestionatioPatientJSONFilename = ""
        };

        agentService.CreateInBound(patientAIInfo, agentsetting);
    }
}
