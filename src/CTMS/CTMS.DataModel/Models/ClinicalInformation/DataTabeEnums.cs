using CTMS.DataModel.Dtos;
using CTMS.Share.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTMS.DataModel.Models.ClinicalInformation
{
    public enum DataTabeEnums
    {
        臨床資料手術,
        臨床資料病理報告,
        臨床資料化學治療,
        臨床資料合併用藥,
        BaselineMedicalHistoryForm,
        抽血檢驗血液,
        抽血檢驗生化,
        Survey化療副作用,
        Survey標靶副作用,
        Survey放療副作用,
        SurveyWhooqol問卷,
        Survey個人史問卷,
        Survey家族史問卷,
        HematologicSideEffects血液副作用,
        SurveySideEffects副作用1,
        SurveySideEffects副作用2,
        其他治療,
        其他治療藥物,
        其他治療影像
    }
}
