using CTMS.AdapterModels;
using CTMS.DataModel.Models;
using CTMS.Services;
using Syncfusion.Blazor;
using Syncfusion.Blazor.Data;

namespace CTMS.Adapters;

public partial class OperationHistoryTraceAdapter : DataAdaptor<OperationHistoryTraceService>
{

    public override async Task<object> ReadAsync(DataManagerRequest dataManagerRequest, string key = null)
    {
        #region 建立查詢物件
        DataRequest dataRequest = new DataRequest()
        {
            Skip = dataManagerRequest.Skip,
            Take = dataManagerRequest.Take,
        };
        if (dataManagerRequest.Search != null && dataManagerRequest.Search.Count > 0)
        {
            var keyword = dataManagerRequest.Search[0].Key;
            dataRequest.Search = keyword;
        }
        #endregion

        #region 發出查詢要求
        try
        {
            DataRequestResult<OperationHistoryTraceAdapterModel> adaptorModelObjects = await Service.GetAsync(dataRequest);
            var item = dataManagerRequest.RequiresCounts
                ? new DataResult() { Result = adaptorModelObjects.Result, Count = adaptorModelObjects.Count }
                : (object)adaptorModelObjects.Result;
            await Task.Yield();
            return item;
        }
        catch (Exception)
        {
            return new DataResult() { Result = new List<OperationHistoryTraceAdapterModel>(), Count = 0 };
        }
        #endregion
    }
}
