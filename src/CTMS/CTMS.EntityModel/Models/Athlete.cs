using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace CTMS.EntityModel.Models
{
    public class Athlete
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string FilesData { get; set; }
        public string ExcelData { get; set; }
        public string ExamineTime { get; set; }

        public int? ProjectId { get; set; }
        public Project? Project { get; set; }
        public ICollection<Examine> Examine { get; } = new List<Examine>();
    }
}
