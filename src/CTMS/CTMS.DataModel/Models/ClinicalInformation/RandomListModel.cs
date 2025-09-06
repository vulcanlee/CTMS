using CTMS.Share.Helpers;
using System.Text;

namespace CTMS.DataModel.Models.ClinicalInformation;

public class RandomListModel
{
    public List<RandomListItem> Items { get; set; } = new();

    public void Reset()
    {
        Items.Clear();
    }

    public string ToJson()
    {
        var options = new System.Text.Json.JsonSerializerOptions
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All)
        };
        return System.Text.Json.JsonSerializer.Serialize(this, options);
    }

    public List<RandomListItem> FromJson(string json)
    {
        var data = System.Text.Json.JsonSerializer.Deserialize<RandomListModel>(json);
        return data.Items;
    }

    public async Task ReadAsync()
    {
        string filenameRuntime = Path.Combine("Data", MagicObjectHelper.RandomListRuntimeJsonFile);
        if (File.Exists(filenameRuntime))
        {
            using (FileStream fs = new FileStream(filenameRuntime, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader reader = new StreamReader(fs, Encoding.UTF8))
                {
                    string json = await reader.ReadToEndAsync();
                    Items = FromJson(json);
                }
            }
        }
    }

    public async Task SaveAsync()
    {
        string filenameRuntime = Path.Combine("Data", MagicObjectHelper.RandomListRuntimeJsonFile);
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
}

public class RandomListItem:ICloneable
{
    public string Hospital { get; set; } = string.Empty;
    public string ECorOC { get; set; } = string.Empty;
    public string EarlyOrAdvance { get; set; } = string.Empty;
    public string Id { get; set; } = string.Empty;
    public string BlockId { get; set; } = string.Empty;
    public string BlockSize { get; set; } = string.Empty;
    public string Treatment { get; set; } = string.Empty;
    public string SubjectNo { get; set; } = string.Empty;

    public void Reset()
    {
        Hospital = string.Empty;
        EarlyOrAdvance = string.Empty;
        ECorOC = string.Empty;

        Id = string.Empty;
        BlockId = string.Empty;
        BlockSize = string.Empty;
        Treatment = string.Empty;
        SubjectNo = string.Empty;
    }

    public RandomListItem Clone()
    {
       return ((ICloneable)this).Clone() as RandomListItem;
    }

    object ICloneable.Clone()
    {
       return this.MemberwiseClone();
    }
}
