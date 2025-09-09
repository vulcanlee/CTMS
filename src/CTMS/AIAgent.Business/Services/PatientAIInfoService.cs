using AIAgent.Models;
using CTMS.DataModel.Models.AIAgent;
using CTMS.Share.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIAgent.Services
{
    public class PatientAIInfoService
    {
        public PatientAIInfoService()
        {
        }

        public async Task<PatientAIInfo> ReadAsync(string filename)
        {
            string json = await File.ReadAllTextAsync(filename);
            PatientAIInfo patientAIInfo = new PatientAIInfo().FromJson(json);
            return patientAIInfo;
        }

    }
}
