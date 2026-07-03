using CTMS.Share.Helpers;
using System.Text;

namespace CTMS.DataModel.Models.ClinicalInformation;

public class SubjectNoGeneratorModel
{
    /// <summary>
    /// 各院流水號計數，鍵為 HospitalRegistry 的 CounterKey（如 "NCKUH成大"）。
    /// 序列化結果與舊版固定屬性的 JSON 形狀完全相同，既有資料檔可直接讀入。
    /// </summary>
    public Dictionary<string, int> Counters { get; set; } =
        HospitalRegistry.PrefixOwners.ToDictionary(x => x.CounterKey, _ => 0);

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
                    Counters = FromJson(json);
                }
            }
        }

        foreach (var owner in HospitalRegistry.PrefixOwners)
        {
            Counters.TryAdd(owner.CounterKey, 0);
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
        return System.Text.Json.JsonSerializer.Serialize(Counters, options);
    }

    public Dictionary<string, int> FromJson(string json)
    {
        var data = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, int>>(json);
        return data ?? new();
    }
}
