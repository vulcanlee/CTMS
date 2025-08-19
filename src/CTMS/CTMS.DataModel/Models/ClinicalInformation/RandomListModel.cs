using CTMS.Share.Helpers;

namespace CTMS.DataModel.Models.ClinicalInformation;

public class RandomListModel
{
    public List<RandomListItem> 成大Early { get; set; } = new();
    public List<RandomListItem> 成大Advance { get; set; } = new();
    public List<RandomListItem> 奇美Early { get; set; } = new();
    public List<RandomListItem> 奇美Advance { get; set; } = new();
    public List<RandomListItem> 郭綜合Early { get; set; } = new();
    public List<RandomListItem> 郭綜合Advance { get; set; } = new();

    public void Reset()
    {
        成大Early.Clear();
        成大Advance.Clear();
        奇美Early.Clear();
        奇美Advance.Clear();
        郭綜合Early.Clear();
        郭綜合Advance.Clear();
    }
}

public class RandomListItem
{
    public string Id { get; set; }= string.Empty;
    public string BlockId { get; set; }= string.Empty;
    public string BlockSize { get; set; }= string.Empty;
    public string Treatment { get; set; }= string.Empty;
    public string StudyCode { get; set; }= string.Empty;

    public void Reset()
    {
        Id = string.Empty;
        BlockId = string.Empty;
        BlockSize = string.Empty;
        Treatment = string.Empty;
        StudyCode = string.Empty;
    }
}
