using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTMS.Business.Helpers
{
    public class ZipHelper
    {
        /// <summary>
        /// 將指定目錄壓縮成 zip 檔。
        /// </summary>
        /// <param name="sourceDirectory">要壓縮的資料夾路徑</param>
        /// <param name="destinationArchiveFilePath">最終壓縮檔檔名 (含完整路徑)</param>
        /// <param name="compressionLevel">壓縮等級，可選 Optimal、Fastest、NoCompression</param>
        /// <param name="includeBaseDirectory">
        ///   若為 true，則會把 <paramref name="sourceDirectory"/> 本身當作壓縮檔內的根目錄資料夾；
        ///   若為 false，則只會壓縮目錄底下的檔案和子目錄，而不包含最外層目錄本身。
        /// </param>
        public static void CompressDirectory(
            string sourceDirectory,
            string destinationArchiveFilePath,
            CompressionLevel compressionLevel = CompressionLevel.Optimal,
            bool includeBaseDirectory = false)
        {
            try
            {
                // 第四個參數 includeBaseDirectory：true 表示保留最外層資料夾
                ZipFile.CreateFromDirectory(
                    sourceDirectory,
                    destinationArchiveFilePath,
                    compressionLevel,
                    includeBaseDirectory);
            }
            catch(Exception ex)
            {
                throw new Exception($"壓縮檔案失敗: {ex.Message}");
            }
        }

        public static void DecompressZipFile(
                string sourceZipFilePath,
                string destinationDirectory,
                bool overwrite = true // 只適用於 .NET 6+
            )
        {
            // 如果目標資料夾不存在，先建立
            if (!Directory.Exists(destinationDirectory))
            {
                Directory.CreateDirectory(destinationDirectory);
            }

            ZipFile.ExtractToDirectory(sourceZipFilePath, destinationDirectory, overwrite);
        }
    }
}
