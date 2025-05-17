namespace CTMS.DataModel.Models.ClinicalInformation
{
    public class BloodTest抽血檢驗
    {
        public List<TestItem檢驗項目> 抽血檢驗血液 { get; set; } = new();
        public List<TestItem檢驗項目> 抽血檢驗生化 { get; set; } = new();
        public List<TestItem檢驗項目> 抽血檢驗生化2 { get; set; } = new();
    }
}
