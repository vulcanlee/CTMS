using CTMS.DataModel.Models.Apis;
using LisServiceReference;
using Syncfusion.Blazor.Schedule;
using System.Data;
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

        #region Helper
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
                        OorderName = GetRowValue(row, nameof(BloodApiModel.OorderName), "OrderName", "OOrderName"),
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
                        OrderCode = GetRowValue(row, nameof(ReportApiModel.OrderCode), "Order_Code"),
                        ProductCode = GetRowValue(row, nameof(ReportApiModel.ProductCode), "Product_Code"),
                        ItemCode = GetRowValue(row, nameof(ReportApiModel.ItemCode)),
                        ItemName = GetRowValue(row, nameof(ReportApiModel.ItemName)),
                        ReportText = GetRowValue(row, nameof(ReportApiModel.ReportText), "ResultText", "TextReport", "Report_Content")
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
                nameof(BloodApiModel.OorderName),
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
                nameof(ReportApiModel.ProductCode),
                nameof(ReportApiModel.ItemCode),
                nameof(ReportApiModel.ItemName),
                nameof(ReportApiModel.ReportText),
                "ResultText",
                "TextReport",
                "Report_Content");
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

        #endregion
    }
}
