using CTMS.Share.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CTMS.DataModel.Models.ClinicalInformation
{
    public class BaselineMedicalHistoryForm
    {
        public List<BaselineMedicalHistoryFormNode> Items { get; set; }=new();
    }
}
