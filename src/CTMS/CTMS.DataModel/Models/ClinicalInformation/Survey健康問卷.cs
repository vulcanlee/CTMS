namespace CTMS.DataModel.Models.ClinicalInformation
{
    public class Survey健康問卷
    {
        public string Title { get; set; }
        public List<Survey健康問卷Node> Items { get; set; } = new();
    }
}
