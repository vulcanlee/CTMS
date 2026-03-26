using AntDesign;
using CTMS.AdapterModels;
using CTMS.DataModel.Models;
using CTMS.DataModel.Models.ClinicalInformation;

namespace CTMS.Services;

public class SystemMaintainServices
{
    private readonly ILogger<SystemMaintainServices> logger;
    private readonly PatientService patientService;
    private readonly NotificationService notificationService;
    string logMessage = string.Empty;
    public SystemMaintainServices(ILogger<SystemMaintainServices> logger,
        PatientService patientService,
        NotificationService notificationService)
    {
        this.logger = logger;
        this.patientService = patientService;
        this.notificationService = notificationService;
    }
    public async Task Fix_20260326_成大抽血生化_eGFR參考區間修正()
    {
        PatientData patientData = new();
        int pageSize = 10;
        int pageIndex = 1;
        int totalCount = 0;

        logMessage = $"開始執行 Fix_20260326_成大抽血生化_eGFR參考區間修正";
        logger.LogInformation(logMessage);
        _ = notificationService.Open(new NotificationConfig
        {
            Message = "系統訊息",
            Description = logMessage,
            NotificationType = NotificationType.Warning,
            Placement = NotificationPlacement.BottomRight
        });
        for (int idx = pageIndex; idx < 100; idx++)
        {
            DataRequest dataRequest = new DataRequest()
            {
                Skip = (idx - 1) * pageSize,
                Take = pageSize,
            };

            var result = await patientService.GetAsync(dataRequest);
            List<PatientAdapterModel> patientAdapterModelts = result.Result.ToList();

            if (patientAdapterModelts == null || patientAdapterModelts.Count() == 0)
            {
                break;
            }

            // 處理 patients
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
                    continue;

                patientAdapterModel.JsonData = patientData.ToJson();
                await patientService.UpdateAsync(patientAdapterModel);

                logMessage = $"已修正患者 {patientData.臨床資訊.SubjectNo} 的 eGFR 參考區間";
                _ = notificationService.Open(new NotificationConfig
                {
                    Message = "系統訊息",
                    Description = logMessage,
                    NotificationType = NotificationType.Info,
                    Placement = NotificationPlacement.BottomRight
                });
                totalCount++;
                await Task.Delay(200); 
            }
        }

        logMessage = $"完成執行 Fix_20260326_成大抽血生化_eGFR參考區間修正，共有 {totalCount} 筆";
        logger.LogInformation(logMessage);
        _ = notificationService.Open(new NotificationConfig
        {
            Message = "系統訊息",
            Description = logMessage,
            NotificationType = NotificationType.Success,
            Placement = NotificationPlacement.BottomRight
        });
    }
}
