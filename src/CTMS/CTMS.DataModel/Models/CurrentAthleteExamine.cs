using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTMS.DataModel.Models
{
    public class CurrentAthleteExamine
    {
        public int AthleteId { get; set; }
        public int ExamineId { get; set; }
        public string ExamineTime { get; set; }=string.Empty;
        public List<string> ExamineList { get; set; } = new List<string>();

        public void Reset()
        {
            AthleteId = 0;
            ExamineId = 0;
            ExamineTime = string.Empty;
            ExamineList = new List<string>();
        }
    }
}
