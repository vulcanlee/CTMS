using AntDesign;
using AutoMapper;
using CTMS.AdapterModels;
using CTMS.Business.Factories;
using CTMS.Business.Helpers;
using CTMS.Business.Services.ClinicalInformation;
using CTMS.DataModel.Dtos;
using CTMS.DataModel.Models;
using CTMS.DataModel.Models.ClinicalInformation;
using CTMS.DataModel.Models.Systems;
using CTMS.EntityModel;
using CTMS.EntityModel.Models;
using CTMS.Share.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.Xml;

namespace CTMS.Services;

public class PatientService
{
    #region 欄位與屬性
    private readonly BackendDBContext context;
    private readonly BloodExameService bloodExameService;
    private readonly SurveyService survey;
    private readonly SubjectNoGeneratorService subjectNoGeneratorService;

    public IMapper Mapper { get; }
    public IConfiguration Configuration { get; }
    public ILogger<PatientService> Logger { get; }
    #endregion

    #region 建構式
    public PatientService(BackendDBContext context, IMapper mapper,
        IConfiguration configuration, ILogger<PatientService> logger,
        BloodExameService bloodExameService, SurveyService survey,
        SubjectNoGeneratorService subjectNoGeneratorService)
    {
        this.context = context;
        Mapper = mapper;
        Configuration = configuration;
        Logger = logger;
        this.bloodExameService = bloodExameService;
        this.survey = survey;
        this.subjectNoGeneratorService = subjectNoGeneratorService;
    }
    #endregion

    #region CRUD 服務
    public async Task<DataRequestResult<PatientAdapterModel>> GetAsync(DataRequest dataRequest)
    {
        List<PatientAdapterModel> data = new();
        DataRequestResult<PatientAdapterModel> result = new();
        var DataSource = context.Patient
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
        #endregion

        #region 進行分頁
        // 取得記錄總數量，將要用於分頁元件面板使用
        result.Count = DataSource.Cast<Patient>().Count();
        DataSource = DataSource.Skip(dataRequest.Skip);
        if (dataRequest.Take != 0)
        {
            DataSource = DataSource.Take(dataRequest.Take);
        }
        #endregion

        #region 在這裡進行取得資料與與額外屬性初始化
        List<PatientAdapterModel> adapterModelObjects =
            Mapper.Map<List<PatientAdapterModel>>(DataSource);

        foreach (var adapterModelItem in adapterModelObjects)
        {
            await OhterDependencyData(adapterModelItem);
        }
        #endregion

        result.Result = adapterModelObjects;
        await Task.Yield();
        return result;
    }

    public async Task<PatientAdapterModel> GetAsync(int id)

    {
        Patient item = await context.Patient
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
        if (item != null)
        {
            PatientAdapterModel result = Mapper.Map<PatientAdapterModel>(item);
            await OhterDependencyData(result);
            return result;
        }
        else
        {
            return new PatientAdapterModel() { };
        }
    }

    public async Task<PatientAdapterModel> GetAsync(string code)

    {
        Patient item = await context.Patient
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Code == code);
        if (item != null)
        {
            PatientAdapterModel result = Mapper.Map<PatientAdapterModel>(item);
            await OhterDependencyData(result);
            return result;
        }
        else
        {
            return new PatientAdapterModel() { };
        }
    }

    public async Task<List<PatientAdapterModel>> GetAsync()

    {
        List<PatientAdapterModel> result = new List<PatientAdapterModel>();
        List<Patient> items = await context.Patient
            .AsNoTracking()
            .ToListAsync();
        if (items.Count > 0)
        {
            List<PatientAdapterModel> results = Mapper.Map<List<PatientAdapterModel>>(items);
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

    public async Task<DataRequestResult<PatientAdapterModel>> GetAsync(BrowseSearchingModel browseSearching)

    {
        DataRequestResult<PatientAdapterModel> result = new DataRequestResult<PatientAdapterModel>();
        List<Patient> items = new();
        List<Patient> tempItems = await context.Patient
            .AsNoTracking()
            .ToListAsync();

        foreach (var item in tempItems)
        {
            if (browseSearching.院別.Count > 0)
            {
                if (!browseSearching.院別.Contains(item.醫院))
                {
                    continue;
                }
            }
            if (browseSearching.癌別.Count > 0)
            {
                if (!browseSearching.癌別.Contains(item.癌別))
                {
                    continue;
                }
            }
            if (browseSearching.狀態.Count > 0)
            {
                if (!browseSearching.狀態.Contains(item.狀態))
                {
                    continue;
                }
            }
            if (!string.IsNullOrEmpty(browseSearching.SearchKeyword))
            {
                if (!(item.癌別.Contains(browseSearching.SearchKeyword) ||
                    item.醫院.Contains(browseSearching.SearchKeyword)))
                {
                    continue;
                }
            }

            items.Add(item);
        }

        if (items.Count > 0)
        {
            List<PatientAdapterModel> results = Mapper.Map<List<PatientAdapterModel>>(items);
            foreach (var item in results)
            {
                await OhterDependencyData(item);
            }
            result.Count = results.Count;
            result.Result = results
                .Skip((browseSearching.PageIndex - 1) * browseSearching.PageSize)
                .Take(browseSearching.PageSize)
                .ToList();
            browseSearching.Total = result.Count;
            return result;
        }
        else
        {
            result.Reset();
            browseSearching.Total = result.Count;
            return result;
        }
    }

    public async Task<(VerifyRecordResult, PatientAdapterModel, string)> AddEmptyAsync(string hospital)
    {
        try
        {
            CleanTrackingHelper.Clean<Patient>(context);
            PatientData patientData = new();

            #region 依據院別，產生出 Name
            string subjectNoPrefix = "";
            switch (hospital)
            {
                case "成大醫院":
                    subjectNoPrefix = MagicObjectHelper.prefix成大醫院;
                    break;
                case "奇美醫院":
                    subjectNoPrefix = MagicObjectHelper.prefix奇美醫院;
                    break;
                case "郭綜合醫院":
                    subjectNoPrefix = MagicObjectHelper.prefix郭綜合醫院;
                    break;
                default:
                    break;
            }

            string newSubjectNo = await subjectNoGeneratorService.GenerateAsync(subjectNoPrefix);
            #endregion

            string subjectNo = newSubjectNo;
            patientData.臨床資訊.SubjectNo = subjectNo;

            Patient itemParameter = new()
            {
                Id = 0,
                Code = Guid.NewGuid().ToString(),
                Name = subjectNo,
                JsonData = patientData.ToJson(),
                醫院 = hospital,
                癌別 = MagicObjectHelper.NA,
                AI評估 = MagicObjectHelper.NA,
                AI處理 = MagicObjectHelper.NA,
                組別 = MagicObjectHelper.NA,
                狀態 = MagicObjectHelper.Patient狀態_收案,
            };
            await context.Patient
                .AddAsync(itemParameter);
            await context.SaveChangesAsync();
            Logger.LogInformation($"新增病患資料，Id={itemParameter.Id}, Name={itemParameter.Name}, Subject No={subjectNo}");

            PatientAdapterModel result = Mapper.Map<PatientAdapterModel>(itemParameter);
            CleanTrackingHelper.Clean<Patient>(context);
            return (VerifyRecordResultFactory.Build(true), result, newSubjectNo);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "新增記錄發生例外異常");
            return (VerifyRecordResultFactory.Build(false, "新增記錄發生例外異常", ex), null, null);
        }
    }

    public async Task<VerifyRecordResult> AddAsync(PatientAdapterModel paraObject)
    {
        try
        {
            Patient itemParameter = Mapper.Map<Patient>(paraObject);

            CleanTrackingHelper.Clean<Patient>(context);
            await context.Patient
                .AddAsync(itemParameter);
            await context.SaveChangesAsync();
            Logger.LogInformation($"新增病患資料，Id={itemParameter.Id}, Name={itemParameter.Name}");
            CleanTrackingHelper.Clean<Patient>(context);
            return VerifyRecordResultFactory.Build(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "新增記錄發生例外異常");
            return VerifyRecordResultFactory.Build(false, "新增記錄發生例外異常", ex);
        }
    }

    public async Task<VerifyRecordResult> UpdateAsync(PatientAdapterModel paraObject)
    {
        try
        {
            Patient itemData = Mapper.Map<Patient>(paraObject);
            PatientData patientData = new();
            patientData.FromJson(itemData.JsonData);

            CleanTrackingHelper.Clean<Patient>(context);
            Patient item = await context.Patient
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == paraObject.Id);
            if (item == null)
            {
                return VerifyRecordResultFactory.Build(false, "無法修改紀錄");
            }
            else
            {
                itemData.Name = patientData.臨床資訊.SubjectNo;
                itemData.癌別 = patientData.臨床資訊.CancerType;
                CleanTrackingHelper.Clean<Patient>(context);
                context.Entry(itemData).State = EntityState.Modified;
                await context.SaveChangesAsync();
                Logger.LogInformation($"修改病患資料，Id={itemData.Id}, Name={itemData.Name}, Subject No={itemData.Name}");
                CleanTrackingHelper.Clean<Patient>(context);
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
            CleanTrackingHelper.Clean<Patient>(context);
            Patient item = await context.Patient
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
            if (item == null)
            {
                return VerifyRecordResultFactory.Build(false, "無法刪除紀錄");
            }
            else
            {
                CleanTrackingHelper.Clean<Patient>(context);
                context.Entry(item).State = EntityState.Deleted;
                await context.SaveChangesAsync();
                Logger.LogInformation($"刪除病患資料，Id={item.Id}, Name={item.Name}");
                CleanTrackingHelper.Clean<Patient>(context);
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
    public async Task<VerifyRecordResult> BeforeAddCheckAsync(PatientAdapterModel paraObject)
    {
        //var searchItem = await context.Patient
        //    .AsNoTracking()
        //    .FirstOrDefaultAsync(x => x.Account == paraObject.Account);
        //if (searchItem != null)
        //{
        //    return VerifyRecordResultFactory.Build(false, "要新增的紀錄已經存在無法新增");
        //}

        return VerifyRecordResultFactory.Build(true);
    }

    public async Task<VerifyRecordResult> BeforeUpdateCheckAsync(PatientAdapterModel paraObject)
    {
        //if (paraObject.Account.ToLower() == MagicObjectHelper.開發者帳號)
        //{
        //    return VerifyRecordResultFactory.Build(false, ErrorMessageEnum.開發者帳號不可以被修改);
        //}

        CleanTrackingHelper.Clean<Patient>(context);
        var searchItem = await context.Patient
         .AsNoTracking()
         .FirstOrDefaultAsync(x => x.Id == paraObject.Id);
        if (searchItem == null)
        {
            return VerifyRecordResultFactory.Build(false, "要更新的紀錄 發生同時存取衝突 已經不存在資料庫上");
        }

        return VerifyRecordResultFactory.Build(true);
    }

    public async Task<VerifyRecordResult> BeforeDeleteCheckAsync(PatientAdapterModel paraObject)
    {
        CleanTrackingHelper.Clean<Patient>(context);
        var searchItem = await context.Patient
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
    async Task OhterDependencyData(PatientAdapterModel data)
    {
        await Task.Yield();
    }

   public async Task ChangeStatusData(PatientAdapterModel data)
    {
        Patient itemData = Mapper.Map<Patient>(data);
        CleanTrackingHelper.Clean<Patient>(context);
        Patient item = await context.Patient
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == data.Id);
        if (item == null)
        {
            return;
        }

        if (data.狀態 == MagicObjectHelper.Patient狀態_收案)
        {
            // 變更為結案
            itemData.狀態 = MagicObjectHelper.Patient狀態_退出;
            CleanTrackingHelper.Clean<Patient>(context);
            context.Entry(itemData).State = EntityState.Modified;
            await context.SaveChangesAsync();
            Logger.LogInformation($"修改病患資料，Id={itemData.Id}, Name={itemData.Name}, Subject No={itemData.Name} 狀態為 {MagicObjectHelper.Patient狀態_退出}");
            CleanTrackingHelper.Clean<Patient>(context);
        }
        else if (data.狀態 == MagicObjectHelper.Patient狀態_退出)
        {
            // 變更為收案
            itemData.狀態 = MagicObjectHelper.Patient狀態_收案;
            CleanTrackingHelper.Clean<Patient>(context);
            context.Entry(itemData).State = EntityState.Modified;
            await context.SaveChangesAsync();
            Logger.LogInformation($"修改病患資料，Id={itemData.Id}, Name={itemData.Name}, Subject No={itemData.Name} 狀態為 {MagicObjectHelper.Patient狀態_收案}");
            CleanTrackingHelper.Clean<Patient>(context);
        }
    }

    #endregion

    #region 紀錄啟用或停用
    #endregion
}
