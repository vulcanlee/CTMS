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
        public string MyProperty { get; set; } = Guid.NewGuid().ToString();
        public BrowseSearchingModel BrowseSearchingModel { get; set; } = new();

        public void Reset()
        {
            BrowseSearchingModel.院別.Clear();
            BrowseSearchingModel.癌別.Clear();
            BrowseSearchingModel.SearchKeyword = string.Empty;
        }

        public void AddHospital(string hospital)
        {
            if (!string.IsNullOrEmpty(hospital) && !BrowseSearchingModel.院別.Contains(hospital))
            {
                BrowseSearchingModel.院別.Add(hospital);
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
    }
}
