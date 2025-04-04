using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTMS.DataModel.Models;

public class ProcessedFile
{
    public FileTypeEnum FileType { get; set; }
    public bool IsUpload { get; set; } = false;
    public int Count { get; set; } = 0;
    public string ClassName
    {
        get
        {
            if (IsUpload)
            {
                if (Count == 1)
                    return "alert alert-success";
                else
                    return "alert alert-danger";
            }
            else
            {
                return "alert alert-secondary";
            }
        }
    }

    public string Title
    {
        get
        {
            return $"{FileType} x {Count}";
        }
    }
    public string Description { get; set; }

    public static List<ProcessedFile> Build()
    {
        List<ProcessedFile> processedFiles = new List<ProcessedFile>();
        processedFiles.Add(new ProcessedFile()
        { FileType = FileTypeEnum.Xlsx, Description = "Xlsx" });
        processedFiles.Add(new ProcessedFile()
        { FileType = FileTypeEnum.OnlyPng, Description = "OnlyPng" });
        processedFiles.Add(new ProcessedFile()
        { FileType = FileTypeEnum.Bar, Description = "Bar" });
        processedFiles.Add(new ProcessedFile()
        { FileType = FileTypeEnum.Hist, Description = "Hist" });
        processedFiles.Add(new ProcessedFile()
        { FileType = FileTypeEnum.Imat, Description = "Imat" });
        processedFiles.Add(new ProcessedFile()
        { FileType = FileTypeEnum.Lama, Description = "Lama" });
        processedFiles.Add(new ProcessedFile()
        { FileType = FileTypeEnum.Muscle5, Description = "Muscle5" });
        processedFiles.Add(new ProcessedFile()
        { FileType = FileTypeEnum.MuscleFat, Description = "MuscleFat" });
        processedFiles.Add(new ProcessedFile()
        { FileType = FileTypeEnum.MuscleQuality, Description = "MuscleQuality" });
        processedFiles.Add(new ProcessedFile()
        { FileType = FileTypeEnum.Myosteotosis, Description = "Myosteotosis" });
        processedFiles.Add(new ProcessedFile()
        { FileType = FileTypeEnum.Nama, Description = "Nama" });
        processedFiles.Add(new ProcessedFile()
        { FileType = FileTypeEnum.Sma, Description = "Sma" });
        return processedFiles;
    }
}
