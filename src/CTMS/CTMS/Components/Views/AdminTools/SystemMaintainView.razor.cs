using AntDesign;
using CTMS.Business.Helpers;
using CTMS.DataModel.Models;
using CTMS.DataModel.Models.Systems;
using CTMS.Services;
using LisServiceReference;
using Microsoft.AspNetCore.Components;
using System.Data;
using System.Globalization;
using System.IO;
using System.ServiceModel;
using System.Text;
using System.Xml;

namespace CTMS.Components.Views.AdminTools;

public partial class SystemMaintainView
{
    [Inject]
    public ILogger<SystemMaintainView> logger { get; set; }
    [Inject]
    public SystemMaintainServices systemMaintainServices { get; set; }
    [Inject]
    public ModalService modalService { get; set; }
    [Inject]
    public NckuhApiService NckuhApiService { get; set; }

    public string ApiTestChartNo { get; set; } = "23061697";
    public string ApiTestBeginTime { get; set; } = "20251001";
    public string ApiTestEndTime { get; set; } = "20260401";
    public bool IsApiCalling { get; set; }
    public string ApiCallMessage { get; set; } = string.Empty;
    public WcfCallResult? LabDataResult { get; set; }
    public WcfCallResult? TextReportResult { get; set; }
    public WcfCallResult? MedicationResult { get; set; }
    public string MedicationRequestRawXml { get; set; } = string.Empty;
    public string MedicationRequestSummary { get; set; } = string.Empty;

    public async Task OnTestApi()
    {
        await NckuhApiService.GetBloodAsync(ApiTestChartNo, ApiTestBeginTime, ApiTestEndTime);
        //await NckuhApiService.GetReportAsync(ApiTestChartNo, ApiTestBeginTime, ApiTestEndTime);
        //await NckuhApiService.GetMedicationAsync(ApiTestChartNo, ApiTestBeginTime, ApiTestEndTime);
    }

    public async Task OnApi呼叫測試()
    {
        ApiCallMessage = string.Empty;
        LabDataResult = null;
        TextReportResult = null;
        MedicationResult = null;
        MedicationRequestRawXml = string.Empty;
        MedicationRequestSummary = string.Empty;

        if (string.IsNullOrWhiteSpace(ApiTestChartNo))
        {
            ApiCallMessage = "請先輸入 chartNo。";
            return;
        }

        IsApiCalling = true;

        BusinessLogicClient? lisClient = null;
        PipServiceReference.PipServiceClient? pipClient = null;

        try
        {
            var chartNo = ApiTestChartNo.Trim();
            var beginTime = ApiTestBeginTime.Trim();
            var endTime = ApiTestEndTime.Trim();

            lisClient = new BusinessLogicClient(BusinessLogicClient.EndpointConfiguration.BasicHttpBinding_IBusinessLogic);
            pipClient = new PipServiceReference.PipServiceClient(PipServiceReference.PipServiceClient.EndpointConfiguration.WSHttpBinding_IPipService);

            var labResponse = await lisClient.SYSPOWERGetLabdataByChartNoAsync(chartNo, beginTime, endTime);
            LabDataResult = BuildWcfCallResult(
                "SYSPOWERGetLabdataByChartNo",
                labResponse.SYSPOWERGetLabdataByChartNoResult?.Any,
                labResponse.SYSPOWERGetLabdataByChartNoResult?.Any1);

            var textResponse = await lisClient.SYSPOWERGetTextReportByChartNoAsync(chartNo, beginTime, endTime);
            TextReportResult = BuildWcfCallResult(
                "SYSPOWERGetTextReportByChartNo",
                textResponse.SYSPOWERGetTextReportByChartNoResult?.Any,
                textResponse.SYSPOWERGetTextReportByChartNoResult?.Any1);

            if (TryNormalizeMedicationQueryDateRange(beginTime, endTime, out var medicationBeginTime, out var medicationEndTime, out var medicationDateError))
            {
                var medicationRequest = BuildGetPatientMedicationsRequest(chartNo, medicationBeginTime, medicationEndTime);
                MedicationRequestRawXml = BuildRawXml(medicationRequest.Any, medicationRequest.Any1);
                MedicationRequestSummary = $"Chart_No={chartNo}, Order_Effect_Start_Date={medicationBeginTime}, Order_Effect_End_Date={medicationEndTime}";
                var medicationResponse = await pipClient.GetPatientMedicationsAsync(medicationRequest);
                MedicationResult = BuildWcfCallResult(
                    "GetPatientMedications",
                    medicationResponse.GetPatientMedicationsResult?.Any,
                    medicationResponse.GetPatientMedicationsResult?.Any1);
                ApiCallMessage = "WCF 呼叫完成。";
            }
            else
            {
                ApiCallMessage = $"LIS WCF 呼叫完成；用藥查詢日期格式錯誤：{medicationDateError}";
            }

            ((ICommunicationObject)lisClient).Close();
            ((ICommunicationObject)pipClient).Close();
        }
        catch (Exception ex)
        {
            lisClient?.Abort();
            pipClient?.Abort();
            logger.LogError(ex, "呼叫 LIS/PIP WCF 服務失敗，chartNo={ChartNo}", ApiTestChartNo);
            ApiCallMessage = $"WCF 呼叫失敗：{ex.Message}";
        }
        finally
        {
            IsApiCalling = false;
        }
    }

    public async Task OnFix成大抽血生化eGFR參考區間()
    {
        var ok = await modalService.ConfirmAsync(new ConfirmOptions
        {
            Title = "再次確認",
            Content = "確定要刪除這個 成大抽血生化_eGFR參考區間修正 需求嗎？",
            OkText = "是",
            CancelText = "取消",
            OkButtonProps = new ButtonProps { Danger = true },
            MaskClosable = false
        });

        if (ok)
        {
            await systemMaintainServices.Fix_20260326_成大抽血生化_eGFR參考區間修正();
        }
    }

    public async Task OnWhooqol問卷缺少數量的修正()
    {
        var ok = await modalService.ConfirmAsync(new ConfirmOptions
        {
            Title = "再次確認",
            Content = "確定要修正這個 Whooqol問卷缺少數量的需求嗎？",
            OkText = "是",
            CancelText = "取消",
            OkButtonProps = new ButtonProps { Danger = true },
            MaskClosable = false
        });

        if (ok)
        {
            await systemMaintainServices.Fix_20260518_Whooqol問卷缺少數量的修正();
        }
    }

    public async Task On奇美與郭綜合抽血項目修正()
    {
        var ok = await modalService.ConfirmAsync(new ConfirmOptions
        {
            Title = "再次確認",
            Content = "確定要修正這個 奇美與郭綜合抽血項目修正 的需求嗎？",
            OkText = "是",
            CancelText = "取消",
            OkButtonProps = new ButtonProps { Danger = true },
            MaskClosable = false
        });

        if (ok)
        {
            await systemMaintainServices.Fix_20260518_奇美與郭綜合抽血項目修正();
        }
    }

    public async Task OnFillBloodTestUnits()
    {
        var ok = await modalService.ConfirmAsync(new ConfirmOptions
        {
            Title = "再次確認",
            Content = "確定要補齊所有既有抽血血液/生化紀錄的單位嗎？",
            OkText = "是",
            CancelText = "取消",
            OkButtonProps = new ButtonProps { Danger = true },
            MaskClosable = false
        });

        if (ok)
        {
            await systemMaintainServices.FillBloodTestUnitsAsync();
        }
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

    public class WcfCallResult
    {
        public string MethodName { get; set; } = string.Empty;
        public string RawXml { get; set; } = string.Empty;
        public string ParseMessage { get; set; } = string.Empty;
        public List<WcfTableResult> Tables { get; set; } = new();
    }

    public class WcfTableResult
    {
        public string TableName { get; set; } = string.Empty;
        public List<string> Columns { get; set; } = new();
        public List<Dictionary<string, string>> Rows { get; set; } = new();
    }
}
