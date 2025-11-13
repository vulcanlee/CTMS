using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace CTMS.EntityModel.Models;

[Index(nameof(CreateAt))]
public class OperationHistoryTrace
{
    public int Id { get; set; }
    public string User { get; set; }
    public string? SubjectCode { get; set; }
    public string Category { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime CreateAt { get; set; }

}
