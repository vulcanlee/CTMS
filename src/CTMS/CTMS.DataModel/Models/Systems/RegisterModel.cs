using System.ComponentModel.DataAnnotations;

namespace CTMS.DataModel.Models.Systems;

public class RegisterModel
{
    [Required(ErrorMessage = "請輸入名稱")]
    public string Name { get; set; } = "";

    [Required(ErrorMessage = "請輸入帳號")]
    public string Account { get; set; } = "";

    [Required(ErrorMessage = "請輸入密碼")]
    public string Password { get; set; } = "";

    [Required(ErrorMessage = "請輸入電子郵件")]
    [EmailAddress(ErrorMessage = "Email 格式不正確")]
    public string Email { get; set; } = "";
    public string Status { get; set; } = "已申請";
    public string  CreateAt { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
}
