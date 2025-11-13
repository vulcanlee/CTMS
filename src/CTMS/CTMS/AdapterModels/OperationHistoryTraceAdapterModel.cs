using CTMS.DataModel.Models;
using CTMS.EntityModel.Models;
using CTMS.Share.Helpers;
using System.ComponentModel.DataAnnotations;

namespace CTMS.AdapterModels;

public class OperationHistoryTraceAdapterModel : ICloneable
{
    public int Id { get; set; }
    public string User { get; set; }
    public string? SubjectCode { get; set; }
    public string Category { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime CreateAt { get; set; }

    public OperationHistoryTraceAdapterModel Clone()
    {
        return ((ICloneable)this).Clone() as OperationHistoryTraceAdapterModel;
    }
    object ICloneable.Clone()
    {
        return this.MemberwiseClone();
    }

    public static OperationHistoryTraceAdapterModel Build(string user, string subjectCode, string category, string name, string description)
    {
        return new OperationHistoryTraceAdapterModel
        {
            User = user,
            SubjectCode = subjectCode,
            Category = category,
            Name = name,
            Description = description,
            CreateAt = DateTime.Now
        };
    }
}
