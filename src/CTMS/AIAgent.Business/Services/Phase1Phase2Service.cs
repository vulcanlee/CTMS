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
    public class Phase1Phase2Service
    {
        public Phase1Phase2Service()
        {
        }

        public Phase1LabelGeneration BuildPhase1標註生成Json(PatientAIInfo patientAIInfo,
            Agentsetting agentsetting)
        {
            var phase1LabelGeneration = new Phase1LabelGeneration()
            {
                optional = new PhaseOptional()
                {
                    age = new List<string> { patientAIInfo.Age },
                    gender = new List<string> { patientAIInfo.Gender },
                    height = new List<string> { patientAIInfo.Height },
                    weight = new List<string> { patientAIInfo.Weight },
                },
                files = new List<string> { patientAIInfo.DicomFilename },
                tmp_folder = Path.Combine(agentsetting.GetPhase1TmpFolderPath(), patientAIInfo.KeyName),
            };
            return phase1LabelGeneration;
        }

        public void SavePhase1標註生成Json(Phase1LabelGeneration phase1LabelGeneration,
            PatientAIInfo patientAIInfo,
            Agentsetting agentsetting)
        {
            string json = phase1LabelGeneration.ToJson();
            string fileName = $"{patientAIInfo.KeyName}.json";
            string fullPath = Path.Combine(agentsetting.GetInferencePath(), fileName);
            File.WriteAllText(fullPath, json);
            return ;
        }
    }
}
