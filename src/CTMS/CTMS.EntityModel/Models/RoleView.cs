using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTMS.EntityModel.Models;

/// <summary>
/// 使用者
/// </summary>
public class RoleView
{
    public RoleView()
    {
    }
    public int Id { get; set; }
    [Required(ErrorMessage = "名稱 不可為空白")]
    public string Name { get; set; }
    [Required(ErrorMessage = "頁面可視權限 Json 不可為空白")]
    public string TabViewJson { get; set; }
    public ICollection<RoleViewProject> RoleViewProject { get; } = new List<RoleViewProject>();
    public ICollection<Project> Project { get; } = new List<Project>();
}
