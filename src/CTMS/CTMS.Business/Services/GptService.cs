using Azure.AI.OpenAI;
using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTMS.Business.Services;

public class GptService
{
    ChatClient chatClient = null;
    public void Build()
    {
        // 讀取環境變數 AOAILabKey 的 API Key
        string apiKey = System.Environment.GetEnvironmentVariable("AOAILabKey");
        AzureOpenAIClient azureClient = new(
            new Uri("https://gpt4tw.openai.azure.com/"),
            new System.ClientModel.ApiKeyCredential(apiKey));
        chatClient = azureClient.GetChatClient("gpt-4");

        //ZeroShot(chatClient, "請判斷以下產品評論的情感是正面還是負面：'這款產品的質量非常糟糕，我絕對不會再買。'");
    }


    public async Task<List<string>> ZeroShotAsync(string promptText)
    {
        if(chatClient == null)
        {
            Build();
        }
        #region GPT
        StringBuilder result = new();
        string userPrompt;
        List<ChatMessage> prompts;
        ChatCompletion completion;
        userPrompt = $"{promptText}";
        prompts = new()
        {
            UserChatMessage.CreateUserMessage(userPrompt),
        };

        completion = await chatClient.CompleteChatAsync(prompts);
        foreach (var message in completion.Content)
        {
            result.AppendLine(message.Text+"\n");
        }
        #endregion
        // StringBuilder 要轉換成為 List<string>
        return result.ToString().Split("\n").ToList();
    }
}

