using CTMS.Share.Helpers;

namespace CTMS.DataModel.Models.ClinicalInformation;

public class CancerStage
{
    public List<StageNode> Stage { get; set; } = new();
}

public class StageNode
{
    public string Name { get; set; }
    public List<StageGroupingNode> Children { get; set; } = new();
}

public class StageGroupingNode
{
    public string Name { get; set; }
}
