using CTMS.DataModel.Models;
using CTMS.EntityModel;
using CTMS.EntityModel.Models;
using CTMS.ExcelUtility.Services;
using CTMS.Share.Helpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SyncExcel.Services;

namespace CTMS.Business.Services
{
    public class BrowseAthleteService
    {
        private readonly BackendDBContext backendDBContext;
        private readonly ExcleService excleService;

        public BrowseAthleteService(BackendDBContext backendDBContext,
            ExcleService excleService)
        {
            this.backendDBContext = backendDBContext;
            this.excleService = excleService;
        }

        public async Task<List<Athlete>> GetAllAsync()
        {
            return await backendDBContext.Athlete
                .AsNoTracking()
                .Include(x => x.Examine)
                .Include(x => x.Project).ToListAsync();
        }

        public async Task<List<Athlete>> GetByProjectIdAsync(int projectId)
        {
            return await backendDBContext.Athlete
                .AsNoTracking()
                .Include(x => x.Examine)
                .Where(x => x.ProjectId == projectId)
                .ToListAsync();
        }

        public async Task<Athlete> GetByCodeAsync(string code, CurrentAthleteExamine CurrentAthleteExamine)
        {
            var athlete = await GetByCodeAsync(code);
            if (CurrentAthleteExamine.AthleteId != athlete.Id)
                CurrentAthleteExamine.Reset();

            if (CurrentAthleteExamine.AthleteId == 0)
            {
                CurrentAthleteExamine.AthleteId = athlete.Id;
                CurrentAthleteExamine.ExamineList.Clear();
                foreach (var item in athlete.Examine)
                {
                    CurrentAthleteExamine.ExamineList.Add(item.ExamineTime);
                }
                CurrentAthleteExamine.ExamineId = athlete.Examine.FirstOrDefault()?.Id ?? 0;
                CurrentAthleteExamine.ExamineTime = athlete.Examine.FirstOrDefault()?.ExamineTime ?? string.Empty;
            }
            return athlete;
        }

        public async Task<(NextGenerationSportsCTMSModel, List<FileListNodeModel>)>
            GetCurrentDataAsync(Athlete athlete, CurrentAthleteExamine CurrentAthleteExamine)
        {
            //await Task.Delay(100);
            var item = athlete.Examine.FirstOrDefault(x => x.ExamineTime == CurrentAthleteExamine.ExamineTime);
            var CTMSModel = JsonConvert
             .DeserializeObject<NextGenerationSportsCTMSModel>(item.ExcelData);

            var fileList = JsonConvert
            .DeserializeObject<List<FileListNodeModel>>(item.FilesData);

            return (CTMSModel, fileList);
        }

        public async Task<(NextGenerationSportsCTMSModel, List<FileListNodeModel>)>
            ChangeCurrentDataAsync(Athlete athlete, CurrentAthleteExamine CurrentAthleteExamine,
            string examineTime)
        {
            //await Task.Delay(100);
            var item = athlete.Examine.FirstOrDefault(x => x.ExamineTime == examineTime);
            CurrentAthleteExamine.ExamineId = item.Id;
            CurrentAthleteExamine.ExamineTime = item.ExamineTime;

            var CTMSModel = JsonConvert
             .DeserializeObject<NextGenerationSportsCTMSModel>(item.ExcelData);

            var fileList = JsonConvert
            .DeserializeObject<List<FileListNodeModel>>(item.FilesData);

            return (CTMSModel, fileList);
        }

        public async Task<Athlete> GetByCodeAsync(string code)
        {
            var result = await backendDBContext.Athlete
                .AsNoTracking()
                .Include(x => x.Examine)
                .FirstOrDefaultAsync(x => x.Code == code);
            return result;
        }

        public async Task DeleteByExamineIdAsync(int id)
        {
            var itemExamine = await backendDBContext.Examine
                .AsNoTracking()
                .Include(x => x.Athlete)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (itemExamine != null)
            {
                backendDBContext.Examine.Remove(itemExamine);
                string pathRoot = FilePathHelper
                    .GetAthleteUploadFinalPath(itemExamine.Athlete.Name);

                string pathExamine = Path.Combine(pathRoot, itemExamine.ExamineTime);
                Directory.Delete(pathExamine, true);
                await backendDBContext.SaveChangesAsync();
            }
        }

        public async Task DeleteByAthleteIdAsync(int id)
        {
            var itemAthlete = await backendDBContext.Athlete
                .AsNoTracking()
                .Include(x => x.Examine)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (itemAthlete != null)
            {
                string pathRoot = FilePathHelper.GetAthleteUploadFinalPath(itemAthlete.Name);
                var examines = await backendDBContext.Examine
                    .AsNoTracking()
                    .Where(x => x.AthleteId == itemAthlete.Id)
                    .ToListAsync();

                foreach (var examine in examines)
                {
                    string pathExamine = Path.Combine(pathRoot, examine.ExamineTime);
                    backendDBContext.Examine.Remove(examine);
                    Directory.Delete(pathExamine, true);
                }
                await backendDBContext.SaveChangesAsync();
                itemAthlete.Examine.Clear();
                backendDBContext.Athlete.Remove(itemAthlete);
                await backendDBContext.SaveChangesAsync();
                Directory.Delete(pathRoot, true);
            }
            return;
        }
    }
}
