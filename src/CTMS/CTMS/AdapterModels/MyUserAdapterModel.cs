using CTMS.DataModel.Models;
using CTMS.EntityModel.Models;
using System.ComponentModel.DataAnnotations;

namespace CTMS.AdapterModels;

public class MyUserAdapterModel : ICloneable
{
    public int Id { get; set; }
    [Required(ErrorMessage = "帳號 不可為空白")]
    public string Account { get; set; } = String.Empty;
    public string Password { get; set; } = String.Empty;
    public string PasswordPlaintext { get; set; } = String.Empty;
    [Required(ErrorMessage = "名稱 不可為空白")]
    public string Name { get; set; } = String.Empty;
    public string? Salt { get; set; }
    public bool Status { get; set; } = true;
    public string? Email { get; set; }
    public bool IsAdmin { get; set; } = false;
    public string RoleJson { get; set; }
    public RolePermission RolePermission { get; set; } = new();
    public int? RoleViewId { get; set; }
    public string RoleViewName { get; set; }
    public RoleViewAdapterModel? RoleView { get; set; }

    public MyUserAdapterModel Clone()
    {
        return ((ICloneable)this).Clone() as MyUserAdapterModel;
    }
    object ICloneable.Clone()
    {
        return this.MemberwiseClone();
    }
}
