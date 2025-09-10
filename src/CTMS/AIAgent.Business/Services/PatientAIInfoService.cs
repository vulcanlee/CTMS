using CTMS.DataModel.Models.AIAgent;

namespace AIAgent.Services;

public class PatientAIInfoService
{
    public PatientAIInfoService()
    {
    }

    public async Task<PatientAIInfo> ReadAsync(string filename)
    {
        string json = await File.ReadAllTextAsync(filename);
        PatientAIInfo patientAIInfo = new PatientAIInfo().FromJson(json);
        return patientAIInfo;
    }

}
