using CTMS.DataModel.Models;
using CTMS.EntityModel;
using CTMS.EntityModel.Models;
using CTMS.Share.Helpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SyncExcel.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CTMS.Business.Services
{
    public class DeleteFileService
    {
        private readonly BackendDBContext backendDBContext;
        private readonly ExcleService excleService;

        public DeleteFileService(BackendDBContext backendDBContext,
            ExcleService excleService)
        {
            this.backendDBContext = backendDBContext;
            this.excleService = excleService;
        }

        public async Task DeleteAsync(int id)
        {
            string pathTemp = MagicObjectHelper.UploadTempPath;
            string pathFinal = MagicObjectHelper.UploadFinalPath;

            var athlete = await backendDBContext.Athlete
                .FirstOrDefaultAsync(x => x.Id == id);
            if (athlete != null)
            {

                if (athlete.ExamineTime.Trim() != "")
                {
                    pathFinal = FilePathHelper
                        .GetAthleteUploadFinalPath(athlete.Name);

                    var examines = await backendDBContext.Examine
                        .Where(x => x.AthleteId == id)
                        .ToListAsync();
                    foreach (var examine in examines)
                    {
                        backendDBContext.Examine.Remove(examine);
                    }
                    await backendDBContext.SaveChangesAsync();

                    backendDBContext.Athlete.Remove(athlete);
                    await backendDBContext.SaveChangesAsync();

                    #region 移除相關檔案
                    Directory.Delete(pathFinal, true);
                    #endregion
                }
                else
                {
                    backendDBContext.Athlete.Remove(athlete);
                    await backendDBContext.SaveChangesAsync();

                    #region 移除相關檔案
                    string filesJsonContent = athlete.FilesData;
                    List<FileListNodeModel> files = JsonConvert
                        .DeserializeObject<List<FileListNodeModel>>(filesJsonContent);
                    foreach (var file in files)
                    {
                        var fileTargetPath = Path.Combine(pathFinal, file.Filename);
                        File.Delete(fileTargetPath);
                    }
                    #endregion
                }
            }
            else
            {
            }
        }

        public async Task AddAsync(List<FileListNodeModel> files)
        {
            string pathTemp = MagicObjectHelper.UploadTempPath;
            string pathFinal = MagicObjectHelper.UploadFinalPath;

            foreach (var file in files)
            {
                var fileSourcePath = Path.Combine(pathTemp, file.Filename);
                var fileTargetPath = Path.Combine(pathFinal, file.Filename);
                File.Move(fileSourcePath, fileTargetPath);
                file.Path = fileTargetPath;
            };

            string filesJsonContent = JsonConvert.SerializeObject(files);
            string excelJsonContent = "";
            var fileItem = files.FirstOrDefault(x => x.FileType == FileTypeEnum.Xlsx);
            if (fileItem != null)
            {
                string excelFilename = fileItem.Path;
                var CTMSModel = excleService.ReadExcel(excelFilename);
                excelJsonContent = JsonConvert.SerializeObject(CTMSModel);

                Athlete athlete = new Athlete()
                {
                    Name = CTMSModel.HomePageModel.姓名,
                    Code = Guid.NewGuid().ToString(),
                    FilesData = filesJsonContent,
                    ExcelData = excelJsonContent
                };
                Console.WriteLine(JsonConvert.SerializeObject(athlete));
                await backendDBContext.Athlete.AddAsync(athlete);
                await backendDBContext.SaveChangesAsync();
            }
        }

        public async Task<List<Athlete>> GetAsync()
        {
            return await backendDBContext.Athlete.ToListAsync();
        }

        public async Task<Athlete> GetAsync(string code)
        {
            var result = await backendDBContext.Athlete
                .FirstOrDefaultAsync(x => x.Code == code);
            return result;
        }
    }
}
