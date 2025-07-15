using CTMS.DataModel.Dtos;
using CTMS.Share.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTMS.DataModel.Models.ClinicalInformation
{
    public class Main臨床資料
    {
        public 臨床資料手術 臨床資料手術 { get; set; } = new();
        public 臨床資料病理報告 臨床資料病理報告 { get; set; } = new();
        public 臨床資料化學治療 臨床資料化學治療 { get; set; } = new();
        public 臨床資料合併用藥 臨床資料合併用藥 { get; set; } = new();
        public BaselineMedicalHistoryForm BaselineMedicalHistoryForm { get; set; } = new();
        public BloodTest抽血檢驗血液 抽血檢驗血液 { get; set; } = new();
        public BloodTest抽血檢驗生化 抽血檢驗生化 { get; set; } = new();
        public Survey化療副作用 Survey化療副作用 { get; set; } = new();
        public Survey標靶副作用 Survey標靶副作用 { get; set; } = new();
        public Survey放療副作用 Survey放療副作用 { get; set; } = new();
        public SurveyWhooqol問卷 SurveyWhooqol問卷 { get; set; } = new();
        public Survey個人史問卷 Survey個人史問卷 { get; set; } = new();
        public Survey家族史問卷 Survey家族史問卷 { get; set; } = new();
        public HematologicSideEffects血液副作用 HematologicSideEffects血液副作用 { get; set; } = new();
        public Survey1SideEffects副作用 SurveySideEffects副作用1 { get; set; } = new();
        public Survey2SideEffects副作用 SurveySideEffects副作用2 { get; set; } = new();
        public OtherTreatment 其他治療 { get; set; } = new();
        public OtherMedication 其他治療藥物 { get; set; } = new();
        public OtherTreatmentImage 其他治療影像 { get; set; } = new();

        public void CollectVisitCode(VisitCodeSetModel VisitCodeSetModel)
        {
            VisitCodeSetModel.VisitCodes.Clear();
            VisitCodeSetModel.Nodes.Clear();
            foreach (var item in 臨床資料手術.Items)
                CheckVisitCode(VisitCodeSetModel, DataTabeEnums.臨床資料手術, item.VisitCode);
            foreach (var item in 臨床資料病理報告.Items)
                CheckVisitCode(VisitCodeSetModel, DataTabeEnums.臨床資料病理報告, item.VisitCode);
            foreach (var item in 臨床資料化學治療.Items)
                CheckVisitCode(VisitCodeSetModel, DataTabeEnums.臨床資料化學治療, item.VisitCode);
            foreach (var item in 臨床資料合併用藥.Items)
                CheckVisitCode(VisitCodeSetModel, DataTabeEnums.臨床資料合併用藥, item.VisitCode);
            foreach (var item in BaselineMedicalHistoryForm.Items)
                CheckVisitCode(VisitCodeSetModel, DataTabeEnums.BaselineMedicalHistoryForm, item.VisitCode);
            foreach (var item in 抽血檢驗血液.Items)
                CheckVisitCode(VisitCodeSetModel, DataTabeEnums.抽血檢驗血液, item.VisitCode);
            foreach (var item in 抽血檢驗生化.Items)
                CheckVisitCode(VisitCodeSetModel, DataTabeEnums.抽血檢驗生化, item.VisitCode);
            foreach (var item in Survey化療副作用.Items)
                CheckVisitCode(VisitCodeSetModel, DataTabeEnums.Survey化療副作用, item.VisitCode);
            foreach (var item in Survey標靶副作用.Items)
                CheckVisitCode(VisitCodeSetModel, DataTabeEnums.Survey標靶副作用, item.VisitCode);
            foreach (var item in Survey放療副作用.Items)
                CheckVisitCode(VisitCodeSetModel, DataTabeEnums.Survey放療副作用, item.VisitCode);
            foreach (var item in SurveyWhooqol問卷.Items)
                CheckVisitCode(VisitCodeSetModel, DataTabeEnums.SurveyWhooqol問卷, item.VisitCode);
            foreach (var item in Survey個人史問卷.Items)
                CheckVisitCode(VisitCodeSetModel, DataTabeEnums.Survey個人史問卷, item.VisitCode);
            foreach (var item in Survey家族史問卷.Items)
                CheckVisitCode(VisitCodeSetModel, DataTabeEnums.Survey家族史問卷, item.VisitCode);
            foreach (var item in HematologicSideEffects血液副作用.Items)
                CheckVisitCode(VisitCodeSetModel, DataTabeEnums.HematologicSideEffects血液副作用, item.VisitCode);
            foreach (var item in SurveySideEffects副作用1.Items)
                CheckVisitCode(VisitCodeSetModel, DataTabeEnums.SurveySideEffects副作用1, item.VisitCode);
            foreach (var item in SurveySideEffects副作用2.Items)
                CheckVisitCode(VisitCodeSetModel, DataTabeEnums.SurveySideEffects副作用2, item.VisitCode);
            foreach (var item in 其他治療.Items)
                CheckVisitCode(VisitCodeSetModel, DataTabeEnums.其他治療, item.VisitCode);
            foreach (var item in 其他治療藥物.Items)
                CheckVisitCode(VisitCodeSetModel, DataTabeEnums.其他治療藥物, item.VisitCode);
            foreach (var item in 其他治療影像.Items)
                CheckVisitCode(VisitCodeSetModel, DataTabeEnums.其他治療影像, item.VisitCode);
        }

        public void CheckVisitCode(VisitCodeSetModel VisitCodeSetModel,
            DataTabeEnums DataTabeEnums, VisitCodeModel visitCodeModel)
        {
            if (visitCodeModel != null)
            {
                var foundItem = VisitCodeSetModel.VisitCodes
                    .FirstOrDefault(x => x.CompareTo(visitCodeModel));
                if (foundItem == null)
                {
                    VisitCodeSetModel.VisitCodes.Add(visitCodeModel);
                    VisitCodeSetModel.Nodes.Add(new VisitCodeSetNodeModel());
                    foundItem = visitCodeModel;
                }

                #region 標註那些節點要有 VisitCode
                int index = VisitCodeSetModel.VisitCodes.IndexOf(foundItem);
                if (index >= 0)
                {
                    var node = VisitCodeSetModel.Nodes[index];
                    switch (DataTabeEnums)
                    {
                        case DataTabeEnums.臨床資料手術:
                            node.Checked臨床資料手術 = true;
                            break;
                        case DataTabeEnums.臨床資料病理報告:
                            node.Checked臨床資料病理報告 = true;
                            break;
                        case DataTabeEnums.臨床資料化學治療:
                            node.Checked臨床資料化學治療 = true;
                            break;
                        case DataTabeEnums.臨床資料合併用藥:
                            node.Checked臨床資料合併用藥 = true;
                            break;
                        case DataTabeEnums.BaselineMedicalHistoryForm:
                            node.CheckedBaselineMedicalHistoryForm = true;
                            break;
                        case DataTabeEnums.抽血檢驗血液:
                            node.Checked抽血檢驗血液 = true;
                            break;
                        case DataTabeEnums.抽血檢驗生化:
                            node.Checked抽血檢驗生化 = true;
                            break;
                        case DataTabeEnums.Survey化療副作用:
                            node.CheckedSurvey化療副作用 = true;
                            break;
                        case DataTabeEnums.Survey標靶副作用:
                            node.CheckedSurvey標靶副作用 = true;
                            break;
                        case DataTabeEnums.Survey放療副作用:
                            node.CheckedSurvey放療副作用 = true;
                            break;
                        case DataTabeEnums.SurveyWhooqol問卷:
                            node.CheckedSurveyWhooqol問卷 = true;
                            break;
                        case DataTabeEnums.Survey個人史問卷:
                            node.CheckedSurvey個人史問卷 = true;
                            break;
                        case DataTabeEnums.Survey家族史問卷:
                            node.CheckedSurvey家族史問卷 = true;
                            break;
                        case DataTabeEnums.HematologicSideEffects血液副作用:
                            node.CheckedHematologicSideEffects血液副作用 = true;
                            break;
                        case DataTabeEnums.SurveySideEffects副作用1:
                            node.CheckedSurveySideEffects副作用1 = true;
                            break;
                        case DataTabeEnums.SurveySideEffects副作用2:
                            node.CheckedSurveySideEffects副作用2 = true;
                            break;
                        case DataTabeEnums.其他治療:
                            node.Checked其他治療 = true;
                            break;
                        case DataTabeEnums.其他治療藥物:
                            node.Checked其他治療藥物 = true;
                            break;
                        case DataTabeEnums.其他治療影像:
                            node.Checked其他治療影像 = true;
                            break;
                    }
                    #endregion
                }

            }
        }
    }
}
