namespace CTMS.DataModel.Models.ClinicalInformation
{
    public class Survey個人史問卷
    {
        public string Title { get; set; }
        public List<Survey個人史問卷Node> Items { get; set; } = new();
    }
}
