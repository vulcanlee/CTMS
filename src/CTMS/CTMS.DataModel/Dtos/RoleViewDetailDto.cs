using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTMS.DataModel.Dtos
{
    public class RoleViewDetailDto : ICloneable, INotifyPropertyChanged
    {
        public int Id { get; set; }
        public int RoleViewId { get; set; }
        public int ProjectId { get; set; }
        public RoleViewDto RoleView { get; set; } = null!;
        public ProjectDto Project { get; set; } = null!;

        #region 介面實作
        public event PropertyChangedEventHandler PropertyChanged;

        public RoleViewDetailDto Clone()
        {
            return ((ICloneable)this).Clone() as RoleViewDetailDto;
        }
        object ICloneable.Clone()
        {
            return this.MemberwiseClone();
        }
        #endregion
    }
}
