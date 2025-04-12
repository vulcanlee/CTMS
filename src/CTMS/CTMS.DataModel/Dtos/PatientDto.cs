using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTMS.DataModel.Dtos
{
    public class PatientDto : ICloneable, INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string 醫院 { get; set; }
        public string 癌別 { get; set; }
        public string JsonData { get; set; }

        #region 介面實作
        public event PropertyChangedEventHandler PropertyChanged;

        public PatientDto Clone()
        {
            return ((ICloneable)this).Clone() as PatientDto;
        }
        object ICloneable.Clone()
        {
            return this.MemberwiseClone();
        }
        #endregion
    }
}
