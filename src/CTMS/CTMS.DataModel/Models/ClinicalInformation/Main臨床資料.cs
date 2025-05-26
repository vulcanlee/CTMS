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
    }
}
