using AIAgent.Models;
using CTMS.DataModel.Models.AIAgent;
using CTMS.Share.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIAgent.Services
{
    public class AgentService
    {
        private readonly ILogger<AgentService> logger;
        private readonly Agentsetting agentsetting;
        private readonly PatientAIInfoService patientAIInfoService;
        private readonly Phase1Phase2Service phase1Phase2Service;

        public AgentService(ILogger<AgentService> logger,
            IOptions<Agentsetting> agentsettingOptions,
            PatientAIInfoService patientAIInfoService,
            Phase1Phase2Service phase1Phase2Service)
        {
            logger = logger;
            this.agentsetting = agentsettingOptions.Value;
            this.patientAIInfoService = patientAIInfoService;
            this.phase1Phase2Service = phase1Phase2Service;
        }

        public async Task RunAsync()
        {
            await ProceeInBoundAsync();
            await ProceePhase1Async();
        }

        #region 不同階段的處理作法
        async Task ProceeInBoundAsync()
        {
            List<string> inBoundDirectories = Directory.GetDirectories(agentsetting.GetInboundQueuePath()).ToList();

            #region 將該資料夾搬到 Phase 1 資料夾內
            foreach (var folder in inBoundDirectories)
            {
                await Task.Delay(1000); // 模擬處理時間\
                var folderName = Path.GetFileName(folder);
                Directory.Move(folder,
                    Path.Combine(agentsetting.GetPhase1QueuePath(), folderName));
            }
            #endregion
        }

        async Task ProceePhase1Async()
        {
            List<string> inBoundDirectories = Directory.GetDirectories(agentsetting.GetPhase1QueuePath()).ToList();

            #region 將該資料夾搬到 Phase 1 資料夾內
            foreach (var folder in inBoundDirectories)
            {
                var files = Directory.GetFiles(folder, "*.json");
                string jsonFile = files.FirstOrDefault(x => Path.GetFileName(x).StartsWith(MagicObjectHelper.PrefixPatientData));
                PatientAIInfo patientAIInfo = await patientAIInfoService.ReadAsync(jsonFile);
                Phase1LabelGeneration phase1LabelGeneration =
                    phase1Phase2Service.BuildPhase1標註生成Json(patientAIInfo, agentsetting);
                phase1Phase2Service.SavePhase1標註生成Json(phase1LabelGeneration, patientAIInfo, agentsetting);
                //    await Task.Delay(1000); // 模擬處理時間\
                //    var folderName = Path.GetFileName(folder);
                //    Directory.Move(folder,
                //        Path.Combine(agentsetting.GetPhase1QueuePath(), folderName));
            }
            #endregion
        }

        #endregion

        public async Task PrepareQueueDirectoryAsync()
        {
            string queueFolderPath = agentsetting.QueueFolderPath;
            if (!Directory.Exists(queueFolderPath))
                Directory.CreateDirectory(queueFolderPath);
            string inBoundQueuePath = agentsetting.GetInboundQueuePath();
            if (!Directory.Exists(inBoundQueuePath))
                Directory.CreateDirectory(inBoundQueuePath);
            string phase1QueuePath = agentsetting.GetPhase1QueuePath();
            if (!Directory.Exists(phase1QueuePath))
                Directory.CreateDirectory(phase1QueuePath);
            string phase1WaitingQueuePath = agentsetting.GetPhase1WaitingQueuePath();
            if (!Directory.Exists(phase1WaitingQueuePath))
                Directory.CreateDirectory(phase1WaitingQueuePath);
            string phase1TmpFolderPath = agentsetting.GetPhase1TmpFolderPath();
            if (!Directory.Exists(phase1TmpFolderPath))
                Directory.CreateDirectory(phase1TmpFolderPath);
            string phase2QueuePath = agentsetting.GetPhase2QueuePath();
            if (!Directory.Exists(phase2QueuePath))
                Directory.CreateDirectory(phase2QueuePath);
            string phase2WaitingQueuePath = agentsetting.GetPhase2WaitingQueuePath();
            if (!Directory.Exists(phase2WaitingQueuePath))
                Directory.CreateDirectory(phase2WaitingQueuePath);
            string phase2TmpFolderPath = agentsetting.GetPhase2TmpFolderPath();
            if (!Directory.Exists(phase2TmpFolderPath))
                Directory.CreateDirectory(phase2TmpFolderPath);
            string phase3QueuePath = agentsetting.GetPhase3QueuePath();
            if (!Directory.Exists(phase3QueuePath))
                Directory.CreateDirectory(phase3QueuePath);
            string phase3WaitingQueuePath = agentsetting.GetPhase3WaitingQueuePath();
            if (!Directory.Exists(phase3WaitingQueuePath))
                Directory.CreateDirectory(phase3WaitingQueuePath);
            string outBoundQueuePath = agentsetting.GetOutBoundQueuePath();
            if (!Directory.Exists(outBoundQueuePath))
                Directory.CreateDirectory(outBoundQueuePath);
            string inferencePath = agentsetting.GetInferencePath();
            if (!Directory.Exists(inferencePath))
                Directory.CreateDirectory(inferencePath);
        }

        public void CreateInBound(PatientAIInfo patientAIInfo,
            Agentsetting agentsetting)
        {
            string queueFolderPath = agentsetting.QueueFolderPath;
            string inBoundQueueName = agentsetting.InBoundQueueName;
            string inBoundQueuePath = Path.Combine(queueFolderPath, inBoundQueueName);
            string patientFolder = Path.Combine(inBoundQueuePath, patientAIInfo.KeyName);
            string sourceDicomFilename = Path.GetFileName(patientAIInfo.DicomFilename);
            patientAIInfo.DestionatioDicomFilename = Path.Combine(patientFolder, sourceDicomFilename);
            patientAIInfo.DestionatioPatientJSONFilename = Path.Combine(patientFolder, $"{MagicObjectHelper.PrefixPatientData}.json");

            #region 確保目錄要存在
            if (!Directory.Exists(inBoundQueuePath))
                Directory.CreateDirectory(inBoundQueuePath);

            if (!Directory.Exists(patientFolder))
                Directory.CreateDirectory(patientFolder);
            #endregion

            #region 將病患資料與DICOM 寫入到 Inbound 資料夾

            #endregion
            File.Copy(patientAIInfo.DicomFilename,
                Path.Combine(patientFolder, patientAIInfo.DestionatioDicomFilename), true);
            var json = patientAIInfo.ToJson();
            File.WriteAllText(patientAIInfo.DestionatioPatientJSONFilename, json, Encoding.UTF8);
        }

    }
}
