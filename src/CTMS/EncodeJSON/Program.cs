using System.Text;
using System.Text.Json;
using static EncodeJSON.Program;

namespace EncodeJSON
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var filename = "202510201648386456.json";
            var content = System.IO.File.ReadAllText(filename);

            // Encode
            JsonCipher.Encode(filename, content);
            // 這個字串可能包含 0..255 的任意字元；若要保存成 JSON 檔，建議再包一層 JsonSerializer.Serialize(cipher)


            // Decode 回原物件
            //var back = JsonCipher.Decode<dynamic>(cipher);
            // back.a == 123, back.b == [1,2,3], back.c == "hello"  }
        }
        public static class JsonCipher
        {
            // 與 Python 相同的數字對應
            // encode 前置替換：'0'..'9' -> "2053769418"[index]
            private static readonly string EncodeDigitMap = "2053769418";
            // decode 後置替換：'0'..'9' -> "1803725496"[index]
            private static readonly string DecodeDigitMap = "1803725496";

            // 7 在 mod 256 下的乘法反元素是 183（Python 使用的 183）
            private const int Mul = 7;
            private const int MulInv = 183; // because 7 * 183 = 1281 ≡ 1 (mod 256)

            /// <summary>
            /// 將任意可序列化物件編碼成字串（與 Python encodeJson 對應）
            /// </summary>
            public static void Encode(string filename, string obj)
            {
                // 1) 先做 JSON 序列化（注意：不同語言的 JSON 文字細節可能不同，但不影響同語言的解碼）
                string json = obj;

                // 2) 數字字元對應 (等同 Python: {str(i): x for i,x in enumerate('2053769418')})
                var corr = new Dictionary<char, char>
                {
                    ['0'] = '2',
                    ['1'] = '0',
                    ['2'] = '5',
                    ['3'] = '3',
                    ['4'] = '7',
                    ['5'] = '6',
                    ['6'] = '9',
                    ['7'] = '4',
                    ['8'] = '1',
                    ['9'] = '8',
                };

                // 3) 逐字元替換
                var replaced = new char[json.Length];
                for (int i = 0; i < json.Length; i++)
                {
                    char ch = json[i];
                    replaced[i] = corr.TryGetValue(ch, out var mapped) ? mapped : ch;
                }

                // 4) 位置相關的字元映射: chr((ord(x)*7 + (i%5)) % 256)
                var encoded = new char[replaced.Length];
                for (int i = 0; i < replaced.Length; i++)
                {
                    int o = replaced[i];                 // 原始 UTF-16 code unit
                    int v = ((o * 7) + (i % 5)) & 0xFF;  // 僅保留低 8 位 (等同 % 256)
                    encoded[i] = (char)v;                // 放進字串；之後 JSON 會跳脫不可列印字元
                }

                // 使用二進位方式保存編碼結果
                System.IO.File.WriteAllBytes(filename + ".enc", Encoding.Latin1.GetBytes(encoded));

            }

            /// <summary>
            /// 將 Encode 產生的字串還原為型別 T（與 Python decodeJson 對應）
            /// </summary>
            public static T Decode<T>(string cipher)
            {
                // 1) 逐字元做 (183 * (ord - (i % 5))) % 256
                var step = new StringBuilder(cipher.Length);
                for (int i = 0; i < cipher.Length; i++)
                {
                    int ord = cipher[i];

                    // 先減 (i % 5)，保持在 0..255 範圍
                    int tmp = ord - (i % 5);
                    tmp &= 0xFF;

                    // 乘上 183 並取 mod 256
                    int val = (MulInv * tmp) & 0xFF;
                    step.Append((char)val);
                }

                // 2) 對數字字元做解碼替換（'0'..'9' -> "1803725496"[index]）
                var restored = new StringBuilder(step.Length);
                foreach (char ch in step.ToString())
                {
                    if (ch >= '0' && ch <= '9')
                    {
                        restored.Append(DecodeDigitMap[ch - '0']);
                    }
                    else
                    {
                        restored.Append(ch);
                    }
                }

                // 3) 還原 JSON
                return JsonSerializer.Deserialize<T>(restored.ToString())!;
            }
        }
    }
}
