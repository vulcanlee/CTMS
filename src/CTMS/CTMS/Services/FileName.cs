using LisServiceReference;
using PipServiceReference;
using System.Data;
using System.ServiceModel;
using System.Xml;

namespace CTMS.Services
{
    public class FileName
    {
        public async Task<DataTable> GetPatientMedicationsAsync(string chartNo, DateOnly orderEffectStartDate, DateOnly orderEffectEndDate)
        {
            PipServiceClient pipServiceClient = new();

            try
            {
                GetPatientMedicationsDT request = CreateGetPatientMedicationsRequest(chartNo, orderEffectStartDate, orderEffectEndDate);
                GetPatientMedicationsResponse response = await pipServiceClient.GetPatientMedicationsAsync(request);
                pipServiceClient.Close();
                return ConvertToDataTable(response.GetPatientMedicationsResult);
            }
            catch
            {
                pipServiceClient.Abort();
                throw;
            }
        }

        public async Task Get()
        {
            BusinessLogicClient businessLogicClient = new BusinessLogicClient();
            businessLogicClient.SYSPOWERGetLabdataByChartNoAsync("123456", "2024-01-01", "2024-12-31");

            DataTable medications = await GetPatientMedicationsAsync(
                "123456",
                new DateOnly(2024, 1, 1),
                new DateOnly(2024, 12, 31));

            foreach (DataRow row in medications.Rows)
            {
                _ = row;
            }
        }

        private static GetPatientMedicationsDT CreateGetPatientMedicationsRequest(
            string chartNo,
            DateOnly orderEffectStartDate,
            DateOnly orderEffectEndDate)
        {
            DataTable table = new("Table1");
            table.Columns.Add("Chart_No", typeof(string));
            table.Columns.Add("Order_Effect_Start_Date", typeof(string));
            table.Columns.Add("Order_Effect_End_Date", typeof(string));

            table.Rows.Add(
                chartNo,
                orderEffectStartDate.ToString("yyyy-MM-dd"),
                orderEffectEndDate.ToString("yyyy-MM-dd"));

            XmlDocument schemaDocument = new();
            using (MemoryStream schemaStream = new())
            {
                table.WriteXmlSchema(schemaStream);
                schemaStream.Position = 0;
                schemaDocument.Load(schemaStream);
            }

            XmlDocument diffgramDocument = new();
            using (MemoryStream diffgramStream = new())
            {
                table.WriteXml(diffgramStream, XmlWriteMode.DiffGram);
                diffgramStream.Position = 0;
                diffgramDocument.Load(diffgramStream);
            }

            return new GetPatientMedicationsDT
            {
                Any = schemaDocument.DocumentElement is null ? [] : [schemaDocument.DocumentElement],
                Any1 = diffgramDocument.DocumentElement
            };
        }

        private static DataTable ConvertToDataTable(GetPatientMedicationsResponseGetPatientMedicationsResult? result)
        {
            DataSet dataSet = new();

            if (result?.Any is { Length: > 0 })
            {
                XmlDocument schemaDocument = new();
                XmlNode schemaNode = schemaDocument.ImportNode(result.Any[0], true);
                schemaDocument.AppendChild(schemaNode);

                using XmlNodeReader schemaReader = new(schemaDocument);
                dataSet.ReadXmlSchema(schemaReader);
            }

            if (result?.Any1 is not null)
            {
                XmlDocument diffgramDocument = new();
                XmlNode diffgramNode = diffgramDocument.ImportNode(result.Any1, true);
                diffgramDocument.AppendChild(diffgramNode);

                using XmlNodeReader diffgramReader = new(diffgramDocument);
                dataSet.ReadXml(diffgramReader, XmlReadMode.DiffGram);
            }

            return dataSet.Tables.Count > 0 ? dataSet.Tables[0] : new DataTable();
        }
    }
}
