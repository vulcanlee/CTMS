using CTMS.DataModel.Models.AIAgent;
using CTMS.DataModel.Models.ClinicalInformation;
using CTMS.Share.Helpers;
using Syncfusion.XlsIO;
using System.Threading.Tasks;

namespace SyncExcel.Services;

public class RiskAssessmentExcelService
{
    public RandomListModel RandomList { get; set; } = new();

    #region 讀取 Excel 內的資料

    public RiskAssessmentAIResult ReadExcel(string filename)
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
    #endregion
}
