using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTMS.DataModel.Dtos
{
    public class MyUserDto : ICloneable, INotifyPropertyChanged
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "帳號 不可為空白")]
        public string Account { get; set; }
        [Required(ErrorMessage = "密碼 不可為空白")]
        public string Password { get; set; }
        [Required(ErrorMessage = "名稱 不可為空白")]
        public string Name { get; set; }
        public bool Status { get; set; } = true;
        public string? Email { get; set; }
        public bool IsAdmin { get; set; } = false;
        public string RoleJson { get; set; }

        #region 介面實作
        public event PropertyChangedEventHandler PropertyChanged;

        public MyUserDto Clone()
        {
            return ((ICloneable)this).Clone() as MyUserDto;
        }
        object ICloneable.Clone()
        {
            return this.MemberwiseClone();
        }
        #endregion
    }
}
