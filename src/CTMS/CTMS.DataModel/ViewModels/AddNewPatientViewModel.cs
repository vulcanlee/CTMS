namespace CTMS.DataModel.ViewModels;

public class AddNewPatientViewModel
{
    public string 院別 { get; set; } = string.Empty;
    public DateTime Select收案日期 { get; set; } = DateTime.Now;
}
