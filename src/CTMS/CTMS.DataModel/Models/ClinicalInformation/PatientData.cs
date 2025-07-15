using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTMS.DataModel.Models.ClinicalInformation;

public class PatientData
{
    public BasicClinicalPresentation_臨床資訊 臨床資訊 { get; set; } = new();
    public Main臨床資料 臨床資料 { get; set; } = new();

    public string ToJson()
    {
        var options = new System.Text.Json.JsonSerializerOptions
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All)
        };
        return System.Text.Json.JsonSerializer.Serialize(this, options);
    }

    public void FromJson(string json)
    {
        var data = System.Text.Json.JsonSerializer.Deserialize<PatientData>(json);
        if (data != null)
        {
            this.臨床資訊 = data.臨床資訊;
            this.臨床資料 = data.臨床資料;
        }
        SyncData();
    }

    public void SyncData()
    {
    //public enum DataTabeEnums
    //{
    //    臨床資料手術,
    //    臨床資料病理報告,
    //    臨床資料化學治療,
    //    臨床資料合併用藥,
    //    BaselineMedicalHistoryForm,
    //    抽血檢驗血液,
    //    抽血檢驗生化,
    //    Survey化療副作用,
    //    Survey標靶副作用,
    //    Survey放療副作用,
    //    SurveyWhooqol問卷,
    //    Survey個人史問卷,
    //    Survey家族史問卷,
    //    HematologicSideEffects血液副作用,
    //    SurveySideEffects副作用1,
    //    SurveySideEffects副作用2,
    //    其他治療,
    //    其他治療藥物,
    //    其他治療影像
    //}
        foreach (var item in this.臨床資料.臨床資料手術.Items)
        {
            item.SubjectNo = this.臨床資訊.SubjectNo;
        }
        foreach (var item in this.臨床資料.臨床資料病理報告.Items)
        {
            item.SubjectNo = this.臨床資訊.SubjectNo;
        }
        foreach (var item in this.臨床資料.臨床資料化學治療.Items)
        {
            item.SubjectNo = this.臨床資訊.SubjectNo;
        }
        foreach (var item in this.臨床資料.臨床資料合併用藥.Items)
        {
            item.SubjectNo = this.臨床資訊.SubjectNo;
        }
        foreach (var item in this.臨床資料.BaselineMedicalHistoryForm.Items)
        {
            item.SubjectNo = this.臨床資訊.SubjectNo;
        }
    }
}
