using CTMS.DataModel.Models.Apis;
using LisServiceReference;
using System.Data;
using System.Globalization;
using System.Text;
using System.Xml;
using static CTMS.Components.Views.AdminTools.SystemMaintainView;

namespace CTMS.Services
{
    public class NckuhApiService
    {
        public WcfCallResult? LabDataResult { get; set; }

        private readonly ILogger<NckuhApiService> logger;
        private readonly BusinessLogicClient? lisClient = null;
        private readonly PipServiceReference.PipServiceClient? pipClient = null;

        public NckuhApiService(ILogger<NckuhApiService> logger)
        {
            this.logger = logger;
            lisClient = new BusinessLogicClient(BusinessLogicClient.EndpointConfiguration.BasicHttpBinding_IBusinessLogic);
            pipClient = new PipServiceReference.PipServiceClient(PipServiceReference.PipServiceClient.EndpointConfiguration.WSHttpBinding_IPipService);
        }

        public async Task<List<BloodApiModel>> GetBloodAsync(string chartNo, string beginDate, string endDate)
        {
            List<BloodApiModel> result = new List<BloodApiModel>();

            var labResponse = await lisClient.SYSPOWERGetLabdataByChartNoAsync(chartNo, beginDate, endDate);
            LabDataResult = BuildWcfCallResult(
                "SYSPOWERGetLabdataByChartNo",
                labResponse.SYSPOWERGetLabdataByChartNoResult?.Any,
                labResponse.SYSPOWERGetLabdataByChartNoResult?.Any1);

            MappingWcfToBloodModel(LabDataResult, result);

            return result;
        }

        public async Task<List<ReportApiModel>> GetReportAsync(string chartNo, string beginDate, string endDate)
        {
            List<ReportApiModel> result = new List<ReportApiModel>();

            var textResponse = await lisClient.SYSPOWERGetTextReportByChartNoAsync(chartNo, beginDate, endDate);
            var TextReportResult = BuildWcfCallResult(
                  "SYSPOWERGetTextReportByChartNo",
                  textResponse.SYSPOWERGetTextReportByChartNoResult?.Any,
                  textResponse.SYSPOWERGetTextReportByChartNoResult?.Any1);

            MappingWcfToReportModel(TextReportResult, result);

            return result;
        }

        public async Task<List<MedicationApiModel>> GetMedicationAsync(string chartNo, string beginDate, string endDate)
        {
            List<MedicationApiModel> result = new List<MedicationApiModel>();

            if (TryNormalizeMedicationQueryDateRange(beginDate, endDate, out var medicationBeginTime, out var medicationEndTime, out var medicationDateError))
            {
                var medicationRequest = BuildGetPatientMedicationsRequest(chartNo, medicationBeginTime, medicationEndTime);
                var MedicationRequestRawXml = BuildRawXml(medicationRequest.Any, medicationRequest.Any1);
                var medicationResponse = await pipClient.GetPatientMedicationsAsync(medicationRequest);
                var MedicationResult = BuildWcfCallResult(
                  "GetPatientMedications",
                  medicationResponse.GetPatientMedicationsResult?.Any,
                  medicationResponse.GetPatientMedicationsResult?.Any1);

                MappingWcfToMedicationModel(MedicationResult, result);
            }

            return result;
        }

        #region Helper

        private static bool TryNormalizeMedicationQueryDateRange(
            string beginTime,
            string endTime,
            out string normalizedBeginTime,
            out string normalizedEndTime,
            out string errorMessage)
        {
            if (!TryNormalizeMedicationQueryDate(beginTime, out normalizedBeginTime))
            {
                normalizedEndTime = string.Empty;
                errorMessage = "開始日期請輸入 yyyyMMdd、yyyy-MM-dd 或 yyyy/MM/dd。";
                return false;
            }

            if (!TryNormalizeMedicationQueryDate(endTime, out normalizedEndTime))
            {
                errorMessage = "結束日期請輸入 yyyyMMdd、yyyy-MM-dd 或 yyyy/MM/dd。";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }

        private static bool TryNormalizeMedicationQueryDate(string value, out string normalizedValue)
        {
            normalizedValue = string.Empty;

            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            var trimmedValue = value.Trim();
            var acceptedFormats = new[] { "yyyyMMdd", "yyyy-MM-dd", "yyyy/MM/dd" };

            if (!DateOnly.TryParseExact(trimmedValue, acceptedFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
            {
                return false;
            }

            normalizedValue = parsedDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            return true;
        }

        private static WcfCallResult BuildWcfCallResult(string methodName, XmlElement[]? schemaElements, XmlElement? diffgramElement)
        {
            var rawXml = BuildRawXml(schemaElements, diffgramElement);
            var result = new WcfCallResult
            {
                MethodName = methodName,
                RawXml = rawXml,
            };

            try
            {
                var dataSet = ReadDataSet(schemaElements, diffgramElement);
                var sortColumn = GetSortColumn(methodName);

                foreach (DataTable table in dataSet.Tables)
                {
                    result.Tables.Add(BuildTableResult(table, sortColumn));
                }

                if (result.Tables.Count == 0)
                {
                    result.ParseMessage = "沒有解析到任何 DataTable，請先查看 Raw XML。";
                }
            }
            catch (Exception ex)
            {
                result.ParseMessage = $"XML 解析失敗：{ex.Message}";
            }

            return result;
        }

        private static string BuildRawXml(XmlElement[]? schemaElements, XmlElement? diffgramElement)
        {
            var builder = new StringBuilder();

            if (schemaElements != null)
            {
                foreach (var schemaElement in schemaElements)
                {
                    builder.AppendLine(schemaElement.OuterXml);
                }
            }

            if (diffgramElement != null)
            {
                builder.AppendLine(diffgramElement.OuterXml);
            }

            return builder.ToString();
        }

        private static DataSet ReadDataSet(XmlElement[]? schemaElements, XmlElement? diffgramElement)
        {
            var dataSet = new DataSet();
            var document = new XmlDocument();
            var root = document.CreateElement("Root");
            document.AppendChild(root);

            if (schemaElements != null)
            {
                foreach (var schemaElement in schemaElements)
                {
                    root.AppendChild(document.ImportNode(schemaElement, true));
                }
            }

            if (diffgramElement != null)
            {
                root.AppendChild(document.ImportNode(diffgramElement, true));
            }

            using var reader = new XmlNodeReader(root);
            dataSet.ReadXml(reader, XmlReadMode.Auto);
            return dataSet;
        }

        private static string? GetSortColumn(string methodName)
        {
            return methodName switch
            {
                "SYSPOWERGetLabdataByChartNo" => "ReportTime",
                "SYSPOWERGetTextReportByChartNo" => "ReportTime",
                "GetPatientMedications" => "Order_Effect_Date",
                _ => null,
            };
        }

        private static WcfTableResult BuildTableResult(DataTable table, string? sortColumn = null)
        {
            var result = new WcfTableResult
            {
                TableName = string.IsNullOrWhiteSpace(table.TableName) ? "未命名資料表" : table.TableName,
            };

            foreach (DataColumn column in table.Columns)
            {
                result.Columns.Add(column.ColumnName);
            }

            var rows = GetOrderedRows(table, sortColumn);

            foreach (DataRow row in rows)
            {
                var rowValues = new Dictionary<string, string>();
                foreach (DataColumn column in table.Columns)
                {
                    rowValues[column.ColumnName] = row[column] == DBNull.Value ? string.Empty : row[column]?.ToString() ?? string.Empty;
                }
                result.Rows.Add(rowValues);
            }

            return result;
        }

        private static IEnumerable<DataRow> GetOrderedRows(DataTable table, string? sortColumn)
        {
            if (string.IsNullOrWhiteSpace(sortColumn) || !table.Columns.Contains(sortColumn))
            {
                foreach (DataRow row in table.Rows)
                {
                    yield return row;
                }

                yield break;
            }

            foreach (var row in table.Select(string.Empty, $"[{sortColumn}] ASC"))
            {
                yield return row;
            }
        }

        private void MappingWcfToBloodModel(WcfCallResult? labDataResult, List<BloodApiModel> result)
        {
            if (labDataResult == null)
            {
                logger.LogWarning("LabDataResult 為空，無法轉換為 BloodApiModel。");
                return;
            }

            if (labDataResult.Tables.Count == 0)
            {
                logger.LogWarning("LabDataResult 沒有任何資料表，MethodName={MethodName}, ParseMessage={ParseMessage}",
                    labDataResult.MethodName,
                    labDataResult.ParseMessage);
                return;
            }

            foreach (var table in labDataResult.Tables)
            {
                if (!IsBloodDataTable(table))
                {
                    continue;
                }

                foreach (var row in table.Rows)
                {
                    result.Add(new BloodApiModel
                    {
                        ReportType = GetRowValue(row, nameof(BloodApiModel.ReportType)),
                        RequestNo = GetRowValue(row, nameof(BloodApiModel.RequestNo)),
                        ChargeTime = GetRowValue(row, nameof(BloodApiModel.ChargeTime)),
                        ExecuteTime = GetRowValue(row, nameof(BloodApiModel.ExecuteTime)),
                        ReportTime = GetRowValue(row, nameof(BloodApiModel.ReportTime)),
                        SpecKind = GetRowValue(row, nameof(BloodApiModel.SpecKind)),
                        SpecName = GetRowValue(row, nameof(BloodApiModel.SpecName)),
                        OrderCode = GetRowValue(row, nameof(BloodApiModel.OrderCode), "Order_Code"),
                        OrderName = GetRowValue(row, nameof(BloodApiModel.OrderName)),
                        ItemCode = GetRowValue(row, nameof(BloodApiModel.ItemCode), "ProductCode"),
                        ItemName = GetRowValue(row, nameof(BloodApiModel.ItemName)),
                        TestValue = GetRowValue(row, nameof(BloodApiModel.TestValue), "ResultValue"),
                        Unit = GetRowValue(row, nameof(BloodApiModel.Unit)),
                        NormalCheck = GetRowValue(row, nameof(BloodApiModel.NormalCheck)),
                        PanicCheck = GetRowValue(row, nameof(BloodApiModel.PanicCheck)),
                        Remark = GetRowValue(row, nameof(BloodApiModel.Remark), "Comments"),
                        RefData = GetRowValue(row, nameof(BloodApiModel.RefData), "ReferenceData", "RefRange")
                    });
                }
            }
        }

        private void MappingWcfToReportModel(WcfCallResult? textReportResult, List<ReportApiModel> result)
        {
            if (textReportResult == null)
            {
                logger.LogWarning("TextReportResult 為空，無法轉換為 ReportApiModel。");
                return;
            }

            if (textReportResult.Tables.Count == 0)
            {
                logger.LogWarning("TextReportResult 沒有任何資料表，MethodName={MethodName}, ParseMessage={ParseMessage}",
                    textReportResult.MethodName,
                    textReportResult.ParseMessage);
                return;
            }

            foreach (var table in textReportResult.Tables)
            {
                if (!IsReportDataTable(table))
                {
                    continue;
                }

                foreach (var row in table.Rows)
                {
                    result.Add(new ReportApiModel
                    {
                        ReportType = GetRowValue(row, nameof(ReportApiModel.ReportType)),
                        RequestNo = GetRowValue(row, nameof(ReportApiModel.RequestNo)),
                        SpecKind = GetRowValue(row, nameof(ReportApiModel.SpecKind)),
                        SpecName = GetRowValue(row, nameof(ReportApiModel.SpecName)),
                        ChargeTime = GetRowValue(row, nameof(ReportApiModel.ChargeTime)),
                        ExecuteTime = GetRowValue(row, nameof(ReportApiModel.ExecuteTime)),
                        ReportTime = GetRowValue(row, nameof(ReportApiModel.ReportTime)),
                        OrderCode = GetRowValue(row, nameof(ReportApiModel.OrderCode)),
                        OrderName = GetRowValue(row, nameof(ReportApiModel.OrderName)),
                        ItemCode = GetRowValue(row, nameof(ReportApiModel.ItemCode)),
                        ItemName = GetRowValue(row, nameof(ReportApiModel.ItemName)),
                        ReportText = GetRowValue(row, nameof(ReportApiModel.ReportText))
                    });
                }
            }
        }

        private void MappingWcfToMedicationModel(WcfCallResult? medicationResult, List<MedicationApiModel> result)
        {
            if (medicationResult == null)
            {
                logger.LogWarning("MedicationResult 為空，無法轉換為 MedicationApiModel。");
                return;
            }

            if (medicationResult.Tables.Count == 0)
            {
                logger.LogWarning("MedicationResult 沒有任何資料表，MethodName={MethodName}, ParseMessage={ParseMessage}",
                    medicationResult.MethodName,
                    medicationResult.ParseMessage);
                return;
            }

            foreach (var table in medicationResult.Tables)
            {
                if (!IsMedicationDataTable(table))
                {
                    continue;
                }

                foreach (var row in table.Rows)
                {
                    result.Add(new MedicationApiModel
                    {
                        Chart_No = GetRowValue(row, nameof(MedicationApiModel.Chart_No)),
                        Order_Source = GetRowValue(row, nameof(MedicationApiModel.Order_Source)),
                        Order_Code = GetRowValue(row, nameof(MedicationApiModel.Order_Code)),
                        Pharmacy_Name = GetRowValue(row, nameof(MedicationApiModel.Pharmacy_Name)),
                        Scientific_Name = GetRowValue(row, nameof(MedicationApiModel.Scientific_Name)),
                        Order_Effect_Date = GetRowValue(row, nameof(MedicationApiModel.Order_Effect_Date)),
                        Order_End_Date = GetRowValue(row, nameof(MedicationApiModel.Order_End_Date)),
                        Frequency_Code = GetRowValue(row, nameof(MedicationApiModel.Frequency_Code)),
                        Usage_Code = GetRowValue(row, nameof(MedicationApiModel.Usage_Code)),
                        Tqty_Unit = GetRowValue(row, nameof(MedicationApiModel.Tqty_Unit)),
                        Dosage_Unit = GetRowValue(row, nameof(MedicationApiModel.Dosage_Unit)),
                        Days = GetRowValue(row, nameof(MedicationApiModel.Days)),
                        Totally_Dosage_Unit = GetRowValue(row, nameof(MedicationApiModel.Totally_Dosage_Unit)),
                        Prescription_Type = GetRowValue(row, nameof(MedicationApiModel.Prescription_Type)),
                        Doctor_Code = GetRowValue(row, nameof(MedicationApiModel.Doctor_Code)),
                        Employee_Name = GetRowValue(row, nameof(MedicationApiModel.Employee_Name)),
                        ATC_Code = GetRowValue(row, nameof(MedicationApiModel.ATC_Code)),
                        Medicine_Type = GetRowValue(row, nameof(MedicationApiModel.Medicine_Type)),
                        Medicine_ID = GetRowValue(row, nameof(MedicationApiModel.Medicine_ID)),
                        chronic_medicine = GetRowValue(row, nameof(MedicationApiModel.chronic_medicine), "Chronic_Medicine"),
                        Hospital_Code = GetRowValue(row, nameof(MedicationApiModel.Hospital_Code)),
                        Hospital_Name = GetRowValue(row, nameof(MedicationApiModel.Hospital_Name))
                    });
                }
            }
        }

        private static bool IsBloodDataTable(WcfTableResult table)
        {
            return ContainsColumn(table,
                nameof(BloodApiModel.ReportType),
                nameof(BloodApiModel.RequestNo),
                nameof(BloodApiModel.OrderCode),
                nameof(BloodApiModel.OrderName),
                nameof(BloodApiModel.ItemCode),
                nameof(BloodApiModel.ItemName),
                nameof(BloodApiModel.TestValue));
        }

        private static bool IsReportDataTable(WcfTableResult table)
        {
            return ContainsColumn(table,
                nameof(ReportApiModel.ReportType),
                nameof(ReportApiModel.RequestNo),
                nameof(ReportApiModel.OrderCode),
                nameof(ReportApiModel.OrderName),
                nameof(ReportApiModel.ItemCode),
                nameof(ReportApiModel.ItemName),
                nameof(ReportApiModel.ReportText),
                "ResultText",
                "TextReport",
                "Report_Content");
        }

        private static bool IsMedicationDataTable(WcfTableResult table)
        {
            return ContainsColumn(table,
                nameof(MedicationApiModel.Chart_No),
                nameof(MedicationApiModel.Order_Code),
                nameof(MedicationApiModel.Pharmacy_Name),
                nameof(MedicationApiModel.Scientific_Name),
                nameof(MedicationApiModel.Order_Effect_Date),
                nameof(MedicationApiModel.Order_End_Date));
        }

        private static bool ContainsColumn(WcfTableResult table, params string[] columnNames)
        {
            foreach (var column in table.Columns)
            {
                foreach (var columnName in columnNames)
                {
                    if (string.Equals(column, columnName, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static string GetRowValue(Dictionary<string, string> row, params string[] keys)
        {
            foreach (var key in keys)
            {
                foreach (var item in row)
                {
                    if (string.Equals(item.Key, key, StringComparison.OrdinalIgnoreCase))
                    {
                        return item.Value ?? string.Empty;
                    }
                }
            }

            return string.Empty;
        }

        private static XmlElement[] ConvertDataTableToSchemaElements(DataTable dataTable)
        {
            using var memoryStream = new MemoryStream();
            dataTable.WriteXmlSchema(memoryStream);
            memoryStream.Position = 0;

            var schemaDocument = new XmlDocument();
            schemaDocument.Load(memoryStream);

            return schemaDocument.DocumentElement is null ? [] : [schemaDocument.DocumentElement];
        }

        private static XmlElement ConvertDataTableToDiffgramElement(DataTable dataTable)
        {
            using var memoryStream = new MemoryStream();
            dataTable.WriteXml(memoryStream, XmlWriteMode.DiffGram);
            memoryStream.Position = 0;

            var diffgramDocument = new XmlDocument();
            diffgramDocument.Load(memoryStream);

            return diffgramDocument.DocumentElement!;
        }

        private static PipServiceReference.GetPatientMedicationsDT BuildGetPatientMedicationsRequest(string chartNo, string orderEffectStartDate, string orderEffectEndDate)
        {
            var table = new DataTable("Table1");

            table.Columns.Add("Chart_No", typeof(string));
            table.Columns.Add("Order_Effect_Start_Date", typeof(string));
            table.Columns.Add("Order_Effect_End_Date", typeof(string));
            table.Columns.Add("Order_End_Start_Date", typeof(string));
            table.Columns.Add("Order_End_End_Date", typeof(string));

            table.Rows.Add(chartNo, orderEffectStartDate, orderEffectEndDate, "", "");

            var schemaElements = ConvertDataTableToSchemaElements(table);
            var diffgramElement = ConvertDataTableToDiffgramElement(table);

            return new PipServiceReference.GetPatientMedicationsDT
            {
                Any = schemaElements,
                Any1 = diffgramElement,
            };
        }

        #endregion
    }
}
