using CTMS.DataModel.Models;
using CTMS.DataModel.Models.ClinicalInformation;
using System;
using System.Collections.Generic;
using System.Linq;
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
            result.Add(new DropDownListDataModel() { Key = $"EC", Name = $"EC" });
            result.Add(new DropDownListDataModel() { Key = $"OC", Name = $"OC" });
            return result;
        }

        public List<DropDownListDataModel> Get月經狀態()
        {
            List<DropDownListDataModel> result = new List<DropDownListDataModel>();
            result.Add(new DropDownListDataModel() { Key = $"0", Name = $"0 停經" });
            result.Add(new DropDownListDataModel() { Key = $"1", Name = $"1 未停經" });
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

        public List<DropDownListDataModel> Get日常體能狀態PS()
        {
            List<DropDownListDataModel> result = new List<DropDownListDataModel>();
            result.Add(new DropDownListDataModel() { Key = $"0", Name = $"0" });
            result.Add(new DropDownListDataModel() { Key = $"1", Name = $"1" });
            result.Add(new DropDownListDataModel() { Key = $"2", Name = $"2" });
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
