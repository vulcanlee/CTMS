using CTMS.Business.Services.ClinicalInformation;
using CTMS.DataModel.Models;
using CTMS.DataModel.Models.ClinicalInformation;
using CTMS.DataModel.Models.Systems;
using CTMS.Helper;
using System.Reflection.PortableExecutable;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CTMS.Share.Helpers;

public class VisitCodeCollectionHelper
{
    public VisitCodeCollectionHelper()
    {
    }

    public List<DropDownListDataModel> GetAll問卷VisitCodes(Main臨床資料 main臨床資料)
    {
        List<DropDownListDataModel> visitCodes = new();

        //public Survey化療副作用 Survey化療副作用 { get; set; } = new();
        //public Survey標靶副作用 Survey標靶副作用 { get; set; } = new();
        //public Survey放療副作用 Survey放療副作用 { get; set; } = new();
        //public SurveyWhooqol問卷 SurveyWhooqol問卷 { get; set; } = new();
        //public Survey個人史問卷 Survey個人史問卷 { get; set; } = new();
        //public Survey家族史問卷 Survey家族史問卷 { get; set; } = new();
        //public Survey生活品質問卷 Survey生活品質問卷 { get; set; } = new();
        //public Survey健康問卷 Survey健康問卷 { get; set; } = new();

        visitCodes.Clear();

        #region Survey化療副作用
        foreach (var nodeItem in main臨床資料.Survey化療副作用.Items)
        {
            var item = visitCodes.FirstOrDefault(x => x.Key == nodeItem.VisitCode.Id);
            if(item != null) continue;
            visitCodes.Add(new DropDownListDataModel()
            {
                Key = nodeItem.VisitCode.Id,
                Name = nodeItem.VisitCode.VisitCodeTitle,
            });
        }
        #endregion

        #region Survey標靶副作用
        foreach (var nodeItem in main臨床資料.Survey標靶副作用.Items)
        {
            var item = visitCodes.FirstOrDefault(x => x.Key == nodeItem.VisitCode.Id);
            if (item != null) continue;
            visitCodes.Add(new DropDownListDataModel()
            {
                Key = nodeItem.VisitCode.Id,
                Name = nodeItem.VisitCode.VisitCodeTitle,
            });
        }
        #endregion

        #region Survey放療副作用
        foreach (var nodeItem in main臨床資料.Survey放療副作用.Items)
        {
            var item = visitCodes.FirstOrDefault(x => x.Key == nodeItem.VisitCode.Id);
            if (item != null) continue;
            visitCodes.Add(new DropDownListDataModel()
            {
                Key = nodeItem.VisitCode.Id,
                Name = nodeItem.VisitCode.VisitCodeTitle,
            });
        }
        #endregion

        #region SurveyWhooqol問卷
        foreach (var nodeItem in main臨床資料.SurveyWhooqol問卷.Items)
        {
            var item = visitCodes.FirstOrDefault(x => x.Key == nodeItem.VisitCode.Id);
            if (item != null) continue;
            visitCodes.Add(new DropDownListDataModel()
            {
                Key = nodeItem.VisitCode.Id,
                Name = nodeItem.VisitCode.VisitCodeTitle,
            });
        }
        #endregion

        #region Survey個人史問卷
        foreach (var nodeItem in main臨床資料.Survey個人史問卷.Items)
        {
            var item = visitCodes.FirstOrDefault(x => x.Key == nodeItem.VisitCode.Id);
            if (item != null) continue;
            visitCodes.Add(new DropDownListDataModel()
            {
                Key = nodeItem.VisitCode.Id,
                Name = nodeItem.VisitCode.VisitCodeTitle,
            });
        }
        #endregion

        #region Survey家族史問卷
        foreach (var nodeItem in main臨床資料.Survey家族史問卷.Items)
        {
            var item = visitCodes.FirstOrDefault(x => x.Key == nodeItem.VisitCode.Id);
            if (item != null) continue;
            visitCodes.Add(new DropDownListDataModel()
            {
                Key = nodeItem.VisitCode.Id,
                Name = nodeItem.VisitCode.VisitCodeTitle,
            });
        }
        #endregion

        #region Survey生活品質問卷
        foreach (var nodeItem in main臨床資料.Survey生活品質問卷.Items)
        {
            var item = visitCodes.FirstOrDefault(x => x.Key == nodeItem.VisitCode.Id);
            if (item != null) continue;
            visitCodes.Add(new DropDownListDataModel()
            {
                Key = nodeItem.VisitCode.Id,
                Name = nodeItem.VisitCode.VisitCodeTitle,
            });
        }
        #endregion

        #region Survey健康問卷
        foreach (var nodeItem in main臨床資料.Survey健康問卷.Items)
        {
            var item = visitCodes.FirstOrDefault(x => x.Key == nodeItem.VisitCode.Id);
            if (item != null) continue;
            visitCodes.Add(new DropDownListDataModel()
            {
                Key = nodeItem.VisitCode.Id,
                Name = nodeItem.VisitCode.VisitCodeTitle,
            });
        }
        #endregion

        return visitCodes;
    }
}
