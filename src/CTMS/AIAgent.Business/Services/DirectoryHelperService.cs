using AIAgent.Models;
using CTMS.DataModel.Models.AIAgent;
using CTMS.Share.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIAgent.Services;

public class DirectoryHelperService
{
    public void MoveDirectoryRecursive(string sourceDir, string destDir, bool overwrite)
    {
        if (!Directory.Exists(sourceDir))
            throw new DirectoryNotFoundException($"來源不存在: {sourceDir}");

        // 若目的不存在且同一磁碟，直接 Move（最快）
        bool sameVolume = string.Equals(
            Path.GetPathRoot(Path.GetFullPath(sourceDir)),
            Path.GetPathRoot(Path.GetFullPath(destDir)),
            StringComparison.OrdinalIgnoreCase);

        if (!Directory.Exists(destDir))
        {
            if (sameVolume)
            {
                Directory.Move(sourceDir, destDir);
                return;
            }
            Directory.CreateDirectory(destDir);
        }
        else
        {
            if (!overwrite)
            {
                // 避免覆寫，直接拋出
                throw new IOException($"目的資料夾已存在: {destDir}");
            }
        }

        // 遞迴複製（支援跨磁碟 / 合併）
        foreach (var file in Directory.GetFiles(sourceDir))
        {
            var fileName = Path.GetFileName(file);
            var targetFile = Path.Combine(destDir, fileName);
            Directory.CreateDirectory(destDir);
            File.Copy(file, targetFile, overwrite: true);
        }

        foreach (var dir in Directory.GetDirectories(sourceDir))
        {
            var dirName = Path.GetFileName(dir);
            var targetSub = Path.Combine(destDir, dirName);
            MoveDirectoryRecursive(dir, targetSub, overwrite);
        }

        // 全部成功後刪除來源（遞迴）
        try
        {
            Directory.Delete(sourceDir, recursive: true);
        }
        catch (Exception ex)
        {
            // 失敗時可記錄 Log；此處先重新拋出
            throw new IOException($"刪除來源資料夾失敗: {sourceDir}", ex);
        }
    }

    public void CopyDirectory(string sourceDir, string destDir, bool overwrite)
    {
        if (!Directory.Exists(sourceDir))
            throw new DirectoryNotFoundException($"來源不存在: {sourceDir}");
        if (!Directory.Exists(destDir))
        {
            Directory.CreateDirectory(destDir);
        }
        else
        {
            if (!overwrite)
            {
                // 避免覆寫，直接拋出
                throw new IOException($"目的資料夾已存在: {destDir}");
            }
        }
        // 遞迴複製
        foreach (var file in Directory.GetFiles(sourceDir))
        {
            var fileName = Path.GetFileName(file);
            var targetFile = Path.Combine(destDir, fileName);
            File.Copy(file, targetFile, overwrite: true);
        }
        foreach (var dir in Directory.GetDirectories(sourceDir))
        {
            var dirName = Path.GetFileName(dir);
            var targetSub = Path.Combine(destDir, dirName);
            CopyDirectory(dir, targetSub, overwrite);
        }
    }

    public void CreateDirectoryIfNotExists(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }
}
