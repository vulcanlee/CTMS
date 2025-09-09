using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CTMS.DataModel.Models.AIAgent;

public class Phase3RiskAssessment
{
    public Phase3RiskAssessment()
    {
        Init();
    }
    public string Module { get; set; }
    public string Model { get; set; }
    public string VarName { get; set; }
    public string C { get; set; }
    public string I { get; set; }
    public string O { get; set; }
    public string WorkingDirectory { get; set; }
    public string Program { get; set; }

    public void Init()
    {
        Module = "Run_Endometrioid_Model";
        Model = "Endometrioid_Analysis_20250610_Model_data.RData";
        VarName = "CaseIn_SMA_Imat_BMI";
        C = "0.5";
        I = "Testing_data.csv";
        O = "output.csv";
        WorkingDirectory = "C:\\EndometrioidCancer";
        Program = "Rscript";
    }

    public string BuildCommand()
    {
        // Rscript Run_Endometrioid_Model.R -m Endometrioid_Analysis_20250610_Model_data.RData --varname CaseIn_SMA_Imat_BMI -c 0.5 -i Testing_data.csv -o output.csv
        string Result = $"{Program} {Module}.R -m {Model} --varname {VarName} -c {C} -i {I} -o {O}";
        return Result;
    }
}
