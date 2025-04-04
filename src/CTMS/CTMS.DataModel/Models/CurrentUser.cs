﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CTMS.DataModel.Models;

public class CurrentUser
{
    public CurrentUser()
    {
    }
    public int Id { get; set; }
    public string Account { get; set; } = String.Empty;
    public string Name { get; set; } = String.Empty;
    public bool Status { get; set; } = true;
    public string? Email { get; set; }
    public bool IsAdmin { get; set; } = false;
    public string RoleJson { get; set; }
    public bool IsAuthenticated { get; set; } = false;
    public List<string> RoleList { get; set; } = new();

    public void CopyFrom(CurrentUser source)
    {
        foreach (PropertyInfo property in typeof(CurrentUser).GetProperties())
        {
            if (property.CanWrite)
            {
                property.SetValue(this, property.GetValue(source));
            }
        }

        RoleList = JsonSerializer.Deserialize<List<string>>(this.RoleJson);
    }
}
