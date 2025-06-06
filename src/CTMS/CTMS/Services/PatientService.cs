using AutoMapper;
using CTMS.AdapterModels;
using CTMS.Business.Factories;
using CTMS.Business.Helpers;
using CTMS.Business.Services.ClinicalInformation;
using CTMS.DataModel.Dtos;
using CTMS.DataModel.Models;
using CTMS.DataModel.Models.ClinicalInformation;
using CTMS.DataModel.Models.Questionnaire;
using CTMS.EntityModel;
using CTMS.EntityModel.Models;
using Microsoft.EntityFrameworkCore;

namespace CTMS.Services;

public class PatientService
{
    #region 欄位與屬性
    private readonly BackendDBContext context;
    private readonly BloodExameService bloodExameService;
    private readonly SurveyService survey;

    public IMapper Mapper { get; }
    public IConfiguration Configuration { get; }
    public ILogger<PatientService> Logger { get; }
    #endregion

    #region 建構式
    public PatientService(BackendDBContext context, IMapper mapper,
        IConfiguration configuration, ILogger<PatientService> logger,
        BloodExameService bloodExameService, SurveyService survey)
    {
        this.context = context;
        Mapper = mapper;
        Configuration = configuration;
        Logger = logger;
        this.bloodExameService = bloodExameService;
        this.survey = survey;
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

    public async Task<VerifyRecordResult> AddEmptyAsync()
    {
        try
        {
            CleanTrackingHelper.Clean<Patient>(context);
            PatientData patientData = new();
            string name = "0-" + DateTime.Now.ToString("yyyyMMddHHmmss");
            patientData.臨床資訊.SubjectNo = name;
            //survey.Read(patientData.臨床資料.問卷);
            //bloodExameService.Read(patientData.臨床資料.抽血檢驗);
            Patient itemParameter = new()
            {
                Id = 0,
                Code = Guid.NewGuid().ToString(),
                Name = name,
                JsonData = patientData.ToJson(),
                醫院 = "NA",
                癌別 = "NA",
            };
            await context.Patient
                .AddAsync(itemParameter);
            await context.SaveChangesAsync();
            CleanTrackingHelper.Clean<Patient>(context);
            return VerifyRecordResultFactory.Build(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "新增記錄發生例外異常");
            return VerifyRecordResultFactory.Build(false, "新增記錄發生例外異常", ex);
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
    #endregion

    #region 紀錄啟用或停用
    #endregion
}
