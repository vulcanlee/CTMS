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

public class ProjectService
{
    #region 欄位與屬性
    private readonly BackendDBContext context;
    private readonly RolePermissionService rolePermissionService;

    public IMapper Mapper { get; }
    public IConfiguration Configuration { get; }
    public ILogger<ProjectService> Logger { get; }
    #endregion

    #region 建構式
    public ProjectService(BackendDBContext context, IMapper mapper,
        IConfiguration configuration, ILogger<ProjectService> logger,
        RolePermissionService rolePermissionService)
    {
        this.context = context;
        Mapper = mapper;
        Configuration = configuration;
        Logger = logger;
        this.rolePermissionService = rolePermissionService;
    }
    #endregion

    #region CRUD 服務
    public async Task<DataRequestResult<ProjectAdapterModel>> GetAsync(DataRequest dataRequest)
    {
        List<ProjectAdapterModel> data = new();
        DataRequestResult<ProjectAdapterModel> result = new();
        var DataSource = context.Project
            .AsNoTracking();

        #region 進行搜尋動作
        if (!string.IsNullOrWhiteSpace(dataRequest.Search))
        {
            DataSource = DataSource
            .Where(x => x.Name.Contains(dataRequest.Search) );
        }
        #endregion

        #region 進行排序動作
        #endregion

        #region 進行分頁
        // 取得記錄總數量，將要用於分頁元件面板使用
        result.Count = DataSource.Cast<Project>().Count();
        DataSource = DataSource.Skip(dataRequest.Skip);
        if (dataRequest.Take != 0)
        {
            DataSource = DataSource.Take(dataRequest.Take);
        }
        #endregion

        #region 在這裡進行取得資料與與額外屬性初始化
        List<ProjectAdapterModel> adapterModelObjects =
            Mapper.Map<List<ProjectAdapterModel>>(DataSource);

        foreach (var adapterModelItem in adapterModelObjects)
        {
            await OhterDependencyData(adapterModelItem);
        }
        #endregion

        result.Result = adapterModelObjects;
        await Task.Yield();
        return result;
    }

    public async Task<ProjectAdapterModel> GetAsync(int id)

    {
        Project item = await context.Project
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
        if (item != null)
        {
            ProjectAdapterModel result = Mapper.Map<ProjectAdapterModel>(item);
            await OhterDependencyData(result);
            return result;
        }
        else
        {
            return new ProjectAdapterModel() { };
        }
    }

    public async Task<VerifyRecordResult> AddAsync(ProjectAdapterModel paraObject)
    {
        try
        {
            Project itemParameter = Mapper.Map<Project>(paraObject);

            CleanTrackingHelper.Clean<Project>(context);
            await context.Project
                .AddAsync(itemParameter);
            await context.SaveChangesAsync();
            CleanTrackingHelper.Clean<Project>(context);
            return VerifyRecordResultFactory.Build(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "新增記錄發生例外異常");
            return VerifyRecordResultFactory.Build(false, "新增記錄發生例外異常", ex);
        }
    }

    public async Task<VerifyRecordResult> UpdateAsync(ProjectAdapterModel paraObject)
    {
        try
        {
            Project itemData = Mapper.Map<Project>(paraObject);

            CleanTrackingHelper.Clean<Project>(context);
            Project item = await context.Project
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == paraObject.Id);
            if (item == null)
            {
                return VerifyRecordResultFactory.Build(false, "無法修改紀錄");
            }
            else
            {
                CleanTrackingHelper.Clean<Project>(context);
                context.Entry(itemData).State = EntityState.Modified;
                await context.SaveChangesAsync();
                CleanTrackingHelper.Clean<Project>(context);
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
            CleanTrackingHelper.Clean<Project>(context);
            Project item = await context.Project
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
            if (item == null)
            {
                return VerifyRecordResultFactory.Build(false, "無法刪除紀錄");
            }
            else
            {
                CleanTrackingHelper.Clean<Project>(context);
                context.Entry(item).State = EntityState.Deleted;
                await context.SaveChangesAsync();
                CleanTrackingHelper.Clean<Project>(context);
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
    public async Task<VerifyRecordResult> BeforeAddCheckAsync(ProjectAdapterModel paraObject)
    {
        var searchItem = await context.Project
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Name == paraObject.Name);
        if (searchItem != null)
        {
            return VerifyRecordResultFactory.Build(false, "要新增的紀錄已經存在無法新增");
        }

        return VerifyRecordResultFactory.Build(true);
    }

    public async Task<VerifyRecordResult> BeforeUpdateCheckAsync(ProjectAdapterModel paraObject)
    {
        //if (paraObject.Account.ToLower() == MagicObjectHelper.開發者帳號)
        //{
        //    return VerifyRecordResultFactory.Build(false, ErrorMessageEnum.開發者帳號不可以被修改);
        //}

        CleanTrackingHelper.Clean<Project>(context);
        var searchItem = await context.Project
         .AsNoTracking()
         .FirstOrDefaultAsync(x => x.Id == paraObject.Id);
        if (searchItem == null)
        {
            return VerifyRecordResultFactory.Build(false, "要更新的紀錄 發生同時存取衝突 已經不存在資料庫上");
        }

        searchItem = await context.Project
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == paraObject.Id);
        if (searchItem != null)
        {
            //if (searchItem.Account.ToLower() == MagicObjectHelper.開發者帳號)
            //{
            //    return VerifyRecordResultFactory.Build(false, ErrorMessageEnum.開發者帳號不可以被修改);
            //}
        }
        else
        {
            return VerifyRecordResultFactory.Build(false, "要更新的紀錄 發生同時存取衝突 已經不存在資料庫上");
        }

        searchItem = await context.Project
           .AsNoTracking()
           .FirstOrDefaultAsync(x => x.Name == paraObject.Name &&
           x.Id != paraObject.Id);
        if (searchItem != null)
        {
            return VerifyRecordResultFactory.Build(false, "要修改的紀錄已經存在無法修改");
        }

        return VerifyRecordResultFactory.Build(true);
    }

    public async Task<VerifyRecordResult> BeforeDeleteCheckAsync(ProjectAdapterModel paraObject)
    {
        CleanTrackingHelper.Clean<Project>(context);
        var searchItem = await context.Project
         .AsNoTracking()
         .FirstOrDefaultAsync(x => x.Id == paraObject.Id);
        if (searchItem == null)
        {
            return VerifyRecordResultFactory.Build(false, "無法刪除紀錄 要刪除的紀錄已經不存在資料庫上");
        }

        searchItem = await context.Project
        .AsNoTracking()
        .FirstOrDefaultAsync(x => x.Id == paraObject.Id);
        if (searchItem != null)
        {
        }
        else
        {
            return VerifyRecordResultFactory.Build(false, "無法刪除紀錄 要刪除的紀錄已經不存在資料庫上");
        }
        return VerifyRecordResultFactory.Build(true);
    }
    #endregion

    #region 其他服務方法

    async Task OhterDependencyData(ProjectAdapterModel data)
    {
        await Task.Yield();
    }
    #endregion

    #region 紀錄啟用或停用
    public async Task DisableIt(ProjectAdapterModel paraObject)
    {
        Project itemData = Mapper.Map<Project>(paraObject);
        CleanTrackingHelper.Clean<Project>(context);
        Project item = await context.Project
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == paraObject.Id);
        if (item == null)
        {
        }
        else
        {
            context.Entry(item).State = EntityState.Modified;
            await context.SaveChangesAsync();
        }
    }

    public async Task EnableIt(ProjectAdapterModel paraObject)
    {
        Project itemData = Mapper.Map<Project>(paraObject);
        CleanTrackingHelper.Clean<Project>(context);
        Project item = await context.Project
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == paraObject.Id);
        if (item == null)
        {
        }
        else
        {
            context.Entry(item).State = EntityState.Modified;
            await context.SaveChangesAsync();
        }
    }
    #endregion
}
