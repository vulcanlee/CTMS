using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace CTMS.EntityModel.Models
{
    public class Examine
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string ExamineTime { get; set; }
        public string FilesData { get; set; }
        public string ExcelData { get; set; }

        public int AthleteId { get; set; }
        public Athlete Athlete { get; set; }
    }
}
