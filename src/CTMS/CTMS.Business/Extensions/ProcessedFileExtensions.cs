using CTMS.DataModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTMS.Business.Extensions;

public static class ProcessedFileCollectionExtensions
{
    public static string Check(this List<ProcessedFile> processedFile, bool byPassMuscle5 = false)
    {
        string result = "";
        string preConnect = " , ";
        foreach (var file in processedFile)
        {
            if (file.FileType == FileTypeEnum.Muscle5 && byPassMuscle5)
            {
                continue;
            }
            if (string.IsNullOrEmpty(result))
            {
                preConnect = "";
            }
            else
            {
                preConnect = " , ";
            }
            if (file.Count == 0)
            {
                result += $"{preConnect}{file.FileType} 尚未上傳 ";
            } else if (file.Count > 1)
            {
                result += $"{preConnect}{file.FileType} 超過一個 ";
            }
        }
        return result;
    }
}
