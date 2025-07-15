using CTMS.Share.Helpers;

namespace CTMS.DataModel.Models.ClinicalInformation;

public class BrowseFilterCondition
{
    public List<BrowseFilterConditionNode> Items { get; set; } = new();
}

public class BrowseFilterConditionNode
{
    public string Category { get; set; }
    public string Name { get; set; }
}
