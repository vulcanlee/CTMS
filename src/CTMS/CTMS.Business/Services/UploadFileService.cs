using CTMS.DataModel.Models;
using CTMS.EntityModel;
using CTMS.EntityModel.Models;
using CTMS.ExcelUtility.Services;
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
    public class UploadFileService
    {
        private readonly BackendDBContext backendDBContext;
        private readonly ExcleService excleService;

        public UploadFileService(BackendDBContext backendDBContext,
            ExcleService excleService)
        {
            this.backendDBContext = backendDBContext;
            this.excleService = excleService;
        }

        public string PreCheck(List<FileListNodeModel> files)
        {
            string result = "";
            string pathTemp = MagicObjectHelper.UploadTempPath;
            string pathFinal = MagicObjectHelper.UploadFinalPath;

            var fileItem = files.FirstOrDefault(x => x.FileType == FileTypeEnum.Xlsx);
            if (fileItem != null)
            {
                string excelFilename = fileItem.Path;
                result = excleService.CheckExcel(excelFilename);
            }
            else
            {
                result = "找不到Excel檔案";
            }
            return result;
        }

        public string PreCheck(string fileItem)
        {
            string result = "";

            result = excleService.CheckExcel(fileItem);

            return result;
        }

        public async Task<(int projectId, string code)>
            VerifyNameAsync(string itemName, string itemTime)
        {
            var result = await backendDBContext.Athlete
                .FirstOrDefaultAsync(x => x.Name == itemName);
            if (result != null)
            {
                var checkExamine = await backendDBContext.Examine
                    .FirstOrDefaultAsync(x => x.AthleteId == result.Id &&
                    x.ExamineTime == itemTime);
                if (checkExamine == null)
                {
                    int projectId = result.ProjectId.Value;
                    string code = result.Code;
                    return (projectId, code);
                }
                else
                {
                    return (-1, $"{result.Name} 的檢測時間 {checkExamine.ExamineTime} 已經存在，請更換檢測時間內容");
                }
            }
            return (0, "");
        }

        public async Task<(int projectId, string code)>
            VerifyUpdateNameAsync(string itemName, string itemTime)
        {
            var result = await backendDBContext.Athlete
                .FirstOrDefaultAsync(x => x.Name == itemName);
            if (result != null)
            {
                var checkExamine = await backendDBContext.Examine
                    .FirstOrDefaultAsync(x => x.AthleteId == result.Id &&
                    x.ExamineTime == itemTime);
                if (checkExamine == null)
                {
                    return (-1, $"{result.Name} 的檢測時間 {checkExamine.ExamineTime} 不存在，無法進行更新");
                }
                else
                {
                    int projectId = result.ProjectId.Value;
                    string code = result.Code;
                    return (projectId, code);
                }
            }
            return (-1, $"{result.Name} 的紀錄不存在，無法進行更新");
        }

        public async Task AddAsync(List<FileListNodeModel> files, string itemName,
            int projectId, string itemTime, string prefixFileName)
        {
            string pathTemp = MagicObjectHelper.UploadTempPath;
            string pathFinal = FilePathHelper
                .GetAthleteUploadFinalPathByTime(itemName, itemTime, prefixFileName);

            #region 對附件檔案作處理
            foreach (var file in files)
            {
                var replaceFilename = FilePathHelper.CleanFilenameKey(file.Filename);
                replaceFilename = replaceFilename.Trim();
                var fileSourcePath = Path.Combine(pathTemp, file.Filename);
                var fileTargetPath = Path.Combine(pathFinal, replaceFilename);
                File.Move(fileSourcePath, fileTargetPath);
                file.Path = fileTargetPath;
                file.Filename = replaceFilename;
            };
            #endregion

            string filesJsonContent = JsonConvert.SerializeObject(files);
            string excelJsonContent = "";
            var fileItem = files
                .FirstOrDefault(x => x.FileType == FileTypeEnum.Xlsx);

            if (fileItem != null)
            {
                string excelFilename = fileItem.Path;
                var CTMSModel = excleService.ReadExcel(excelFilename);
                excelJsonContent = JsonConvert.SerializeObject(CTMSModel);

                string name = CTMSModel.Home首頁2.姓名;
                if (!string.IsNullOrEmpty(itemName))
                    name = itemName;

                #region 更新資料庫
                var fooAthlete = await backendDBContext.Athlete
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Name == itemName &&
                    x.ProjectId == projectId);

                int athleteId = 0;
                if (fooAthlete == null)
                {
                    #region 寫入 Athlete
                    Athlete athlete = new Athlete()
                    {
                        Name = name,
                        ProjectId = projectId,
                        ExamineTime = itemTime,
                        Code = prefixFileName,
                        FilesData = filesJsonContent,
                        ExcelData = excelJsonContent
                    };

                    await backendDBContext.Athlete.AddAsync(athlete);
                    await backendDBContext.SaveChangesAsync();
                    athleteId = athlete.Id;
                    #endregion
                }
                else
                {
                    athleteId = fooAthlete.Id;
                }

                #region 寫入 Examine
                Examine examine = new()
                {
                    Code = prefixFileName,
                    ExamineTime = itemTime,
                    FilesData = filesJsonContent,
                    ExcelData = excelJsonContent,
                    AthleteId = athleteId,
                    Athlete = null,
                };

                await backendDBContext.Examine.AddAsync(examine);
                await backendDBContext.SaveChangesAsync();
                #endregion

                #endregion
            }
        }

        public async Task AddAdditionalAsync(string itemName, int projectId, string code,
            List<FileListNodeModel> originalFiles, List<FileListNodeModel> additionalFiles)
        {
            List<FileListNodeModel> processingFiles = new List<FileListNodeModel>(originalFiles);
            string pathTemp = MagicObjectHelper.UploadTempPath;
            string pathFinal = MagicObjectHelper.UploadFinalPath;
            //string pathTemp = MagicObjectHelper.UploadTempPath;
            //string pathFinal = FilePathHelper
            //    .GetAthleteUploadFinalPathByTime(itemName, itemTime, prefixFileName);

            foreach (var file in additionalFiles)
            {
                var fileSourcePath = Path.Combine(pathTemp, file.Filename);
                var fileTargetPath = Path.Combine(pathFinal, file.Filename);
                File.Move(fileSourcePath, fileTargetPath, true);
                file.Path = fileTargetPath;

                var originalFile = processingFiles.FirstOrDefault(x => x.FileType == file.FileType);
                if (originalFile != null)
                {
                    processingFiles.Remove(originalFile);
                }
                processingFiles.Add(file);
            };

            string filesJsonContent = JsonConvert.SerializeObject(processingFiles);
            string excelJsonContent = "";
            var fileItem = processingFiles
                .FirstOrDefault(x => x.FileType == FileTypeEnum.Xlsx);

            if (fileItem != null)
            {
                string excelFilename = fileItem.Path;
                var CTMSModel = excleService.ReadExcel(excelFilename);

                #region 還原原先設定文字
                var resultAthlete = await backendDBContext.Athlete
                    .FirstOrDefaultAsync(x => x.Code == code);

                var tempCTMSModel = JsonConvert
                .DeserializeObject<NextGenerationSportsCTMSModel>(resultAthlete.ExcelData);

                CTMSModel.ComprehensiveAssessmentRecommendation綜合評估建議Model
                    .CopyFrom(tempCTMSModel.ComprehensiveAssessmentRecommendation綜合評估建議Model);

                CTMSModel.報告摘要Model.CopyFrom(tempCTMSModel.報告摘要Model);
                #endregion

                excelJsonContent = JsonConvert.SerializeObject(CTMSModel);

                string name = CTMSModel.Home首頁2.姓名;
                if (!string.IsNullOrEmpty(itemName))
                    name = itemName;

                Athlete athlete = await backendDBContext.Athlete
                    .FirstOrDefaultAsync(x => x.Code == code);
                athlete.FilesData = filesJsonContent;
                athlete.ExcelData = excelJsonContent;
                athlete.Name = name;
                athlete.Code = code;
                athlete.ProjectId = projectId;
                backendDBContext.Athlete.Update(athlete);
                await backendDBContext.SaveChangesAsync();
            }
        }

        public async Task UpdateAsync(string code, Athlete athlete)
        {
            //var result = await backendDBContext.Athlete
            //    .FirstOrDefaultAsync(x => x.Code == code);

            backendDBContext.Athlete.Update(athlete);
            await backendDBContext.SaveChangesAsync();
        }

        public async Task UpdateExamineAsync(Examine examine)
        {
            examine.Athlete = null;
            backendDBContext.Examine.Update(examine);
            await backendDBContext.SaveChangesAsync();
        }

        public async Task UpdateAthleteAsync(Athlete athlete)
        {
            athlete.Project = null;
            athlete.Examine.Clear();
            backendDBContext.Athlete.Update(athlete);
            await backendDBContext.SaveChangesAsync();
        }

        public async Task<List<Athlete>> GetAsync()
        {
            return await backendDBContext.Athlete
                .AsNoTracking()
                .Include(x => x.Project)
                .Include(x=>x.Examine)
                .ToListAsync();
        }

        public async Task<List<Athlete>> GetAsync(int projectId)
        {
            return await backendDBContext.Athlete
                .AsNoTracking()
                .Where(x => x.ProjectId == projectId)
                .ToListAsync();
        }

        public async Task<Athlete> GetAsync(string code)
        {
            var result = await backendDBContext.Athlete
                .AsNoTracking()
                .Include(x => x.Project)
                .Include(x => x.Examine)
                .FirstOrDefaultAsync(x => x.Code == code);
            return result;
        }
    }
}
