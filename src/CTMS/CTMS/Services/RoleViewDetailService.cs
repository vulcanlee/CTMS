using CTMS.EntityModel.Models;
using CTMS.EntityModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CTMS.DataModel.Models;
using CTMS.AdapterModels;
using AutoMapper;
using CTMS.Business.Helpers;
using CTMS.Business.Factories;
using CTMS.Share.Helpers;
using CTMS.Business.Services;
using System.Text.Json;

namespace CTMS.Services;

public class RoleViewDetailService
{
    #region 欄位與屬性
    private readonly BackendDBContext context;
    public IMapper Mapper { get; }
    public ILogger<RoleViewDetailService> Logger { get; }
    #endregion

    #region 建構式
    public RoleViewDetailService(BackendDBContext context, IMapper mapper,
        ILogger<RoleViewDetailService> logger)
    {
        this.context = context;
        Mapper = mapper;
        Logger = logger;
    }
    #endregion

    #region CRUD 服務
    public async Task<DataRequestResult<RoleViewDetailAdapterModel>> GetAsync(DataRequest dataRequest)
    {
        List<RoleViewDetailAdapterModel> data = new();
        DataRequestResult<RoleViewDetailAdapterModel> result = new();
        var DataSource = context.RoleViewProject
            .AsNoTracking()
            .Include(x => x.Project)
            .AsQueryable();
        #region 進行搜尋動作
        #endregion

        #region 進行排序動作
        #endregion

        #region 進行分頁
        // 取得記錄總數量，將要用於分頁元件面板使用
        result.Count = DataSource.Cast<RoleViewProject>().Count();
        DataSource = DataSource.Skip(dataRequest.Skip);
        if (dataRequest.Take != 0)
        {
            DataSource = DataSource.Take(dataRequest.Take);
        }
        #endregion

        #region 在這裡進行取得資料與與額外屬性初始化
        //List<RoleViewDetailAdapterModel> adapterModelObjects =
        //    Mapper.Map<List<RoleViewDetailAdapterModel>>(DataSource);

        var items = DataSource.ToList();
        foreach (var item in items)
        {
            RoleViewDetailAdapterModel adapterModel = new RoleViewDetailAdapterModel();
            adapterModel.ProjectId = item.ProjectId;
            adapterModel.ProjectName = item.Project.Name;
            adapterModel.RoleViewId = item.RoleViewId;
            data.Add(adapterModel);
        }

        foreach (var adapterModelItem in data)
        {
            await OhterDependencyData(adapterModelItem);

        }
        #endregion

        result.Result = data;
        await Task.Yield();
        return result;
    }

    public async Task<DataRequestResult<RoleViewDetailAdapterModel>> GetByHeaderIDAsync(int id, DataRequest dataRequest)
    {
        List<RoleViewDetailAdapterModel> data = new();
        DataRequestResult<RoleViewDetailAdapterModel> result = new();
        var DataSource = context.RoleViewProject
            .AsNoTracking()
            .Include(x => x.Project)
            .Where(x => x.RoleViewId == id);
        #region 進行搜尋動作
        #endregion

        #region 進行排序動作
        #endregion

        #region 進行分頁
        // 取得記錄總數量，將要用於分頁元件面板使用
        result.Count = DataSource.Cast<RoleViewProject>().Count();
        DataSource = DataSource.Skip(dataRequest.Skip);
        if (dataRequest.Take != 0)
        {
            DataSource = DataSource.Take(dataRequest.Take);
        }
        #endregion

        #region 在這裡進行取得資料與與額外屬性初始化
        var items = DataSource.ToList();
        foreach (var item in items)
        {
            RoleViewDetailAdapterModel adapterModel = new RoleViewDetailAdapterModel();
            adapterModel.Id = item.Id;
            adapterModel.ProjectId = item.ProjectId;
            adapterModel.ProjectName = item.Project.Name;
            adapterModel.RoleViewId = item.RoleViewId;
            adapterModel.Project = Mapper.Map<ProjectAdapterModel>(item.Project);

            data.Add(adapterModel);
        }

        foreach (var adapterModelItem in data)
        {
            await OhterDependencyData(adapterModelItem);
        }
        #endregion

        result.Result = data;
        await Task.Yield();
        return result;
    }

    public async Task<RoleViewDetailAdapterModel> GetAsync(int id)
    {
        RoleViewProject item = await context.RoleViewProject
            .AsNoTracking()
            .Include(x => x.Project)
            .FirstOrDefaultAsync(x => x.Id == id);
        RoleViewDetailAdapterModel result = new RoleViewDetailAdapterModel()
        {
            Id = item.Id,
            ProjectId = item.ProjectId,
            ProjectName = item.Project.Name,
            RoleViewId = item.RoleViewId
        };
        await OhterDependencyData(result);
        return result;
    }

    public async Task<VerifyRecordResult> AddAsync(RoleViewDetailAdapterModel paraObject)
    {
        try
        {
            // RoleViewDetail itemParameter = Mapper.Map<RoleViewDetail>(paraObject);
            RoleViewProject itemParameter = new RoleViewProject()
            {
                ProjectId = paraObject.ProjectId,
                RoleViewId = paraObject.RoleViewId
            };
            CleanTrackingHelper.Clean<RoleViewProject>(context);
            await context.RoleViewProject
                .AddAsync(itemParameter);
            await context.SaveChangesAsync();
            CleanTrackingHelper.Clean<RoleViewProject>(context);
            return VerifyRecordResultFactory.Build(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "新增記錄發生例外異常");
            return VerifyRecordResultFactory.Build(false, "新增記錄發生例外異常", ex);
        }
    }

    public async Task<VerifyRecordResult> UpdateAsync(RoleViewDetailAdapterModel paraObject)
    {
        try
        {
            // RoleViewDetail itemData = Mapper.Map<RoleViewDetail>(paraObject);
            RoleViewProject itemParameter = new RoleViewProject()
            {
                Id = paraObject.Id,
                ProjectId = paraObject.ProjectId,
                RoleViewId = paraObject.RoleViewId
            };

            CleanTrackingHelper.Clean<RoleViewProject>(context);
            RoleViewProject item = await context.RoleViewProject
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == paraObject.Id);
            if (item == null)
            {
                return VerifyRecordResultFactory.Build(false, "無法修改紀錄");
            }
            else
            {
                CleanTrackingHelper.Clean<RoleViewProject>(context);
                context.Entry(item).State = EntityState.Modified;
                await context.SaveChangesAsync();
                CleanTrackingHelper.Clean<RoleViewProject>(context);
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
            CleanTrackingHelper.Clean<RoleViewProject>(context);
            RoleViewProject item = await context.RoleViewProject
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
            if (item == null)
            {
                return VerifyRecordResultFactory.Build(false, "無法刪除紀錄");
            }
            else
            {
                CleanTrackingHelper.Clean<RoleViewProject>(context);
                context.Entry(item).State = EntityState.Deleted;
                await context.SaveChangesAsync();
                CleanTrackingHelper.Clean<RoleViewProject>(context);
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
    public async Task<VerifyRecordResult> BeforeAddCheckAsync(RoleViewDetailAdapterModel paraObject)
    {
        CleanTrackingHelper.Clean<RoleViewProject>(context);
        if (paraObject.RoleViewId == 0)
        {
            return VerifyRecordResultFactory.Build(false, "尚未輸入該角色");
        }
        var item = await context.RoleViewProject
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.RoleViewId == paraObject.RoleViewId &&
            x.ProjectId == paraObject.ProjectId);
        if (item != null)
        {
            return VerifyRecordResultFactory.Build(false, "該角色內已經存在該專案_不能重複同樣的專案在一角色內");
        }
        return VerifyRecordResultFactory.Build(true);
    }

    public async Task<VerifyRecordResult> BeforeUpdateCheckAsync(RoleViewDetailAdapterModel paraObject)
    {
        CleanTrackingHelper.Clean<RoleViewProject>(context);
        if (paraObject.RoleViewId == 0)
        {
            return VerifyRecordResultFactory.Build(false, "尚未輸入該角色");
        }

        var searchItem = await context.RoleViewProject
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == paraObject.Id);
        if (searchItem == null)
        {
            return VerifyRecordResultFactory.Build(false, "要更新的紀錄_發生同時存取衝突_已經不存在資料庫上");
        }

        searchItem = await context.RoleViewProject
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.RoleViewId == paraObject.RoleViewId &&
            x.ProjectId == paraObject.ProjectId &&
            x.Id != paraObject.Id);
        if (searchItem != null)
        {
            return VerifyRecordResultFactory.Build(false, "該角色內已經存在該專案_不能重複同樣的專案在一角色內");
        }
        return VerifyRecordResultFactory.Build(true);
    }

    public async Task<VerifyRecordResult> BeforeDeleteCheckAsync(RoleViewDetailAdapterModel paraObject)
    {
        CleanTrackingHelper.Clean<RoleViewProject>(context);
        var searchItem = await context.RoleViewProject
         .AsNoTracking()
         .FirstOrDefaultAsync(x => x.Id == paraObject.Id);
        if (searchItem == null)
        {
            return VerifyRecordResultFactory.Build(false, "無法刪除紀錄_要刪除的紀錄已經不存在資料庫上");
        }

        return VerifyRecordResultFactory.Build(true);
    }
    #endregion

    #region 其他服務方法
    Task OhterDependencyData(RoleViewDetailAdapterModel data)
    {
        data.ProjectName = data.Project.Name;
        return Task.FromResult(0);
    }
    #endregion
}
