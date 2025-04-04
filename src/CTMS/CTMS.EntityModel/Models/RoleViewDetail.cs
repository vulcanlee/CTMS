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
public class RoleViewDetail
{
    public RoleViewDetail()
    {
    }
    public int Id { get; set; }
    public int RoleViewId { get; set; }
    public int ProjectId { get; set; }
    public RoleView RoleView { get; set; } = null!;
    public Project Project { get; set; } = null!;
}
