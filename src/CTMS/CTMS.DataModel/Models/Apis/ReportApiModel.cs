namespace CTMS.DataModel.Models.Apis;

public class ReportApiModel
{
    public string ReportType { get; set; } = string.Empty;
    public string RequestNo { get; set; } = string.Empty;
    public string SpecKind { get; set; } = string.Empty;
    public string SpecName { get; set; } = string.Empty;
    public string ChargeTime { get; set; } = string.Empty;
    public string ExecuteTime { get; set; } = string.Empty;
    public string ReportTime { get; set; } = string.Empty;
    public string OrderCode { get; set; } = string.Empty;
    public string ProductCode { get; set; } = string.Empty;
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public string ReportText { get; set; } = string.Empty;
}
