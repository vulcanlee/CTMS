using CTMS.Business.Services.ClinicalInformation;
using CTMS.DataModel.Dtos;
using CTMS.DataModel.Models;
using CTMS.DataModel.Models.ClinicalInformation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTMS.Business.Services;

public class Main臨床資料HelperService
{
    private readonly BloodExameService bloodExameService;
    private readonly SurveyService surveyService;
    private readonly SideEffectsService sideEffectsService;
    private readonly SurveySideEffectsService surveySideEffectsService;

    public Main臨床資料HelperService(BloodExameService bloodExameService,
        SurveyService surveyService,
        SideEffectsService sideEffectsService,
        SurveySideEffectsService surveySideEffectsService)
    {
        this.bloodExameService = bloodExameService;
        this.surveyService = surveyService;
        this.sideEffectsService = sideEffectsService;
        this.surveySideEffectsService = surveySideEffectsService;
    }
    public void Check(Main臨床資料 Main臨床資料)
    {
    }

    public void SyncData(
        string subjectNo,
        Main臨床資料 Main臨床資料,
        VisitCodeSetModel VisitCodeSetModel)
    {
        int count = VisitCodeSetModel.VisitCodes.Count;

        for (int idx = 0; idx < count; idx++)
        {
            VisitCodeModel visitCodeModel = VisitCodeSetModel.VisitCodes[idx];
            VisitCodeSetNodeModel node = VisitCodeSetModel.Nodes[idx];

            SyncItem(Main臨床資料, visitCodeModel, node, DataTabeEnums.臨床資料手術);
            SyncItem(Main臨床資料, visitCodeModel, node, DataTabeEnums.臨床資料病理報告);
            SyncItem(Main臨床資料, visitCodeModel, node, DataTabeEnums.臨床資料化學治療);
            SyncItem(Main臨床資料, visitCodeModel, node, DataTabeEnums.臨床資料合併用藥);
            SyncItem(Main臨床資料, visitCodeModel, node, DataTabeEnums.BaselineMedicalHistoryForm);
            SyncItem(Main臨床資料, visitCodeModel, node, DataTabeEnums.抽血檢驗血液, subjectNo);
            SyncItem(Main臨床資料, visitCodeModel, node, DataTabeEnums.抽血檢驗生化, subjectNo);
            SyncItem(Main臨床資料, visitCodeModel, node, DataTabeEnums.Survey化療副作用);
            SyncItem(Main臨床資料, visitCodeModel, node, DataTabeEnums.Survey標靶副作用);
            SyncItem(Main臨床資料, visitCodeModel, node, DataTabeEnums.Survey放療副作用);
            SyncItem(Main臨床資料, visitCodeModel, node, DataTabeEnums.SurveyWhooqol問卷);
            SyncItem(Main臨床資料, visitCodeModel, node, DataTabeEnums.Survey個人史問卷);
            SyncItem(Main臨床資料, visitCodeModel, node, DataTabeEnums.Survey家族史問卷);
            SyncItem(Main臨床資料, visitCodeModel, node, DataTabeEnums.HematologicSideEffects血液副作用);
            SyncItem(Main臨床資料, visitCodeModel, node, DataTabeEnums.SurveySideEffects副作用1);
            SyncItem(Main臨床資料, visitCodeModel, node, DataTabeEnums.SurveySideEffects副作用2);
            SyncItem(Main臨床資料, visitCodeModel, node, DataTabeEnums.其他治療);
            SyncItem(Main臨床資料, visitCodeModel, node, DataTabeEnums.其他治療藥物);
            SyncItem(Main臨床資料, visitCodeModel, node, DataTabeEnums.其他治療影像);
        }
    }

    public void SyncItem(Main臨床資料 Main臨床資料,
        VisitCodeModel visitCodeModel,
        VisitCodeSetNodeModel node,
        DataTabeEnums DataTabeEnums,
        string subjectNo = "")
    {
        switch (DataTabeEnums)
        {
            case DataTabeEnums.臨床資料手術:
                {
                    var item = Main臨床資料.臨床資料手術.Items
                        .FirstOrDefault(x => x.VisitCode.CompareTo(visitCodeModel));
                    if (item == null)
                    {
                        if (node.Checked臨床資料手術 == false)
                            return;
                        item = new 臨床資料手術Node
                        {
                            VisitCode = visitCodeModel
                        };
                        Main臨床資料.臨床資料手術.Items.Add(item);
                    }
                    else
                    {
                        if (node.Checked臨床資料手術 == true)
                        {
                            return;
                        }
                        Main臨床資料.臨床資料手術.Items.Remove(item);
                    }
                }
                break;
            case DataTabeEnums.臨床資料病理報告:
                {
                    var item = Main臨床資料.臨床資料病理報告.Items
                        .FirstOrDefault(x => x.VisitCode.CompareTo(visitCodeModel));
                    if (item == null)
                    {
                        if (node.Checked臨床資料病理報告 == false)
                            return;
                        item = new 臨床資料病理報告Node
                        {
                            VisitCode = visitCodeModel
                        };
                        Main臨床資料.臨床資料病理報告.Items.Add(item);
                    }
                    else
                    {
                        if (node.Checked臨床資料病理報告 == true)
                        {
                            return;
                        }
                        Main臨床資料.臨床資料病理報告.Items.Remove(item);
                    }
                }
                break;
            case DataTabeEnums.臨床資料化學治療:
                {
                    var item = Main臨床資料.臨床資料化學治療.Items
                        .FirstOrDefault(x => x.VisitCode.CompareTo(visitCodeModel));
                    if (item == null)
                    {
                        if (node.Checked臨床資料化學治療 == false)
                            return;
                        item = new 臨床資料化學治療Node
                        {
                            VisitCode = visitCodeModel
                        };
                        Main臨床資料.臨床資料化學治療.Items.Add(item);
                    }
                    else
                    {
                        if (node.Checked臨床資料化學治療 == true)
                        {
                            return;
                        }
                        Main臨床資料.臨床資料化學治療.Items.Remove(item);
                    }
                }
                break;
            case DataTabeEnums.臨床資料合併用藥:
                {
                    var item = Main臨床資料.臨床資料合併用藥.Items
                        .FirstOrDefault(x => x.VisitCode.CompareTo(visitCodeModel));
                    if (item == null)
                    {
                        if (node.Checked臨床資料合併用藥 == false)
                            return;
                        item = new 臨床資料合併用藥Node
                        {
                            VisitCode = visitCodeModel
                        };
                        Main臨床資料.臨床資料合併用藥.Items.Add(item);
                    }
                    else
                    {
                        if (node.Checked臨床資料合併用藥 == true)
                        {
                            return;
                        }
                        Main臨床資料.臨床資料合併用藥.Items.Remove(item);
                    }
                }
                break;
            case DataTabeEnums.BaselineMedicalHistoryForm:
                {
                    var item = Main臨床資料.BaselineMedicalHistoryForm.Items
                        .FirstOrDefault(x => x.VisitCode.CompareTo(visitCodeModel));
                    if (item == null)
                    {
                        if (node.CheckedBaselineMedicalHistoryForm == false)
                            return;
                        item = new BaselineMedicalHistoryFormNode
                        {
                            VisitCode = visitCodeModel
                        };
                        Main臨床資料.BaselineMedicalHistoryForm.Items.Add(item);
                    }
                    else
                    {
                        if (node.CheckedBaselineMedicalHistoryForm == true)
                        {
                            return;
                        }
                        Main臨床資料.BaselineMedicalHistoryForm.Items.Remove(item);
                    }
                }
                break;
            case DataTabeEnums.抽血檢驗血液:
                {
                    var item = Main臨床資料.抽血檢驗血液.Items
                        .FirstOrDefault(x => x.VisitCode.CompareTo(visitCodeModel));
                    if (item == null)
                    {
                        if (node.Checked抽血檢驗血液 == false)
                            return;
                        item = new BloodTest抽血檢驗血液Node
                        {
                            VisitCode = visitCodeModel
                        };
                        bloodExameService.Read血液Node(item, subjectNo);
                        Main臨床資料.抽血檢驗血液.Items.Add(item);
                    }
                    else
                    {
                        if (node.Checked抽血檢驗血液 == true)
                        {
                            return;
                        }
                        Main臨床資料.抽血檢驗血液.Items.Remove(item);
                    }
                }
                break;
            case DataTabeEnums.抽血檢驗生化:
                {
                    var item = Main臨床資料.抽血檢驗生化.Items
                        .FirstOrDefault(x => x.VisitCode.CompareTo(visitCodeModel));
                    if (item == null)
                    {
                        if (node.Checked抽血檢驗生化 == false)
                            return;
                        item = new BloodTest抽血檢驗生化Node
                        {
                            VisitCode = visitCodeModel
                        };
                        bloodExameService.Read生化Node(item, subjectNo);
                        Main臨床資料.抽血檢驗生化.Items.Add(item);
                    }
                    else
                    {
                        if (node.Checked抽血檢驗生化 == true)
                        {
                            return;
                        }
                        Main臨床資料.抽血檢驗生化.Items.Remove(item);
                    }
                }
                break;
            case DataTabeEnums.Survey化療副作用:
                {
                    var item = Main臨床資料.Survey化療副作用.Items
                        .FirstOrDefault(x => x.VisitCode.CompareTo(visitCodeModel));
                    if (item == null)
                    {
                        if (node.CheckedSurvey化療副作用 == false)
                            return;
                        item = new Survey化療副作用Node
                        {
                            VisitCode = visitCodeModel
                        };

                        Survey問卷 survey = new();
                        surveyService.Read(survey);
                        item.Questions = survey.化療副作用.Questions;

                        surveyService.Reset(item.Questions);
                        foreach (var itemQuestion in item.Questions)
                        {
                            surveyService.RefreshByQuestionChanged(item.Questions, itemQuestion);
                        }

                        Main臨床資料.Survey化療副作用.Items.Add(item);
                    }
                    else
                    {
                        if (node.CheckedSurvey化療副作用 == true)
                        {
                            return;
                        }
                        Main臨床資料.Survey化療副作用.Items.Remove(item);
                    }
                }
                break;
            case DataTabeEnums.Survey標靶副作用:
                {
                    var item = Main臨床資料.Survey標靶副作用.Items
                        .FirstOrDefault(x => x.VisitCode.CompareTo(visitCodeModel));
                    if (item == null)
                    {
                        if (node.CheckedSurvey標靶副作用 == false)
                            return;
                        item = new Survey標靶副作用Node
                        {
                            VisitCode = visitCodeModel
                        };

                        Survey問卷 survey = new();
                        surveyService.Read(survey);
                        item.Questions = survey.標靶副作用.Questions;

                        surveyService.Reset(item.Questions);
                        foreach (var itemQuestion in item.Questions)
                        {
                            surveyService.RefreshByQuestionChanged(item.Questions, itemQuestion);
                        }

                        Main臨床資料.Survey標靶副作用.Items.Add(item);
                    }
                    else
                    {
                        if (node.CheckedSurvey標靶副作用 == true)
                        {
                            return;
                        }
                        Main臨床資料.Survey標靶副作用.Items.Remove(item);
                    }
                }
                break;
            case DataTabeEnums.Survey放療副作用:
                {
                    var item = Main臨床資料.Survey放療副作用.Items
                        .FirstOrDefault(x => x.VisitCode.CompareTo(visitCodeModel));
                    if (item == null)
                    {
                        if (node.CheckedSurvey放療副作用 == false)
                            return;
                        item = new Survey放療副作用Node
                        {
                            VisitCode = visitCodeModel
                        };

                        Survey問卷 survey = new();
                        surveyService.Read(survey);
                        item.Questions = survey.放療副作用.Questions;

                        surveyService.Reset(item.Questions);
                        foreach (var itemQuestion in item.Questions)
                        {
                            surveyService.RefreshByQuestionChanged(item.Questions, itemQuestion);
                        }

                        Main臨床資料.Survey放療副作用.Items.Add(item);
                    }
                    else
                    {
                        if (node.CheckedSurvey放療副作用 == true)
                        {
                            return;
                        }
                        Main臨床資料.Survey放療副作用.Items.Remove(item);
                    }
                }
                break;
            case DataTabeEnums.SurveyWhooqol問卷:
                {
                    var item = Main臨床資料.SurveyWhooqol問卷.Items
                        .FirstOrDefault(x => x.VisitCode.CompareTo(visitCodeModel));
                    if (item == null)
                    {
                        if (node.CheckedSurveyWhooqol問卷 == false)
                            return;
                        item = new SurveyWhooqol問卷Node
                        {
                            VisitCode = visitCodeModel
                        };

                        Survey問卷 survey = new();
                        surveyService.Read(survey);
                        item.Questions = survey.whooqol問卷.Questions;

                        surveyService.Reset(item.Questions);
                        foreach (var itemQuestion in item.Questions)
                        {
                            surveyService.RefreshByQuestionChanged(item.Questions, itemQuestion);
                        }

                        Main臨床資料.SurveyWhooqol問卷.Items.Add(item);
                    }
                    else
                    {
                        if (node.CheckedSurveyWhooqol問卷 == true)
                        {
                            return;
                        }
                        Main臨床資料.SurveyWhooqol問卷.Items.Remove(item);
                    }
                }
                break;
            case DataTabeEnums.Survey個人史問卷:
                {
                    var item = Main臨床資料.Survey個人史問卷.Items
                        .FirstOrDefault(x => x.VisitCode.CompareTo(visitCodeModel));
                    if (item == null)
                    {
                        if (node.CheckedSurvey個人史問卷 == false)
                            return;
                        item = new Survey個人史問卷Node
                        {
                            VisitCode = visitCodeModel
                        };

                        Survey問卷 survey = new();
                        surveyService.Read(survey);
                        item.Questions = survey.個人史問卷.Questions;

                        surveyService.Reset(item.Questions);
                        foreach (var itemQuestion in item.Questions)
                        {
                            surveyService.RefreshByQuestionChanged(item.Questions, itemQuestion);
                        }

                        Main臨床資料.Survey個人史問卷.Items.Add(item);
                    }
                    else
                    {
                        if (node.CheckedSurvey個人史問卷 == true)
                        {
                            return;
                        }
                        Main臨床資料.Survey個人史問卷.Items.Remove(item);
                    }
                }
                break;
            case DataTabeEnums.Survey家族史問卷:
                {
                    var item = Main臨床資料.Survey家族史問卷.Items
                        .FirstOrDefault(x => x.VisitCode.CompareTo(visitCodeModel));
                    if (item == null)
                    {
                        if (node.CheckedSurvey家族史問卷 == false)
                            return;
                        item = new Survey家族史問卷Node
                        {
                            VisitCode = visitCodeModel
                        };

                        Survey問卷 survey = new();
                        surveyService.Read(survey);
                        item.Questions = survey.家族史問卷.Questions;

                        surveyService.Reset(item.Questions);
                        foreach (var itemQuestion in item.Questions)
                        {
                            surveyService.RefreshByQuestionChanged(item.Questions, itemQuestion);
                        }

                        Main臨床資料.Survey家族史問卷.Items.Add(item);
                    }
                    else
                    {
                        if (node.CheckedSurvey家族史問卷 == true)
                        {
                            return;
                        }
                        Main臨床資料.Survey家族史問卷.Items.Remove(item);
                    }
                }
                break;
            case DataTabeEnums.HematologicSideEffects血液副作用:
                {
                    var item = Main臨床資料.HematologicSideEffects血液副作用.Items
                        .FirstOrDefault(x => x.VisitCode.CompareTo(visitCodeModel));
                    if (item == null)
                    {
                        if (node.CheckedHematologicSideEffects血液副作用 == false)
                            return;
                        item = new HematologicSideEffects血液副作用Node
                        {
                            VisitCode = visitCodeModel
                        };

                        sideEffectsService.InitAll(item);
                        sideEffectsService.Update副作用All(Main臨床資料, item);

                        Main臨床資料.HematologicSideEffects血液副作用.Items.Add(item);
                    }
                    else
                    {
                        if (node.CheckedHematologicSideEffects血液副作用 == true)
                        {
                            return;
                        }

                        Main臨床資料.HematologicSideEffects血液副作用.Items.Remove(item);
                    }
                }
                break;
            case DataTabeEnums.SurveySideEffects副作用1:
                {
                    var item = Main臨床資料.SurveySideEffects副作用1.Items
                        .FirstOrDefault(x => x.VisitCode.CompareTo(visitCodeModel));
                    if (item == null)
                    {
                        if (node.CheckedSurveySideEffects副作用1 == false)
                            return;
                        item = new Survey1SideEffects副作用Node
                        {
                            VisitCode = visitCodeModel
                        };

                        surveySideEffectsService.Init1All(item);
                        surveySideEffectsService.Update副作用1All(Main臨床資料, item);

                        Main臨床資料.SurveySideEffects副作用1.Items.Add(item);
                    }
                    else
                    {
                        if (node.CheckedSurveySideEffects副作用1 == true)
                        {
                            return;
                        }
                        Main臨床資料.SurveySideEffects副作用1.Items.Remove(item);
                    }
                }
                break;
            case DataTabeEnums.SurveySideEffects副作用2:
                {
                    var item = Main臨床資料.SurveySideEffects副作用2.Items
                        .FirstOrDefault(x => x.VisitCode.CompareTo(visitCodeModel));
                    if (item == null)
                    {
                        if (node.CheckedSurveySideEffects副作用2 == false)
                            return;
                        item = new Survey2SideEffects副作用Node
                        {
                            VisitCode = visitCodeModel
                        };

                        surveySideEffectsService.Init2All(item);
                        surveySideEffectsService.Update副作用2All(Main臨床資料, item);
                        
                        Main臨床資料.SurveySideEffects副作用2.Items.Add(item);
                    }
                    else
                    {
                        if (node.CheckedSurveySideEffects副作用2 == true)
                        {
                            return;
                        }
                        Main臨床資料.SurveySideEffects副作用2.Items.Remove(item);
                    }
                }
                break;
            case DataTabeEnums.其他治療:
                {
                    var item = Main臨床資料.其他治療.Items
                        .FirstOrDefault(x => x.VisitCode.CompareTo(visitCodeModel));
                    if (item == null)
                    {
                        if (node.Checked其他治療 == false)
                            return;
                        item = new OtherTreatmentNode
                        {
                            VisitCode = visitCodeModel
                        };
                        item.BuildItem();
                        Main臨床資料.其他治療.Items.Add(item);
                    }
                    else
                    {
                        if (node.Checked其他治療 == true)
                        {
                            return;
                        }
                        Main臨床資料.其他治療.Items.Remove(item);
                    }
                }
                break;
            case DataTabeEnums.其他治療藥物:
                {
                    var item = Main臨床資料.其他治療藥物.Items
                        .FirstOrDefault(x => x.VisitCode.CompareTo(visitCodeModel));
                    if (item == null)
                    {
                        if (node.Checked其他治療藥物 == false)
                            return;
                        item = new OtherMedicationNode
                        {
                            VisitCode = visitCodeModel
                        };
                        Main臨床資料.其他治療藥物.Items.Add(item);
                    }
                    else
                    {
                        if (node.Checked其他治療藥物 == true)
                        {
                            return;
                        }
                        Main臨床資料.其他治療藥物.Items.Remove(item);
                    }
                }
                break;
            case DataTabeEnums.其他治療影像:
                {
                    var item = Main臨床資料.其他治療影像.Items
                        .FirstOrDefault(x => x.VisitCode.CompareTo(visitCodeModel));
                    if (item == null)
                    {
                        if (node.Checked其他治療影像 == false)
                            return;
                        item = new OtherTreatmentImageNode
                        {
                            VisitCode = visitCodeModel
                        };
                        Main臨床資料.其他治療影像.Items.Add(item);
                    }
                    else
                    {
                        if (node.Checked其他治療影像 == true)
                        {
                            return;
                        }
                        Main臨床資料.其他治療影像.Items.Remove(item);
                    }
                }
                break;
        }
    }
}
