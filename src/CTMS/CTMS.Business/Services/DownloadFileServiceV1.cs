using CTMS.Business.Helpers;
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
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CTMS.Business.Services
{
    public class DownloadFileServiceV1
    {
        private readonly BackendDBContext backendDBContext;
        string pathTemp = MagicObjectHelper.UploadTempPath;
        string pathUploadFinal = MagicObjectHelper.UploadFinalPath;
        string pathDownload = MagicObjectHelper.DownloadPath;
        string pathDbTable = MagicObjectHelper.DbTablePath;
        string pathCurrent = Directory.GetCurrentDirectory();

        public DownloadFileServiceV1(BackendDBContext backendDBContext,
            ExcleService excleService)
        {
            this.backendDBContext = backendDBContext;
        }

        public async Task PrepareAsync(List<string> ProcessingMessing, Action RefreshMessages)
        {
            await CleanAllFile();
            #region Project & MyUser & RoleView 紀錄備份
            var Projects = await GetProjectsAsync();
            var MyUsers = await GetMyUsersAsync();
            var RoleViews = await GetRoleViewsAsync();

            await PrepareProjectMyUserRoleViewFiles(Projects, MyUsers, RoleViews);
            #endregion

            #region Athletes
            var Athletes = await GetAthletesAsync();
            foreach (var athlete in Athletes)
            {
                var athleteCode = athlete.Code;
                var athleteName = athlete.Name.Replace(".", "").Trim();
                athlete.Name = athleteName;
                var athleteFilesData = athlete.FilesData;
                var athleteExcelData = athlete.ExcelData;
                ProcessingMessing.Insert(0, $"處理 {athleteName} 的紀錄");
                RefreshMessages();
                if (athlete.Name.Contains("林X鋐_LCH"))
                {
                    Console.WriteLine("Debug");
                }

                var CTMSModel = JsonConvert
                    .DeserializeObject<NextGenerationSportsCTMSModel>(athlete.ExcelData);
                var fileList = JsonConvert
                    .DeserializeObject<List<FileListNodeModel>>(athlete.FilesData);

                await PreparePersonFiles(athleteCode, athleteName, fileList);
                await PreparePersonDbData(athlete);

                ProcessingMessing.Insert(0, $"   完成 {athleteName} 的紀錄");
                RefreshMessages();
                #endregion

            }
            ProcessingMessing.Insert(0, $"準備壓縮檔案中");
            RefreshMessages();
            await GenerateZipFileAsync();
            ProcessingMessing.Insert(0, $"已經準備好所有檔案");
            RefreshMessages();
        }

        #region 產生壓縮檔案
        private async Task GenerateZipFileAsync()
        {
            string path = Path.Combine(pathCurrent, pathDownload);
            string pathTempZipFilename = Path.Combine(pathCurrent, pathTemp, "MyDownload.zip");
            string pathDownloadZipFilename = Path.Combine(pathCurrent, pathDownload, "MyDownload.zip");

            if(File.Exists(pathTempZipFilename))
            {
                File.Delete(pathTempZipFilename);
            }

            ZipHelper.CompressDirectory(
                sourceDirectory: path,
                destinationArchiveFilePath: pathTempZipFilename,
                compressionLevel: CompressionLevel.Optimal,
                includeBaseDirectory: false);

            File.Move(pathTempZipFilename, pathDownloadZipFilename, true);
        }
        #endregion

        private async Task PrepareProjectMyUserRoleViewFiles(List<Project> projects,
            List<MyUser> myUsers, List<RoleView> roleViews)
        {
            string path = Path.Combine(pathCurrent, pathDownload, MagicObjectHelper.DbTablePath);
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
            Directory.CreateDirectory(path);

            string json = "";
            #region Project
            var filename = Path.Combine(path, MagicObjectHelper.FilenameProject);
            json = JsonConvert.SerializeObject(projects);
            await File.WriteAllTextAsync(filename, json);
            #endregion

            #region myUsers
            filename = Path.Combine(path, MagicObjectHelper.FilenameMyUser);
            json = JsonConvert.SerializeObject(myUsers);
            await File.WriteAllTextAsync(filename, json);
            #endregion

            #region RoleView
            filename = Path.Combine(path, MagicObjectHelper.FilenameRoleView);
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            json = JsonConvert.SerializeObject(roleViews, settings);
            await File.WriteAllTextAsync(filename, json);
            #endregion
        }

        async Task PreparePersonFiles(string code, string name, List<FileListNodeModel> fileList)
        {
            string path = Path.Combine(pathCurrent, pathDownload, name);
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
            Directory.CreateDirectory(path);
            foreach (var file in fileList)
            {
                var sourceFilename = file.Filename.Trim();
                string realFilename = sourceFilename;
                #region 退去多餘的 GUID 文字
                string space = " ";
                string empty = "";
                while (true)
                {
                    var nameItems = realFilename.Split(' ');
                    if (nameItems.Count() > 1)
                    {
                        string part1 = nameItems[0];

                        if (part1.Length >= 30)
                        {
                            realFilename = realFilename.Replace(part1 + space, empty);
                            //nameItems = realFilename.Split(' ');
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                #endregion
                var sourceFile = Path.Combine(pathCurrent, file.Path);
                var TargetFilePath = Path.Combine(path, realFilename);
                string sourceFilePath = Path.Combine(pathCurrent, pathUploadFinal, sourceFilename);
                if (!File.Exists(sourceFilePath))
                {
                    continue;
                }
                File.Copy(sourceFile, TargetFilePath);
            }
        }

        async Task PreparePersonDbData(Athlete athlete)
        {
            string path = Path.Combine(pathCurrent, pathDownload, athlete.Name);
            string filename = Path.Combine(path, $"DbData.json");
            string json = JsonConvert.SerializeObject(athlete);
            await File.WriteAllTextAsync(filename, json);
        }
        async Task CleanAllFile()
        {
            var path = Path.Combine(pathCurrent, pathDownload);
            var files = Directory.GetFiles(path);
            foreach (var file in files)
            {
                if (file.Contains("readme.md"))
                {
                    continue;
                }
                File.Delete(file);
            }

            var directories = Directory.GetDirectories(path);
            foreach (var directory in directories)
            {
                Directory.Delete(directory, true);
            }
        }

        public async Task<List<Athlete>> GetAthletesAsync()
        {
            return await backendDBContext.Athlete
                .AsNoTracking()
                .Include(x => x.Project)
                .ToListAsync();
        }

        public async Task<List<Project>> GetProjectsAsync()
        {
            return await backendDBContext.Project
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<MyUser>> GetMyUsersAsync()
        {
            return await backendDBContext.MyUser
                .AsNoTracking()
                .Include(x => x.RoleView)
                .ToListAsync();
        }

        public async Task<List<RoleView>> GetRoleViewsAsync()
        {
            return await backendDBContext.RoleView
                .AsNoTracking()
                .Include(x => x.RoleViewProject)
                .ThenInclude(x => x.Project)
                .ToListAsync();
        }

        public async Task<Athlete> GetAsync(string code)
        {
            var result = await backendDBContext.Athlete
                .FirstOrDefaultAsync(x => x.Code == code);
            return result;
        }
    }
}
