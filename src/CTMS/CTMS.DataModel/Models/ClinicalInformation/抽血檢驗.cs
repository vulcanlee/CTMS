namespace CTMS.DataModel.Models.ClinicalInformation
{
    public class 抽血檢驗
    {
        public List<檢驗項目> 抽血檢驗血液 { get; set; } = new();
        public List<檢驗項目> 抽血檢驗生化 { get; set; } = new();
        public List<檢驗項目> 抽血檢驗生化2 { get; set; } = new();
    }
}
