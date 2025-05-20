using CTMS.Share.Extensions;
using CTMS.Share.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTMS.DataModel.Models.ClinicalInformation
{
    public class Survey化療副作用
    {
        public string Title { get; set; }
        public List<Survey化療副作用Node> Items { get; set; } = new();
    }
}
