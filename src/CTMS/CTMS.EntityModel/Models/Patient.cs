using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace CTMS.EntityModel.Models
{
    public class Patient
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string 醫院 { get; set; }
        public string 癌別 { get; set; }
        public string 組別 { get; set; }
        public string AI評估 { get; set; }
        public string JsonData { get; set; }

    }
}
