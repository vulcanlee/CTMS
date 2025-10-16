using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace CTMS.Business.Services
{
    public class SendEmailService
    {
        // ✅ 請改成你自己的 Gmail 與應用程式密碼
        private readonly string _smtpServer = "smtp.gmail.com";
        private readonly int _smtpPort = 587; // 使用 TLS
        private readonly string _fromEmail = "exetest0845@gmail.com";
        private readonly string _appPassword = "zghfhkjdvjdwyrey"; // 這裡請填你的 16 碼 App Password（無空白）

        public async Task SendNotifyEmailAsync(string 單號, string 病歷號, string url)
        {
            try
            {
                using (var client = new SmtpClient(_smtpServer, _smtpPort))
                {
                    // 設定 SMTP
                    client.EnableSsl = true;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(_fromEmail, _appPassword);

                    // 建立信件內容
                    var mail = new MailMessage();
                    mail.From = new MailAddress(_fromEmail, "AI 臨床試驗管理平臺");
                    mail.To.Add("chi0602@exentric.com.tw"); // 📩 收件人

                    mail.Subject = "AI 臨床試驗管理平臺 - Dicom 影像上傳通知";
                    mail.IsBodyHtml = false; // ❌ 不使用 HTML

                    mail.Body = $@"
蔡主任您好：

這封信件是由 AI 臨床試驗管理平臺 所發送
對應單號為：{單號}
對應病歷號為：{病歷號}

請您點擊以下連結以上傳 Dicom 影像檔案：
{url}

若有任何問題，請隨時與我們聯繫。
謝謝您的協助！
";

                    // 寄送
                    await client.SendMailAsync(mail);
                    Console.WriteLine("✅ 郵件寄送成功！");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ 郵件寄送失敗：" + ex.ToString());
            }
        }
    }
}
