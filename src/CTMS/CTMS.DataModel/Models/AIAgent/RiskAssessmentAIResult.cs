using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CTMS.DataModel.Models.AIAgent;

public class RiskAssessmentAIResult
{
    public string ID { get; set; }
    public string Age { get; set; }
    public string BodyHeight { get; set; }
    public string BodyWeight { get; set; }
    public string VertebralBodyAreaCm2 { get; set; }
    public string TotalSMD { get; set; }
    public string TotalImatA { get; set; }
    public string TotalLamaA { get; set; }
    public string TotalNamaA { get; set; }
    public string VatA { get; set; }
    public string SatA { get; set; }
}