using AutoMapper;
using CTMS.AdapterModels;
using CTMS.Business.Factories;
using CTMS.Business.Helpers;
using CTMS.Business.Services.ClinicalInformation;
using CTMS.DataModel.Models;
using CTMS.DataModel.Models.ClinicalInformation;
using CTMS.DataModel.Models.Systems;
using CTMS.EntityModel;
using CTMS.EntityModel.Models;
using CTMS.Share.Helpers;
using Microsoft.EntityFrameworkCore;

namespace CTMS.Services;

public class OperationHistoryTraceService
{
    #region 欄位與屬性
    private readonly BackendDBContext context;

    public IMapper Mapper { get; }
    public IConfiguration Configuration { get; }
    public ILogger<OperationHistoryTraceService> Logger { get; }
    #endregion

    #region 建構式
    public OperationHistoryTraceService(BackendDBContext context, IMapper mapper,
        IConfiguration configuration, ILogger<OperationHistoryTraceService> logger)
    {
        this.context = context;
        Mapper = mapper;
        Configuration = configuration;
        Logger = logger;
    }
    #endregion

    #region CRUD 服務
    public async Task<DataRequestResult<OperationHistoryTraceAdapterModel>> GetAsync(DataRequest dataRequest)
    {
        List<OperationHistoryTraceAdapterModel> data = new();
        DataRequestResult<OperationHistoryTraceAdapterModel> result = new();
        var DataSource = context.OperationHistoryTrace
            .AsNoTracking();

        #region 進行搜尋動作
        //if (!string.IsNullOrWhiteSpace(dataRequest.Search))
        //{
        //    DataSource = DataSource
        //    .Where(x => x.Name.Contains(dataRequest.Search) ||
        //    x.Account.Contains(dataRequest.Search));
        //}
        #endregion

        #region 進行排序動作
        DataSource = DataSource
            .OrderByDescending(x => x.CreateAt);
        #endregion

        #region 進行分頁
        // 取得記錄總數量，將要用於分頁元件面板使用
        result.Count = DataSource.Cast<OperationHistoryTrace>().Count();
        DataSource = DataSource.Skip(dataRequest.Skip);
        if (dataRequest.Take != 0)
        {
            DataSource = DataSource.Take(dataRequest.Take);
        }
        #endregion

        #region 在這裡進行取得資料與與額外屬性初始化
        List<OperationHistoryTraceAdapterModel> adapterModelObjects =
            Mapper.Map<List<OperationHistoryTraceAdapterModel>>(DataSource);

        foreach (var adapterModelItem in adapterModelObjects)
        {
            await OhterDependencyData(adapterModelItem);
        }
        #endregion

        result.Result = adapterModelObjects;
        await Task.Yield();
        return result;
    }

    public async Task<OperationHistoryTraceAdapterModel> GetAsync(int id)

    {
        OperationHistoryTrace item = await context.OperationHistoryTrace
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
        if (item != null)
        {
            OperationHistoryTraceAdapterModel result = Mapper.Map<OperationHistoryTraceAdapterModel>(item);
            await OhterDependencyData(result);
            return result;
        }
        else
        {
            return new OperationHistoryTraceAdapterModel() { };
        }
    }

    public async Task<List<OperationHistoryTraceAdapterModel>> GetAsync()

    {
        List<OperationHistoryTraceAdapterModel> result = new List<OperationHistoryTraceAdapterModel>();
        List<OperationHistoryTrace> items = await context.OperationHistoryTrace
            .AsNoTracking()
            .ToListAsync();
        if (items.Count > 0)
        {
            List<OperationHistoryTraceAdapterModel> results = Mapper.Map<List<OperationHistoryTraceAdapterModel>>(items);
            foreach (var item in results)
            {
                await OhterDependencyData(item);
            }
            return results;
        }
        else
        {
            return result;
        }
    }

    public async Task<VerifyRecordResult> AddAsync(OperationHistoryTraceAdapterModel paraObject)
    {
        try
        {
            OperationHistoryTrace itemParameter = Mapper.Map<OperationHistoryTrace>(paraObject);
            itemParameter.CreateAt = DateTime.Now;

            CleanTrackingHelper.Clean<OperationHistoryTrace>(context);
            await context.OperationHistoryTrace
                .AddAsync(itemParameter);
            await context.SaveChangesAsync();
            CleanTrackingHelper.Clean<OperationHistoryTrace>(context);
            return VerifyRecordResultFactory.Build(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "新增記錄發生例外異常");
            return VerifyRecordResultFactory.Build(false, "新增記錄發生例外異常", ex);
        }
    }

    public async Task<VerifyRecordResult> UpdateAsync(OperationHistoryTraceAdapterModel paraObject)
    {
        try
        {
            OperationHistoryTrace itemData = Mapper.Map<OperationHistoryTrace>(paraObject);

            CleanTrackingHelper.Clean<OperationHistoryTrace>(context);
            OperationHistoryTrace item = await context.OperationHistoryTrace
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == paraObject.Id);
            if (item == null)
            {
                return VerifyRecordResultFactory.Build(false, "無法修改紀錄");
            }
            else
            {
                CleanTrackingHelper.Clean<OperationHistoryTrace>(context);
                context.Entry(itemData).State = EntityState.Modified;
                await context.SaveChangesAsync();
                CleanTrackingHelper.Clean<OperationHistoryTrace>(context);
                return VerifyRecordResultFactory.Build(true);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "修改記錄發生例外異常");
            return VerifyRecordResultFactory.Build(false, "修改記錄發生例外異常", ex);
        }
    }

    public async Task<VerifyRecordResult> DeleteAsync(int id)
    {
        try
        {
            CleanTrackingHelper.Clean<OperationHistoryTrace>(context);
            OperationHistoryTrace item = await context.OperationHistoryTrace
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
            if (item == null)
            {
                return VerifyRecordResultFactory.Build(false, "無法刪除紀錄");
            }
            else
            {
                CleanTrackingHelper.Clean<OperationHistoryTrace>(context);
                context.Entry(item).State = EntityState.Deleted;
                await context.SaveChangesAsync();
                CleanTrackingHelper.Clean<OperationHistoryTrace>(context);
                return VerifyRecordResultFactory.Build(true);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "刪除記錄發生例外異常");
            return VerifyRecordResultFactory.Build(false, "刪除記錄發生例外異常", ex);
        }
    }
    #endregion

    #region CRUD 的限制條件檢查
    public async Task<VerifyRecordResult> BeforeAddCheckAsync(OperationHistoryTraceAdapterModel paraObject)
    {

        return VerifyRecordResultFactory.Build(true);
    }

    public async Task<VerifyRecordResult> BeforeUpdateCheckAsync(OperationHistoryTraceAdapterModel paraObject)
    {
        CleanTrackingHelper.Clean<OperationHistoryTrace>(context);
        var searchItem = await context.OperationHistoryTrace
         .AsNoTracking()
         .FirstOrDefaultAsync(x => x.Id == paraObject.Id);
        if (searchItem == null)
        {
            return VerifyRecordResultFactory.Build(false, "要更新的紀錄 發生同時存取衝突 已經不存在資料庫上");
        }

        return VerifyRecordResultFactory.Build(true);
    }

    public async Task<VerifyRecordResult> BeforeDeleteCheckAsync(OperationHistoryTraceAdapterModel paraObject)
    {
        CleanTrackingHelper.Clean<OperationHistoryTrace>(context);
        var searchItem = await context.OperationHistoryTrace
         .AsNoTracking()
         .FirstOrDefaultAsync(x => x.Id == paraObject.Id);
        if (searchItem == null)
        {
            return VerifyRecordResultFactory.Build(false, "無法刪除紀錄 要刪除的紀錄已經不存在資料庫上");
        }

        return VerifyRecordResultFactory.Build(true);
    }
    #endregion

    #region 其他服務方法
    async Task OhterDependencyData(OperationHistoryTraceAdapterModel data)
    {
        await Task.Yield();
    }

    #endregion

    #region 紀錄啟用或停用
    #endregion
}
