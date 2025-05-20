namespace CTMS.DataModel.Models.ClinicalInformation
{
    public class Survey
    {
        public string Title { get; set; }
        public List<Question> Questions { get; set; } = new();
    }
}
