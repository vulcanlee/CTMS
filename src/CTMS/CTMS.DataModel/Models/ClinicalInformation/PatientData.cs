using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTMS.DataModel.Models.ClinicalInformation
{
    public class PatientData
    {
        public BasicClinicalPresentation_臨床資訊 臨床資訊 { get; set; } = new();
    }
}
