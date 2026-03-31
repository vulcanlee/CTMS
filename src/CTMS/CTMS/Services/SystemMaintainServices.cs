using AntDesign;
using CTMS.AdapterModels;
using CTMS.Business.Services.ClinicalInformation;
using CTMS.DataModel.Models;
using CTMS.DataModel.Models.ClinicalInformation;
using CTMS.Share.Helpers;

namespace CTMS.Services;

public class SystemMaintainServices
{
    private readonly ILogger<SystemMaintainServices> logger;
    private readonly PatientService patientService;
    private readonly NotificationService notificationService;
    private readonly BloodExameService bloodExameService;
    string logMessage = string.Empty;

    public SystemMaintainServices(ILogger<SystemMaintainServices> logger,
        PatientService patientService,
        NotificationService notificationService,
        BloodExameService bloodExameService)
    {
        this.logger = logger;
        this.patientService = patientService;
        this.notificationService = notificationService;
        this.bloodExameService = bloodExameService;
    }

    public async Task Fix_20260326_成大抽血生化_eGFR參考區間修正()
    {
        PatientData patientData = new();
        int pageSize = 10;
        int pageIndex = 1;
        int totalCount = 0;

        logMessage = $"開始執行 Fix_20260326_成大抽血生化_eGFR參考區間修正";
        logger.LogInformation(logMessage);
        OpenNotification(logMessage, NotificationType.Warning);
        for (int idx = pageIndex; idx < 100; idx++)
        {
            DataRequest dataRequest = new DataRequest()
            {
                Skip = (idx - 1) * pageSize,
                Take = pageSize,
            };

            var result = await patientService.GetAsync(dataRequest);
            List<PatientAdapterModel> patientAdapterModelts = result.Result.ToList();

            if (patientAdapterModelts == null || patientAdapterModelts.Count == 0)
            {
                break;
            }

            foreach (PatientAdapterModel patientAdapterModel in patientAdapterModelts)
            {
                patientData = new();
                patientData.FromJson(patientAdapterModel.JsonData);

                if (patientAdapterModel.醫院 == "成大醫院" && patientData.臨床資訊 != null && patientData.臨床資料.抽血檢驗生化 != null)
                {
                    foreach (var item抽血檢驗生化 in patientData.臨床資料.抽血檢驗生化.Items)
                    {
                        foreach (var item in item抽血檢驗生化.抽血檢驗生化)
                        {
                            if (item.項目名稱 == "腎絲球過濾率 eGFR")
                            {
                                item.參考區間 = "≧60";
                            }
                        }
                    }
                }
                else
                {
                    continue;
                }

                patientAdapterModel.JsonData = patientData.ToJson();
                await patientService.UpdateAsync(patientAdapterModel);

                logMessage = $"已修正患者 {patientData.臨床資訊.SubjectNo} 的 eGFR 參考區間";
                OpenNotification(logMessage, NotificationType.Info);
                totalCount++;
                await Task.Delay(200);
            }
        }

        logMessage = $"完成執行 Fix_20260326_成大抽血生化_eGFR參考區間修正，共有 {totalCount} 筆";
        logger.LogInformation(logMessage);
        OpenNotification(logMessage, NotificationType.Success);
    }

    public async Task FillBloodTestUnitsAsync()
    {
        PatientData patientData = new();
        int pageSize = 10;
        int pageIndex = 1;
        int totalCount = 0;

        logMessage = "開始執行抽血血液/生化單位補齊";
        logger.LogInformation(logMessage);
        OpenNotification(logMessage, NotificationType.Warning);

        for (int idx = pageIndex; idx < 100; idx++)
        {
            DataRequest dataRequest = new DataRequest()
            {
                Skip = (idx - 1) * pageSize,
                Take = pageSize,
            };

            var result = await patientService.GetAsync(dataRequest);
            List<PatientAdapterModel> patientAdapterModels = result.Result.ToList();

            if (patientAdapterModels == null || patientAdapterModels.Count == 0)
            {
                break;
            }

            foreach (PatientAdapterModel patientAdapterModel in patientAdapterModels)
            {
                patientData = new();
                patientData.FromJson(patientAdapterModel.JsonData);

                var changed = FillBloodTestUnits(patientData);
                if (changed == false)
                {
                    continue;
                }

                patientAdapterModel.JsonData = patientData.ToJson();
                await patientService.UpdateAsync(patientAdapterModel);

                logMessage = $"已補齊患者 {patientData.臨床資訊.SubjectNo} 的抽血單位";
                OpenNotification(logMessage, NotificationType.Info);
                totalCount++;
                await Task.Delay(200);
            }
        }

        logMessage = $"完成執行抽血血液/生化單位補齊，共有 {totalCount} 筆";
        logger.LogInformation(logMessage);
        OpenNotification(logMessage, NotificationType.Success);
    }

    private bool FillBloodTestUnits(PatientData patientData)
    {
        var changed = false;
        var subjectNo = patientData.臨床資訊.SubjectNo;

        var bloodSource = new BloodTest抽血檢驗血液Node();
        bloodExameService.Read血液Node(bloodSource, subjectNo);
        var bloodUnitLookup = bloodSource.抽血檢驗血液.ToDictionary(x => x.項目名稱, x => x.單位 ?? string.Empty);

        var chemistrySource = new BloodTest抽血檢驗生化Node();
        bloodExameService.Read生化Node(chemistrySource, subjectNo);
        var chemistryUnitLookup = chemistrySource.抽血檢驗生化.ToDictionary(x => x.項目名稱, x => x.單位 ?? string.Empty);

        foreach (var bloodNode in patientData.臨床資料.抽血檢驗血液.Items)
        {
            changed |= FillTestItemUnits(bloodNode.抽血檢驗血液, bloodUnitLookup);
        }

        foreach (var chemistryNode in patientData.臨床資料.抽血檢驗生化.Items)
        {
            changed |= FillTestItemUnits(chemistryNode.抽血檢驗生化, chemistryUnitLookup);
        }

        return changed;
    }

    private static bool FillTestItemUnits(List<TestItem檢驗項目> items, Dictionary<string, string> unitLookup)
    {
        var changed = false;

        foreach (var item in items)
        {
            var originalName = item.項目名稱 ?? string.Empty;
            var originalUnit = item.單位 ?? string.Empty;
            var (normalizedName, normalizedUnit) = BloodTestItemNameUnitHelper.Parse(originalName);

            item.項目名稱 = normalizedName;
            item.單位 = normalizedUnit;

            changed |= originalName != normalizedName || originalUnit != normalizedUnit;

            if (unitLookup.TryGetValue(item.項目名稱, out var unitFromSource) &&
                (item.單位 ?? string.Empty) != unitFromSource)
            {
                item.單位 = unitFromSource;
                changed = true;
            }
        }

        return changed;
    }

    private void OpenNotification(string description, NotificationType notificationType)
    {
        _ = notificationService.Open(new NotificationConfig
        {
            Message = "系統訊息",
            Description = description,
            NotificationType = notificationType,
            Placement = NotificationPlacement.BottomRight
        });
    }
}
