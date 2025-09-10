namespace AIAgent.Models;

public class Agentsetting
{
    public string DicomFolderPath { get; set; }
    public string QueueFolderPath { get; set; }
    public string InBoundQueueName { get; set; }
    public string Phase1QueueName { get; set; }
    public string Phase1WaitingQueueName { get; set; }
    public string Phase1TmpFolder { get; set; }
    public string Phase2QueueName { get; set; }
    public string Phase2WaitingQueueName { get; set; }
    public string Phase2TmpFolder { get; set; }
    public string Phase3QueueName { get; set; }
    public string Phase3WaitingQueueName { get; set; }
    public string OutBoundQueueName { get; set; }
    public string CompleteQueueName { get; set; }
    public string InferencePath { get; set; }
    public string 風險評估模型 { get; set; }

    public string GetDicomFolderPath() => DicomFolderPath;
    public string GetInboundQueuePath() => Path.Combine(QueueFolderPath, InBoundQueueName);

    public string GetPhase1QueuePath() => Path.Combine(QueueFolderPath, Phase1QueueName);
    public string GetPhase1WaitingQueuePath() => Path.Combine(QueueFolderPath, Phase1WaitingQueueName);
    public string GetPhase1TmpFolderPath() => Path.Combine(Phase1TmpFolder);
    public string GetPhase2QueuePath() => Path.Combine(QueueFolderPath, Phase2QueueName);
    public string GetPhase2WaitingQueuePath() => Path.Combine(QueueFolderPath, Phase2WaitingQueueName);
    public string GetPhase2TmpFolderPath() => Path.Combine(Phase2TmpFolder);
    public string GetPhase3QueuePath() => Path.Combine(QueueFolderPath, Phase3QueueName);
    public string GetPhase3WaitingQueuePath() => Path.Combine(QueueFolderPath, Phase3WaitingQueueName);
    public string GetOutBoundQueuePath() => Path.Combine(QueueFolderPath, OutBoundQueueName);
    public string GetCompleteQueuePath() => Path.Combine(QueueFolderPath, CompleteQueueName);

    public string GetInferencePath() => InferencePath;

}

