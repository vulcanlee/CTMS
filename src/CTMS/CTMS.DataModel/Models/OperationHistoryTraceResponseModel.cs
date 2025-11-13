using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CTMS.DataModel.Models;

public class OperationHistoryTraceResponseModel
{
    public string Summary { get; set; } = String.Empty;
    public string Detail { get; set; } = String.Empty;
    public bool IsChanged { get; set; } = false;
}
