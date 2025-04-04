using CTMS.Business.Helpers;
using CTMS.DataModel.Models;
using CTMS.EntityModel;
using CTMS.EntityModel.Models;
using CTMS.Share.Helpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTMS.Business.Services
{
    public class MigrationExamineService
    {
        private readonly BackendDBContext backendDBContext;

        public MigrationExamineService(BackendDBContext backendDBContext)
        {
            this.backendDBContext = backendDBContext;
        }

        public async Task MigrateExamineAsync(List<string> ProcessingMessing, Action RefreshMessages)
        {
            string pathTemp = MagicObjectHelper.UploadTempPath;
            string pathFinal = MagicObjectHelper.UploadFinalPath;

            CleanTrackingHelper.Clean<Athlete>(backendDBContext);
            CleanTrackingHelper.Clean<Examine>(backendDBContext);
            var athletes = await backendDBContext.Athlete
                .AsNoTracking()
                .ToListAsync();
            foreach (var athlete in athletes)
            {
                ProcessingMessing.Insert(0, $"處理 {athlete.Name} 的紀錄");

                CleanTrackingHelper.Clean<Athlete>(backendDBContext);
                CleanTrackingHelper.Clean<Examine>(backendDBContext);

                #region 移除相關檔案
                string pathFinalForExamine = FilePathHelper
                      .GetAthleteUploadFinalPath(athlete.Name);
                pathFinalForExamine = Path.Combine(pathFinalForExamine, MagicObjectHelper.預設檢驗時間);
                Directory.CreateDirectory(pathFinalForExamine);

                string filesJsonContent = athlete.FilesData;
                List<FileListNodeModel> files = JsonConvert
                    .DeserializeObject<List<FileListNodeModel>>(filesJsonContent);
                foreach (var file in files)
                {
                    var replaceFilename = FilePathHelper.CleanFilenameKey(file.Filename);

                    var fileSourcePath = Path.Combine(pathFinal, file.Filename);
                    var fileTargetPath = Path.Combine(pathFinalForExamine, replaceFilename);
                    file.Path = fileTargetPath;
                    file.Filename = replaceFilename;

                    File.Move(fileSourcePath, fileTargetPath);

                    File.Delete(fileSourcePath);
                }
                #endregion

                string filesData = JsonConvert.SerializeObject(files);
                athlete.FilesData = filesData;
                var fooExamine = await backendDBContext.Examine
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.AthleteId == athlete.Id &&
                    x.ExamineTime == MagicObjectHelper.預設檢驗時間);
                if (fooExamine == null)
                {
                    var examine = new Examine
                    {
                        AthleteId = athlete.Id,
                        Athlete = null,
                        Code = athlete.Code,
                        ExamineTime = MagicObjectHelper.預設檢驗時間,
                        FilesData = athlete.FilesData,
                        ExcelData = athlete.ExcelData
                    };
                    backendDBContext.Examine.Add(examine);

                    athlete.ExamineTime = MagicObjectHelper.預設檢驗時間;
                    backendDBContext.Athlete.Update(athlete);

                    backendDBContext.SaveChanges();
                }
            }
            ProcessingMessing.Insert(0, $"處理完成所有的紀錄");
        }
    }
}
