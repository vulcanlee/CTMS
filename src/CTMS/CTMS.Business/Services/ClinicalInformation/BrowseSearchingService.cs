using CTMS.DataModel.Models;
using CTMS.DataModel.Models.ClinicalInformation;
using CTMS.DataModel.Models.Systems;
using CTMS.Share.Extensions;
using CTMS.Share.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTMS.Business.Services.ClinicalInformation
{
    public class BrowseSearchingService
    {
        public BrowseSearchingModel BrowseSearchingModel { get; set; } = new();

        public void Reset()
        {
            BrowseSearchingModel.院別.Clear();
            BrowseSearchingModel.癌別.Clear();
            BrowseSearchingModel.狀態.Clear();
            BrowseSearchingModel.收案開始日 = null;
            BrowseSearchingModel.收案結束日 = null;
            BrowseSearchingModel.SearchKeyword = string.Empty;
            BrowseSearchingModel.PageIndex = 1;
            BrowseSearchingModel.Total = 0;
            BrowseSearchingModel.Current = 10;
        }

        public void AddHospital(string hospital)
        {
            if (!string.IsNullOrEmpty(hospital) && !BrowseSearchingModel.院別.Contains(hospital))
            {
                BrowseSearchingModel.院別.Add(hospital);
            }
        }

        public void AddStatus(string Status)
        {
            if (!string.IsNullOrEmpty(Status) && !BrowseSearchingModel.狀態.Contains(Status))
            {
                BrowseSearchingModel.狀態.Add(Status);
            }
        }

        public void AddCancerType(string cancerType)
        {
            if (!string.IsNullOrEmpty(cancerType) && !BrowseSearchingModel.癌別.Contains(cancerType))
            {
                BrowseSearchingModel.癌別.Add(cancerType);
            }
        }

        public void RemoveHospital(string hospital)
        {
            if (!string.IsNullOrEmpty(hospital) && BrowseSearchingModel.院別.Contains(hospital))
            {
                BrowseSearchingModel.院別.Remove(hospital);
            }
        }

        public void RemoveStatus(string status)
        {
            if (!string.IsNullOrEmpty(status) && BrowseSearchingModel.狀態.Contains(status))
            {
                BrowseSearchingModel.狀態.Remove(status);
            }
        }

        public void RemoveCancerType(string cancerType)
        {
            if (!string.IsNullOrEmpty(cancerType) && BrowseSearchingModel.癌別.Contains(cancerType))
            {
                BrowseSearchingModel.癌別.Remove(cancerType);
            }
        }

        public void SetSearchKeyword(string keyword)
        {
            if (!string.IsNullOrEmpty(keyword))
            {
                BrowseSearchingModel.SearchKeyword = keyword.Trim();
            }
            else
            {
                BrowseSearchingModel.SearchKeyword = string.Empty;
            }
        }

        public void Set收案開始日(DateTime? date)
        {
            BrowseSearchingModel.收案開始日 = date?.Date;
        }

        public void Set收案結束日(DateTime? date)
        {
            BrowseSearchingModel.收案結束日 = date?.Date;
        }
    }
}
