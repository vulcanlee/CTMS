using AIAgent.Models;
using CTMS.DataModel.Models.AIAgent;
using CTMS.Share.Helpers;

namespace AIAgent.Services;

public class Phase1Phase2Service
{
    private readonly DirectoryHelperService directoryHelperService;

    public Phase1Phase2Service(DirectoryHelperService directoryHelperService)
    {
        this.directoryHelperService = directoryHelperService;
    }

    public async Task CopyDicomAsync(PatientAIInfo patientAIInfo,
        Agentsetting agentsetting)
    {
        var phase2Path = agentsetting.GetPhase1QueuePath();
        var phase1WaitingPath = agentsetting.GetPhase1WaitingQueuePath();

        var sourceFile = Path.Combine(phase2Path, patientAIInfo.KeyName, $"{patientAIInfo.KeyName}.dcm");
        var destinationFile = Path.Combine(agentsetting.DicomFolderPath, $"{patientAIInfo.KeyName}.dcm");
        //var sourcePath = Path.Combine(phase1WaitingPath, patientAIInfo.KeyName);
        //var destinationPath = Path.Combine(phase1WaitingPath, patientAIInfo.KeyName);

        File.Copy(sourceFile, destinationFile, overwrite: true);
        //directoryHelperService.MoveDirectoryRecursive(sourcePath, destinationPath, overwrite: true);
    }

    public async Task MoveToPhase2(PatientAIInfo patientAIInfo,
        Agentsetting agentsetting)
    {
        var phase2Path = agentsetting.GetPhase2QueuePath();
        var phase1WaitingPath = agentsetting.GetPhase1WaitingQueuePath();

        var sourcePath = Path.Combine(phase1WaitingPath, patientAIInfo.KeyName);
        var destinationPath = Path.Combine(phase2Path, patientAIInfo.KeyName);

        directoryHelperService.MoveDirectoryRecursive(sourcePath, destinationPath, overwrite: true);
    }

    public async Task MoveToPhase3(PatientAIInfo patientAIInfo,
        Agentsetting agentsetting)
    {
        var phase3Path = agentsetting.GetPhase3QueuePath();
        var phase2WaitingPath = agentsetting.GetPhase2WaitingQueuePath();

        var sourcePath = Path.Combine(phase2WaitingPath, patientAIInfo.KeyName);
        var destinationPath = Path.Combine(phase3Path, patientAIInfo.KeyName);

        directoryHelperService.CopyDirectory(sourcePath, destinationPath, overwrite: true);

        Directory.Delete(sourcePath, recursive: true);
    }

    public async Task CopyToOutBound(PatientAIInfo patientAIInfo,
        Agentsetting agentsetting)
    {
        var outboundPath = agentsetting.GetOutBoundQueuePath();
        var phase3WaitingPath = agentsetting.GetPhase3WaitingQueuePath();

        var sourcePath = Path.Combine(phase3WaitingPath, patientAIInfo.KeyName);
        var destinationPath = Path.Combine(outboundPath, patientAIInfo.KeyName);

        directoryHelperService.CopyDirectory(sourcePath, destinationPath, overwrite: true);

        //Directory.Delete(sourcePath, recursive: true);
    }

    public async Task MoveToPhase1Waiting(PatientAIInfo patientAIInfo,
        Agentsetting agentsetting)
    {
        var phase1Path = agentsetting.GetPhase1QueuePath();
        var phase1WaitingPath = agentsetting.GetPhase1WaitingQueuePath();

        var sourcePath = Path.Combine(phase1Path, patientAIInfo.KeyName);
        var destinationPath = Path.Combine(phase1WaitingPath, patientAIInfo.KeyName);

        directoryHelperService.MoveDirectoryRecursive(sourcePath, destinationPath, overwrite: true);
    }

    public async Task MoveToPhase2Waiting(PatientAIInfo patientAIInfo,
        Agentsetting agentsetting)
    {
        var phase1Path = agentsetting.GetPhase2QueuePath();
        var phase1WaitingPath = agentsetting.GetPhase2WaitingQueuePath();

        var sourcePath = Path.Combine(phase1Path, patientAIInfo.KeyName);
        var destinationPath = Path.Combine(phase1WaitingPath, patientAIInfo.KeyName);

        directoryHelperService.CopyDirectory(sourcePath, destinationPath, overwrite: true);

        Directory.Delete(sourcePath, recursive: true);
    }

    public async Task MoveToPhase3Waiting(PatientAIInfo patientAIInfo,
        Agentsetting agentsetting)
    {
        var phase1Path = agentsetting.GetPhase3QueuePath();
        var phase1WaitingPath = agentsetting.GetPhase3WaitingQueuePath();

        var sourcePath = Path.Combine(phase1Path, patientAIInfo.KeyName);
        var destinationPath = Path.Combine(phase1WaitingPath, patientAIInfo.KeyName);

        directoryHelperService.CopyDirectory(sourcePath, destinationPath, overwrite: true);

        Directory.Delete(sourcePath, recursive: true);
    }

    public async Task MoveToCompletionWaiting(PatientAIInfo patientAIInfo,
        Agentsetting agentsetting)
    {
        var phase1Path = agentsetting.GetOutBoundQueuePath();
        var phase1WaitingPath = agentsetting.GetCompleteQueuePath();

        var sourcePath = Path.Combine(phase1Path, patientAIInfo.KeyName);
        var destinationPath = Path.Combine(phase1WaitingPath, patientAIInfo.KeyName);

        directoryHelperService.CopyDirectory(sourcePath, destinationPath, overwrite: true);

        Directory.Delete(sourcePath, recursive: true);
    }

    private static void MoveDirectoryRecursive(string sourceDir, string destDir, bool overwrite)
    {
        if (!Directory.Exists(sourceDir))
            throw new DirectoryNotFoundException($"來源不存在: {sourceDir}");

        // 若目的不存在且同一磁碟，直接 Move（最快）
        bool sameVolume = string.Equals(
            Path.GetPathRoot(Path.GetFullPath(sourceDir)),
            Path.GetPathRoot(Path.GetFullPath(destDir)),
            StringComparison.OrdinalIgnoreCase);

        if (!Directory.Exists(destDir))
        {
            if (sameVolume)
            {
                Directory.Move(sourceDir, destDir);
                return;
            }
            Directory.CreateDirectory(destDir);
        }
        else
        {
            if (!overwrite)
            {
                // 避免覆寫，直接拋出
                throw new IOException($"目的資料夾已存在: {destDir}");
            }
        }

        // 遞迴複製（支援跨磁碟 / 合併）
        foreach (var file in Directory.GetFiles(sourceDir))
        {
            var fileName = Path.GetFileName(file);
            var targetFile = Path.Combine(destDir, fileName);
            Directory.CreateDirectory(destDir);
            File.Copy(file, targetFile, overwrite: true);
        }

        foreach (var dir in Directory.GetDirectories(sourceDir))
        {
            var dirName = Path.GetFileName(dir);
            var targetSub = Path.Combine(destDir, dirName);
            MoveDirectoryRecursive(dir, targetSub, overwrite);
        }

        // 全部成功後刪除來源（遞迴）
        try
        {
            Directory.Delete(sourceDir, recursive: true);
        }
        catch (Exception ex)
        {
            // 失敗時可記錄 Log；此處先重新拋出
            throw new IOException($"刪除來源資料夾失敗: {sourceDir}", ex);
        }
    }

    public Phase1LabelGeneration BuildPhase1標註生成Json(PatientAIInfo patientAIInfo,
        Agentsetting agentsetting)
    {
        var phase1LabelGeneration = new Phase1LabelGeneration()
        {
            optional = new PhaseOptional()
            {
                age = new List<string> { patientAIInfo.Age },
                gender = new List<string> { patientAIInfo.Gender },
                height = new List<string> { patientAIInfo.Height },
                weight = new List<string> { patientAIInfo.Weight },
            },
            files = new List<string> { patientAIInfo.DicomFilename },
            tmp_folder = Path.Combine(agentsetting.GetPhase1TmpFolderPath(), patientAIInfo.KeyName),
        };
        
        return phase1LabelGeneration;
    }

    public Phase2QuantitativeAnalysis BuildPhase2標註生成Json(PatientAIInfo patientAIInfo,
        Agentsetting agentsetting)
    {
        var phase1ResultJsonPath = Path.Combine(agentsetting.GetPhase2QueuePath(), patientAIInfo.KeyName, MagicObjectHelper.Phase1ResultPath,
            $"{patientAIInfo.KeyName}.json");
        var phase2LabelGeneration = new Phase2QuantitativeAnalysis()
        {
            optional = new PhaseOptional()
            {
                age = new List<string> { patientAIInfo.Age },
                gender = new List<string> { patientAIInfo.Gender },
                height = new List<string> { patientAIInfo.Height },
                weight = new List<string> { patientAIInfo.Weight },
            },
            files = new List<string> { patientAIInfo.DicomFilename },
            jsons = new List<string> { phase1ResultJsonPath },
            tmp_folder = Path.Combine(agentsetting.GetPhase2TmpFolderPath(), patientAIInfo.KeyName),
        };

        return phase2LabelGeneration;
    }

    public void SavePhase1標註生成Json(Phase1LabelGeneration phase1LabelGeneration,
        PatientAIInfo patientAIInfo,
        Agentsetting agentsetting)
    {
        if(!Directory.Exists(phase1LabelGeneration.tmp_folder))
        {
            Directory.CreateDirectory(phase1LabelGeneration.tmp_folder);
        }
        string json = phase1LabelGeneration.ToJson();
        string fileName = $"{patientAIInfo.KeyName}.json";
        string fullPath = Path.Combine(agentsetting.GetInferencePath(), fileName);
        File.WriteAllText(fullPath, json);
        return ;
    }

    public void SavePhase1定量分析Json(Phase2QuantitativeAnalysis phase2LabelGeneration,
        PatientAIInfo patientAIInfo,
        Agentsetting agentsetting)
    {
        if (!Directory.Exists(phase2LabelGeneration.tmp_folder))
        {
            Directory.CreateDirectory(phase2LabelGeneration.tmp_folder);
        }
        string json = phase2LabelGeneration.ToJson();
        string fileName = $"{patientAIInfo.KeyName}.json";
        string fullPath = Path.Combine(agentsetting.GetInferencePath(), fileName);
        File.WriteAllText(fullPath, json);
        return;
    }
}
