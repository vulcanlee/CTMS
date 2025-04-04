using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace CTMS.DataModel.Models
{
    public class FileListNodeModel
    {
        public string Filename { get; set; }
        public string Path { get; set; }
        public FileTypeEnum FileType { get; set; }
    }

    public enum FileTypeEnum
    {
        NA,
        OnlyPng,
        Bar,
        Hist,
        Imat,
        Lama,
        Muscle5,
        MuscleFat,
        MuscleQuality,
        Myosteotosis,
        Nama,
        Sma,
        Xlsx,
        Photo
    }
}
