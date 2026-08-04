using CTMS.DataModel.Models.ClinicalInformation;
using CTMS.DataModel.Models.Dashboard;
using CTMS.EntityModel;
using CTMS.Share.Helpers;
using Microsoft.EntityFrameworkCore;

namespace CTMS.Business.Services;

/// <summary>
/// 收案進度統計頁的統計服務（醫院 × 癌別 × 期別 × 組別 交叉統計）
/// 統計口徑與 DashboardService 一致：只計 狀態=收案 且 組別為 AI/Dr 的病人
/// </summary>
public class EnrollmentProgressService
{
    private readonly Microsoft.Extensions.Logging.ILogger<EnrollmentProgressService> logger;
    private readonly BackendDBContext context;

    public const string 卵巢癌 = "卵巢癌";
    public const string 內膜癌 = "內膜癌";
    private static readonly string[] StageNames = new[] { "I", "II", "III", "IV" };

    public EnrollmentProgressViewModel Progress { get; set; } = new();

    public EnrollmentProgressService(Microsoft.Extensions.Logging.ILogger<EnrollmentProgressService> logger,
        BackendDBContext context)
    {
        this.logger = logger;
        this.context = context;
    }

    public void Build()
    {
        Progress = new EnrollmentProgressViewModel();

        var hospitalNames = HospitalRegistry.PrefixOwners
            .Select(x => x.ShortName)
            .ToList();

        foreach (var hospitalName in hospitalNames)
        {
            var hospital = new HospitalEnrollment
            {
                HospitalName = hospitalName,
                Subtotal = 0,
            };

            foreach (var cancerName in new[] { 卵巢癌, 內膜癌 })
            {
                var cancer = new CancerEnrollment { CancerName = cancerName };

                foreach (var stageName in StageNames)
                {
                    cancer.Stages.Add(new StageCell
                    {
                        StageName = stageName,
                        ControlCount = 0,
                        ExperimentalCount = 0,
                    });
                }

                hospital.Cancers.Add(cancer);
            }

            Progress.Hospitals.Add(hospital);
        }
    }

    public async System.Threading.Tasks.Task RefreshAsync()
    {
        PatientData patientData = new();
        int pageSize = 10;
        int page = 1;

        while (true)
        {
            var patients = await context.Patient
                    .Where(patient => patient.狀態 == MagicObjectHelper.Patient狀態_收案)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

            // 沒有資料時退出
            if (patients.Count == 0)
                break;

            page++;

            foreach (var patient in patients)
            {
                // 未隨機分組者不計入（與儀表板統計口徑一致）
                bool isExperimental = patient.組別 == MagicObjectHelper.組別實驗組英文;
                bool isControl = patient.組別 == MagicObjectHelper.組別對照組英文;
                if (isExperimental == false && isControl == false)
                    continue;

                patientData.FromJson(patient.JsonData);

                var hospital = Progress.Hospitals
                    .FirstOrDefault(a => a.HospitalName == GetHospitalName(patient.醫院));
                var cancerName = GetCancerName(patientData.臨床資訊?.CancerType);
                var stageName = GetStageName(patientData.臨床資訊?.FIGOStaging);

                if (hospital is null || cancerName is null || stageName is null)
                {
                    Progress.UnclassifiedCount++;
                    continue;
                }

                var stageCell = hospital.Cancers
                    .First(a => a.CancerName == cancerName)
                    .Stages.First(a => a.StageName == stageName);

                if (isExperimental)
                {
                    stageCell.ExperimentalCount++;
                }
                else
                {
                    stageCell.ControlCount++;
                }

                hospital.Subtotal++;
                Progress.TotalCount++;
            }
        }
    }

    private static string? GetHospitalName(string? hospital)
    {
        if (string.IsNullOrEmpty(hospital))
        {
            return null;
        }

        // 精確比對 DisplayName 後歸戶到 prefix 擁有者短名（柳營奇美醫院 → 奇美）；
        // 「高雄榮民總醫院」不含短名「高榮」，不能只靠 Contains 比對。
        return HospitalRegistry.NormalizeToOwnerShortName(hospital);
    }

    private static string? GetCancerName(string? cancerType)
    {
        if (string.IsNullOrEmpty(cancerType))
        {
            return null;
        }

        if (cancerType.Contains("卵巢癌"))
        {
            return 卵巢癌;
        }

        if (cancerType.Contains("子宮內膜癌"))
        {
            return 內膜癌;
        }

        return null;
    }

    private static string? GetStageName(string? figoStaging)
    {
        if (string.IsNullOrEmpty(figoStaging))
        {
            return null;
        }

        // 由長到短比對，避免 IA 誤判為 IV 等問題
        if (figoStaging.StartsWith("IV"))
        {
            return "IV";
        }

        if (figoStaging.StartsWith("III"))
        {
            return "III";
        }

        if (figoStaging.StartsWith("II"))
        {
            return "II";
        }

        if (figoStaging.StartsWith("I"))
        {
            return "I";
        }

        return null;
    }
}
