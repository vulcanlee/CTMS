using CTMS.Share.Extensions;
using CTMS.Share.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTMS.DataModel.Models.ClinicalInformation
{
    public class Survey個人史問卷Node
    {
        public VisitCodeModel VisitCode { get; set; } = new();
        public List<Question> Questions { get; set; }=new();
    }
}
