using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTMS.DataModel.Dtos;

public class OperationHistoryTraceDto : ICloneable, INotifyPropertyChanged
{
    public int Id { get; set; }
    public string User { get; set; }
    public string? SubjectCode { get; set; }
    public string Category { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime CreateAt { get; set; }

    #region 介面實作
    public event PropertyChangedEventHandler PropertyChanged;

    public OperationHistoryTraceDto Clone()
    {
        return ((ICloneable)this).Clone() as OperationHistoryTraceDto;
    }
    object ICloneable.Clone()
    {
        return this.MemberwiseClone();
    }
    #endregion
}
