using CTMS.DataModel.Models.AIAgent;
using CTMS.DataModel.Models.ClinicalInformation;
using CTMS.Share.Helpers;
using Syncfusion.XlsIO;
using System.Threading.Tasks;

namespace SyncExcel.Services;

public class InputCsvService
{
    public InputCsvModel InputCsvModel { get; set; } = new();

    public InputCsvModel Read(string filename)
    {
        // 讀取 該 csv 檔案
        //ID,Age,Tumor.Grade,body.height.cm,body.weight.kg,Vertebral.Body.Area.cm2,Total.SMD,Total.ImatA,Total.LamaA,Total.NamaA,VatA,SatA
        //A19R5802373,049,1,1.55,63.0,15.56396484375,40.60484780157835,17.1875,40.72265625,121.69189453125,131.8115234375,129.3212890625
        InputCsvModel = new InputCsvModel();
        var lines = File.ReadAllLines(filename);
        if (lines.Length < 2)
            throw new Exception("CSV 檔案內容不足");
        var headers = lines[0].Split(',');
        var values = lines[1].Split(',');
        for (int i = 0; i < headers.Length; i++)
        {
            var header = headers[i].Trim();
            var value = values[i].Trim();
            switch (header)
            {
                case "ID":
                    InputCsvModel.ID = value;
                    break;
                case "Age":
                    InputCsvModel.Age = value;
                    break;
                case "Tumor.Grade":
                    InputCsvModel.Tumor_Grade = value;
                    break;
                case "body.height.cm":
                    InputCsvModel.body_height_cm = value;
                    break;
                case "body.weight.kg":
                    InputCsvModel.body_weight_kg = value;
                    break;
                case "Vertebral.Body.Area.cm2":
                    InputCsvModel.Vertebral_Body_Area_cm2 = value;
                    break;
                case "Total.SMD":
                    InputCsvModel.Total_SMD = value;
                    break;
                case "Total.ImatA":
                    InputCsvModel.Total_ImatA = value;
                    break;
                case "Total.LamaA":
                    InputCsvModel.Total_LamaA = value;
                    break;
                case "Total.NamaA":
                    InputCsvModel.Total_NamaA = value;
                    break;
                case "VatA":
                    InputCsvModel.VatA = value;
                    break;
                case "SatA":
                    InputCsvModel.SatA = value;
                    break;
                    //default:
                    // 未知的欄位名稱，可以選擇忽略或拋出例外
                    //throw new Exception($"未知的欄位名稱: {header}");
            }
        }
        return InputCsvModel;
    }
}
