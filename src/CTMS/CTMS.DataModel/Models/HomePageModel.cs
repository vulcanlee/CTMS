namespace CTMS.DataModel.Models;

public class HomePageModel
{
    public string 姓名 { get; set; }
    public string 性別 { get; set; }
    public string 年齡 { get; set; }
    public string 真實年齡
    {
        get
        {
            if (string.IsNullOrEmpty(生日))
            {
                return "未知";
            }

            if (!DateTime.TryParse(生日, out var birth))
            {
                return "格式不正確";
            }

            var now = DateTime.Now;
            var age = now.Year - birth.Year;
            if (now.Month < birth.Month || (now.Month == birth.Month && now.Day < birth.Day))
            {
                age--;
            }
            return age.ToString();
        }
    }
    public string 身高 { get; set; }
    public string 體重 { get; set; }
    public string BMI { get; set; }
    public string 運動類別 { get; set; }
    public string 擔任位置 { get; set; }
    public string 所屬隊伍 { get; set; }
    public string Photo { get; set; }
    public string 生日 { get; set; }
}
