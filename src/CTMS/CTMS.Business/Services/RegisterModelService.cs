using CTMS.DataModel.Models;
using CTMS.DataModel.Models.ClinicalInformation;
using CTMS.DataModel.Models.Systems;
using CTMS.Share.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTMS.Business.Services
{
    public class RegisterModelService
    {
        public List<RegisterModel> RegisterModels { get; set; } = new();
        string FilenameRuntime = Path.Combine("Data", MagicObjectHelper.FilenameRegisterModel);
        public RegisterModelService()
        {
        }
        public string ToJson()
        {
            var options = new System.Text.Json.JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All)
            };
            return System.Text.Json.JsonSerializer.Serialize(RegisterModels, options);
        }

        public void FromJson(string json)
        {
            var data = System.Text.Json.JsonSerializer.Deserialize<List<RegisterModel>  >(json);
            if (data != null)
            {
                this.RegisterModels = data;
            }
        }

        public async Task Save()
        {
            var json = ToJson();
            await System.IO.File.WriteAllTextAsync(FilenameRuntime, json);
        }
        public async Task Get()
        {
            if (System.IO.File.Exists(FilenameRuntime))
            {
                var json = await System.IO.File.ReadAllTextAsync(FilenameRuntime);
                FromJson(json);
                RegisterModels = RegisterModels.OrderByDescending(x => x.CreateAt).ToList();
            }
        }
    }
}
