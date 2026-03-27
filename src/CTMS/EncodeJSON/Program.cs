using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace EncodeJSON
{
    internal class Program
    {
        private static readonly JsonSerializerSettings PythonCompatibleJsonSettings = new()
        {
            StringEscapeHandling = StringEscapeHandling.EscapeNonAscii
        };

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

            // 序列化為與 Python json.dumps(..., ensure_ascii=True) 相近的 JSON 字串
            string jsonString = SerializeJsonLikePython(obj);

            // 第一步：字符替換
            var j = jsonString.Select(x => corr.ContainsKey(x) ? corr[x] : x).ToArray();

            // 第二步：加密
            var encode = j.Select((x, i) =>
                (char)((x * 7 + (i % 5)) % 256)
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
                (char)((NormalizeByte(x - (i % 5)) * 183) % 256)
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
                var utf8NoBom = new UTF8Encoding(false);

                // 讀取原始 JSON 檔案
                string inputFile = "202510201648386456.json";
                string jsonContent = File.ReadAllText(inputFile, Encoding.UTF8);
                var originalData = ParseJsonLikePython(jsonContent);

                // 加密並輸出
                string encoded = EncodeJson(originalData);
                string outputFile = "202510201648386456.crypt.json";
                File.WriteAllText(outputFile,
                    JsonConvert.SerializeObject(encoded, PythonCompatibleJsonSettings),
                    utf8NoBom);

                Console.WriteLine($"加密完成，已產生檔案：{outputFile}");

                // 測試解密（確認是否能還原）
                string encodedText = JsonConvert.DeserializeObject<string>(
                    File.ReadAllText(outputFile, utf8NoBom)
                );

                var decoded = DecodeJson<JToken>(encodedText);
                string restoredFile = "202510201648386456.decrypt.json";
                File.WriteAllText(restoredFile,
                    SerializeJsonLikePython(decoded),
                    utf8NoBom);

                Console.WriteLine($"已解密回原始內容：{restoredFile}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"錯誤：{ex.Message}");
            }
        }

        private static JToken ParseJsonLikePython(string jsonContent)
        {
            using var stringReader = new StringReader(jsonContent);
            using var jsonReader = new JsonTextReader(stringReader)
            {
                DateParseHandling = DateParseHandling.None
            };

            return JToken.ReadFrom(jsonReader);
        }

        private static string SerializeJsonLikePython(object obj)
        {
            var json = JsonConvert.SerializeObject(obj, PythonCompatibleJsonSettings);
            return AddSpacesAfterSeparators(json);
        }

        private static string AddSpacesAfterSeparators(string json)
        {
            var builder = new StringBuilder(json.Length);
            var inString = false;
            var isEscaped = false;

            foreach (var ch in json)
            {
                builder.Append(ch);

                if (inString && isEscaped)
                {
                    isEscaped = false;
                    continue;
                }

                if (ch == '\\' && inString)
                {
                    isEscaped = true;
                    continue;
                }

                if (ch == '"')
                {
                    inString = !inString;
                    continue;
                }

                if (!inString && (ch == ',' || ch == ':'))
                {
                    builder.Append(' ');
                }
            }

            return builder.ToString();
        }

        private static int NormalizeByte(int value) => ((value % 256) + 256) % 256;
    }
}
