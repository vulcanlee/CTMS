using Newtonsoft.Json;
using System.Text;
using System.Text.Json;
using System.Xml;
using static EncodeJSON.Program;

namespace EncodeJSON
{
    internal class Program
    {
        /// <summary>
        /// 將物件序列化並加密為字串
        /// </summary>
        public static string EncodeJson(object obj)
        {
            // 字符映射表 (對應 Python 的 '2053769418')
            var corr = new Dictionary<char, char>
        {
            {'0', '2'}, {'1', '0'}, {'2', '5'}, {'3', '3'},
            {'4', '7'}, {'5', '6'}, {'6', '9'}, {'7', '4'},
            {'8', '1'}, {'9', '8'}
        };

            // 序列化為 JSON 字串
            string jsonString = JsonConvert.SerializeObject(obj);

            // 第一步：字符替換
            var j = jsonString.Select(x => corr.ContainsKey(x) ? corr[x] : x).ToArray();

            // 第二步：加密
            var encode = j.Select((x, i) =>
                (char)(((byte)x * 7 + (i % 5)) % 256)
            ).ToArray();

            return new string(encode);
        }

        /// <summary>
        /// 解密字串並反序列化為物件
        /// </summary>
        public static T DecodeJson<T>(string encodedString)
        {
            // 字符映射表 (對應 Python 的 '1803725496')
            var corr = new Dictionary<char, char>
        {
            {'0', '1'}, {'1', '8'}, {'2', '0'}, {'3', '3'},
            {'4', '7'}, {'5', '2'}, {'6', '5'}, {'7', '4'},
            {'8', '9'}, {'9', '6'}
        };

            // 第一步：解密
            var decode = encodedString.Select((x, i) =>
                (char)((183 * ((byte)x - (i % 5))) % 256)
            ).ToArray();

            // 第二步：字符還原
            var restored = decode.Select(x => corr.ContainsKey(x) ? corr[x] : x).ToArray();

            string jsonString = new string(restored);
            return JsonConvert.DeserializeObject<T>(jsonString);
        }

        static void Main(string[] args)
        {
            try
            {
                // 讀取原始 JSON 檔案
                string inputFile = "202510201648386456.json";
                string jsonContent = File.ReadAllText(inputFile, Encoding.UTF8);
                var originalData = JsonConvert.DeserializeObject(jsonContent);

                // 加密並輸出
                string encoded = EncodeJson(originalData);
                string outputFile = "202510201648386456.crypt.json";
                File.WriteAllText(outputFile,
                    JsonConvert.SerializeObject(encoded, Newtonsoft.Json.Formatting.Indented),
                    Encoding.UTF8);

                Console.WriteLine($"加密完成，已產生檔案：{outputFile}");

                // 測試解密（確認是否能還原）
                string encodedText = JsonConvert.DeserializeObject<string>(
                    File.ReadAllText(outputFile, Encoding.UTF8)
                );

                var decoded = DecodeJson<object>(encodedText);
                string restoredFile = "202510201648386456.decrypt.json";
                File.WriteAllText(restoredFile,
                    JsonConvert.SerializeObject(decoded, Newtonsoft.Json.Formatting.Indented),
                    Encoding.UTF8);

                Console.WriteLine($"已解密回原始內容：{restoredFile}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"錯誤：{ex.Message}");
            }
        }
    }
}
