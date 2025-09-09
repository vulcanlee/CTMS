using AIAgent.Models;
using AIAgent.Services;
using CTMS.DataModel.Models.AIAgent;
using Microsoft.Extensions.Options;

namespace AIAgent
{
    public class AIAgentWorker : BackgroundService
    {
        private readonly ILogger<AIAgentWorker> _logger;
        private readonly AgentService agentService;
        private readonly Agentsetting agentsetting;

        public AIAgentWorker(ILogger<AIAgentWorker> logger,
            AgentService agentService,
            IOptions<Agentsetting> agentsettingOptions)
        {
            _logger = logger;
            this.agentService = agentService;
            agentsetting = agentsettingOptions.Value;
        }

        PatientAIInfo Sample()
        {
            PatientAIInfo patientAIInfo = new PatientAIInfo()
            {
                Code = "P2024001",
                SubjectCode = "S2024001",
                Height = "183.0",
                Weight = "55",
                Age = "49",
                Gender = "M",
                癌別 = "EC",
                DicomFilename = @"C:\temp\A19R5802373.DCM",
            };
            patientAIInfo.InitKeyName();
            return patientAIInfo;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await agentService.PrepareQueueDirectoryAsync();
                while (!stoppingToken.IsCancellationRequested)
                {
                    if (_logger.IsEnabled(LogLevel.Information))
                    {
                        _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                    }

                    #region 建立測試資料
                    //PatientAIInfo patientAIInfo = Sample();
                    //agentService.CreateInBound(patientAIInfo, agentsetting);
                    #endregion

                    await agentService.RunAsync();

                    await Task.Delay(1000, stoppingToken);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // 正常取消，忽略例外
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Worker 發生未預期的例外，服務將結束。");
                throw;
            }
            finally
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker stopping at: {time}", DateTimeOffset.Now);
                }
            }
        }
    }
}
