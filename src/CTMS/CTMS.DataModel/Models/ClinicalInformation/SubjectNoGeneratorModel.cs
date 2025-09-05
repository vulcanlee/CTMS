using CTMS.Share.Helpers;
using System.Text;

namespace CTMS.DataModel.Models.ClinicalInformation;

public class SubjectNoGeneratorModel
{
    public int NCKUH成大 { get; set; } = 0;
    public int CHIMEIH奇美 { get; set; } = 0;
    public int KGH郭綜合 { get; set; } = 0;

    public async Task ReadAsync()
    {
        string filenameRuntime = Path.Combine("Data", MagicObjectHelper.SubjectNoGeneratorJsonFile);
        if (File.Exists(filenameRuntime))
        {
            using (FileStream fs = new FileStream(filenameRuntime, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader reader = new StreamReader(fs, Encoding.UTF8))
                {
                    string json = await reader.ReadToEndAsync();
                    var item = FromJson(json);
                    this.NCKUH成大 = item.NCKUH成大;
                    this.CHIMEIH奇美 = item.CHIMEIH奇美;
                    this.KGH郭綜合 = item.KGH郭綜合;
                }
            }
        }
    }

    public async Task SaveAsync()
    {
        string filenameRuntime = Path.Combine("Data", MagicObjectHelper.SubjectNoGeneratorJsonFile);
        string directory = Path.GetDirectoryName(filenameRuntime) ?? "Data";
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        using (FileStream fs = new FileStream(filenameRuntime, FileMode.Create, FileAccess.Write))
        {
            using (StreamWriter writer = new StreamWriter(fs, Encoding.UTF8))
            {
                string json = ToJson();
                await writer.WriteAsync(json);
            }
        }
    }

    public string ToJson()
    {
        var options = new System.Text.Json.JsonSerializerOptions
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All)
        };
        return System.Text.Json.JsonSerializer.Serialize(this, options);
    }

    public SubjectNoGeneratorModel FromJson(string json)
    {
        var data = System.Text.Json.JsonSerializer.Deserialize<SubjectNoGeneratorModel>(json);
        return data;
    }
}
