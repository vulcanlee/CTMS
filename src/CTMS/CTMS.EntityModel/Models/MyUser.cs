﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTMS.EntityModel.Models;

/// <summary>
/// 使用者
/// </summary>
public class MyUser
{
    public MyUser()
    {
    }
    public int Id { get; set; }
    [Required(ErrorMessage = "帳號 不可為空白")]
    public string Account { get; set; } = String.Empty;
    [Required(ErrorMessage = "密碼 不可為空白")]
    public string Password { get; set; } = String.Empty;
    [Required(ErrorMessage = "名稱 不可為空白")]
    public string Name { get; set; } = String.Empty;
    public string? Salt { get; set; }
    public bool Status { get; set; }=true;
    public string? Email { get; set; }
    public bool IsAdmin { get; set; }=false;
    public string RoleJson { get; set; }
    public int? RoleViewId { get; set; }
    public RoleView? RoleView { get; set; }
}
