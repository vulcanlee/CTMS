using CTMS.DataModel.Dtos;
using CTMS.DataModel.Models;
using System.ComponentModel.DataAnnotations;

namespace CTMS.AdapterModels;

public class RoleViewDetailAdapterModel : ICloneable
{
    public int Id { get; set; }
    public int RoleViewId { get; set; }
    public int ProjectId { get; set; }
    public string ProjectName { get; set; }

    public RoleViewAdapterModel RoleView { get; set; } = null!;
    public ProjectAdapterModel Project { get; set; } = null!;
    public RoleViewDetailAdapterModel Clone()
    {
        return ((ICloneable)this).Clone() as RoleViewDetailAdapterModel;
    }
    object ICloneable.Clone()
    {
        return this.MemberwiseClone();
    }
}
