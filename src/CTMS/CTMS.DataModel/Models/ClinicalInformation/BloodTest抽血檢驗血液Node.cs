namespace CTMS.DataModel.Models.ClinicalInformation
{
    public class BloodTest抽血檢驗血液Node
    {
        public VisitCodeModel VisitCode { get; set; } = new();
        public List<TestItem檢驗項目> 抽血檢驗血液 { get; set; } = new();
    }
}
