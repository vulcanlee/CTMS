using AIAgent.Models;
using CTMS.DataModel.Models.AIAgent;
using CTMS.Share.Extensions;
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
            this.logger = logger;
            this.agentsetting = agentsettingOptions.Value;
            this.patientAIInfoService = patientAIInfoService;
            this.phase1Phase2Service = phase1Phase2Service;
            this.directoryHelperService = directoryHelperService;
            this.riskAssessmentExcelService = riskAssessmentExcelService;
        }

        /// <summary>
        /// 執行代理流程的單次輪詢，依序處理各佇列階段。
        /// </summary>
        /// <remarks>
        /// 順序：
        /// 1. ProceeInBoundAsync：Inbound 搬移至 Phase 1 佇列。
        /// 2. ProceePhase1Async：準備 Phase 1 標註生成並移轉至 Phase1Waiting。
        /// 3. ProceePhase1WaitingAsync：回填外部標註結果，移轉至 Phase 2。
        /// 4. ProceePhase2Async：準備 Phase 2 定量分析並移轉至 Phase2Waiting。
        /// 5. ProceePhase2WaitingAsync：回填外部定量分析結果，移轉至 Phase 3。
        /// 6. ProceePhase3Async：移轉至 Phase3Waiting。
        /// 7. ProceePhase3WaitingAsync：產生風險評估輸入、呼叫 Rscript 產生輸出。
        /// 8. ProceeCompleteAsync：檢查輸出完成並移轉至 Complete 佇列。
        /// 各階段間以 500ms 延遲節流以避免過度頻繁 I/O。
        /// </remarks>
        /// <returns>非同步作業。</returns>
        public async Task RunAsync()
        {
            await ProceeInBoundAsync();
            await Task.Delay(150);
            await ProceePhase1Async();
            await Task.Delay(150);
            await ProceePhase1WaitingAsync();
            await Task.Delay(150);
            await ProceePhase2Async();
            await Task.Delay(150);
            await ProceePhase2WaitingAsync();
            await Task.Delay(150);
            await ProceePhase3Async();
            await Task.Delay(150);
            await ProceePhase3WaitingAsync();
            await Task.Delay(150);
            await ProceeCompleteAsync();
            await Task.Delay(150);
        }

        #region 不同階段的處理作法
        /// <summary>
        /// 將 Inbound 佇列中的每個病患資料夾移轉到 Phase 1 佇列。
        /// </summary>
        /// <remarks>
        /// 流程：
        /// 1. 列出 Inbound 根目錄下的所有病患資料夾。
        /// 2. 逐一以同名目錄搬移至 Phase 1 佇列。
        /// 注意：
        /// - 本方法未處理目的端同名資料夾衝突；若 Phase 1 已存在同名目錄，Directory.Move 會擲出例外。
        /// - Task.Delay 僅用於模擬處理時間。
        /// </remarks>
        /// <returns>非同步作業。</returns>
        /// <exception cref="IOException">來源/目的目錄存在衝突，或跨磁碟區搬移失敗。</exception>
        /// <exception cref="UnauthorizedAccessException">目錄存取權限不足。</exception>
        /// <exception cref="DirectoryNotFoundException">指定的路徑不存在。</exception>
        async Task ProceeInBoundAsync()
        {
            // 取得 Inbound 佇列下的所有病患資料夾
            List<string> inBoundDirectories = Directory.GetDirectories(agentsetting.GetInboundQueuePath()).ToList();

            #region 將該資料夾搬到 Phase 1 資料夾內
            foreach (var folder in inBoundDirectories)
            {
                await Task.Delay(1000); // 模擬處理時間
                var folderName = Path.GetFileName(folder);

                // 以同名目錄搬移至 Phase 1 佇列；若目的端已存在同名資料夾將擲出例外
                Directory.Move(folder,
                    Path.Combine(agentsetting.GetPhase1QueuePath(), folderName));
            }
            #endregion
        }

        /// <summary>
        /// 處理 Phase 1 佇列中的病患資料。
        /// 流程：
        /// 1. 列出 Phase 1 佇列資料夾中的所有病患資料夾。
        /// 2. 若 Phase 1 Waiting 佇列已存在同名資料夾，先刪除以確保重新處理狀態一致。
        /// 3. 讀取病患基本資料 JSON（前綴為 MagicObjectHelper.PrefixPatientData）。
        /// 4. 複製病患 DICOM 至標註/推論所需的位置。
        /// 5. 產生並儲存 Phase 1「標註生成」設定 JSON。
        /// 6. 將病患資料移動至 Phase 1 Waiting 佇列，等待外部標註結果。
        /// </summary>
        /// <remarks>
        /// 此方法僅負責前置準備與資料夾流轉，不會等待外部標註完成。
        /// 可能會拋出目錄/檔案存取相關例外（如 IO、權限、路徑不存在等）。
        /// </remarks>
        /// <returns>非同步作業工作。</returns>
        async Task ProceePhase1Async()
        {
            // Queue Phase 1 資料夾內的所有病患資料夾
            List<string> inBoundDirectories = Directory.GetDirectories(agentsetting.GetPhase1QueuePath()).ToList();

            #region 將該資料夾搬到 Phase 1 資料夾內
            foreach (var folder in inBoundDirectories)
            {
                // 目標等待佇列的病患資料夾路徑
                string folderName = Path.GetFileName(folder);
                string destFolder = Path.Combine(agentsetting.GetPhase1WaitingQueuePath(), folderName);

                // 若等待 Phase1Waiting 佇列已存在同名資料夾，先刪除確保重跑時乾淨狀態
                if (Directory.Exists(destFolder))
                {
                    Directory.Delete(destFolder, true);
                }

                // 尋找病患基本資料 JSON（前綴為 MagicObjectHelper.PrefixPatientData）
                var files = Directory.GetFiles(folder, "*.json");
                string jsonFile = files.FirstOrDefault(x => Path.GetFileName(x).StartsWith(MagicObjectHelper.PrefixPatientData));

                // 載入病患資訊
                PatientAIInfo patientAIInfo = await patientAIInfoService.ReadAsync(jsonFile);

                // 複製 DICOM 至標註/推論所需位置
                await phase1Phase2Service.CopyDicomAsync(patientAIInfo, agentsetting);

                // 產生 Phase 1 標註生成設定並寫入 JSON
                Phase1LabelGeneration phase1LabelGeneration =
                    phase1Phase2Service.BuildPhase1標註生成Json(patientAIInfo, agentsetting);
                phase1Phase2Service.SavePhase1標註生成Json(phase1LabelGeneration, patientAIInfo, agentsetting);

                // 將資料移至 Phase 1 Waiting 佇列等待外部標註結果
                await phase1Phase2Service.MoveToPhase1WaitingAsync(patientAIInfo, agentsetting);
            }
            #endregion
        }

        /// <summary>
        /// 監聽 Phase 1 Waiting 佇列，當對應的暫存資料夾（Phase1Tmp）中之外部標註結果就緒時，
        /// 將結果複製回病患資料夾並把個案推進到 Phase 2。
        /// </summary>
        /// <remarks>
        /// 流程：
        /// 1. 列出 Phase1Waiting 與 Phase1Tmp 兩個根目錄下的病患資料夾。
        /// 2. 以資料夾名稱比對等待佇列與暫存資料夾的對應關係。
        /// 3. 檢查暫存資料夾內檔案數量（>=2 視為結果已生成）。
        /// 4. 將暫存資料夾內容拷貝回等待佇列中的病患資料夾（Phase1ResultPath）。
        /// 5. 讀取病患基本資料 JSON，將個案移轉至 Phase 2 佇列。
        /// 注意：此方法為輪詢式處理，不阻塞等待單一個案完成。
        /// </remarks>
        /// <returns>非同步作業。</returns>
        /// <exception cref="IOException">在目錄或檔案複製/讀取過程中發生 I/O 錯誤。</exception>
        /// <exception cref="UnauthorizedAccessException">目錄或檔案存取權限不足。</exception>
        /// <exception cref="DirectoryNotFoundException">指定的路徑不存在。</exception>
        async Task ProceePhase1WaitingAsync()
        {
            // 取得 Phase 1 Waiting 與 Phase 1 Tmp 根目錄下的所有病患資料夾
            List<string> phase1WaitingDirectories = Directory.GetDirectories(agentsetting.GetPhase1WaitingQueuePath()).ToList();
            List<string> phase1TmpDirectories = Directory.GetDirectories(agentsetting.GetPhase1TmpFolderPath()).ToList();

            #region 將該資料夾搬到 Phase 1 資料夾內
            foreach (var folder in phase1WaitingDirectories)
            {
                // 病患資料夾名稱（用於對應暫存資料夾）
                var waitingFolderName = Path.GetFileName(folder);

                // 找到暫存根目錄中與 Waiting 同名的資料夾（外部標註結果所在）
                var tmpFolder = phase1TmpDirectories
                    .FirstOrDefault(x => Path.GetFileName(x) == waitingFolderName);

                if (tmpFolder != null)
                {
                    // 若暫存資料夾中的檔案數量達到門檻（視為結果已就緒）
                    var tmpFolderfiles = Directory.GetFiles(tmpFolder);
                    if (tmpFolderfiles.Length >= 2)
                    {
                        await Task.Delay(1000); // 模擬處理時間（例如等待外部程序釋放檔案）

                        // 來源：暫存結果位置
                        // 注意：tmpFolder 已為完整路徑，若再與根路徑 Combine 可能造成重複路徑
                        var sourcePath = Path.Combine(agentsetting.GetPhase1TmpFolderPath(), tmpFolder);

                        // 目的：病患資料夾下的 Phase 1 結果目錄
                        var destinationPath = Path.Combine(folder, MagicObjectHelper.Phase1ResultPath);

                        // 複製暫存結果回 Waiting 佇列的病患資料夾
                        directoryHelperService.CopyDirectory(sourcePath, destinationPath, overwrite: true);

                        // 讀取病患基本資料 JSON（以 MagicObjectHelper.PrefixPatientData 為前綴）
                        var files = Directory.GetFiles(folder, "*.json");
                        string jsonFile = files.FirstOrDefault(x => Path.GetFileName(x).StartsWith(MagicObjectHelper.PrefixPatientData));
                        PatientAIInfo patientAIInfo = await patientAIInfoService.ReadAsync(jsonFile);

                        // 將病患從 Phase 1 Waiting 移轉到 Phase 2 佇列
                        await phase1Phase2Service.MoveToPhase2Async(patientAIInfo, agentsetting);
                    }
                }
            }
            #endregion
        }

        /// <summary>
        /// 處理 Phase 2 佇列中的病患資料，為定量分析階段準備輸入。
        /// </summary>
        /// <remarks>
        /// 流程：
        /// 1. 列出 Phase 2 佇列（Phase2Queue）內所有病患資料夾。
        /// 2. 若 Phase 2 Waiting 佇列已存在同名資料夾，先刪除以確保重跑時為乾淨狀態。
        /// 3. 從病患資料夾讀取基本資料 JSON（檔名前綴為 MagicObjectHelper.PrefixPatientData）。
        /// 4. 將個案移動到 Phase 2 Waiting 佇列。
        /// 5. 產生 Phase 2「定量分析」設定 JSON 並寫入對應位置，供外部程序使用。
        /// 注意：此方法僅進行前置準備與資料夾流轉，不等待外部定量分析完成。
        /// </remarks>
        /// <returns>非同步作業。</returns>
        /// <exception cref="IOException">在檔案/目錄操作（讀取、刪除、移動、寫入）過程中發生 I/O 錯誤。</exception>
        /// <exception cref="UnauthorizedAccessException">檔案或目錄存取權限不足。</exception>
        /// <exception cref="DirectoryNotFoundException">指定的路徑不存在。</exception>
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

        /// <summary>
        /// 監聽 Phase 2 Waiting 佇列；當對應的暫存資料夾（Phase2Tmp）中之外部定量分析結果就緒時，
        /// 將結果複製回病患資料夾並把個案推進到 Phase 3。
        /// </summary>
        /// <remarks>
        /// 流程：
        /// 1. 列出 Phase2Waiting 與 Phase2Tmp 兩個根目錄下的病患資料夾。
        /// 2. 以資料夾名稱比對等待佇列與暫存資料夾的對應關係。
        /// 3. 檢查暫存資料夾內檔案數量（>= 23 視為結果已生成）。
        /// 4. 將暫存資料夾內容拷貝回等待佇列中的病患資料夾（Phase2ResultPath）。
        /// 5. 讀取病患基本資料 JSON，將個案移轉至 Phase 3 佇列。
        /// 注意：此方法為輪詢式處理，不阻塞等待單一個案完成。
        /// </remarks>
        /// <returns>非同步作業。</returns>
        /// <exception cref="IOException">在目錄或檔案複製/讀取過程中發生 I/O 錯誤。</exception>
        /// <exception cref="UnauthorizedAccessException">目錄或檔案存取權限不足。</exception>
        /// <exception cref="DirectoryNotFoundException">指定的路徑不存在。</exception>
        /// <summary>
        /// 監聽 Phase 2 Waiting 佇列；當對應的暫存資料夾（Phase2Tmp）中之外部定量分析結果就緒時，
        /// 將結果複製回病患資料夾並把個案推進到 Phase 3。
        /// </summary>
        /// <remarks>
        /// 流程：
        /// 1. 列出 Phase2Waiting 與 Phase2Tmp 兩個根目錄下的病患資料夾。
        /// 2. 以資料夾名稱比對等待佇列與暫存資料夾的對應關係。
        /// 3. 檢查暫存資料夾內檔案數量（>= 23 視為結果已生成）。
        /// 4. 將暫存資料夾內容拷貝回等待佇列中的病患資料夾（Phase2ResultPath）。
        /// 5. 讀取病患基本資料 JSON，將個案移轉至 Phase 3 佇列。
        /// 注意：此方法為輪詢式處理，不阻塞等待單一個案完成。
        /// </remarks>
        /// <returns>非同步作業。</returns>
        /// <exception cref="IOException">在目錄或檔案複製/讀取過程中發生 I/O 錯誤。</exception>
        /// <exception cref="UnauthorizedAccessException">目錄或檔案存取權限不足。</exception>
        /// <exception cref="DirectoryNotFoundException">指定的路徑不存在。</exception>
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

        /// <summary>
        /// 處理 Phase 3 佇列中的病患資料，將個案移轉至 Phase 3 Waiting 以等待後續風險評估。
        /// </summary>
        /// <remarks>
        /// 流程：
        /// 1. 列出 Phase3Queue 內所有病患資料夾。
        /// 2. 若 Phase3Waiting 已存在同名資料夾，先刪除以確保重跑為乾淨狀態。
        /// 3. 讀取病患基本資料 JSON（檔名前綴為 MagicObjectHelper.PrefixPatientData）。
        /// 4. 呼叫 MoveToPhase3WaitingAsync 將個案移轉至 Phase 3 Waiting。
        /// 注意：本方法僅負責前置流轉，不等待後續風險評估處理完成。
        /// </remarks>
        /// <returns>非同步作業。</returns>
        /// <exception cref="IOException">在檔案/目錄操作過程發生 I/O 錯誤。</exception>
        /// <exception cref="UnauthorizedAccessException">檔案或目錄存取權限不足。</exception>
        /// <exception cref="DirectoryNotFoundException">指定的路徑不存在。</exception>
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

        /// <summary>
        /// 完成佇列處理：掃描 OutBound 佇列，凡已具備 Phase 3 風險評估輸出檔者，移轉至完成佇列（Complete）。
        /// </summary>
        /// <remarks>
        /// 流程：
        /// 1. 列出 OutBound 根目錄下的病患資料夾。
        /// 2. 讀取病患基本資料 JSON（檔名前綴為 MagicObjectHelper.PrefixPatientData）。
        /// 3. 檢查 Phase3ResultPath 下的風險評估輸出檔是否存在（MagicObjectHelper.風險評估輸出csv）。
        /// 4. 若存在，呼叫 MoveToCompletionWaitingAsync 將個案移轉至完成佇列。
        /// 注意：此方法僅負責檢查與流轉，不執行任何模型或計算。
        /// </remarks>
        /// <returns>非同步作業。</returns>
        /// <exception cref="IOException">在目錄或檔案操作過程中發生 I/O 錯誤。</exception>
        /// <exception cref="UnauthorizedAccessException">檔案或目錄存取權限不足。</exception>
        /// <exception cref="DirectoryNotFoundException">指定的路徑不存在。</exception>
        /// <exception cref="FileNotFoundException">找不到必要的病患 JSON 或輸出檔案。</exception>
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

        /// <summary>
        /// 處理 Phase 3 Waiting 佇列：將個案複製至 OutBound、產生風險評估輸入 CSV，並呼叫 R 腳本執行風險評估。
        /// </summary>
        /// <remarks>
        /// 流程：
        /// 1. 逐一讀取 Phase3Waiting 內的病患資料夾與基本資料 JSON。
        /// 2. 複製病患資料至 OutBound 佇列（供外部流程與輸出彙整）。
        /// 3. 從 Phase2 結果 CSV 讀值，生成 Phase3 的風險評估輸入 CSV（依癌別決定欄位格式）。
        /// 4. 刪除 Phase3Waiting 中的個案資料夾（表示已交由外部流程處理）。
        /// 5. 根據癌別選擇對應 R 模型與參數，透過 Rscript 執行風險評估並輸出 CSV。
        /// 6. 擷取標準輸出與錯誤；例外時記錄至 logger。
        /// 注意：本方法不等待外部後續流程（除 R 腳本執行期間），輸出檔案位於 OutBound 下的 Phase3ResultPath。
        /// </remarks>
        /// <returns>非同步作業。</returns>
        /// <exception cref="System.IO.IOException">在檔案/目錄讀寫或刪除時發生 I/O 錯誤。</exception>
        /// <exception cref="System.UnauthorizedAccessException">檔案或目錄存取權限不足。</exception>
        /// <exception cref="System.ComponentModel.Win32Exception">啟動 Rscript 或外部程序時發生錯誤。</exception>
        /// <exception cref="System.IO.DirectoryNotFoundException">指定的路徑不存在。</exception>
        async Task ProceePhase3WaitingAsync()
        {
            // 取得 Phase 3 Waiting 佇列中的所有病患資料夾
            List<string> phase3WaitingDirectories = Directory.GetDirectories(agentsetting.GetPhase3WaitingQueuePath()).ToList();

            foreach (var folder in phase3WaitingDirectories)
            {
                // 讀取病患基本資料（前綴為 PrefixPatientData 的 JSON）
                var files = Directory.GetFiles(folder, "*.json");
                string jsonFile = files.FirstOrDefault(x => Path.GetFileName(x).StartsWith(MagicObjectHelper.PrefixPatientData));
                PatientAIInfo patientAIInfo = await patientAIInfoService.ReadAsync(jsonFile);

                // 複製個案至 OutBound（彙整 Phase2 結果、準備 Phase3 輸入/輸出目錄）
                await phase1Phase2Service.CopyToOutBoundAsync(patientAIInfo, agentsetting);

                var waitingFolderName = Path.GetFileName(folder);

                // Phase2 的定量分析結果（CSV）來源
                string excelFile = Path.Combine(agentsetting.GetOutBoundQueuePath(), waitingFolderName, MagicObjectHelper.Phase2ResultPath, $"{waitingFolderName}.csv");

                // Phase3 的輸入/輸出 CSV 路徑
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

                // 建立輸出 CSV 內容（依癌別決定欄位）
                var sb = new StringBuilder();

                if (patientAIInfo.癌別 == "EC")
                {
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
                }
                else
                {
                    sb.AppendLine("ID,Body.Height.cm,Body.Weight.kg,SMA,SMD,ImatA,LamaA,NamaA,MyosteatosisA,VatA,SatA");
                    sb.AppendLine(string.Join(",", new string[] {
                        riskResult.ID,
                        // Body.Height.cm
                        (riskResult.BodyHeight.ToFloat()*100).ToString(),
                        // Body.Weight.kg
                        riskResult.BodyWeight,
                        // SMA : SMA (Skeletal Muscle Area) TotalLamaA + TotalNamaA 骨骼肌面積
                        (riskResult.TotalLamaA.ToFloat()+riskResult.TotalNamaA.ToFloat()).ToString(),
                        // SMD 骨骼肌密度 Skeletal Muscle Density
                        riskResult.TotalSMD,
                        // ImatA 肌間/肌內脂肪組織面積 Intermuscular Adipose Tissue Area
                        riskResult.TotalImatA,
                        // LamaA 低密度肌肉面積  Low Attenuation Muscle Area 
                        riskResult.TotalLamaA,
                        // NamaA 正常密度肌肉面積 Normal Attenuation Muscle Area     
                        riskResult.TotalNamaA,
                        // Myosteatosis = IMAT + LAMA 肌肉脂肪變性面積
                        (riskResult.TotalImatA.ToFloat()+riskResult.TotalLamaA.ToFloat()).ToString(),
                        // VatA 內臟脂肪面積 Visceral Adipose Tissue Area                                                                                  
                        riskResult.VatA,
                        // SatA 皮下脂肪面積 Subcutaneous Adipose Tissue Area      
                        riskResult.SatA
                    }.Select(v => v?.Trim() ?? "")));
                }

                var genContent = sb.ToString();
                File.WriteAllText(resultCsvFile, genContent, Encoding.UTF8);
                #endregion

                #region 在此處理風險評估
                // 清理 Phase3Waiting 佇列中的個案資料夾（視為已交由外部流程與 OutBound 管理）
                Directory.Delete(folder, true);

                // 根據癌別選擇 R 模型與命令列參數
                string workingPath = "";
                string command = "";

                if (patientAIInfo.癌別 == "EC")
                {
                    workingPath = agentsetting.風險評估模型;
                    // 範例：Rscript Run_Endometrioid_Model.R -m Endometrioid_Analysis_20250610_Model_data.RData --varname CaseIn_SMA_Imat_BMI -c 0.5 -i Testing_data.csv -o output.csv
                    command = $" Run_Endometrioid_Model.R -m Endometrioid_Analysis_20250610_Model_data.RData --varname CaseIn_SMA_Imat_BMI -c 0.5 -i {resultCsvFile} -o {outputPath}";
                }
                else
                {
                    workingPath = agentsetting.風險評估模型OC;
                    // 範例：Rscript Run_Ovarian_Model.R -m Ovarian_Analysis_20250908_Model_data.RData -v Case_SMI.BH2_Imat_BMI -d 3 -c 0.5 -i Testing.data.csv -o output.csv
                    command = $" Run_Ovarian_Model.R -m Ovarian_Analysis_20250908_Model_data.RData -v Case_SMI.BH2_Imat_BMI -d 3 -c 0.5 -i {resultCsvFile} -o {outputPath}";
                }

                try
                {
                    // 啟動 Rscript 執行風險評估模型
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
                    string output = await process.StandardOutput.ReadToEndAsync();
                    string error = await process.StandardError.ReadToEndAsync();
                    // 此處可視需要記錄 output/error
                }
                catch (Exception ex)
                {
                    // 記錄外部程序執行失敗等例外
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
            logger.LogInformation("CreateInBound for {KeyName}", patientAIInfo.KeyName);
            string queueFolderPath = agentsetting.QueueFolderPath;
            string completeQueueName = agentsetting.CompleteQueueName;
            string inBoundQueueName = agentsetting.InBoundQueueName;
            string completeQueuePath = Path.Combine(queueFolderPath, completeQueueName);
            string inBoundQueuePath = Path.Combine(queueFolderPath, inBoundQueueName);
            string patientFolder = Path.Combine(inBoundQueuePath, patientAIInfo.KeyName);
            string sourceDicomFilename = Path.GetFileName(patientAIInfo.DicomFilename);
            patientAIInfo.DestionatioDicomFilename = Path.Combine(patientFolder, sourceDicomFilename);
            patientAIInfo.DestionatioPatientJSONFilename = Path.Combine(patientFolder, $"{MagicObjectHelper.PrefixPatientData}.json");

            #region 清除 Completion Queue
            logger.LogInformation("Clear Completion Queue for {KeyName}", patientAIInfo.KeyName);
            string targetCompletionFolder = Path.Combine(completeQueuePath, patientAIInfo.KeyName);
            if(Directory.Exists(targetCompletionFolder))
            {
                Directory.Delete(targetCompletionFolder, true);
            }
            #endregion

            #region 確保目錄要存在
            if (!Directory.Exists(inBoundQueuePath))
                Directory.CreateDirectory(inBoundQueuePath);

            if (!Directory.Exists(patientFolder))
                Directory.CreateDirectory(patientFolder);
            #endregion

            #region 將病患資料與DICOM 寫入到 Inbound 資料夾

            #endregion

            logger.LogInformation("Copy DICOM for {KeyName}", patientAIInfo.KeyName);
            var destinationFile = Path.Combine(agentsetting.DicomFolderPath, $"{patientAIInfo.KeyName}.dcm");
            var foo = Path.Combine(patientFolder, patientAIInfo.DestionatioDicomFilename);
            File.Copy(patientAIInfo.DicomFilename,
                destinationFile, true);
           
            string oldFilename = Path.GetFileName(patientAIInfo.DestionatioDicomFilename);
            string newFilename = Path.GetFileName(destinationFile);

            patientAIInfo.DicomFilename = destinationFile;
            patientAIInfo.DestionatioDicomFilename = patientAIInfo.DestionatioDicomFilename.Replace(oldFilename, newFilename);

            logger.LogInformation("Copy DICOM to {Destination} for {KeyName}", patientAIInfo.DestionatioDicomFilename, patientAIInfo.KeyName);
            File.Copy(destinationFile, patientAIInfo.DestionatioDicomFilename, true);

            var json = patientAIInfo.ToJson();
            logger.LogInformation("Write Patient JSON for {KeyName}", patientAIInfo.KeyName);
            File.WriteAllText(patientAIInfo.DestionatioPatientJSONFilename, json, Encoding.UTF8);
        }
        #endregion
    }
}
