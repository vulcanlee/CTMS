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
public class Project
{
    public Project()
    {
    }
    public int Id { get; set; }
    [Required(ErrorMessage = "名稱 不可為空白")]
    public string Name { get; set; } = String.Empty;
    public ICollection<RoleViewProject> RoleViewProject { get; } = new List<RoleViewProject>();
    public ICollection<RoleView> RoleView { get; } = new List<RoleView>();
}
