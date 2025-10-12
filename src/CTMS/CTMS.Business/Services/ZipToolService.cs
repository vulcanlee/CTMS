using CTMS.DataModel.Models;
using CTMS.EntityModel;
using CTMS.EntityModel.Models;
using CTMS.Share.Helpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SyncExcel.Services;

namespace CTMS.Business.Services;

public class ZipToolService
{

    public ZipToolService()
    {
    }

    public async Task<string> CompressDirectoryToZipAsync(
        string sourceDirectory, string zipFilePath)
    {
        string zipFileName = string.Empty;

        string directoryName = Path.GetFileName(sourceDirectory);
        zipFileName = Path.Combine(zipFilePath, $"{directoryName}.zip");
        try
        {
            // 對於指定的目錄，將其底下的所有檔案與目錄，使用 zip 進行壓縮成為一個 zip 檔案the directory
            if (File.Exists(zipFileName))
            {
                File.Delete(zipFileName);
            }
            System.IO.Compression.ZipFile.CreateFromDirectory(sourceDirectory, zipFileName);
            return zipFileName;
        }
        catch (Exception ex)
        {
            // Handle exceptions
            return $"Error: {ex.Message}";
        }
    }
}
