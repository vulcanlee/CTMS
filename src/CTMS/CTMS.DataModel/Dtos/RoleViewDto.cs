using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTMS.DataModel.Dtos
{
    public class RoleViewDto : ICloneable, INotifyPropertyChanged
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "名稱 不可為空白")]
        public string Name { get; set; }
        public string TabViewJson { get; set; }

        #region 介面實作
        public event PropertyChangedEventHandler PropertyChanged;

        public RoleViewDto Clone()
        {
            return ((ICloneable)this).Clone() as RoleViewDto;
        }
        object ICloneable.Clone()
        {
            return this.MemberwiseClone();
        }
        #endregion
    }
}
