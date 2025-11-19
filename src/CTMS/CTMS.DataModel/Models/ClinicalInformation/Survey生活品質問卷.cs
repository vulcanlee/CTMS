namespace CTMS.DataModel.Models.ClinicalInformation
{
    public class Survey生活品質問卷
    {
        public string Title { get; set; }
        public List<Survey生活品質問卷Node> Items { get; set; } = new();
    }
}
