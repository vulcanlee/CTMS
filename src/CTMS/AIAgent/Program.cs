using AIAgent.Models;
using AIAgent.Services;
using CTMS.Share.Helpers;
using NLog;
using NLog.Web;
using SyncExcel.Services;
using System.Diagnostics;

namespace AIAgent
{
    public class Program
    {
        public static void Main(string[] args)
        {
            #region �Y�s�b���檺�P�˵{���X�A�h�����N����
            var processes = Process.GetProcesses().ToList();
            var LaunchPacsProcesses = processes.Where(x => x.ProcessName.ToLower().Contains("AIAgent".ToLower())).ToList();
            var currentProcess = Process.GetCurrentProcess();
            foreach (var item in LaunchPacsProcesses)
            {
                if (item.Id != currentProcess.Id)
                {
                    Console.WriteLine($"Process {item.Id} will be killed!");
                    return;
                }
            }
            #endregion

            #region NLog ��l��
            var logger = NLog.LogManager.Setup()
                .LoadConfigurationFromAppSettings().GetCurrentClassLogger();
            logger.Debug("init main");
            #endregion

            try
            {
                var builder = Host.CreateApplicationBuilder(args);
                builder.Services.AddHostedService<AIAgentWorker>();
                builder.Services.AddTransient<AgentService>();
                builder.Services.AddTransient<PatientAIInfoService>();
                builder.Services.AddTransient<Phase1Phase2Service>();
                builder.Services.AddTransient<DirectoryHelperService>();
                builder.Services.AddTransient<RiskAssessmentExcelService>();

                #region �[�J�]�w�j���O�`�J�ŧi
                builder.Services.Configure<Agentsetting>(builder.Configuration
                    .GetSection(MagicObjectHelper.Agentsetting));
                #endregion

                var host = builder.Build();
                host.Run();
            }
            catch (Exception exception)
            {
                // NLog: catch setup errors
                logger.Error(exception, "Stopped program because of exception");
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                NLog.LogManager.Shutdown();
            }
        }
    }
}