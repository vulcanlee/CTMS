using Azure.AI.OpenAI;
using CTMS.Business.Services;
using CTMS.DataModel.Models;
using CTMS.EntityModel.Models;
using CTMS.Share.Helpers;
using FellowOakDicom.Log;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTMS.Services;

public class OperationHistoryTraceContentService
{
    private readonly Microsoft.Extensions.Logging.ILogger<OperationHistoryTraceContentService> logger;
    private readonly IServiceScopeFactory serviceScopeFactory;

    public OperationHistoryTraceContentService(Microsoft.Extensions.Logging.ILogger<OperationHistoryTraceContentService> logger,
        IServiceScopeFactory serviceScopeFactory)
    {
        this.logger = logger;
        this.serviceScopeFactory = serviceScopeFactory;
    }

    public async Task Process操作差異摘要async(OperationHistoryTrace operationHistoryTrace, string sourceObjectJson, string targetObjectJson, string 操作對象)
    {
        var task = Task.Run(async () =>
        {
            StringBuilder builder = new StringBuilder();
            string Prompt最初說明Instruction = MagicObjectHelper.Prompt最初說明Instruction.Replace("@@", 操作對象);
            builder.AppendLine(Prompt最初說明Instruction + Environment.NewLine);
            builder.AppendLine(MagicObjectHelper.Prompt原始資料Instruction + Environment.NewLine);
            builder.AppendLine(sourceObjectJson + Environment.NewLine);
            builder.AppendLine(MagicObjectHelper.Prompt編輯後Instruction + Environment.NewLine);
            builder.AppendLine(targetObjectJson + Environment.NewLine);
            builder.AppendLine(MagicObjectHelper.PromptOperationHistoryInstruction + Environment.NewLine);
            string chatCompletions = string.Empty;
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var operationHistoryTraceService = scope.ServiceProvider.GetRequiredService<OperationHistoryTraceService>();
                var gptService = scope.ServiceProvider.GetRequiredService<GptService>();
                var logger = scope.ServiceProvider.GetRequiredService<Microsoft.Extensions.Logging.ILogger<OperationHistoryTraceContentService>>();
                logger.LogInformation($"開始呼叫 GPT 產生操作差異摘要 (ID={operationHistoryTrace.Id}) ...{Environment.NewLine}{builder.ToString()}");
                chatCompletions = await gptService.Get操作差異摘要Async(builder.ToString());

                if (!string.IsNullOrWhiteSpace(chatCompletions))
                {
                    OperationHistoryTraceResponseModel operationHistoryTraceResponseModel = JsonConvert.DeserializeObject<OperationHistoryTraceResponseModel>(chatCompletions);
                    if (operationHistoryTraceResponseModel != null && operationHistoryTraceResponseModel.IsChanged == true)
                    {
                        operationHistoryTrace.Name = operationHistoryTraceResponseModel.Summary;
                        operationHistoryTrace.Description = operationHistoryTraceResponseModel.Detail;

                        await operationHistoryTraceService.UpdateAsync(operationHistoryTrace);
                        logger.LogInformation($"GPT 已回傳有效的操作差異摘要內容 (ID={operationHistoryTrace.Id}) Name={operationHistoryTrace.Name} ___ {operationHistoryTrace.Description}");
                    }
                }
                else
                {
                    logger.LogWarning($"GPT 未回傳有效的操作差異摘要內容 (ID={operationHistoryTrace.Id})");
                }
            }
        });
    }

    public async Task SaveAsync(OperationHistoryTrace operationHistoryTrace, string sourceObjectJson, string targetObjectJson, string 操作對象)
    {
        string savePath = Path.Combine(MagicObjectHelper.Folder操作歷程);
        string keyFilename = operationHistoryTrace.CreateAt.ToString("yyyyMMdd_HHmmss_fff");
        string fileNameSourceJson = Path.Combine(savePath, $"{keyFilename}_{operationHistoryTrace.Id}_source.json");
        string fileNameTargetJson = Path.Combine(savePath, $"{keyFilename}_{operationHistoryTrace.Id}_target.json");

        await File.WriteAllTextAsync(fileNameSourceJson, sourceObjectJson, Encoding.UTF8);
        await File.WriteAllTextAsync(fileNameTargetJson, targetObjectJson, Encoding.UTF8);

        Process操作差異摘要async(operationHistoryTrace, sourceObjectJson, targetObjectJson, 操作對象);
    }

    public async Task<(string sourceObjectJson, string targetObjectJson)> ReadAsync(OperationHistoryTrace operationHistoryTrace)
    {
        string savePath = Path.Combine(MagicObjectHelper.Folder操作歷程);
        string keyFilename = operationHistoryTrace.CreateAt.ToString("yyyyMMdd_HHmmss_fff");
        string fileNameSourceJson = Path.Combine(savePath, $"{keyFilename}_{operationHistoryTrace.Id}_source.json");
        string fileNameTargetJson = Path.Combine(savePath, $"{keyFilename}_{operationHistoryTrace.Id}_target.json");
        string sourceObjectJson = await File.ReadAllTextAsync(fileNameSourceJson, Encoding.UTF8);
        string targetObjectJson = await File.ReadAllTextAsync(fileNameTargetJson, Encoding.UTF8);
        return (sourceObjectJson, targetObjectJson);
    }
}

