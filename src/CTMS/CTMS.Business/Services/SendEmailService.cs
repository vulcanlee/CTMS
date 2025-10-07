using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTMS.Business.Services
{
    public class SendEmailService
    {
        public async Task SendNotifyEmailAsync(string 單號, string url)
        {
            string apiKey = "49dfa69fd825f184e0d79ba1c241280c-e1076420-d53a7605";  //MAILGUN_API_KEY
            string domain = "sandboxb5e1b9ed3198413b9161bc169acae141.mailgun.org";
            string mailgunBaseUrl = $"https://api.mailgun.net/v3/{domain}/messages";

            var client = new HttpClient();
            var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"api:{apiKey}"));
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authToken);

            var form = new MultipartFormDataContent();
            form.Add(new StringContent("chi0602@exentric.com.tw"), "from");
            form.Add(new StringContent("chi0602@exentric.com.tw"), "to");
            form.Add(new StringContent("AI 臨床試驗管理平臺 - Dicom 影像上傳通知"), "subject");
            form.Add(new StringContent($@"蔡主任您好：

                                                                                這封信件是由 AI 臨床試驗管理平臺所發送，對應單號為：{單號}。
                                                                                請您點擊以下連結以上傳 Dicom 影像檔案：

                                                                                上傳連結: {url}

                                                                                若有任何問題，請隨時與我們聯繫。
                                                                                謝謝您的協助！"), "text");
            form.Add(new StringContent($@"<p>蔡主任您好：</p>
                                                                                <p>這封信件是由 <strong>AI 臨床試驗管理平臺</strong> 所發送，對應單號為：{單號}。</p>
                                                                                <p>請您點擊以下按鈕以轉跳至上傳 Dicom 影像檔案介面：</p>
                                                                                <p>
                                                                                  <a href="" {url}""
                                                                                     style=""display:inline-block;padding:10px 20px;background-color:#4CAF50;color:white;text-decoration:none;border-radius:5px;"">
                                                                                    點此轉跳頁面
                                                                                  </a>
                                                                                </p>
                                                                                <p>若有任何問題，請隨時與我們聯繫。<br/>謝謝您的協助！</p>"), "html");


            var response = await client.PostAsync(mailgunBaseUrl, form);
            var responseContent = await response.Content.ReadAsStringAsync();
        }
    }
}
