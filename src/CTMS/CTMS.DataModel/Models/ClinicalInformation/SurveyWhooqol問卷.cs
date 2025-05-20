using CTMS.Share.Extensions;
using CTMS.Share.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTMS.DataModel.Models.ClinicalInformation
{
    public class SurveyWhooqol問卷
    {
        public string Title { get; set; }
        public List<SurveyWhooqol問卷Node> Items { get; set; } = new();
    }
}
