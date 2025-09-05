using CTMS.DataModel.Models.ClinicalInformation;
using CTMS.Share.Helpers;
using Microsoft.Extensions.Logging;
using System.Text;

namespace CTMS.Business.Services.ClinicalInformation;

public class SubjectNoGeneratorService
{
    SubjectNoGeneratorModel SubjectNoGeneratorModel = new();

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
        string subjectNo = string.Empty;
        await SubjectNoGeneratorModel.ReadAsync();
        switch (site)
        {
            case MagicObjectHelper.prefix成大醫院:
                SubjectNoGeneratorModel.NCKUH成大++;
                subjectNo = $"{MagicObjectHelper.prefix成大醫院}{SubjectNoGeneratorModel.NCKUH成大:0000}";
                break;
            case MagicObjectHelper.prefix奇美醫院:
                SubjectNoGeneratorModel.CHIMEIH奇美++;
                subjectNo = $"{MagicObjectHelper.prefix奇美醫院}{SubjectNoGeneratorModel.CHIMEIH奇美:0000}";
                break;
            case MagicObjectHelper.prefix郭綜合醫院:
                SubjectNoGeneratorModel.KGH郭綜合++;
                subjectNo = $"{MagicObjectHelper.prefix郭綜合醫院}{SubjectNoGeneratorModel.KGH郭綜合:0000}";
                break;
            default:
                logger.LogError($"不支援的院區代碼 {site} !");
                throw new Exception($"不支援的院區代碼 {site} !");
        }
        await SubjectNoGeneratorModel.SaveAsync();
        return subjectNo;
    }
}
