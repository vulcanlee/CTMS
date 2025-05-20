namespace CTMS.DataModel.Models.ClinicalInformation
{
    public class BloodTest抽血檢驗生化Node
    {
        public VisitCodeModel VisitCode { get; set; } = new();
        public List<TestItem檢驗項目> 抽血檢驗生化 { get; set; } = new();
    }
}
