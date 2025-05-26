namespace CTMS.DataModel.Models.ClinicalInformation
{
    public class Survey問卷
    {
        public Survey 化療副作用 { get; set; } = new();
        public Survey 標靶副作用 { get; set; } = new();
        public Survey 放療副作用 { get; set; } = new();
        public Survey whooqol問卷 { get; set; } = new();
        public Survey 個人史問卷 { get; set; } = new();
        public Survey 家族史問卷 { get; set; } = new();
    }
}
