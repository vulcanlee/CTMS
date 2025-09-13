using AIAgent.Models;
using CTMS.DataModel.Models.AIAgent;
using CTMS.Share.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SyncExcel.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
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
        private readonly DirectoryHelperService directoryHelperService;
        private readonly RiskAssessmentExcelService riskAssessmentExcelService;

        public AgentService(ILogger<AgentService> logger,
            IOptions<Agentsetting> agentsettingOptions,
            PatientAIInfoService patientAIInfoService,
            Phase1Phase2Service phase1Phase2Service,
            DirectoryHelperService directoryHelperService,
            RiskAssessmentExcelService riskAssessmentExcelService)
        {
            logger = logger;
            this.agentsetting = agentsettingOptions.Value;
            this.patientAIInfoService = patientAIInfoService;
            this.phase1Phase2Service = phase1Phase2Service;
            this.directoryHelperService = directoryHelperService;
            this.riskAssessmentExcelService = riskAssessmentExcelService;
        }

        public async Task RunAsync()
        {
            await ProceeInBoundAsync();
            await Task.Delay(500);
            await ProceePhase1Async();
            await Task.Delay(500);
            await ProceePhase1WaitingAsync();
            await Task.Delay(500);
            await ProceePhase2Async();
            await Task.Delay(500);
            await ProceePhase2WaitingAsync();
            await Task.Delay(500);
            await ProceePhase3Async();
            await Task.Delay(500);
            await ProceePhase3WaitingAsync();
            await Task.Delay(500);
            await ProceeCompleteAsync();
            await Task.Delay(500);
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
            // Queue Phase 1 資料夾內的所有病患資料夾
            List<string> inBoundDirectories = Directory.GetDirectories(agentsetting.GetPhase1QueuePath()).ToList();

            #region 將該資料夾搬到 Phase 1 資料夾內
            foreach (var folder in inBoundDirectories)
            {
                string folderName = Path.GetFileName(folder);
                string destFolder = Path.Combine(agentsetting.GetPhase1WaitingQueuePath(), folderName);
                if (Directory.Exists(destFolder))
                {
                    Directory.Delete(destFolder, true);
                }

                var files = Directory.GetFiles(folder, "*.json");
                string jsonFile = files.FirstOrDefault(x => Path.GetFileName(x).StartsWith(MagicObjectHelper.PrefixPatientData));
                PatientAIInfo patientAIInfo = await patientAIInfoService.ReadAsync(jsonFile);

                await phase1Phase2Service.CopyDicomAsync(patientAIInfo, agentsetting);
                Phase1LabelGeneration phase1LabelGeneration =
                    phase1Phase2Service.BuildPhase1標註生成Json(patientAIInfo, agentsetting);
                phase1Phase2Service.SavePhase1標註生成Json(phase1LabelGeneration, patientAIInfo, agentsetting);

                await phase1Phase2Service.MoveToPhase1WaitingAsync(patientAIInfo, agentsetting);
            }
            #endregion
        }

        async Task ProceePhase1WaitingAsync()
        {
            List<string> phase1WaitingDirectories = Directory.GetDirectories(agentsetting.GetPhase1WaitingQueuePath()).ToList();
            List<string> phase1TmpDirectories = Directory.GetDirectories(agentsetting.GetPhase1TmpFolderPath()).ToList();

            #region 將該資料夾搬到 Phase 1 資料夾內
            foreach (var folder in phase1WaitingDirectories)
            {
                var waitingFolderName = Path.GetFileName(folder);
                var tmpFolder = phase1TmpDirectories
                    .FirstOrDefault(x => Path.GetFileName(x) == waitingFolderName);
                if (tmpFolder != null)
                {
                    var tmpFolderfiles = Directory.GetFiles(tmpFolder);
                    if (tmpFolderfiles.Length >= 2)
                    {
                        await Task.Delay(1000); // 模擬處理時間
                        var sourcePath = Path.Combine(agentsetting.GetPhase1TmpFolderPath(), tmpFolder);
                        var destinationPath = Path.Combine(folder, MagicObjectHelper.Phase1ResultPath);

                        directoryHelperService.CopyDirectory(sourcePath, destinationPath, overwrite: true);

                        var files = Directory.GetFiles(folder, "*.json");
                        string jsonFile = files.FirstOrDefault(x => Path.GetFileName(x).StartsWith(MagicObjectHelper.PrefixPatientData));
                        PatientAIInfo patientAIInfo = await patientAIInfoService.ReadAsync(jsonFile);
                        await phase1Phase2Service.MoveToPhase2Async(patientAIInfo, agentsetting);
                    }
                }
            }
            #endregion
        }

        async Task ProceePhase2Async()
        {
            // Queue Phase 2 資料夾內的所有病患資料夾
            List<string> inBoundDirectories = Directory.GetDirectories(agentsetting.GetPhase2QueuePath()).ToList();

            #region 將該資料夾搬到 Phase 2 資料夾內
            foreach (var folder in inBoundDirectories)
            {
                string folderName = Path.GetFileName(folder);
                string destFolder = Path.Combine(agentsetting.GetPhase2WaitingQueuePath(), folderName);
                if (Directory.Exists(destFolder))
                {
                    Directory.Delete(destFolder, true);
                }

                var files = Directory.GetFiles(folder, "*.json");
                string jsonFile = files.FirstOrDefault(x => Path.GetFileName(x).StartsWith(MagicObjectHelper.PrefixPatientData));
                PatientAIInfo patientAIInfo = await patientAIInfoService.ReadAsync(jsonFile);

                await phase1Phase2Service.MoveToPhase2WaitingAsync(patientAIInfo, agentsetting);

                Phase2QuantitativeAnalysis phase2LabelGeneration =
                    phase1Phase2Service.BuildPhase2標註生成Json(patientAIInfo, agentsetting);
                phase1Phase2Service.SavePhase1定量分析Json(phase2LabelGeneration, patientAIInfo, agentsetting);
            }
            #endregion
        }

        async Task ProceePhase2WaitingAsync()
        {
            List<string> phase2WaitingDirectories = Directory.GetDirectories(agentsetting.GetPhase2WaitingQueuePath()).ToList();
            List<string> phase2TmpDirectories = Directory.GetDirectories(agentsetting.GetPhase2TmpFolderPath()).ToList();

            #region 將該資料夾搬到 Phase 2 資料夾內
            foreach (var folder in phase2WaitingDirectories)
            {
                var waitingFolderName = Path.GetFileName(folder);
                var tmpFolder = phase2TmpDirectories
                    .FirstOrDefault(x => Path.GetFileName(x) == waitingFolderName);
                if (tmpFolder != null)
                {
                    var tmpFolderfiles = Directory.GetFiles(tmpFolder);
                    if (tmpFolderfiles.Length >= 23)
                    {
                        await Task.Delay(1000); // 模擬處理時間
                        var sourcePath = Path.Combine(agentsetting.GetPhase2TmpFolderPath(), tmpFolder);
                        var destinationPath = Path.Combine(folder, MagicObjectHelper.Phase2ResultPath);

                        directoryHelperService.CopyDirectory(sourcePath, destinationPath, overwrite: true);

                        var files = Directory.GetFiles(folder, "*.json");
                        string jsonFile = files.FirstOrDefault(x => Path.GetFileName(x).StartsWith(MagicObjectHelper.PrefixPatientData));
                        PatientAIInfo patientAIInfo = await patientAIInfoService.ReadAsync(jsonFile);
                        await phase1Phase2Service.MoveToPhase3Async(patientAIInfo, agentsetting);
                    }
                }

            }
            #endregion
        }

        async Task ProceePhase3Async()
        {
            // Queue Phase 2 資料夾內的所有病患資料夾
            List<string> inBoundDirectories = Directory.GetDirectories(agentsetting.GetPhase3QueuePath()).ToList();

            #region 將該資料夾搬到 Phase 2 資料夾內
            foreach (var folder in inBoundDirectories)
            {
                string folderName = Path.GetFileName(folder);
                string destFolder = Path.Combine(agentsetting.GetPhase3WaitingQueuePath(), folderName);
                if (Directory.Exists(destFolder))
                {
                    Directory.Delete(destFolder, true);
                }

                var files = Directory.GetFiles(folder, "*.json");
                string jsonFile = files.FirstOrDefault(x => Path.GetFileName(x).StartsWith(MagicObjectHelper.PrefixPatientData));
                PatientAIInfo patientAIInfo = await patientAIInfoService.ReadAsync(jsonFile);

                await phase1Phase2Service.MoveToPhase3WaitingAsync(patientAIInfo, agentsetting);
            }
            #endregion
        }

        async Task ProceeCompleteAsync()
        {
            List<string> inBoundDirectories = Directory.GetDirectories(agentsetting.GetOutBoundQueuePath()).ToList();

            #region 將該資料夾搬到 Phase 2 資料夾內
            foreach (var folder in inBoundDirectories)
            {
                var files = Directory.GetFiles(folder, "*.json");
                string jsonFile = files.FirstOrDefault(x => Path.GetFileName(x).StartsWith(MagicObjectHelper.PrefixPatientData));
                PatientAIInfo patientAIInfo = await patientAIInfoService.ReadAsync(jsonFile);

                string folderName = Path.GetFileName(folder);
                string checkResultPath = Path.Combine(folder, MagicObjectHelper.Phase3ResultPath);

                string checkOutputFile = Path.Combine(checkResultPath, MagicObjectHelper.風險評估輸出csv);

                if (!File.Exists(checkOutputFile))
                {
                    continue;
                }
                string destFolder = Path.Combine(agentsetting.GetCompleteQueuePath(), folderName);
                await phase1Phase2Service.MoveToCompletionWaitingAsync(patientAIInfo, agentsetting);
            }
            #endregion
        }

        async Task ProceePhase3WaitingAsync()
        {
            List<string> phase3WaitingDirectories = Directory.GetDirectories(agentsetting.GetPhase3WaitingQueuePath()).ToList();

            foreach (var folder in phase3WaitingDirectories)
            {
                var files = Directory.GetFiles(folder, "*.json");
                string jsonFile = files.FirstOrDefault(x => Path.GetFileName(x).StartsWith(MagicObjectHelper.PrefixPatientData));
                PatientAIInfo patientAIInfo = await patientAIInfoService.ReadAsync(jsonFile);
                await phase1Phase2Service.CopyToOutBoundAsync(patientAIInfo, agentsetting);

                var waitingFolderName = Path.GetFileName(folder);
                string excelFile = Path.Combine(agentsetting.GetOutBoundQueuePath(), waitingFolderName, MagicObjectHelper.Phase2ResultPath, $"{waitingFolderName}.csv");
                string resultCsvFile = Path.Combine(agentsetting.GetOutBoundQueuePath(), waitingFolderName, MagicObjectHelper.Phase3ResultPath, MagicObjectHelper.風險評估輸入csv);
                if (!Directory.Exists(Path.GetDirectoryName(resultCsvFile)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(resultCsvFile)!);
                }
                var checkFolder = Path.Combine(agentsetting.GetOutBoundQueuePath(), waitingFolderName);
                if (!Directory.Exists(checkFolder))
                {
                    Directory.CreateDirectory(checkFolder);
                }
                var outputPath = Path.Combine(checkFolder, MagicObjectHelper.Phase3ResultPath, MagicObjectHelper.風險評估輸出csv);

                #region 產生出要計算風險評估的 CSV
                var riskResult = riskAssessmentExcelService.ReadExcel(excelFile);

                // 若需要從病患資訊推 Tumor Grade，可在此調整
                string tumorGrade = "1";

                // 建立輸出 CSV 內容
                var sb = new StringBuilder();
                sb.AppendLine("ID,Age,Tumor.Grade,body.height.cm,body.weight.kg,Vertebral.Body.Area.cm2,Total.SMD,Total.ImatA,Total.LamaA,Total.NamaA,VatA,SatA");
                sb.AppendLine(string.Join(",", new string[] {
                    riskResult.ID,
                    riskResult.Age.ToLower().Replace("y",""),
                    tumorGrade,
                    riskResult.BodyHeight,
                    riskResult.BodyWeight,
                    riskResult.VertebralBodyAreaCm2,
                    riskResult.TotalSMD,
                    riskResult.TotalImatA,
                    riskResult.TotalLamaA,
                    riskResult.TotalNamaA,
                    riskResult.VatA,
                    riskResult.SatA
                }.Select(v => v?.Trim() ?? "")));

                File.WriteAllText(resultCsvFile, sb.ToString(), Encoding.UTF8);
                #endregion

                #region 在此處理風險評估
                Directory.Delete(folder, true);

                string workingPath = "";
                string command = "";

                if (patientAIInfo.癌別 == "EC")
                {
                    workingPath = agentsetting.風險評估模型;
                    // Rscript Run_Endometrioid_Model.R -m Endometrioid_Analysis_20250610_Model_data.RData --varname CaseIn_SMA_Imat_BMI -c 0.5 -i Testing_data.csv -o output.csv
                    command = $" Run_Endometrioid_Model.R -m Endometrioid_Analysis_20250610_Model_data.RData --varname CaseIn_SMA_Imat_BMI -c 0.5 -i {resultCsvFile} -o {outputPath}";
                }
                else
                {
                    workingPath = agentsetting.風險評估模型OC;
                    // Rscript Run_Ovarian_Model.R -m Ovarian_Analysis_20250908_Model_data.RData -v Case_SMI.BH2_Imat_BMI -d 3 -c 0.5 -i Testing.data.csv -o  output.csv
                    command = $" Run_Ovarian_Model.R -m Ovarian_Analysis_20250908_Model_data.RData -v Case_SMI.BH2_Imat_BMI -d 3 -c 0.5 -i {resultCsvFile} -o {outputPath}";
                }

                try
                {
                    var psi = new ProcessStartInfo
                    {
                        FileName = "Rscript",
                        Arguments = command,
                        WorkingDirectory = workingPath,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };

                    var process = new Process { StartInfo = psi, EnableRaisingEvents = true };
                    process.Start();
                    //string output = await process.StandardOutput.ReadToEndAsync();
                    //string error = await process.StandardError.ReadToEndAsync();

                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "執行風險評估命令時發生例外");
                }
                #endregion
            }
        }

        #endregion

        #region 初始化作業
        public async Task PrepareQueueDirectoryAsync()
        {
            string completionFolderPath = agentsetting.GetCompleteQueuePath();
            if (!Directory.Exists(completionFolderPath))
                Directory.CreateDirectory(completionFolderPath);
            string dicomFolderPath = agentsetting.GetDicomFolderPath();
            if (!Directory.Exists(dicomFolderPath))
                Directory.CreateDirectory(dicomFolderPath);
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

            var destinationFile = Path.Combine(agentsetting.DicomFolderPath, $"{patientAIInfo.KeyName}.dcm");
            var foo = Path.Combine(patientFolder, patientAIInfo.DestionatioDicomFilename);
            File.Copy(patientAIInfo.DicomFilename,
                destinationFile, true);
            string oldFilename = Path.GetFileName(patientAIInfo.DestionatioDicomFilename);
            string newFilename = Path.GetFileName(destinationFile);

            patientAIInfo.DicomFilename = destinationFile;
            patientAIInfo.DestionatioDicomFilename = patientAIInfo.DestionatioDicomFilename.Replace(oldFilename, newFilename);

            File.Copy(destinationFile, patientAIInfo.DestionatioDicomFilename, true);

            var json = patientAIInfo.ToJson();
            File.WriteAllText(patientAIInfo.DestionatioPatientJSONFilename, json, Encoding.UTF8);
        }
        #endregion
    }
}
