using CTMS.DataModel.Models.AIAgent;
using CTMS.DataModel.Models.ClinicalInformation;
using CTMS.Share.Helpers;
using OfficeOpenXml;
using Syncfusion.XlsIO;
using System.Threading.Tasks;

namespace SyncExcel.Services;

public class RiskAssessmentExcelService
{
    public RandomListModel RandomList { get; set; } = new();

    #region 讀取 Excel 內的資料

    public RiskAssessmentAIResult ReadExcel20250916(string filename)
    {
        RiskAssessmentAIResult result = new();
        // RiskAssessmentResult
        using (ExcelEngine excelEngine = new ExcelEngine())
        {
            //Instantiate the Excel application object
            IApplication application = excelEngine.Excel;

            //Assigns default application version
            application.DefaultVersion = ExcelVersion.Xlsx;

            string filenameRandomList = filename;

            using (FileStream sampleFile = new FileStream(filenameRandomList, FileMode.Open))
            {
                IWorkbook workbook = application.Workbooks.Open(sampleFile);
                IWorksheet worksheet = workbook.Worksheets.FirstOrDefault();

                result.ID = worksheet.Range["B2"].DisplayText;
                result.Age = worksheet.Range["D2"].DisplayText;
                result.BodyHeight = worksheet.Range["F2"].DisplayText;
                result.BodyWeight = worksheet.Range["G2"].DisplayText;
                result.VertebralBodyAreaCm2 = worksheet.Range["B41"].DisplayText;
                result.TotalSMD = worksheet.Range["C13"].DisplayText;
                result.TotalImatA = worksheet.Range["I13"].DisplayText;
                result.TotalLamaA = worksheet.Range["N13"].DisplayText;
                result.TotalNamaA = worksheet.Range["S13"].DisplayText;
                result.VatA = worksheet.Range["B31"].DisplayText;
                result.SatA = worksheet.Range["G31"].DisplayText;
            }
        }
        return result;
    }

    public RiskAssessmentAIResult ReadExcel(string filename)
    {
        RiskAssessmentAIResult result = new();

        // 1. 讀取 CSV
        var csvData = File.ReadAllLines(filename);

        // 2. 建立 Excel 檔案
        ExcelPackage.License.SetNonCommercialOrganization("My Noncommercial organization");
        //ExcelPackage.LicenseContext = LicenseContext.NonCommercial
        using (var package = new ExcelPackage())
        {
            var worksheet = package.Workbook.Worksheets.Add("Sheet1");

            // 寫入資料到 Excel
            for (int i = 0; i < csvData.Length; i++)
            {
                var values = csvData[i].Split(',');
                for (int j = 0; j < values.Length; j++)
                {
                    worksheet.Cells[i + 1, j + 1].Value = values[j];
                }
            }

            // 現在可以像處理 Excel 一樣讀取
            result.ID = worksheet.Cells["B2"].Text;
            result.Age = worksheet.Cells["D2"].Text;
            result.BodyHeight = worksheet.Cells["F2"].Text;
            result.BodyWeight = worksheet.Cells["G2"].Text;
            result.VertebralBodyAreaCm2 = worksheet.Cells["B41"].Text;
            result.TotalSMD = worksheet.Cells["C13"].Text;
            result.TotalImatA = worksheet.Cells["I13"].Text;
            result.TotalLamaA = worksheet.Cells["N13"].Text;
            result.TotalNamaA = worksheet.Cells["S13"].Text;
            result.VatA = worksheet.Cells["B31"].Text;
            result.SatA = worksheet.Cells["G31"].Text;
        }




        // RiskAssessmentResult
        //using (ExcelEngine excelEngine = new ExcelEngine())
        //{
        //    //Instantiate the Excel application object
        //    IApplication application = excelEngine.Excel;

        //    //Assigns default application version
        //    application.DefaultVersion = ExcelVersion.Xlsx;

        //    string filenameRandomList = filename;

        //    using (FileStream sampleFile = new FileStream(filenameRandomList, FileMode.Open))
        //    {
        //        IWorkbook workbook = application.Workbooks.Open(sampleFile);
        //        IWorksheet worksheet = workbook.Worksheets.FirstOrDefault();

        //        result.ID = worksheet.Range["B2"].DisplayText;
        //        result.Age = worksheet.Range["D2"].DisplayText;
        //        result.BodyHeight = worksheet.Range["F2"].DisplayText;
        //        result.BodyWeight = worksheet.Range["G2"].DisplayText;
        //        result.VertebralBodyAreaCm2 = worksheet.Range["B41"].DisplayText;
        //        result.TotalSMD = worksheet.Range["C13"].DisplayText;
        //        result.TotalImatA = worksheet.Range["I13"].DisplayText;
        //        result.TotalLamaA = worksheet.Range["N13"].DisplayText;
        //        result.TotalNamaA = worksheet.Range["S13"].DisplayText;
        //        result.VatA = worksheet.Range["B31"].DisplayText;
        //        result.SatA = worksheet.Range["G31"].DisplayText;
        //    }
        //}
        return result;
    }
    #endregion
}
