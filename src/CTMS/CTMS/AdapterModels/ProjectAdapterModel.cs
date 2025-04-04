using CTMS.DataModel.Models;
using System.ComponentModel.DataAnnotations;

namespace CTMS.AdapterModels;

public class ProjectAdapterModel : ICloneable
{
    public int Id { get; set; }
    [Required(ErrorMessage = "名稱 不可為空白")]
    public string Name { get; set; } = String.Empty;

    public ProjectAdapterModel Clone()
    {
        return ((ICloneable)this).Clone() as ProjectAdapterModel;
    }
    object ICloneable.Clone()
    {
        return this.MemberwiseClone();
    }
}
