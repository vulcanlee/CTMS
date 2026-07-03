using CTMS.DataModel.Models.ClinicalInformation;
using CTMS.Share.Helpers;
using Microsoft.Extensions.Logging;
using System.Text;

namespace CTMS.Business.Services.ClinicalInformation;

public class SubjectNoGeneratorService
{
    public SubjectNoGeneratorModel SubjectNoGeneratorModel { get; set; } = new();

    private readonly ILogger<SubjectNoGeneratorService> logger;

    public SubjectNoGeneratorService(ILogger<SubjectNoGeneratorService> logger)
    {
        this.logger = logger;
    }

    public async Task InitialAsync()
    {
        string filename = Path.Combine("Data", MagicObjectHelper.SubjectNoGeneratorJsonFile);

        if (File.Exists(filename) == false)
        {
            SubjectNoGeneratorModel = new();
            await SubjectNoGeneratorModel.SaveAsync();
        }
        else
        {
            await SubjectNoGeneratorModel.ReadAsync();
        }
    }

    public async Task<string> GenerateAsync(string site)
    {
        await SubjectNoGeneratorModel.ReadAsync();
        var owner = HospitalRegistry.GetOwnerByPrefix(site);
        if (owner == null)
        {
            logger.LogError($"不支援的院區代碼 {site} !");
            throw new Exception($"不支援的院區代碼 {site} !");
        }
        SubjectNoGeneratorModel.Counters[owner.CounterKey]++;
        string subjectNo = $"{owner.Prefix}{SubjectNoGeneratorModel.Counters[owner.CounterKey]:0000}";
        await SubjectNoGeneratorModel.SaveAsync();
        return subjectNo;
    }
}
