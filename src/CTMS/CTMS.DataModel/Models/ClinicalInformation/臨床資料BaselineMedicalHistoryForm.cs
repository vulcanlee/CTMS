using CTMS.Share.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CTMS.DataModel.Models.ClinicalInformation
{
    public class 臨床資料BaselineMedicalHistoryForm
    {
        public List<臨床資料BaselineMedicalHistoryFormNode> Items { get; set; }=new();
    }
}
