using CTMS.DataModel.Models;
using CTMS.DataModel.Models.ClinicalInformation;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CTMS.Business.Services.ClinicalInformation
{
    public class DropDownListDataService
    {
        #region Basic clinical presentation 臨床資訊
        public List<DropDownListDataModel> GetAge()
        {
            List<DropDownListDataModel> result = new List<DropDownListDataModel>();
            for (int i = 20; i <= 80; i++)
            {
                result.Add(new DropDownListDataModel() { Key = $"{i}", Name = $"{i}" });
            }
            return result;
        }

        public List<DropDownListDataModel> Get癌別()
        {
            List<DropDownListDataModel> result = new List<DropDownListDataModel>();
            result.Add(new DropDownListDataModel() { Key = $"子宮內膜癌", Name = $"子宮內膜癌" });
            result.Add(new DropDownListDataModel() { Key = $"卵巢癌", Name = $"卵巢癌" });
            return result;
        }

        public List<DropDownListDataModel> Get院別()
        {
            List<DropDownListDataModel> result = new List<DropDownListDataModel>();
            result.Add(new DropDownListDataModel() { Key = $"成大醫院", Name = $"成大醫院" });
            result.Add(new DropDownListDataModel() { Key = $"奇美醫院", Name = $"奇美醫院" });
            result.Add(new DropDownListDataModel() { Key = $"郭綜合醫院", Name = $"郭綜合醫院" }); 
            return result;
        }

        public List<DropDownListDataModel> Get月經狀態()
        {
            List<DropDownListDataModel> result = new List<DropDownListDataModel>();
            result.Add(new DropDownListDataModel() { Key = $"停經", Name = $"0 停經" });
            result.Add(new DropDownListDataModel() { Key = $"未停經", Name = $"1 未停經" });
            return result;
        }

        public List<DropDownListDataModel> GetYesNo()
        {
            List<DropDownListDataModel> result = new List<DropDownListDataModel>();
            result.Add(new DropDownListDataModel() { Key = $"Yes", Name = $"Yes" });
            result.Add(new DropDownListDataModel() { Key = $"No", Name = $"No" });
            return result;
        }

        public List<string> GetStringYesNo()
        {
            List<string> result = new List<string>();
            result.Add("Yes");
            result.Add("No");
            return result;
        }

        public List<DropDownListDataModel> Get身高Height()
        {
            List<DropDownListDataModel> result = new List<DropDownListDataModel>();
            for (int i = 140; i <= 180; i++)
            {
                result.Add(new DropDownListDataModel() { Key = $"{i}", Name = $"{i}cm" });
            }
            return result;
        }

        public List<DropDownListDataModel> Get體重BW()
        {
            List<DropDownListDataModel> result = new List<DropDownListDataModel>();
            for (int i = 30; i <= 120; i++)
            {
                result.Add(new DropDownListDataModel() { Key = $"{i}", Name = $"{i}kg" });
            }
            return result;
        }

        public List<DropDownListDataModel> GetFIGO癌症分期()
        {
            List<DropDownListDataModel> result = new List<DropDownListDataModel>();
            result.Add(new DropDownListDataModel() { Key = $"I", Name = $"I" });
            result.Add(new DropDownListDataModel() { Key = $"IA", Name = $"IA" });
            result.Add(new DropDownListDataModel() { Key = $"IB", Name = $"IB" });
            result.Add(new DropDownListDataModel() { Key = $"II", Name = $"II" });
            result.Add(new DropDownListDataModel() { Key = $"III", Name = $"III" });
            result.Add(new DropDownListDataModel() { Key = $"IIIA", Name = $"IIIA" });
            result.Add(new DropDownListDataModel() { Key = $"IIIB", Name = $"IIIB" });
            result.Add(new DropDownListDataModel() { Key = $"IIIC", Name = $"IIIC" });
            result.Add(new DropDownListDataModel() { Key = $"IIIC1", Name = $"IIIC1" });
            result.Add(new DropDownListDataModel() { Key = $"IIIC2", Name = $"IIIC2" });
            result.Add(new DropDownListDataModel() { Key = $"IV", Name = $"IV" });
            result.Add(new DropDownListDataModel() { Key = $"IVA", Name = $"IVA" });
            result.Add(new DropDownListDataModel() { Key = $"IVB", Name = $"IVB" });
            return result;
        }

        public List<DropDownListDataModel> GetFIGO癌症分期子宮內膜癌EndometrialCancer()
        {
            List<DropDownListDataModel> result = new List<DropDownListDataModel>();
            result.Add(new DropDownListDataModel() { Key = $"IA", Name = $"IA" });
            result.Add(new DropDownListDataModel() { Key = $"IB", Name = $"IB" });
            result.Add(new DropDownListDataModel() { Key = $"IC", Name = $"IC" });
            result.Add(new DropDownListDataModel() { Key = $"IIA", Name = $"IIA" });
            result.Add(new DropDownListDataModel() { Key = $"IIB", Name = $"IIB" });
            result.Add(new DropDownListDataModel() { Key = $"IIC", Name = $"IIC" });
            result.Add(new DropDownListDataModel() { Key = $"IIIA", Name = $"IIIA" });
            result.Add(new DropDownListDataModel() { Key = $"IIIB", Name = $"IIIB" });
            result.Add(new DropDownListDataModel() { Key = $"IIIC1", Name = $"IIIC1" });
            result.Add(new DropDownListDataModel() { Key = $"IIIC1i", Name = $"IIIC1i" });
            result.Add(new DropDownListDataModel() { Key = $"IIIC1ii", Name = $"IIIC1ii" });
            result.Add(new DropDownListDataModel() { Key = $"IIIC2", Name = $"IIIC2" });
            result.Add(new DropDownListDataModel() { Key = $"IIIC2i", Name = $"IIIC2i" });
            result.Add(new DropDownListDataModel() { Key = $"IIIC2ii", Name = $"IIIC2ii" });
            result.Add(new DropDownListDataModel() { Key = $"IVA", Name = $"IVA" });
            result.Add(new DropDownListDataModel() { Key = $"IVB", Name = $"IVB" });
            result.Add(new DropDownListDataModel() { Key = $"IA3", Name = $"IA3" });
            result.Add(new DropDownListDataModel() { Key = $"IAmPOLEmut", Name = $"IAmPOLEmut" });
            result.Add(new DropDownListDataModel() { Key = $"IICmp53abn", Name = $"IICmp53abn" });

            return result;
        }

        public List<DropDownListDataModel> GetFIGO癌症分期卵巢癌OvarianCancer()
        {
            List<DropDownListDataModel> result = new List<DropDownListDataModel>();
            result.Add(new DropDownListDataModel() { Key = $"IA", Name = $"IA" });
            result.Add(new DropDownListDataModel() { Key = $"IB", Name = $"IB" });
            result.Add(new DropDownListDataModel() { Key = $"IC1", Name = $"IC1" });
            result.Add(new DropDownListDataModel() { Key = $"IC2", Name = $"IC2" });
            result.Add(new DropDownListDataModel() { Key = $"IC3", Name = $"IC3" });
            result.Add(new DropDownListDataModel() { Key = $"IIA", Name = $"IIA" });
            result.Add(new DropDownListDataModel() { Key = $"IIB", Name = $"IIB" });
            result.Add(new DropDownListDataModel() { Key = $"IIIA1(i)", Name = $"IIIA1(i)" });
            result.Add(new DropDownListDataModel() { Key = $"IIIA1(ii)", Name = $"IIIA1(ii)" });
            result.Add(new DropDownListDataModel() { Key = $"IIIA2", Name = $"IIIA2" });
            result.Add(new DropDownListDataModel() { Key = $"IIIB", Name = $"IIIB" });
            result.Add(new DropDownListDataModel() { Key = $"IIIC", Name = $"IIIC" });
            result.Add(new DropDownListDataModel() { Key = $"IVA", Name = $"IVA" });
            result.Add(new DropDownListDataModel() { Key = $"IVB", Name = $"IVB" });

            return result;
        }

        public List<DropDownListDataModel> Get日常體能狀態PS()
        {

            List<DropDownListDataModel> result = new List<DropDownListDataModel>();
            result.Add(new DropDownListDataModel() { Key = $"0", Name = $"0" });
            result.Add(new DropDownListDataModel() { Key = $"1", Name = $"1" });
            result.Add(new DropDownListDataModel() { Key = $"2", Name = $"2" });
            return result;
        }

        public List<DropDownListDataModel> GetAJCC_CP_Stage()
        {
            List<DropDownListDataModel> result = new List<DropDownListDataModel>();
            result.Add(new DropDownListDataModel() { Key = $"TX NX  M0", Name = $"TX NX  M0" });
            result.Add(new DropDownListDataModel() { Key = $"T0  N1 M1", Name = $"T0  N1 M1" });
            result.Add(new DropDownListDataModel() { Key = $"T1 N0(i+)", Name = $"T1 N0(i+)" });
            result.Add(new DropDownListDataModel() { Key = $"T1a N1", Name = $"T1a N1" });
            result.Add(new DropDownListDataModel() { Key = $"T1b N1mi", Name = $"T1b N1mi" });
            result.Add(new DropDownListDataModel() { Key = $"T2 N1a", Name = $"T2 N1a" });
            result.Add(new DropDownListDataModel() { Key = $"T3 N2", Name = $"T3 N2" });
            result.Add(new DropDownListDataModel() { Key = $"T3a N2mi", Name = $"T3a N2mi" });
            result.Add(new DropDownListDataModel() { Key = $"T3b N2a", Name = $"T3b N2a" });
            result.Add(new DropDownListDataModel() { Key = $"T4", Name = $"T4" });
            return result;
        }

        public List<DropDownListDataModel> GetAJCC_CP_StageT()
        {
            //TX, T0, T1, T1a, T1b, T2, T3, T3a, T3b, T4
            List<DropDownListDataModel> result = new List<DropDownListDataModel>();
            result.Add(new DropDownListDataModel() { Key = $"TX", Name = $"TX" });
            result.Add(new DropDownListDataModel() { Key = $"T0", Name = $"T0" });
            result.Add(new DropDownListDataModel() { Key = $"T1", Name = $"T1" });
            result.Add(new DropDownListDataModel() { Key = $"T1a", Name = $"T1a" });
            result.Add(new DropDownListDataModel() { Key = $"T1b", Name = $"T1b" });
            result.Add(new DropDownListDataModel() { Key = $"T2", Name = $"T2" });
            result.Add(new DropDownListDataModel() { Key = $"T3", Name = $"T3" });
            result.Add(new DropDownListDataModel() { Key = $"T3a", Name = $"T3a" });
            result.Add(new DropDownListDataModel() { Key = $"T3b", Name = $"T3b" });
            result.Add(new DropDownListDataModel() { Key = $"T4", Name = $"T4" });
            return result;
        }

        public List<DropDownListDataModel> GetAJCC_CP_StageN()
        {
            // NX, N0, N0(i+), N1, N1mi, N1a, N2, N2mi, N2a
            List<DropDownListDataModel> result = new List<DropDownListDataModel>();
            result.Add(new DropDownListDataModel() { Key = $"NX", Name = $"NX" });
            result.Add(new DropDownListDataModel() { Key = $"N0", Name = $"N0" });
            result.Add(new DropDownListDataModel() { Key = $"N0(i+)", Name = $"N0(i+)" });
            result.Add(new DropDownListDataModel() { Key = $"N1", Name = $"N1" });
            result.Add(new DropDownListDataModel() { Key = $"N1mi", Name = $"N1mi" });
            result.Add(new DropDownListDataModel() { Key = $"N1a", Name = $"N1a" });
            result.Add(new DropDownListDataModel() { Key = $"N2", Name = $"N2" });
            result.Add(new DropDownListDataModel() { Key = $"N2mi", Name = $"N2mi" });
            result.Add(new DropDownListDataModel() { Key = $"N2a", Name = $"N2a" });
            return result;
        }

        public List<DropDownListDataModel> GetAJCC_CP_StageM()
        {
            // M0, M1
            List<DropDownListDataModel> result = new List<DropDownListDataModel>();
            result.Add(new DropDownListDataModel() { Key = $"M0", Name = $"M0" });
            result.Add(new DropDownListDataModel() { Key = $"M1", Name = $"M1" });
            result.Add(new DropDownListDataModel() { Key = $"M1a", Name = $"M1a" });
            result.Add(new DropDownListDataModel() { Key = $"M1b", Name = $"M1b" });
            return result;
        }

        public List<DropDownListDataModel> GetMMRProtein()
        {
            List<DropDownListDataModel> result = new List<DropDownListDataModel>();
            result.Add(new DropDownListDataModel() { Key = $"PMS2", Name = $"PMS2" });
            result.Add(new DropDownListDataModel() { Key = $"MSH6", Name = $"MSH6" });
            result.Add(new DropDownListDataModel() { Key = $"MLH1", Name = $"MLH1" });
            result.Add(new DropDownListDataModel() { Key = $"MSH2", Name = $"MSH2" });
            return result;
        }

        public List<DropDownListDataModel> GetMMRProteinDetail()
        {
            List<DropDownListDataModel> result = new List<DropDownListDataModel>();
            result.Add(new DropDownListDataModel() { Key = $"Preserved", Name = $"Preserved" });
            result.Add(new DropDownListDataModel() { Key = $"Loss", Name = $"Loss" });
            return result;
        }

        public List<DropDownListDataModel> GetP53()
        {
            List<DropDownListDataModel> result = new List<DropDownListDataModel>();
            result.Add(new DropDownListDataModel() { Key = $"Wild-type", Name = $"Wild-type" });
            result.Add(new DropDownListDataModel() { Key = $"Abnormal - null", Name = $"Abnormal - null" });
            result.Add(new DropDownListDataModel() { Key = $"expression", Name = $"expression" });
            result.Add(new DropDownListDataModel() { Key = $"Overexpression", Name = $"Overexpression" });

            return result;
        }

        public List<DropDownListDataModel> GetHormonStatus()
        {
            List<DropDownListDataModel> result = new List<DropDownListDataModel>();
            result.Add(new DropDownListDataModel() { Key = $"ER Positive(%)", Name = $"ER Positive(%)" });
            result.Add(new DropDownListDataModel() { Key = $"ER Negative", Name = $"ER Negative" });
            result.Add(new DropDownListDataModel() { Key = $"PR Positive(%)", Name = $"PR Positive(%)" });
            result.Add(new DropDownListDataModel() { Key = $"PR Negative", Name = $"PR Negative" });
            return result;
        }

        public List<DropDownListDataModel> GetHormonStatusER()
        {
            List<DropDownListDataModel> result = new List<DropDownListDataModel>();
            result.Add(new DropDownListDataModel() { Key = $"ER Positive(%)", Name = $"ER Positive(%)" });
            result.Add(new DropDownListDataModel() { Key = $"ER Negative", Name = $"ER Negative" });
            return result;
        }

        public List<DropDownListDataModel> GetHormonStatusPR()
        {
            List<DropDownListDataModel> result = new List<DropDownListDataModel>();
            result.Add(new DropDownListDataModel() { Key = $"PR Positive(%)", Name = $"PR Positive(%)" });
            result.Add(new DropDownListDataModel() { Key = $"PR Negative", Name = $"PR Negative" });
            return result;
        }

        public List<DropDownListDataModel> Get組織型態()
        {
            List<DropDownListDataModel> result = new List<DropDownListDataModel>();
            result.Add(new DropDownListDataModel() { Key = $"Type I", Name = $"Type I" });
            result.Add(new DropDownListDataModel() { Key = $"Type II", Name = $"Type II" });
            return result;
        }

        public List<DropDownListDataModel> Get組織型態Detail(string typeDescription)
        {
            List<DropDownListDataModel> result = new List<DropDownListDataModel>();
            if (typeDescription == "Type I")
            {
                result.Add(new DropDownListDataModel() { Key = $"Endometrioid adenocarcinoma Grade 1", Name = $"Endometrioid adenocarcinoma Grade 1" });
                result.Add(new DropDownListDataModel() { Key = $"Endometrioid adenocarcinoma Grade 2", Name = $"Endometrioid adenocarcinoma Grade 2" });
                result.Add(new DropDownListDataModel() { Key = $"Endometrioid adenocarcinoma Grade 3", Name = $"Endometrioid adenocarcinoma Grade 3" });
            }
            else if (typeDescription == "Type II")
            {
                result.Add(new DropDownListDataModel() { Key = $"Serous", Name = $"Serous" });
                result.Add(new DropDownListDataModel() { Key = $"Clear cell", Name = $"Clear cell" });
                result.Add(new DropDownListDataModel() { Key = $"Carcinosarcoma", Name = $"Carcinosarcoma" });
                result.Add(new DropDownListDataModel() { Key = $"Mixed cell adenocarcinoma", Name = $"Mixed cell adenocarcinoma" });
                result.Add(new DropDownListDataModel() { Key = $"Neuroendocrine tumors", Name = $"Neuroendocrine tumors" });
                result.Add(new DropDownListDataModel() { Key = $"Dedifferentiated carcinoma", Name = $"Dedifferentiated carcinoma" });
                result.Add(new DropDownListDataModel() { Key = $"Undifferentiated carcinoma", Name = $"Undifferentiated carcinoma" });
                result.Add(new DropDownListDataModel() { Key = $"Others", Name = $"Others" });
            }
            return result;
        }

        #endregion
        #region 臨床資料 手術
        public List<DropDownListDataModel> GetVisitCode()
        {
            List<DropDownListDataModel> result = new List<DropDownListDataModel>();
            List<string> list = new() {
                "Baseline", "FU1(3M)", "FU2(6M)", "FU3(9M)", "FU4(12M)", "FU5(15M)", "FU6(18M)", "FU7(21M)", "FU8(24M)"
            };
            foreach (var item in list)
            {
                result.Add(new DropDownListDataModel() { Key = $"{item}", Name = $"{item}" });
            }
            return result;
        }

        public List<DropDownListDataModel> GetAscites()
        {
            List<DropDownListDataModel> result = new List<DropDownListDataModel>();
            List<string> list = new() {
                "Yes", "No"
            };
            foreach (var item in list)
            {
                result.Add(new DropDownListDataModel() { Key = $"{item}", Name = $"{item}" });
            }
            return result;
        }

        public List<DropDownListDataModel> GetUterus()
        {
            List<DropDownListDataModel> result = new List<DropDownListDataModel>();
            List<string> list = new() {
                "normal", "abnormal"
            };
            foreach (var item in list)
            {
                result.Add(new DropDownListDataModel() { Key = $"{item}", Name = $"{item}" });
            }
            return result;
        }

        public List<DropDownListDataModel> GetSite()
        {
            List<DropDownListDataModel> result = new List<DropDownListDataModel>();
            List<string> list = new() {
                "cervix", "Endometrium", "myometrium"
            };
            foreach (var item in list)
            {
                result.Add(new DropDownListDataModel() { Key = $"{item}", Name = $"{item}" });
            }
            return result;
        }

        public List<DropDownListDataModel> GetTumorNumber()
        {
            List<DropDownListDataModel> result = new List<DropDownListDataModel>();
            List<string> list = new() {
                "1", "2", "more"
            };
            foreach (var item in list)
            {
                result.Add(new DropDownListDataModel() { Key = $"{item}", Name = $"{item}" });
            }
            return result;
        }

        public List<DropDownListDataModel> GetCervix()
        {
            List<DropDownListDataModel> result = new List<DropDownListDataModel>();
            List<string> list = new() {
                "normal", "abnormal"
            };
            foreach (var item in list)
            {
                result.Add(new DropDownListDataModel() { Key = $"{item}", Name = $"{item}" });
            }
            return result;
        }

        public List<DropDownListDataModel> GetEndometrium()
        {
            List<DropDownListDataModel> result = new List<DropDownListDataModel>();
            List<string> list = new() {
                "normal", "abnormal"
            };
            foreach (var item in list)
            {
                result.Add(new DropDownListDataModel() { Key = $"{item}", Name = $"{item}" });
            }
            return result;
        }

        public List<DropDownListDataModel> GetOvarianSurfaceRuptureOrNotRightOvary()
        {
            List<DropDownListDataModel> result = new List<DropDownListDataModel>();
            List<string> list = new() {
                "no rupture", "rupture during the operation", "rupture before operation"
            };
            foreach (var item in list)
            {
                result.Add(new DropDownListDataModel() { Key = $"{item}", Name = $"{item}" });
            }
            return result;
        }

        public List<DropDownListDataModel> GetOvarianSurfaceRuptureOrNotLeftOvary()
        {
            List<DropDownListDataModel> result = new List<DropDownListDataModel>();
            List<string> list = new() {
                "no rupture", "rupture during the operation", "rupture before operation"
            };
            foreach (var item in list)
            {
                result.Add(new DropDownListDataModel() { Key = $"{item}", Name = $"{item}" });
            }
            return result;
        }

        public List<DropDownListDataModel> GetLeftAdnexa()
        {
            List<DropDownListDataModel> result = new List<DropDownListDataModel>();
            List<string> list = new() {
                "normal", "abnormal"
            };
            foreach (var item in list)
            {
                result.Add(new DropDownListDataModel() { Key = $"{item}", Name = $"{item}" });
            }
            return result;
        }

        public List<DropDownListDataModel> GetRightAdnexa()
        {
            List<DropDownListDataModel> result = new List<DropDownListDataModel>();
            List<string> list = new() {
                "normal", "abnormal"
            };
            foreach (var item in list)
            {
                result.Add(new DropDownListDataModel() { Key = $"{item}", Name = $"{item}" });
            }
            return result;
        }

        public List<DropDownListDataModel> GetPelvicPeritonealCavity()
        {
            List<DropDownListDataModel> result = new List<DropDownListDataModel>();
            List<string> list = new() {
                "free of tumor", "Not free"
            };
            foreach (var item in list)
            {
                result.Add(new DropDownListDataModel() { Key = $"{item}", Name = $"{item}" });
            }
            return result;
        }

        public List<DropDownListDataModel> GetExtrapelvicPeritonealCavity()
        {
            List<DropDownListDataModel> result = new List<DropDownListDataModel>();
            List<string> list = new() {
                "free of tumor", "other finding"
            };
            foreach (var item in list)
            {
                result.Add(new DropDownListDataModel() { Key = $"{item}", Name = $"{item}" });
            }
            return result;
        }

        public List<DropDownListDataModel> GetExtrapelvicPeritonealCavityOtherFinding()
        {
            List<DropDownListDataModel> result = new List<DropDownListDataModel>();
            List<string> list = new() {
                "adhesion between spleen and abdominal wall",
                "adhesion between liver and abdominal wall",
                "adhesion between omentum and small intestine"
            };
            foreach (var item in list)
            {
                result.Add(new DropDownListDataModel() { Key = $"{item}", Name = $"{item}" });
            }
            return result;
        }

        public List<DropDownListDataModel> GetResidualTumor()
        {
            List<DropDownListDataModel> result = new List<DropDownListDataModel>();
            List<string> list = new() {
                "No visible residual (R0)",
                "residual tumor <1 cm (R1)",
                "gross tumor >1 cm (R2)",
            };
            foreach (var item in list)
            {
                result.Add(new DropDownListDataModel() { Key = $"{item}", Name = $"{item}" });
            }
            return result;
        }

        #endregion
    }
}
