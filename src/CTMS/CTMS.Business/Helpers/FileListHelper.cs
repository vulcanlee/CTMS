using CTMS.DataModel.Models;

namespace CTMS.Share.Helpers;

public class FileListHelper
{
    //IMAGE_202178520.png
    //IMAGE_202178520_bar.png
    //IMAGE_202178520_hist.png
    //IMAGE_202178520_imat.png
    //IMAGE_202178520_lama.png
    //IMAGE_202178520_muscle5.png
    //IMAGE_202178520_muscleFat.png
    //IMAGE_202178520_muscleQuality.png
    //IMAGE_202178520_myosteotosis.png
    //IMAGE_202178520_nama.png
    //IMAGE_202178520_sma.png
    //Test for PE dashboard.xlsx
    public static bool CheckFileType(List<FileListNodeModel> fileListNodeModels)
    {
        bool result = false;
        int successFileCount = 0;
        int maxSuccessFileCount = 12;

        foreach (var item in fileListNodeModels)
        {
            if (item.Filename.EndsWith("_bar.png"))
            {
                item.FileType = FileTypeEnum.Bar;
                successFileCount++;
                continue;
            }
            else if (item.Filename.EndsWith("_hist.png"))
            {
                item.FileType = FileTypeEnum.Hist;
                successFileCount++;
                continue;
            }
            else if (item.Filename.EndsWith("_imat.png"))
            {
                item.FileType = FileTypeEnum.Imat;
                successFileCount++;
                continue;
            }
            else if (item.Filename.EndsWith("_lama.png"))
            {
                item.FileType = FileTypeEnum.Lama;
                successFileCount++;
                continue;
            }
            else if (item.Filename.EndsWith("_muscle5.png"))
            {
                item.FileType = FileTypeEnum.Muscle5;
                successFileCount++;
                continue;
            }
            else if (item.Filename.EndsWith("_muscleFat.png"))
            {
                item.FileType = FileTypeEnum.MuscleFat;
                successFileCount++;
                continue;
            }
            else if (item.Filename.EndsWith("_muscleQuality.png"))
            {
                item.FileType = FileTypeEnum.MuscleQuality;
                successFileCount++;
                continue;
            }
            else if (item.Filename.EndsWith("_myosteotosis.png"))
            {
                item.FileType = FileTypeEnum.Myosteotosis;
                successFileCount++;
                continue;
            }
            else if (item.Filename.EndsWith("_nama.png"))
            {
                item.FileType = FileTypeEnum.Nama;
                successFileCount++;
                continue;
            }
            else if (item.Filename.EndsWith("_sma.png"))
            {
                item.FileType = FileTypeEnum.Sma;
                successFileCount++;
                continue;
            }
            else if (item.Filename.ToLower().Contains("photo"))
            {
                item.FileType = FileTypeEnum.Photo;
                successFileCount++;
                continue;
            }
            else if (item.Filename.EndsWith(".xlsx"))
            {
                item.FileType = FileTypeEnum.Xlsx;
                successFileCount++;
                continue;
            }
            else if (item.Filename.EndsWith(".png"))
            {
                item.FileType = FileTypeEnum.OnlyPng;
                successFileCount++;
                continue;
            }
        }

        if (successFileCount == maxSuccessFileCount)
        {
            result = true;
        }
        return result;
    }

    public static string GetPhoto(List<FileListNodeModel> fileLists)
    {
        string result = "";
        // IMAGE_病歷號碼(加一碼亂碼)_muscle5
        var item = fileLists.FirstOrDefault(x=>x.FileType == FileTypeEnum.Photo);
        if(item != null)
        {
            result = item.Path;
        }
        return result;
    }

    public static string GetMuscle5(List<FileListNodeModel> fileLists)
    {
        string result = "";
        // IMAGE_病歷號碼(加一碼亂碼)_muscle5
        var item = fileLists.FirstOrDefault(x=>x.FileType == FileTypeEnum.Muscle5);
        if(item != null)
        {
            result = item.Path;
        }
        return result;
    }

    public static string GetMuscleFat(List<FileListNodeModel> fileLists)
    {
        string result = "";
        // IMAGE_病歷號碼(加一碼亂碼)_muscle5
        var item = fileLists.FirstOrDefault(x=>x.FileType == FileTypeEnum.MuscleFat);
        if(item != null)
        {
            result = item.Path;
        }
        return result;
    }

    public static string GetHist(List<FileListNodeModel> fileLists)
    {
        string result = "";
        // IMAGE_病歷號碼(加一碼亂碼)_hist
        var item = fileLists.FirstOrDefault(x=>x.FileType == FileTypeEnum.Hist);
        if(item != null)
        {
            result = item.Path;
        }
        return result;
    }

    public static string GetMuscleQuality(List<FileListNodeModel> fileLists)
    {
        string result = "";
        // IMAGE_病歷號碼(加一碼亂碼)_muscleQuality
        var item = fileLists.FirstOrDefault(x => x.FileType == FileTypeEnum.MuscleQuality);
        if (item != null)
        {
            result = item.Path;
        }
        return result;
    }

    public static string GetNama(List<FileListNodeModel> fileLists)
    {
        string result = "";
        // IMAGE_病歷號碼(加一碼亂碼)_nama
        var item = fileLists.FirstOrDefault(x => x.FileType == FileTypeEnum.Nama);
        if (item != null)
        {
            result = item.Path;
        }
        return result;
    }

    public static string GetLama(List<FileListNodeModel> fileLists)
    {
        string result = "";
        // IMAGE_病歷號碼(加一碼亂碼)_lama
        var item = fileLists.FirstOrDefault(x => x.FileType == FileTypeEnum.Lama);
        if (item != null)
        {
            result = item.Path;
        }
        return result;
    }

    public static string GetImat(List<FileListNodeModel> fileLists)
    {
        string result = "";
        // IMAGE_病歷號碼(加一碼亂碼)_imat
        var item = fileLists.FirstOrDefault(x => x.FileType == FileTypeEnum.Imat);
        if (item != null)
        {
            result = item.Path;
        }
        return result;
    }

    public static string GetXlsx(List<FileListNodeModel> fileLists)
    {            
        string result = "";
        // IMAGE_病歷號碼(加一碼亂碼)_imat
        var item = fileLists.FirstOrDefault(x => x.FileType == FileTypeEnum.Xlsx);
        if (item != null)
        {
            result = item.Path;
        }
        return result;
    }
}
