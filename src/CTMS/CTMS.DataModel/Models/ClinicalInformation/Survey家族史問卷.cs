namespace CTMS.DataModel.Models.ClinicalInformation
{
    public class Survey家族史問卷
    {
        public string Title { get; set; }
        public List<Survey家族史問卷Node> Items { get; set; } = new();
    }
}
