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
        public List<DropDownListDataModel> GetAge()
        {
            List<DropDownListDataModel> result = new List<DropDownListDataModel>();
            for (int i = 20; i <= 80; i++)
            {
                result.Add(new DropDownListDataModel() { Key= $"{i}", Name=$"{i}" });
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

    }
}
