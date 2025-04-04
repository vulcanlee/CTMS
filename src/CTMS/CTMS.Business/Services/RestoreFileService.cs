using CTMS.Business.Helpers;
using CTMS.DataModel.Models;
using CTMS.EntityModel;
using CTMS.EntityModel.Models;
using CTMS.ExcelUtility.Services;
using CTMS.Share.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;
using SyncExcel.Services;
using System.IO.Compression;

namespace CTMS.Business.Services;

public class RestoreFileService
{
    private readonly BackendDBContext backendDBContext;
    string pathTemp = MagicObjectHelper.UploadTempPath;
    string pathUploadFinal = MagicObjectHelper.UploadFinalPath;
    string pathDownload = MagicObjectHelper.DownloadPath;
    string pathDecompress = MagicObjectHelper.DecompressPath;
    string pathCurrent = Directory.GetCurrentDirectory();
    record AthleteRecord(Athlete Athlete,
        NextGenerationSportsCTMSModel CTMSModel,
        List<FileListNodeModel> FileLists);

    public RestoreFileService(BackendDBContext backendDBContext,
        ExcleService excleService)
    {
        this.backendDBContext = backendDBContext;
    }

    public async Task PrepareAsync(string pathZip, List<string> ProcessingMessing, Action RefreshMessages)
    {
        try
        {
            await CleanAllFile();
            ProcessingMessing.Insert(0, $"進行解開壓縮檔案中");
            RefreshMessages();
            await DecompressZipFileAsync(pathZip);
            ProcessingMessing.Insert(0, $"已經準備好開壓縮檔案內所有檔案");
            RefreshMessages();
            ProcessingMessing.Insert(0, $"進行資料庫紀錄還原作業中...");
            RefreshMessages();
            await PrepareProjectMyUserRoleViewFiles();
            ProcessingMessing.Insert(0, $"進行還原所有紀錄作業中...");
            RefreshMessages();
            await PreparePersonFilesAsync(ProcessingMessing, RefreshMessages);
            ProcessingMessing.Insert(0, $"還原作業全部完成");
            RefreshMessages();
            await CleanAllFile();
            await RemoveZipFileAsync(pathZip);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    #region 解開壓縮檔案
    private async Task DecompressZipFileAsync(string pathZip)
    {
        string path = Path.Combine(pathCurrent, pathDecompress);
        string pathTempZipFilename = Path.Combine(pathCurrent, pathTemp, "MyDownload.zip");
        string pathDownloadZipFilename = Path.Combine(pathCurrent, pathDownload, "MyDownload.zip");

        pathTempZipFilename = @$"{pathZip}";
        ZipHelper.DecompressZipFile(
            sourceZipFilePath: pathTempZipFilename,
            destinationDirectory: path,
            overwrite: true);
    }
    private async Task RemoveZipFileAsync(string pathZip)
    {
        string path = Path.Combine(pathCurrent, pathDecompress);
        string pathTempZipFilename = Path.Combine(pathCurrent, pathTemp, "MyDownload.zip");
        string pathDownloadZipFilename = Path.Combine(pathCurrent, pathDownload, "MyDownload.zip");

        File.Delete(pathTempZipFilename);
    }
    #endregion

    private async Task PrepareProjectMyUserRoleViewFiles()
    {
        string path = Path.Combine(pathCurrent, pathDecompress, MagicObjectHelper.DbTablePath);
        Directory.CreateDirectory(path);

        string json = "";
        #region Project
        var filename = Path.Combine(path, MagicObjectHelper.FilenameProject);
        json = File.ReadAllText(filename);
        var projects = JsonConvert.DeserializeObject<List<Project>>(json);
        CleanTrackingHelper.Clean<Project>(backendDBContext);

        foreach (var project in projects)
        {
            var projectExist = await backendDBContext.Project
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Name == project.Name);
            if (projectExist == null)
            {
                project.Id = 0;
                await backendDBContext.Project.AddAsync(project);
            }
        }
        await backendDBContext.SaveChangesAsync();
        #endregion

        #region RoleView
        filename = Path.Combine(path, MagicObjectHelper.FilenameRoleView);
        json = File.ReadAllText(filename);
        var roleViews = JsonConvert.DeserializeObject<List<RoleView>>(json);
        CleanTrackingHelper.Clean<RoleView>(backendDBContext);
        foreach (var roleView in roleViews)
        {
            var roleViewExist = await backendDBContext.RoleView
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Name == roleView.Name);
            if (roleViewExist == null)
            {
                roleView.Id = 0;
                roleView.RoleViewProject.Clear();
                await backendDBContext.RoleView.AddAsync(roleView);
            }
        }

        int roleViewId = 0;
        string roleViewName = "";
        int projectId = 0;
        string projectName = "";
        CleanTrackingHelper.Clean<RoleView>(backendDBContext);
        foreach (var roleView in roleViews)
        {
            roleViewId = 0;
            roleViewName = "";
            projectId = 0;
            projectName = "";

            roleViewName = roleView.Name;
            var roleViewExist = await backendDBContext.RoleView
                .AsNoTracking()
                .Include(x => x.RoleViewProject)
                .FirstOrDefaultAsync(x => x.Name == roleViewName);
            if (roleViewExist != null)
            {
                roleViewId = roleViewExist.Id;
            }
            else
            {

            }

            foreach (var roleViewProjectItem in roleView.RoleViewProject)
            {
                projectName = roleViewProjectItem.Project.Name;
                projectId = 0;

                var projectExist = await backendDBContext.Project
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Name == projectName);
                if (projectExist != null)
                {
                    projectId = projectExist.Id;
                }

                var roleViewProjects = await backendDBContext.RoleViewProject
                 .AsNoTracking()
                 .Include(x => x.Project)
                 .Include(x => x.RoleView)
                 .FirstOrDefaultAsync(x => x.RoleView.Name == roleViewName &&
                 x.Project.Name == projectName);
                if (roleViewProjects == null)
                {
                    roleViewProjectItem.Id = 0;
                    roleViewProjectItem.RoleView = null;
                    roleViewProjectItem.RoleViewId = roleViewId;
                    roleViewProjectItem.Project = null;
                    roleViewProjectItem.ProjectId = projectId;
                    await backendDBContext.RoleViewProject.AddAsync(roleViewProjectItem);
                }
            }
            await backendDBContext.SaveChangesAsync();
        }
        #endregion

        #region myUsers
        filename = Path.Combine(path, MagicObjectHelper.FilenameMyUser);
        json = File.ReadAllText(filename);
        var myUsers = JsonConvert.DeserializeObject<List<MyUser>>(json);
        foreach (var myUser in myUsers)
        {
            var myUserExist = await backendDBContext.MyUser
                .AsNoTracking()
                .Include(x => x.RoleView)
                .FirstOrDefaultAsync(x => x.Name == myUser.Name);
            if (myUserExist == null)
            {
                myUser.RoleViewId = null;
                if (myUser.RoleView != null)
                {
                    string rolwViewName = myUser.RoleView.Name;
                    var roleView = await backendDBContext.RoleView
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.Name == rolwViewName);
                    if (roleView != null)
                    {
                        myUser.RoleViewId = roleView.Id;
                    }
                }
                myUser.RoleView = null;
                myUser.Id = 0;
                await backendDBContext.MyUser.AddAsync(myUser);
            }
        }
        #endregion
    }


    async Task PreparePersonFilesAsync(List<string> ProcessingMessing, Action RefreshMessages)
    {
        FileListNodeModel fileListNode = new();

        #region 針對還原回來的每筆紀錄檔案進行還原處理
        var directoryList = Directory.GetDirectories(Path.Combine(pathCurrent, pathDecompress));
        foreach (var currentRestorePath in directoryList)
        {
            var folder = Path.GetFileName(currentRestorePath);
            if (folder == "DbTable") continue;
            ProcessingMessing.Insert(0, $"還原 {folder}");
            RefreshMessages();

            #region 取得原有的紀錄資訊
            string pathTarget = FilePathHelper.GetAthleteDecompressPath(currentRestorePath);

            string DbDataFilename = Path.Combine(pathTarget, "DbData.json");
            var content = await File.ReadAllTextAsync(DbDataFilename);
            var (Athlete, CTMSModel, fileList) = ParseRecord(content);
            AthleteRecord athleteRecord = new(Athlete, CTMSModel, fileList);
            #endregion

            #region 還原檔案內容
            string pathUpload = FilePathHelper.GetAthleteUploadFinalPath(Athlete.Name);
            FilePathHelper.CopyDirectory(pathTarget, pathUpload);
            var fooDbDataFilename = Path.Combine(pathUpload, "DbData.json");
            File.Delete(fooDbDataFilename);
            #endregion


            #region 儲存到資料庫內
            var jsonFileList = JsonConvert.SerializeObject(athleteRecord.FileLists);
            athleteRecord.Athlete.FilesData = jsonFileList;

            var onlyExamine = Athlete.Examine.ToList();
            Athlete.Examine.Clear();

            ProcessingMessing.Insert(0, $"儲存 {folder} 紀錄中，請稍後...");
            RefreshMessages();
            Athlete athlete = await backendDBContext.Athlete
                .FirstOrDefaultAsync(x => x.Code == athleteRecord.Athlete.Code);
            if (athlete == null)
            {
                #region 找出 Project Id
                int? projectId = null;
                if (athleteRecord.Athlete.Project != null)
                {
                    var project = await backendDBContext.Project
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.Name == athleteRecord.Athlete.Project.Name);
                    if (project != null)
                        projectId = project.Id;
                }
                #endregion

                Athlete athleteNew = new Athlete()
                {
                    Name = athleteRecord.Athlete.Name,
                    Code = athleteRecord.Athlete.Code,
                    FilesData = athleteRecord.Athlete.FilesData,
                    ExcelData = athleteRecord.Athlete.ExcelData,
                    ProjectId = projectId,
                    ExamineTime = athleteRecord.Athlete.ExamineTime
                };

                await backendDBContext.Athlete.AddAsync(athleteNew);
                await backendDBContext.SaveChangesAsync();
                ProcessingMessing.Insert(0, $"針對 {folder} 紀錄還原完成 (新增)");
                RefreshMessages();

                #region 後側資料處理
                ProcessingMessing.Insert(0, $"儲存 {folder} 紀錄 - 後側資料 處理，請稍後...");
                RefreshMessages();

                var athleteNewId = athleteNew.Id;
                foreach (var examine in onlyExamine)
                {
                    examine.AthleteId = athleteNewId;
                    examine.Athlete = null;
                    examine.Id = 0;
                    await backendDBContext.Examine.AddAsync(examine);
                }
                await backendDBContext.SaveChangesAsync();

                ProcessingMessing.Insert(0, $"針對 {folder} 紀錄 - 後側資料 還原完成 (新增)");
                RefreshMessages();
                #endregion
            }
            else
            {
                athlete.FilesData = athleteRecord.Athlete.FilesData;
                athlete.ExcelData = athleteRecord.Athlete.ExcelData;
                backendDBContext.Athlete.Update(athlete);
                await backendDBContext.SaveChangesAsync();
                ProcessingMessing.Insert(0, $"針對 {folder} 紀錄還原完成 (更新)");
                RefreshMessages();
            }

            #endregion
        }
        #endregion
    }

    private void CopyFile(FileListNodeModel fileListNode,
        AthleteRecord athleteRecord, List<string> files, string fileKeyword, FileTypeEnum fileType)
    {
        #region _bar.png
        var filefull = files.FirstOrDefault(x => x.Contains(fileKeyword));
        files.Remove(filefull);
        var file = Path.GetFileName(filefull);
        if (file != null)
        {
            string inputFileName = $"{athleteRecord.Athlete.Code} {file}";
            string inputPathName = Path
                .Combine(MagicObjectHelper.UploadFinalPath, inputFileName);

            string sourcePath = filefull;
            string targetPath = Path.Combine(pathCurrent, pathUploadFinal, inputFileName);
            File.Copy(sourcePath, targetPath, true);

            fileListNode = new FileListNodeModel
            {
                Filename = inputFileName,
                Path = inputPathName,
                FileType = fileType,
            };
            athleteRecord.FileLists.Add(fileListNode);
        }
        #endregion
    }

    (Athlete, NextGenerationSportsCTMSModel, List<FileListNodeModel>) ParseRecord(string content)
    {
        Athlete Athlete = JsonConvert
           .DeserializeObject<Athlete>(content);
        NextGenerationSportsCTMSModel CTMSModel = JsonConvert
            .DeserializeObject<NextGenerationSportsCTMSModel>(Athlete.ExcelData);
        List<FileListNodeModel> fileList = JsonConvert
            .DeserializeObject<List<FileListNodeModel>>(Athlete.FilesData);
        return (Athlete, CTMSModel, fileList);
    }

    async Task CleanAllFile()
    {
        var path = Path.Combine(pathCurrent, pathDecompress);
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

    public async Task<Athlete> GetAsync(string code)
    {
        var result = await backendDBContext.Athlete
            .FirstOrDefaultAsync(x => x.Code == code);
        return result;
    }
}
