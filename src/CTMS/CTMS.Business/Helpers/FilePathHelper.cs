using CTMS.DataModel.Models;

namespace CTMS.Share.Helpers;

public class FilePathHelper
{
    public static void CopyDirectory(string sourcePath, string targetPath)
    {
        if (!Directory.Exists(targetPath))
        {
            Directory.CreateDirectory(targetPath);
        }

        // 复制所有文件
        foreach (string file in Directory.GetFiles(sourcePath))
        {
            string fileName = Path.GetFileName(file);
            string destFile = Path.Combine(targetPath, fileName);
            File.Copy(file, destFile, true);
        }

        // 递归复制所有子目录
        foreach (string directory in Directory.GetDirectories(sourcePath))
        {
            string dirName = Path.GetFileName(directory);
            string destDir = Path.Combine(targetPath, dirName);
            CopyDirectory(directory, destDir); // 递归调用
        }
    }

    public static string GetAthleteUploadFinalPath(string itemName)
    {
        string result = string.Empty;
        string pathFinal = MagicObjectHelper.UploadFinalPath;
        result = Path.Combine(pathFinal, itemName);

        if(!Directory.Exists(result))
        {
            Directory.CreateDirectory(result);
        }

        return result;
    }

    public static string GetAthleteDownloadPath(string itemName)
    {
        string result = string.Empty;
        string pathFinal = MagicObjectHelper.DownloadPath;
        result = Path.Combine(pathFinal, itemName);

        if(!Directory.Exists(result))
        {
            Directory.CreateDirectory(result);
        }

        return result;
    }

    public static string GetAthleteDecompressPath(string itemName)
    {
        string result = string.Empty;
        string pathFinal = MagicObjectHelper.DecompressPath;
        result = Path.Combine(pathFinal, itemName);

        if(!Directory.Exists(result))
        {
            Directory.CreateDirectory(result);
        }

        return result;
    }

    public static string GetAthleteUploadFinalPathByTime(string itemName,
        string examineTime, string prefixFileName)
    {
        string prefixFolderName = "Data";
        string result = string.Empty;
        string pathFinal = MagicObjectHelper.UploadFinalPath;
        result = GetAthleteUploadFinalPath(itemName);
        result = Path.Combine(result, examineTime);

        if (!Directory.Exists(result))
        {
            Directory.CreateDirectory(result);
        }

        return result;
    }

    public static string GetFilenameKey(string filename)
    {
        string result = string.Empty;
        var items = filename.Split(' ');
        if (items.Length > 1)
        {
            result = items[0];
        }
        return result;
    }

    public static string CleanFilenameKey(string filename)
    {
        string result = string.Empty;
        var filenameKey = GetFilenameKey(filename);
        result = filename.Replace(filenameKey, "").Trim();
        return result;
    }
}
