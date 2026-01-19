using CTMS.DataModel.Models.ClinicalInformation;

namespace CTMS.Helper;

public class VisitCodeHelper
{
    public static void Sort(List<DropDownListDataModel> listVisitCode)
    {
        var sortedList = listVisitCode
            .OrderBy(item => item.Name)
            .ToList();
        listVisitCode.Clear();
        foreach (var item in sortedList)
        {
            listVisitCode.Add(item);
        }
    }
}
