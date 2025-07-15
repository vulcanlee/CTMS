using CTMS.Share.Extensions;
using CTMS.Share.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CTMS.DataModel.Models.ClinicalInformation
{
    public class VisitCodeSetModel
    {
        public List<VisitCodeModel> VisitCodes { get; set; } = new();
        public List<VisitCodeSetNodeModel> Nodes { get; set; }= new();

        public int GetVisitCodeIndex(VisitCodeModel visitCodeModel)
        {
            return VisitCodes.FindIndex(x => x.CompareTo(visitCodeModel));
        }

        public VisitCodeSetNodeModel GetVisitCodeSetNodeModel(VisitCodeModel visitCodeModel)
        {
            int index = GetVisitCodeIndex(visitCodeModel);
            if (index >= 0 && index < Nodes.Count)
            {
                return Nodes[index];
            }
            return null;
        }
    }
    public class VisitCodeSetNodeModel
    {
        public bool Checked臨床資料手術 { get; set; } = false;
        public bool Checked臨床資料病理報告 { get; set; } = false;
        public bool Checked臨床資料化學治療 { get; set; } = false;
        public bool Checked臨床資料合併用藥 { get; set; } = false;
        public bool CheckedBaselineMedicalHistoryForm { get; set; } = false;
        public bool Checked抽血檢驗血液 { get; set; } = false;
        public bool Checked抽血檢驗生化 { get; set; } = false;
        public bool CheckedSurvey化療副作用 { get; set; } = false;
        public bool CheckedSurvey標靶副作用 { get; set; } = false;
        public bool CheckedSurvey放療副作用 { get; set; } = false;
        public bool CheckedSurveyWhooqol問卷 { get; set; } = false;
        public bool CheckedSurvey個人史問卷 { get; set; } = false;
        public bool CheckedSurvey家族史問卷 { get; set; } = false;
        public bool CheckedHematologicSideEffects血液副作用 { get; set; } = false;
        public bool CheckedSurveySideEffects副作用1 { get; set; } = false;
        public bool CheckedSurveySideEffects副作用2 { get; set; } = false;
        public bool Checked其他治療 { get; set; } = false;
        public bool Checked其他治療藥物 { get; set; } = false;
        public bool Checked其他治療影像 { get; set; } = false;
    }
}
