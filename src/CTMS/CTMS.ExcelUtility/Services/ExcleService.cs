using CTMS.ExcelUtility.Services;
using CTMS.Share.Helpers;
using Syncfusion.XlsIO;
using CTMS.ExcelUtility.Extensions;
using CTMS.DataModel.Models;
using System;

namespace SyncExcel.Services;

public class ExcleService
{
    public NextGenerationSportsCTMSModel ReadExcel()
    {
        NextGenerationSportsCTMSModel CTMSModel = new();
        //New instance of ExcelEngine is created 
        //Equivalent to launching Microsoft Excel with no workbooks open
        //Instantiate the spreadsheet creation engine
        using (ExcelEngine excelEngine = new ExcelEngine())
        {
            //Instantiate the Excel application object
            IApplication application = excelEngine.Excel;

            //Assigns default application version
            application.DefaultVersion = ExcelVersion.Xlsx;

            //A existing workbook is opened.             
            using (FileStream sampleFile = new FileStream(@"Data\Test for PE dashboard.xlsx", FileMode.Open))
            {
                IWorkbook workbook = application.Workbooks.Open(sampleFile);

                Collect首頁FromExcel(CTMSModel, workbook);
                Collect身體組成_肌肉質量FromExcel(CTMSModel, workbook);
                Collect身體組成_動作分析FromExcel(CTMSModel, workbook);
                Collect身體組成_肌肉品質FromExcel(CTMSModel, workbook);
                Collect身體組成_脂肪分析FromExcel(CTMSModel, workbook);
                Collect綜合評估建議FromExcel(CTMSModel, workbook);

                ShowInformation(CTMSModel);
            }
        }
        return CTMSModel;
    }

    public string CheckExcel(string filename)
    {
        string result = "";
        try
        {
            using (ExcelEngine excelEngine = new ExcelEngine())
            {
                //Instantiate the Excel application object
                IApplication application = excelEngine.Excel;

                //Assigns default application version
                application.DefaultVersion = ExcelVersion.Xlsx;

                //A existing workbook is opened.             
                using (FileStream sampleFile = new FileStream(filename, FileMode.Open))
                {
                    IWorkbook workbook = application.Workbooks.Open(sampleFile);
                    IWorksheet worksheetCT身體組成 = workbook
                        .Worksheets[MagicObjectHelper.CT身體組成原始擋20241212WorkSheetName];
                    IWorksheet worksheetBIA身體組成 = workbook
                        .Worksheets[MagicObjectHelper.身體組成_肌肉品質20241212WorkSheetName];
                    if (worksheetCT身體組成 == null)
                    {
                        result = "Excel 檔案中 [CT身體組成原始擋] 工作表 不存在";
                    }
                    else if (worksheetBIA身體組成 == null)
                    {
                        result = "Excel 檔案中 [身體組成_肌肉品質] 工作表 不存在";
                    }
                }
            }
        }
        catch (Exception ex)
        {
            result = $"開啟 Excel 檔案發生異常 : {ex.Message}";
        }
        return result;
    }

    public NextGenerationSportsCTMSModel ReadExcel(string filename)
    {
        NextGenerationSportsCTMSModel CTMSModel = new();
        using (ExcelEngine excelEngine = new ExcelEngine())
        {
            IApplication application = excelEngine.Excel;

            application.DefaultVersion = ExcelVersion.Xlsx;

            using (FileStream sampleFile = new FileStream(filename, FileMode.Open))
            {
                IWorkbook workbook = application.Workbooks.Open(sampleFile);

                if (Check20241212Requirement(workbook) == false)
                {
                    Collect首頁FromExcel(CTMSModel, workbook);
                    Collect身體組成_肌肉質量FromExcel(CTMSModel, workbook);
                    Collect身體組成_動作分析FromExcel(CTMSModel, workbook);
                    Collect身體組成_肌肉品質FromExcel(CTMSModel, workbook);
                    Collect身體組成_脂肪分析FromExcel(CTMSModel, workbook);
                    Collect綜合評估建議FromExcel(CTMSModel, workbook);
                }
                else
                {
                    Collect首頁20241212FromExcel(CTMSModel, workbook);
                    Collect動作分析能力20241212FromExcel(CTMSModel, workbook);
                    Collect心肺功能20241212FromExcel(CTMSModel, workbook);
                    Collect心理韌性20241212FromExcel(CTMSModel, workbook);
                    Collect身體組成_肌肉質量20241212FromExcel(CTMSModel, workbook);
                    Collect身體組成_肌肉品質20241212FromExcel(CTMSModel, workbook);
                    Collect身體組成_脂肪分析20241212FromExcel(CTMSModel, workbook);
                    Collect身體組成_基因體分析20241212FromExcel(CTMSModel, workbook);
                    Collect代謝體分析20241212FromExcel(CTMSModel, workbook);
                    Collect數值縱向軌跡_肌肉崩解20241212FromExcel(CTMSModel, workbook);
                    Collect抽血檢驗_血液20241212FromExcel(CTMSModel, workbook);
                    Collect抽血檢驗_生化20241212FromExcel(CTMSModel, workbook);
                    Collect抽血檢驗_特殊20241212FromExcel(CTMSModel, workbook);
                    Collect綜合評估建議20241212FromExcel(CTMSModel, workbook);
                }
            }
        }
        return CTMSModel;
    }

    #region 第二版的需求
    public bool Check20241212Requirement(IWorkbook workbook)
    {
        bool result = false;
        IWorksheet worksheet主頁 = workbook
         .Worksheets[MagicObjectHelper.主頁20241212WorkSheetName];
        // 20241212版的Excel檔案，包含 "主頁"
        if (worksheet主頁 == null)
            result = false;
        else
            result = true;
        return result;
    }

    private void Collect首頁20241212FromExcel(NextGenerationSportsCTMSModel CTMSModel, IWorkbook workbook)
    {
        IWorksheet worksheet主頁 = workbook
            .Worksheets[MagicObjectHelper.主頁20241212WorkSheetName];
        IWorksheet worksheetBIA身體組成 = workbook
            .Worksheets[MagicObjectHelper.BIA身體組成原始擋20241212WorkSheetName];
        IWorksheet worksheet代謝体分析原始擋 = workbook
            .Worksheets[MagicObjectHelper.代謝体分析原始擋20241212WorkSheetName];

        CTMSModel.Home首頁2.姓名 = worksheet代謝体分析原始擋.Range["D27"].Value;
        CTMSModel.Home首頁2.性別 = worksheet主頁.Range["C2"].Value;
        CTMSModel.Home首頁2.年齡 = worksheet代謝体分析原始擋.Range["G27"].Value;
        CTMSModel.Home首頁2.身高 = worksheetBIA身體組成.Range["B2"].Value;
        CTMSModel.Home首頁2.體重 = worksheetBIA身體組成.Range["B3"].Value;
        CTMSModel.Home首頁2.BMI = worksheetBIA身體組成.Range["B5"].Value;
        CTMSModel.Home首頁2.運動類別 = worksheet主頁.Range["C7"].Value;
        CTMSModel.Home首頁2.擔任位置 = worksheet主頁.Range["C8"].Value;
        CTMSModel.Home首頁2.所屬隊伍 = worksheet主頁.Range["C9"].Value;
        CTMSModel.Home首頁2.Photo = worksheet主頁.Range["C10"].Value;
        worksheet代謝体分析原始擋.Range["F27"].NumberFormat = "yyyy-m-d";
        CTMSModel.Home首頁2.生日 = worksheet代謝体分析原始擋.Range["F27"].Value;
    }

    private void Collect動作分析能力20241212FromExcel(NextGenerationSportsCTMSModel CTMSModel, IWorkbook workbook)
    {
        IWorksheet worksheetCT身體組成 = workbook
            .Worksheets[MagicObjectHelper.CT身體組成原始擋20241212WorkSheetName];

        var MotionAnalysis動作分析 = CTMSModel.MotionAnalysis動作分析2;
        double F17 = worksheetCT身體組成.Range["F17"].Value.ToDouble();
        double F18 = worksheetCT身體組成.Range["F18"].Value.ToDouble();
        double F19 = worksheetCT身體組成.Range["F19"].Value.ToDouble();
        double F20 = worksheetCT身體組成.Range["F20"].Value.ToDouble();
        double F21 = worksheetCT身體組成.Range["F21"].Value.ToDouble();
        // F18*80%+F20*10%+F21*10%
        MotionAnalysis動作分析.旋轉 = (F18 * 0.8 + F20 * 0.1 + F21 * 0.1).ToString("F0");

        // F17 * 20 % +F18 * 20 % +F19 * 20 % +F20 * 10 % +F21 * 30 %
        MotionAnalysis動作分析.穩定 = (F17 * 0.2 + F18 * 0.2 + F19 * 0.2 + F20 * 0.1 + F21 * 0.3).ToString("F0");

        // F17 * 60 % +F18 * 40 %
        MotionAnalysis動作分析.前彎 = (F17 * 0.6 + F18 * 0.4).ToString("F0");

        // FF20 * 10 % +F21 * 90 %
        MotionAnalysis動作分析.伸展 = (F20 * 0.1 + F21 * 0.9).ToString("F0");

        // F18 * 50 % +F20 * 50 %
        MotionAnalysis動作分析.側彎 = (F18 * 0.5 + F20 * 0.5).ToString("F0");

        // F19 * 100 %
        MotionAnalysis動作分析.抬腿 = (F19 * 1.0).ToString("F0");

        #region 肌肉量 雷達圖
        var 肌肉量Radar = MotionAnalysis動作分析.肌肉量Radar;

        double B17 = worksheetCT身體組成.Range["B17"].Value.ToDouble();
        double B18 = worksheetCT身體組成.Range["B18"].Value.ToDouble();
        double B19 = worksheetCT身體組成.Range["B19"].Value.ToDouble();
        double B20 = worksheetCT身體組成.Range["B20"].Value.ToDouble();
        double B21 = worksheetCT身體組成.Range["B21"].Value.ToDouble();

        // B18*80%+B20*10%+B21*10%	
        肌肉量Radar.Rotate旋轉 = (B18 * 0.8 + B20 * 0.1 + B21 * 0.1).ToString("F1");
        // B17*20%+B18*20%+B19*20%+B20*10%+B21*30%	
        肌肉量Radar.Stablize穩定 =
            (B17 * 0.2 + B18 * 0.2 + B19 * 0.2 + B20 * 0.1 + B21 * 0.3).ToString("F1");
        // 	B17*60%+B18*40%	
        肌肉量Radar.BendForward前彎 = (B17 * 0.6 + B18 * 0.4).ToString("F1");
        // B20*10%+B21*90%	
        肌肉量Radar.Stretch伸展 = (B20 * 0.1 + B21 * 0.9).ToString("F1");
        // B18*50%+B20*50%	
        肌肉量Radar.SideBend側彎 = (B18 * 0.5 + B20 * 0.5).ToString("F1");
        // B19*100%
        肌肉量Radar.LiftLeg抬腿 = (B19 * 1.0).ToString("F1");
        MotionAnalysis動作分析.肌肉量Radar = 肌肉量Radar;
        #endregion

        #region 肌肉品質 雷達圖

        double C17 = worksheetCT身體組成.Range["C17"].Value.ToDouble();
        double C18 = worksheetCT身體組成.Range["C18"].Value.ToDouble();
        double C19 = worksheetCT身體組成.Range["C19"].Value.ToDouble();
        double C20 = worksheetCT身體組成.Range["C20"].Value.ToDouble();
        double C21 = worksheetCT身體組成.Range["C21"].Value.ToDouble();

        var 肌肉品質Radar = MotionAnalysis動作分析.肌肉品質Radar;
        // C18*80%+C20*10%+C21*10%	
        肌肉品質Radar.Rotate旋轉 = (C18 * 0.8 + C20 * 0.1 + C21 * 0.1).ToString("F1");
        // C17*20%+C18*20%+C19*20%+C20*10%+C21*30%	
        肌肉品質Radar.Stablize穩定 = (C17 * 0.2 + C18 * 0.2 + C19 * 0.2 + C20 * 0.1 + C21 * 0.3).ToString("F1");
        // C17*60%+C18*40%	
        肌肉品質Radar.BendForward前彎 = (C17 * 0.6 + C18 * 0.4).ToString("F1");
        // C20*10%+C21*90%	
        肌肉品質Radar.Stretch伸展 = (C20 * 0.1 + C21 * 0.9).ToString("F1");
        // C18*50%+C20*50%	
        肌肉品質Radar.SideBend側彎 = (C18 * 0.5 + C20 * 0.5).ToString("F1");
        // C19*100%
        肌肉品質Radar.LiftLeg抬腿 = (C19 * 1.0).ToString("F1");
        MotionAnalysis動作分析.肌肉品質Radar = 肌肉品質Radar;
        #endregion

        #region 肌力表現 雷達圖

        var 肌力表現Radar = MotionAnalysis動作分析.肌力表現Radar;
        // F18*80%+F20*10%+F21*10%	
        肌力表現Radar.Rotate旋轉 = (F18 * 0.8 + F20 * 0.1 + F21 * 0.1).ToString("F1");
        // F17*20%+F18*20%+F19*20%+F20*10%+F21*30%	
        肌力表現Radar.Stablize穩定 = (F17 * 0.2 + F18 * 0.2 + F19 * 0.2 + F20 * 0.1 + F21 * 0.3).ToString("F1");
        // F17*60%+F18*40%	
        肌力表現Radar.BendForward前彎 = (F17 * 0.6 + F18 * 0.4).ToString("F1");
        // F20*10%+F21*90%	
        肌力表現Radar.Stretch伸展 = (F20 * 0.1 + F21 * 0.9).ToString("F1");
        // F18*50%+F20*50%	
        肌力表現Radar.SideBend側彎 = (F18 * 0.5 + F20 * 0.5).ToString("F1");
        // F19*100%
        肌力表現Radar.LiftLeg抬腿 = (F19 * 1.0).ToString("F1");
        MotionAnalysis動作分析.肌力表現Radar = 肌力表現Radar;
        #endregion
    }

    private void Collect心肺功能20241212FromExcel(NextGenerationSportsCTMSModel CTMSModel, IWorkbook workbook)
    {
        IWorksheet worksheet心肺功能 = workbook
            .Worksheets[MagicObjectHelper.心肺功能原始擋20241212WorkSheetName];

        var 心肺功能 = CTMSModel.心肺功能;

        心肺功能.用力呼氣肺活量 = worksheet心肺功能.Range["B1"].Value;
        心肺功能.用力呼氣肺活量百分比 = worksheet心肺功能.Range["B2"].Value;
        心肺功能.每公斤最大耗氧量 = worksheet心肺功能.Range["B9"].Value;
        心肺功能.同齡標準化每公斤最大耗氧量 = worksheet心肺功能.Range["B15"].Value;
        心肺功能.每公斤最大耗氧量百分比 = worksheet心肺功能.Range["B10"].Value;
    }

    private void Collect心理韌性20241212FromExcel(NextGenerationSportsCTMSModel CTMSModel, IWorkbook workbook)
    {
        IWorksheet worksheet心理韌性 = workbook
            .Worksheets[MagicObjectHelper.心理韌性原始擋20241212WorkSheetName];
        IWorksheet worksheet代謝物含量壓力情緒指標 = workbook
            .Worksheets[MagicObjectHelper.代謝物含量壓力情緒指標原始檔20241212WorkSheetName];

        if (worksheet心理韌性 == null)
            return;

        var 心理韌性 = CTMSModel.心理韌性;

        心理韌性.冒險性 = worksheet心理韌性.Range["B1"].Value;
        心理韌性.抗壓力 = worksheet心理韌性.Range["B2"].Value;
        心理韌性.堅持度 = worksheet心理韌性.Range["B3"].Value;

        #region 代謝物含量-壓力情緒指標
        if (worksheet代謝物含量壓力情緒指標 == null)
            return;

        var 代謝物含量壓力情緒指標 = CTMSModel.代謝物含量壓力情緒指標;

        #region 取得整數
        代謝物含量壓力情緒指標.血清素.運動前 = worksheet代謝物含量壓力情緒指標.Range["B6"].DisplayText;
        代謝物含量壓力情緒指標.多巴胺.運動前 = worksheet代謝物含量壓力情緒指標.Range["B7"].DisplayText;
        代謝物含量壓力情緒指標.血清素原料.運動前 = worksheet代謝物含量壓力情緒指標.Range["B8"].DisplayText;
        代謝物含量壓力情緒指標.抗憂鬱指標.運動前 = worksheet代謝物含量壓力情緒指標.Range["B9"].DisplayText;

        代謝物含量壓力情緒指標.血清素.運動後15分 = worksheet代謝物含量壓力情緒指標.Range["B17"].DisplayText;
        代謝物含量壓力情緒指標.多巴胺.運動後15分 = worksheet代謝物含量壓力情緒指標.Range["B18"].DisplayText;
        代謝物含量壓力情緒指標.血清素原料.運動後15分 = worksheet代謝物含量壓力情緒指標.Range["B19"].DisplayText;
        代謝物含量壓力情緒指標.抗憂鬱指標.運動後15分 = worksheet代謝物含量壓力情緒指標.Range["B20"].DisplayText;

        代謝物含量壓力情緒指標.血清素.運動後30分 = worksheet代謝物含量壓力情緒指標.Range["B28"].DisplayText;
        代謝物含量壓力情緒指標.多巴胺.運動後30分 = worksheet代謝物含量壓力情緒指標.Range["B29"].DisplayText;
        代謝物含量壓力情緒指標.血清素原料.運動後30分 = worksheet代謝物含量壓力情緒指標.Range["B30"].DisplayText;
        代謝物含量壓力情緒指標.抗憂鬱指標.運動後30分 = worksheet代謝物含量壓力情緒指標.Range["B31"].DisplayText;


        代謝物含量壓力情緒指標.血清素.運動前 = ((int)Math.Round(代謝物含量壓力情緒指標.血清素.運動前.ToDouble())).ToString();
        代謝物含量壓力情緒指標.多巴胺.運動前 = ((int)Math.Round(代謝物含量壓力情緒指標.多巴胺.運動前.ToDouble())).ToString();
        代謝物含量壓力情緒指標.血清素原料.運動前 = ((int)Math.Round(代謝物含量壓力情緒指標.血清素原料.運動前.ToDouble())).ToString();
        代謝物含量壓力情緒指標.抗憂鬱指標.運動前 = ((int)Math.Round(代謝物含量壓力情緒指標.抗憂鬱指標.運動前.ToDouble())).ToString();

        代謝物含量壓力情緒指標.血清素.運動後15分 = ((int)Math.Round(代謝物含量壓力情緒指標.血清素.運動後15分.ToDouble())).ToString();
        代謝物含量壓力情緒指標.多巴胺.運動後15分 = ((int)Math.Round(代謝物含量壓力情緒指標.多巴胺.運動後15分.ToDouble())).ToString();
        代謝物含量壓力情緒指標.血清素原料.運動後15分 = ((int)Math.Round(代謝物含量壓力情緒指標.血清素原料.運動後15分.ToDouble())).ToString();
        代謝物含量壓力情緒指標.抗憂鬱指標.運動後15分 = ((int)Math.Round(代謝物含量壓力情緒指標.抗憂鬱指標.運動後15分.ToDouble())).ToString();

        代謝物含量壓力情緒指標.血清素.運動後30分 = ((int)Math.Round(代謝物含量壓力情緒指標.血清素.運動後30分.ToDouble())).ToString();
        代謝物含量壓力情緒指標.多巴胺.運動後30分 = ((int)Math.Round(代謝物含量壓力情緒指標.多巴胺.運動後30分.ToDouble())).ToString();
        代謝物含量壓力情緒指標.血清素原料.運動後30分 = ((int)Math.Round(代謝物含量壓力情緒指標.血清素原料.運動後30分.ToDouble())).ToString();
        代謝物含量壓力情緒指標.抗憂鬱指標.運動後30分 = ((int)Math.Round(代謝物含量壓力情緒指標.抗憂鬱指標.運動後30分.ToDouble())).ToString();

        #endregion

        #region 設定格子顏色
        代謝物含量壓力情緒指標.血清素.運動前ClassText = GetCellClassText(代謝物含量壓力情緒指標.血清素.運動前);
        代謝物含量壓力情緒指標.多巴胺.運動前ClassText = GetCellClassText(代謝物含量壓力情緒指標.多巴胺.運動前);
        代謝物含量壓力情緒指標.血清素原料.運動前ClassText = GetCellClassText(代謝物含量壓力情緒指標.血清素原料.運動前);
        代謝物含量壓力情緒指標.抗憂鬱指標.運動前ClassText = GetCellClassText(代謝物含量壓力情緒指標.抗憂鬱指標.運動前);

        代謝物含量壓力情緒指標.血清素.運動後15分ClassText = GetCellClassText(代謝物含量壓力情緒指標.血清素.運動後15分);
        代謝物含量壓力情緒指標.多巴胺.運動後15分ClassText = GetCellClassText(代謝物含量壓力情緒指標.多巴胺.運動後15分);
        代謝物含量壓力情緒指標.血清素原料.運動後15分ClassText = GetCellClassText(代謝物含量壓力情緒指標.血清素原料.運動後15分);
        代謝物含量壓力情緒指標.抗憂鬱指標.運動後15分ClassText = GetCellClassText(代謝物含量壓力情緒指標.抗憂鬱指標.運動後15分);

        代謝物含量壓力情緒指標.血清素.運動後30分ClassText = GetCellClassText(代謝物含量壓力情緒指標.血清素.運動後30分);
        代謝物含量壓力情緒指標.多巴胺.運動後30分ClassText = GetCellClassText(代謝物含量壓力情緒指標.多巴胺.運動後30分);
        代謝物含量壓力情緒指標.血清素原料.運動後30分ClassText = GetCellClassText(代謝物含量壓力情緒指標.血清素原料.運動後30分);
        代謝物含量壓力情緒指標.抗憂鬱指標.運動後30分ClassText = GetCellClassText(代謝物含量壓力情緒指標.抗憂鬱指標.運動後30分);
        #endregion

        string GetCellClassText(string value)
        {
            double doubleValue = value.ToDouble();
            if (doubleValue > 80 && doubleValue <= 100)
                return "cell-100-80";
            else if (doubleValue > 60 && doubleValue <= 80)
                return "cell-80-60";
            else if (doubleValue > 40 && doubleValue <= 60)
                return "cell-60-40";
            else if (doubleValue > 20 && doubleValue <= 40)
                return "cell-40-20";
            else if (doubleValue > 0 && doubleValue <= 20)
                return "cell-20-0";
            //if (doubleValue > 90 && doubleValue <= 100)
            //    return "cell-90-100";
            //else if (doubleValue > 80 && doubleValue <= 90)
            //    return "cell-80-90";
            //else if (doubleValue > 70 && doubleValue <= 80)
            //    return "cell-70-80";
            //else if (doubleValue > 60 && doubleValue <= 70)
            //    return "cell-60-70";
            //else if (doubleValue > 50 && doubleValue <= 60)
            //    return "cell-50-60";
            //else if (doubleValue > 40 && doubleValue <= 50)
            //    return "cell-40-50";
            //else if (doubleValue > 30 && doubleValue <= 40)
            //    return "cell-30-40";
            //else if (doubleValue > 20 && doubleValue <= 30)
            //    return "cell-20-30";
            //else if (doubleValue > 10 && doubleValue <= 20)
            //    return "cell-10-20";
            //else if (doubleValue >= 0 && doubleValue <= 10)
            //    return "cell-00-10";
            return "";
        }
        #endregion
    }

    private void Collect身體組成_肌肉質量20241212FromExcel(NextGenerationSportsCTMSModel CTMSModel, IWorkbook workbook)
    {
        IWorksheet worksheetCT身體組成 = workbook
            .Worksheets[MagicObjectHelper.CT身體組成原始擋20241212WorkSheetName];

        var MuscleMassModel肌肉質量 = CTMSModel.BodyMuscleMassModel肌肉質量Model;

        MuscleMassModel肌肉質量.Waistline腰圍 = worksheetCT身體組成.Range["B40"].Value.ToDouble().ToString("F1");
        MuscleMassModel肌肉質量.VertebralBody椎體 = worksheetCT身體組成.Range["B45"].Value
            .ToDouble().ToString("F1");

        #region 計算Skeleton骨架
        var Skeleton骨架 = worksheetCT身體組成.Range["B46"].Value;
        int Skeleton骨架Int = 0;
        int.TryParse(Skeleton骨架, out Skeleton骨架Int);
        if (Skeleton骨架Int <= 34)
        {
            MuscleMassModel肌肉質量.Skeleton骨架 = "小";
        }
        else if (Skeleton骨架Int > 34 && Skeleton骨架Int <= 67)
        {
            MuscleMassModel肌肉質量.Skeleton骨架 = "中";
        }
        else
        {
            MuscleMassModel肌肉質量.Skeleton骨架 = "大";
        }
        #endregion

        MuscleMassModel肌肉質量.Area面積 = worksheetCT身體組成.Range["B13"].Value.ToDouble().ToString("F1");
        MuscleMassModel肌肉質量.Density密度 = worksheetCT身體組成.Range["C13"].Value.ToDouble().ToString("F1");
        MuscleMassModel肌肉質量.Index指標 = worksheetCT身體組成.Range["E13"].Value.ToDouble().ToString("F1");
        MuscleMassModel肌肉質量.CoreMuscleMass核心肌群肌肉量 = worksheetCT身體組成.Range["B22"].Value;
        MuscleMassModel肌肉質量.CoreMuscleEndurance核心肌群肌肉品質 = worksheetCT身體組成.Range["C22"].Value;
        MuscleMassModel肌肉質量.CorrectCoreMuscleStrength校正肌力 = worksheetCT身體組成.Range["F22"].Value;

        #region 雷達圖的數據
        var CoreMuscleMass核心肌群肌肉量Radar = MuscleMassModel肌肉質量.CoreMuscleMass核心肌群肌肉量RadarChartModel;
        CoreMuscleMass核心肌群肌肉量Radar.RectusAbdominis腹直肌 = worksheetCT身體組成.Range["B17"].Value.ToDouble().ToString("F1");
        CoreMuscleMass核心肌群肌肉量Radar.InternalObliqueExternalObliqueTransversusAbdominis內斜肌_外斜肌_腹橫肌 =
            worksheetCT身體組成.Range["B18"].Value.ToDouble().ToString("F1");
        CoreMuscleMass核心肌群肌肉量Radar.PsoasMuscle腰肌 =
            worksheetCT身體組成.Range["B19"].Value.ToDouble().ToString("F1");
        CoreMuscleMass核心肌群肌肉量Radar.QuadratusLumborum腰方肌 =
            worksheetCT身體組成.Range["B20"].Value.ToDouble().ToString("F1");
        CoreMuscleMass核心肌群肌肉量Radar.ErectorSpinaeMultifidus豎脊肌_多裂肌 =
            worksheetCT身體組成.Range["B21"].Value.ToDouble().ToString("F1");

        var CoreMuscleEndurance核心肌群肌耐力Radar = MuscleMassModel肌肉質量.CoreMuscleEndurance核心肌群肌耐力RadarChartModel;
        CoreMuscleEndurance核心肌群肌耐力Radar.RectusAbdominis腹直肌 = worksheetCT身體組成.Range["C17"].Value.ToDouble().ToString("F1");
        CoreMuscleEndurance核心肌群肌耐力Radar.InternalObliqueExternalObliqueTransversusAbdominis內斜肌_外斜肌_腹橫肌 =
            worksheetCT身體組成.Range["C18"].Value.ToDouble().ToString("F1");
        CoreMuscleEndurance核心肌群肌耐力Radar.PsoasMuscle腰肌 =
            worksheetCT身體組成.Range["C19"].Value.ToDouble().ToString("F1");
        CoreMuscleEndurance核心肌群肌耐力Radar.QuadratusLumborum腰方肌 =
            worksheetCT身體組成.Range["C20"].Value.ToDouble().ToString("F1");
        CoreMuscleEndurance核心肌群肌耐力Radar.ErectorSpinaeMultifidus豎脊肌_多裂肌 =
            worksheetCT身體組成.Range["C21"].Value.ToDouble().ToString("F1");

        var CorrectCoreMuscleStrength核心肌群校正肌力強度Radar = MuscleMassModel肌肉質量.CorrectCoreMuscleStrength核心肌群校正肌力強度RadarChartModel;
        CorrectCoreMuscleStrength核心肌群校正肌力強度Radar.RectusAbdominis腹直肌 = worksheetCT身體組成.Range["F17"].Value.ToDouble().ToString("F1");
        CorrectCoreMuscleStrength核心肌群校正肌力強度Radar.InternalObliqueExternalObliqueTransversusAbdominis內斜肌_外斜肌_腹橫肌 =
            worksheetCT身體組成.Range["F18"].Value.ToDouble().ToString("F1");
        CorrectCoreMuscleStrength核心肌群校正肌力強度Radar.PsoasMuscle腰肌 =
            worksheetCT身體組成.Range["F19"].Value.ToDouble().ToString("F1");
        CorrectCoreMuscleStrength核心肌群校正肌力強度Radar.QuadratusLumborum腰方肌 =
            worksheetCT身體組成.Range["F20"].Value.ToDouble().ToString("F1");
        CorrectCoreMuscleStrength核心肌群校正肌力強度Radar.ErectorSpinaeMultifidus豎脊肌_多裂肌 =
            worksheetCT身體組成.Range["F21"].Value.ToDouble().ToString("F1");

        #endregion
    }

    private void Collect身體組成_肌肉品質20241212FromExcel(NextGenerationSportsCTMSModel CTMSModel, IWorkbook workbook)
    {
        IWorksheet worksheetCT身體組成 = workbook
            .Worksheets[MagicObjectHelper.CT身體組成原始擋20241212WorkSheetName];
        var BodyMuscleQuality肌肉品質 = CTMSModel.BodyMuscleQuality肌肉品質Model;
        BodyMuscleQuality肌肉品質.Skewness偏度 = worksheetCT身體組成.Range["B31"].Value.ToDouble().ToString("F1"); ;
        BodyMuscleQuality肌肉品質.Kurtosis峰度 = worksheetCT身體組成.Range["C31"].Value.ToDouble().ToString("F1"); ;
        #region 健康肌肉比
        double S13 = worksheetCT身體組成.Range["S13"].Value.ToDouble();
        double I13 = worksheetCT身體組成.Range["I13"].Value.ToDouble();
        double N13 = worksheetCT身體組成.Range["N13"].Value.ToDouble();
        //S13/(I13+N13+S13)
        BodyMuscleQuality肌肉品質.HealthyMuscleRatio健康肌肉比 = (S13 / (I13 + N13 + S13) * 100.0).ToString("F0") + "%";
        #endregion
        BodyMuscleQuality肌肉品質.HighQualityMuscleHealthScore高品質肌肉健康度 =
            worksheetCT身體組成.Range["AC22"].Value;

        #region 肌肉健康度
        double AB22 = worksheetCT身體組成.Range["AB22"].Value.ToDouble();
        // 100-AB22
        BodyMuscleQuality肌肉品質.MuscleHealthScore肌肉健康度 = (100.0 - AB22).ToString("F0");
        #endregion

        #region 肌肉健康度 雷達圖
        var MuscleHealthScore肌肉健康度 = BodyMuscleQuality肌肉品質.MuscleHealthScore肌肉健康度RadarChartModel;
        double AB17 = worksheetCT身體組成.Range["AB17"].Value.ToDouble();
        double AB18 = worksheetCT身體組成.Range["AB18"].Value.ToDouble();
        double AB19 = worksheetCT身體組成.Range["AB19"].Value.ToDouble();
        double AB20 = worksheetCT身體組成.Range["AB20"].Value.ToDouble();
        double AB21 = worksheetCT身體組成.Range["AB21"].Value.ToDouble();
        //100 - AB17
        MuscleHealthScore肌肉健康度.RectusAbdominis腹直肌 = (100 - AB17).ToString("F1");
        //100 - AB18
        MuscleHealthScore肌肉健康度.InternalObliqueExternalObliqueTransversusAbdominis內斜肌_外斜肌_腹橫肌 =
            (100 - AB18).ToString("F1");
        //100 - AB19
        MuscleHealthScore肌肉健康度.PsoasMuscle腰肌 = (100 - AB19).ToString("F1");
        //100 - AB20
        MuscleHealthScore肌肉健康度.QuadratusLumborum腰方肌 = (100 - AB20).ToString("F1");
        //100 - AB21
        MuscleHealthScore肌肉健康度.ErectorSpinaeMultifidus豎脊肌_多裂肌 = (100 - AB21).ToString("F1");
        #endregion

        #region 高品質肌肉健康度 雷達圖
        var HighQualityMuscleHealthScore高品質肌肉健康度 = BodyMuscleQuality肌肉品質.HighQualityMuscleHealthScore高品質肌肉健康度RadarChartModel;
        double AC17 = worksheetCT身體組成.Range["AC17"].Value.ToDouble();
        double AC18 = worksheetCT身體組成.Range["AC18"].Value.ToDouble();
        double AC19 = worksheetCT身體組成.Range["AC19"].Value.ToDouble();
        double AC20 = worksheetCT身體組成.Range["AC20"].Value.ToDouble();
        double AC21 = worksheetCT身體組成.Range["AC21"].Value.ToDouble();
        //AC17
        HighQualityMuscleHealthScore高品質肌肉健康度.RectusAbdominis腹直肌 = AC17.ToString("F1");
        //AC18
        HighQualityMuscleHealthScore高品質肌肉健康度.InternalObliqueExternalObliqueTransversusAbdominis內斜肌_外斜肌_腹橫肌 =
            AC18.ToString("F1");
        //AC19
        HighQualityMuscleHealthScore高品質肌肉健康度.PsoasMuscle腰肌 = AC19.ToString("F1");
        //AC20
        HighQualityMuscleHealthScore高品質肌肉健康度.QuadratusLumborum腰方肌 = AC20.ToString("F1");
        //AC21
        HighQualityMuscleHealthScore高品質肌肉健康度.ErectorSpinaeMultifidus豎脊肌_多裂肌 =
            AC21.ToString("F1");
        #endregion
    }

    private void Collect身體組成_脂肪分析20241212FromExcel(NextGenerationSportsCTMSModel CTMSModel, IWorkbook workbook)
    {
        IWorksheet worksheetCT身體組成 = workbook
            .Worksheets[MagicObjectHelper.CT身體組成原始擋20241212WorkSheetName];
        IWorksheet worksheetBIA身體組成 = workbook
            .Worksheets[MagicObjectHelper.BIA身體組成原始擋20241212WorkSheetName];
        var BodyFatAnalysis脂肪分析 = CTMSModel.BodyFatAnalysis脂肪分析Model;
        BodyFatAnalysis脂肪分析.VisceralFatPercentile內臟脂肪百分位 =
            worksheetCT身體組成.Range["C36"].Value.ToDouble().ToString("F0");
        BodyFatAnalysis脂肪分析.SubcutaneousFatPercentile皮下脂肪百分位 =
            worksheetCT身體組成.Range["H36"].Value.ToDouble().ToString("F0");
        BodyFatAnalysis脂肪分析.WaistCircumferencePercentile腰圍百分位 =
            worksheetCT身體組成.Range["B41"].Value.ToDouble().ToString("F0");
        BodyFatAnalysis脂肪分析.MetabolicSyndromeRisk代謝失調風險 =
            worksheetCT身體組成.Range["Y22"].Value;
        BodyFatAnalysis脂肪分析.SpineSkeleton脊椎骨架 =
            worksheetCT身體組成.Range["B46"].Value;

        BodyFatAnalysis脂肪分析.Height身高 = worksheetBIA身體組成.Range["B2"].Value;
        BodyFatAnalysis脂肪分析.Weight體重 = worksheetBIA身體組成.Range["B3"].Value;
        BodyFatAnalysis脂肪分析.BMI = worksheetBIA身體組成.Range["B5"].Value;
        BodyFatAnalysis脂肪分析.BodyFatPercentage體脂率 = worksheetBIA身體組成.Range["B7"].Value;
        BodyFatAnalysis脂肪分析.BasalMetabolicRate基礎代謝率 =
            worksheetBIA身體組成.Range["B8"].Value;
        BodyFatAnalysis脂肪分析.TotalDailyEnergyExpenditure每日消耗總熱量 =
                        worksheetBIA身體組成.Range["B9"].Value;
        BodyFatAnalysis脂肪分析.BodyWater身體水分 = worksheetBIA身體組成.Range["B10"].Value;

        #region 各肌群脂肪變性百分位 雷達圖
        var FatDegenerationPercentileInMuscleGroups各肌群脂肪變性百分位 = BodyFatAnalysis脂肪分析.FatDegenerationPercentileInMuscleGroups各肌群脂肪變性百分位RadarChartModel;
        FatDegenerationPercentileInMuscleGroups各肌群脂肪變性百分位.RectusAbdominis腹直肌 =
            worksheetCT身體組成.Range["Y17"].Value;
        FatDegenerationPercentileInMuscleGroups各肌群脂肪變性百分位.InternalObliqueExternalObliqueTransversusAbdominis內斜肌_外斜肌_腹橫肌 =
            worksheetCT身體組成.Range["Y18"].Value;
        FatDegenerationPercentileInMuscleGroups各肌群脂肪變性百分位.PsoasMuscle腰肌 =
            worksheetCT身體組成.Range["Y19"].Value;
        FatDegenerationPercentileInMuscleGroups各肌群脂肪變性百分位.QuadratusLumborum腰方肌 =
            worksheetCT身體組成.Range["Y20"].Value;
        FatDegenerationPercentileInMuscleGroups各肌群脂肪變性百分位.ErectorSpinaeMultifidus豎脊肌_多裂肌 =
            worksheetCT身體組成.Range["Y21"].Value;
        #endregion
    }

    private void Collect身體組成_基因體分析20241212FromExcel(NextGenerationSportsCTMSModel CTMSModel, IWorkbook workbook)
    {
        IWorksheet worksheet基因體分析定義 = workbook
            .Worksheets[MagicObjectHelper.基因體分析定義20241212WorkSheetName];
        IWorksheet worksheet基因體分析 = workbook
            .Worksheets[MagicObjectHelper.基因體分析原始擋20241212WorkSheetName];
        IWorksheet worksheet基因體細項 = workbook
            .Worksheets[MagicObjectHelper.基因體分析原始擋20241212WorkSheetName];
        var 基因體分析 = CTMSModel.基因體分析;
        var 基因體細項 = CTMSModel.基因體細項;

        基因體分析.名稱 = worksheet基因體分析定義.Range["C1"].Value;
        基因體分析.疾病 = worksheet基因體分析定義.Range["C2"].Value;
        基因體分析.基因變異 = worksheet基因體分析定義.Range["C3"].Value;
        基因體分析.致病性 = worksheet基因體分析定義.Range["C4"].Value;
        基因體分析.基因頻率 = worksheet基因體分析定義.Range["C5"].Value;
        基因體分析.遺傳模式 = worksheet基因體分析定義.Range["C6"].Value;
        基因體分析.心臟單基因 = worksheet基因體分析定義.Range["C7"].Value;
        基因體分析.肌功能多基因 = worksheet基因體分析定義.Range["C8"].Value;
        基因體分析.心理韌性基因 = worksheet基因體分析定義.Range["C9"].Value;

        基因體分析.名稱1 = worksheet基因體分析定義.Range["D1"].Value;
        基因體分析.疾病1 = worksheet基因體分析定義.Range["D2"].Value;
        基因體分析.基因變異1 = worksheet基因體分析定義.Range["D3"].Value;
        基因體分析.致病性1 = worksheet基因體分析定義.Range["D4"].Value;
        基因體分析.基因頻率1 = worksheet基因體分析定義.Range["D5"].Value;
        基因體分析.遺傳模式1 = worksheet基因體分析定義.Range["D6"].Value;

        基因體分析.基因體分析Radar.耐力 = worksheet基因體分析.Range["B2"].Value;
        基因體分析.基因體分析Radar.爆發力 = worksheet基因體分析.Range["C2"].Value;
        基因體分析.基因體分析Radar.肌力 = worksheet基因體分析.Range["D2"].Value;
        基因體分析.基因體分析Radar.協調性與敏捷性 = worksheet基因體分析.Range["E2"].Value;
        基因體分析.基因體分析Radar.得獎狀況 = worksheet基因體分析.Range["F2"].Value;
        基因體分析.基因體分析Radar.受傷狀態 = worksheet基因體分析.Range["G2"].Value;
        基因體分析.基因體分析Radar.正向復原力 = worksheet基因體分析.Range["H2"].Value;
        基因體分析.基因體分析Radar.負向復原力 = worksheet基因體分析.Range["I2"].Value;

        基因體細項.Omega3_ALA = worksheet基因體分析.Range["J2"].Value;
        基因體細項.Omega3_DHA = worksheet基因體分析.Range["K2"].Value;
        基因體細項.Omega3_DPA = worksheet基因體分析.Range["L2"].Value;
        基因體細項.Omega3_EPA = worksheet基因體分析.Range["M2"].Value;
        基因體細項.Glycerophospholipid = worksheet基因體分析.Range["N2"].Value;
        基因體細項.Carotene = worksheet基因體分析.Range["O2"].Value;
        基因體細項.Lycopene = worksheet基因體分析.Range["P2"].Value;
        基因體細項.Lecithin = worksheet基因體分析.Range["Q2"].Value;

        基因體細項.Glycine = worksheet基因體分析.Range["R2"].Value;
        基因體細項.Leucine = worksheet基因體分析.Range["S2"].Value;
        基因體細項.Phenylalanine = worksheet基因體分析.Range["T2"].Value;
        基因體細項.Proline = worksheet基因體分析.Range["U2"].Value;
        基因體細項.Serine = worksheet基因體分析.Range["V2"].Value;
        基因體細項.Arginine = worksheet基因體分析.Range["W2"].Value;
        基因體細項.Valine = worksheet基因體分析.Range["X2"].Value;

        基因體細項.Selenium = worksheet基因體分析.Range["Y2"].Value;
        基因體細項.Calcium = worksheet基因體分析.Range["Z2"].Value;
        基因體細項.Copper = worksheet基因體分析.Range["AA2"].Value;
        基因體細項.Zinc = worksheet基因體分析.Range["AB2"].Value;
        基因體細項.Phosphorus = worksheet基因體分析.Range["AC2"].Value;
        基因體細項.Magnesium = worksheet基因體分析.Range["AD2"].Value;
        基因體細項.Iron = worksheet基因體分析.Range["AE2"].Value;

        基因體細項.Folic_acid = worksheet基因體分析.Range["AF2"].Value;
        基因體細項.VitaminsA = worksheet基因體分析.Range["AG2"].Value;
        基因體細項.VitaminsB1 = worksheet基因體分析.Range["AH2"].Value;
        基因體細項.VitaminsB12 = worksheet基因體分析.Range["AI2"].Value;
        基因體細項.VitaminsB6 = worksheet基因體分析.Range["AJ2"].Value;
        基因體細項.VitaminsD = worksheet基因體分析.Range["AK2"].Value;
        基因體細項.VitaminsE = worksheet基因體分析.Range["AL2"].Value;
        基因體細項.VitaminsK = worksheet基因體分析.Range["AM2"].Value;

        基因體細項.Lower_body_strength = worksheet基因體分析.Range["AN2"].Value;
        基因體細項.Muscle_strength = worksheet基因體分析.Range["AO2"].Value;
        基因體細項.Bone_density = worksheet基因體分析.Range["AP2"].Value;
        基因體細項.Ankle_injury = worksheet基因體分析.Range["AQ2"].Value;
        基因體細項.Post_Exercise_performance = worksheet基因體分析.Range["AR2"].Value;
        基因體細項.Exercise_performance = worksheet基因體分析.Range["AS2"].Value;
        基因體細項.Calorie_expenditure = worksheet基因體分析.Range["AT2"].Value;
        基因體細項.Metabolic_capacity = worksheet基因體分析.Range["AU2"].Value;
    }

    private void Collect代謝體分析20241212FromExcel(NextGenerationSportsCTMSModel CTMSModel, IWorkbook workbook)
    {
        IWorksheet worksheetCT代謝体分析原始擋 = workbook
            .Worksheets[MagicObjectHelper.代謝体分析原始擋20241212WorkSheetName];
        IWorksheet worksheetCT代謝体分析 = workbook
            .Worksheets[MagicObjectHelper.代謝體分析20241212WorkSheetName];

        var 代謝体分析 = CTMSModel.代謝体分析;

        代謝体分析.肌肉崩解 = worksheetCT代謝体分析.Range["A1"].Value;
        代謝体分析.發炎反應 = worksheetCT代謝体分析.Range["A2"].Value;
        代謝体分析.甲基化 = worksheetCT代謝体分析.Range["A3"].Value;

        #region 肌肉能量及耗損雷達圖
        var 肌肉能量及耗損 = 代謝体分析.肌肉能量及耗損;
        肌肉能量及耗損.Leucine = worksheetCT代謝体分析原始擋.Range["AA35"].DisplayText;
        肌肉能量及耗損.Isoleucine = worksheetCT代謝体分析原始擋.Range["Z35"].DisplayText;
        肌肉能量及耗損.Valine = worksheetCT代謝体分析原始擋.Range["AB35"].DisplayText;
        肌肉能量及耗損.BCAAs = worksheetCT代謝体分析原始擋.Range["AC35"].DisplayText;
        肌肉能量及耗損.Histidine = worksheetCT代謝体分析原始擋.Range["AN35"].DisplayText;
        肌肉能量及耗損.x1_Methylhistidine = worksheetCT代謝体分析原始擋.Range["BB35"].DisplayText;
        肌肉能量及耗損.x3_Methylhistidine = worksheetCT代謝体分析原始擋.Range["BC35"].DisplayText;

        var 肌肉能量及耗損_校隊平均 = 代謝体分析.肌肉能量及耗損_校隊平均;
        var 肌肉能量及耗損_職業平均 = 代謝体分析.肌肉能量及耗損_職業平均;
        肌肉能量及耗損_職業平均.Leucine = worksheetCT代謝体分析原始擋.Range["AA47"].DisplayText;
        肌肉能量及耗損_職業平均.Isoleucine = worksheetCT代謝体分析原始擋.Range["Z47"].DisplayText;
        肌肉能量及耗損_職業平均.Valine = worksheetCT代謝体分析原始擋.Range["AB47"].DisplayText;
        肌肉能量及耗損_職業平均.BCAAs = worksheetCT代謝体分析原始擋.Range["AC47"].DisplayText;
        肌肉能量及耗損_職業平均.Histidine = worksheetCT代謝体分析原始擋.Range["AN47"].DisplayText;
        肌肉能量及耗損_職業平均.x1_Methylhistidine = worksheetCT代謝体分析原始擋.Range["BB47"].DisplayText;
        肌肉能量及耗損_職業平均.x3_Methylhistidine = worksheetCT代謝体分析原始擋.Range["BC47"].DisplayText;

        肌肉能量及耗損_校隊平均.Leucine = worksheetCT代謝体分析原始擋.Range["AA60"].DisplayText;
        肌肉能量及耗損_校隊平均.Isoleucine = worksheetCT代謝体分析原始擋.Range["Z60"].DisplayText;
        肌肉能量及耗損_校隊平均.Valine = worksheetCT代謝体分析原始擋.Range["AB60"].DisplayText;
        肌肉能量及耗損_校隊平均.BCAAs = worksheetCT代謝体分析原始擋.Range["AC60"].DisplayText;
        肌肉能量及耗損_校隊平均.Histidine = worksheetCT代謝体分析原始擋.Range["AN60"].DisplayText;
        肌肉能量及耗損_校隊平均.x1_Methylhistidine = worksheetCT代謝体分析原始擋.Range["BB60"].DisplayText;
        肌肉能量及耗損_校隊平均.x3_Methylhistidine = worksheetCT代謝体分析原始擋.Range["BC60"].DisplayText;
        #endregion

        #region 發炎狀態雷達圖
        var 發炎狀態 = 代謝体分析.發炎狀態;
        發炎狀態.KynTrp = worksheetCT代謝体分析原始擋.Range["AQ35"].DisplayText;
        發炎狀態.Tryptophan = worksheetCT代謝体分析原始擋.Range["AM35"].DisplayText;
        發炎狀態.Kynurenine = worksheetCT代謝体分析原始擋.Range["AR35"].DisplayText;

        var 發炎狀態_校隊平均 = 代謝体分析.發炎狀態_校隊平均;
        var 發炎狀態_職業平均 = 代謝体分析.發炎狀態_職業平均;
        發炎狀態_職業平均.KynTrp = worksheetCT代謝体分析原始擋.Range["AQ47"].DisplayText;
        發炎狀態_職業平均.Tryptophan = worksheetCT代謝体分析原始擋.Range["AM47"].DisplayText;
        發炎狀態_職業平均.Kynurenine = worksheetCT代謝体分析原始擋.Range["AR47"].DisplayText;

        發炎狀態_校隊平均.KynTrp = worksheetCT代謝体分析原始擋.Range["AQ60"].DisplayText;
        發炎狀態_校隊平均.Tryptophan = worksheetCT代謝体分析原始擋.Range["AM60"].DisplayText;
        發炎狀態_校隊平均.Kynurenine = worksheetCT代謝体分析原始擋.Range["AR60"].DisplayText;
        #endregion

        #region 甲基化胺基酸雷達圖
        var 甲基化胺基酸 = 代謝体分析.甲基化胺基酸;
        甲基化胺基酸.TMAO = worksheetCT代謝体分析原始擋.Range["BP35"].DisplayText;
        甲基化胺基酸.Serine = worksheetCT代謝体分析原始擋.Range["AS35"].DisplayText;
        甲基化胺基酸.Glycine = worksheetCT代謝体分析原始擋.Range["U35"].DisplayText;
        甲基化胺基酸.Sarcosine = worksheetCT代謝体分析原始擋.Range["S35"].DisplayText;
        甲基化胺基酸.Choline = worksheetCT代謝体分析原始擋.Range["T35"].DisplayText;

        var 甲基化胺基酸_校隊平均 = 代謝体分析.甲基化胺基酸_校隊平均;
        var 甲基化胺基酸_職業平均 = 代謝体分析.甲基化胺基酸_職業平均;
        甲基化胺基酸_職業平均.TMAO = worksheetCT代謝体分析原始擋.Range["BP47"].DisplayText;
        甲基化胺基酸_職業平均.Serine = worksheetCT代謝体分析原始擋.Range["AS47"].DisplayText;
        甲基化胺基酸_職業平均.Glycine = worksheetCT代謝体分析原始擋.Range["U47"].DisplayText;
        甲基化胺基酸_職業平均.Sarcosine = worksheetCT代謝体分析原始擋.Range["S47"].DisplayText;
        甲基化胺基酸_職業平均.Choline = worksheetCT代謝体分析原始擋.Range["T47"].DisplayText;

        甲基化胺基酸_校隊平均.TMAO = worksheetCT代謝体分析原始擋.Range["BP60"].DisplayText;
        甲基化胺基酸_校隊平均.Serine = worksheetCT代謝体分析原始擋.Range["AS60"].DisplayText;
        甲基化胺基酸_校隊平均.Glycine = worksheetCT代謝体分析原始擋.Range["U60"].DisplayText;
        甲基化胺基酸_校隊平均.Sarcosine = worksheetCT代謝体分析原始擋.Range["S60"].DisplayText;
        甲基化胺基酸_校隊平均.Choline = worksheetCT代謝体分析原始擋.Range["T60"].DisplayText;
        #endregion
    }

    private void Collect數值縱向軌跡_肌肉崩解20241212FromExcel(NextGenerationSportsCTMSModel CTMSModel, IWorkbook workbook)
    {
        IWorksheet worksheetCT代謝体分析原始擋 = workbook
            .Worksheets[MagicObjectHelper.代謝体分析原始擋20241212WorkSheetName];
        IWorksheet worksheetCT代謝体分析 = workbook
            .Worksheets[MagicObjectHelper.代謝體分析20241212WorkSheetName];

        var 數值縱向軌跡肌肉崩解 = CTMSModel.數值縱向軌跡肌肉崩解;
        var 數值縱向軌跡發炎反應 = CTMSModel.數值縱向軌跡發炎反應;
        var 數值縱向軌跡甲基化 = CTMSModel.數值縱向軌跡甲基化;

        #region 數值縱向軌跡肌肉崩解
        var BCAAs軌跡 = 數值縱向軌跡肌肉崩解.BCAAs軌跡;
        BCAAs軌跡.Leucine職棒平均.Baseline = worksheetCT代謝体分析原始擋.Range["AA45"].DisplayText;
        BCAAs軌跡.Leucine職棒平均.Interval15Min = worksheetCT代謝体分析原始擋.Range["AA46"].DisplayText;
        BCAAs軌跡.Leucine職棒平均.Interval30Min = worksheetCT代謝体分析原始擋.Range["AA47"].DisplayText;
        BCAAs軌跡.Leucine.Baseline = worksheetCT代謝体分析原始擋.Range["AA33"].DisplayText;
        BCAAs軌跡.Leucine.Interval15Min = worksheetCT代謝体分析原始擋.Range["AA34"].DisplayText;
        BCAAs軌跡.Leucine.Interval30Min = worksheetCT代謝体分析原始擋.Range["AA35"].DisplayText;

        BCAAs軌跡.Isoleucine職棒平均.Baseline = worksheetCT代謝体分析原始擋.Range["Z45"].DisplayText;
        BCAAs軌跡.Isoleucine職棒平均.Interval15Min = worksheetCT代謝体分析原始擋.Range["Z46"].DisplayText;
        BCAAs軌跡.Isoleucine職棒平均.Interval30Min = worksheetCT代謝体分析原始擋.Range["Z47"].DisplayText;
        BCAAs軌跡.Isoleucine.Baseline = worksheetCT代謝体分析原始擋.Range["Z33"].DisplayText;
        BCAAs軌跡.Isoleucine.Interval15Min = worksheetCT代謝体分析原始擋.Range["Z34"].DisplayText;
        BCAAs軌跡.Isoleucine.Interval30Min = worksheetCT代謝体分析原始擋.Range["Z35"].DisplayText;

        BCAAs軌跡.Valine職棒平均.Baseline = worksheetCT代謝体分析原始擋.Range["AB45"].DisplayText;
        BCAAs軌跡.Valine職棒平均.Interval15Min = worksheetCT代謝体分析原始擋.Range["AB46"].DisplayText;
        BCAAs軌跡.Valine職棒平均.Interval30Min = worksheetCT代謝体分析原始擋.Range["AB47"].DisplayText;
        BCAAs軌跡.Valine.Baseline = worksheetCT代謝体分析原始擋.Range["AB33"].DisplayText;
        BCAAs軌跡.Valine.Interval15Min = worksheetCT代謝体分析原始擋.Range["AB34"].DisplayText;
        BCAAs軌跡.Valine.Interval30Min = worksheetCT代謝体分析原始擋.Range["AB35"].DisplayText;

        BCAAs軌跡.BCAAs職棒平均.Baseline = worksheetCT代謝体分析原始擋.Range["AC45"].DisplayText;
        BCAAs軌跡.BCAAs職棒平均.Interval15Min = worksheetCT代謝体分析原始擋.Range["AC46"].DisplayText;
        BCAAs軌跡.BCAAs職棒平均.Interval30Min = worksheetCT代謝体分析原始擋.Range["AC47"].DisplayText;
        BCAAs軌跡.BCAAs.Baseline = worksheetCT代謝体分析原始擋.Range["AC33"].DisplayText;
        BCAAs軌跡.BCAAs.Interval15Min = worksheetCT代謝体分析原始擋.Range["AC34"].DisplayText;
        BCAAs軌跡.BCAAs.Interval30Min = worksheetCT代謝体分析原始擋.Range["AC35"].DisplayText;

        var 組胺酸 = 數值縱向軌跡肌肉崩解.組胺酸;
        組胺酸.Histidine職棒平均.Baseline = worksheetCT代謝体分析原始擋.Range["AN45"].DisplayText;
        組胺酸.Histidine職棒平均.Interval15Min = worksheetCT代謝体分析原始擋.Range["AN46"].DisplayText;
        組胺酸.Histidine職棒平均.Interval30Min = worksheetCT代謝体分析原始擋.Range["AN47"].DisplayText;
        組胺酸.Histidine.Baseline = worksheetCT代謝体分析原始擋.Range["AN33"].DisplayText;
        組胺酸.Histidine.Interval15Min = worksheetCT代謝体分析原始擋.Range["AN34"].DisplayText;
        組胺酸.Histidine.Interval30Min = worksheetCT代謝体分析原始擋.Range["AN35"].DisplayText;

        組胺酸.x1Methylhistidine職棒平均.Baseline = worksheetCT代謝体分析原始擋.Range["BB45"].DisplayText;
        組胺酸.x1Methylhistidine職棒平均.Interval15Min = worksheetCT代謝体分析原始擋.Range["BB46"].DisplayText;
        組胺酸.x1Methylhistidine職棒平均.Interval30Min = worksheetCT代謝体分析原始擋.Range["BB47"].DisplayText;
        組胺酸.x1Methylhistidine.Baseline = worksheetCT代謝体分析原始擋.Range["BB33"].DisplayText;
        組胺酸.x1Methylhistidine.Interval15Min = worksheetCT代謝体分析原始擋.Range["BB34"].DisplayText;
        組胺酸.x1Methylhistidine.Interval30Min = worksheetCT代謝体分析原始擋.Range["BB35"].DisplayText;

        組胺酸.x3Methylhistidine職棒平均.Baseline = worksheetCT代謝体分析原始擋.Range["BC45"].DisplayText;
        組胺酸.x3Methylhistidine職棒平均.Interval15Min = worksheetCT代謝体分析原始擋.Range["BC46"].DisplayText;
        組胺酸.x3Methylhistidine職棒平均.Interval30Min = worksheetCT代謝体分析原始擋.Range["BC47"].DisplayText;
        組胺酸.x3Methylhistidine.Baseline = worksheetCT代謝体分析原始擋.Range["BC33"].DisplayText;
        組胺酸.x3Methylhistidine.Interval15Min = worksheetCT代謝体分析原始擋.Range["BC34"].DisplayText;
        組胺酸.x3Methylhistidine.Interval30Min = worksheetCT代謝体分析原始擋.Range["BC35"].DisplayText;
        #endregion

        #region 數值縱向軌跡發炎反應
        var KynurenineTyptophan軌跡 = 數值縱向軌跡發炎反應.KynurenineTyptophan軌跡;
        KynurenineTyptophan軌跡.Tryptophan職棒平均.Baseline = worksheetCT代謝体分析原始擋.Range["AM45"].DisplayText;
        KynurenineTyptophan軌跡.Tryptophan職棒平均.Interval15Min = worksheetCT代謝体分析原始擋.Range["AM46"].DisplayText;
        KynurenineTyptophan軌跡.Tryptophan職棒平均.Interval30Min = worksheetCT代謝体分析原始擋.Range["AM47"].DisplayText;
        KynurenineTyptophan軌跡.Tryptophan.Baseline = worksheetCT代謝体分析原始擋.Range["AM33"].DisplayText;
        KynurenineTyptophan軌跡.Tryptophan.Interval15Min = worksheetCT代謝体分析原始擋.Range["AM34"].DisplayText;
        KynurenineTyptophan軌跡.Tryptophan.Interval30Min = worksheetCT代謝体分析原始擋.Range["AM35"].DisplayText;

        KynurenineTyptophan軌跡.Kynurenine職棒平均.Baseline = worksheetCT代謝体分析原始擋.Range["AR45"].DisplayText;
        KynurenineTyptophan軌跡.Kynurenine職棒平均.Interval15Min = worksheetCT代謝体分析原始擋.Range["AR46"].DisplayText;
        KynurenineTyptophan軌跡.Kynurenine職棒平均.Interval30Min = worksheetCT代謝体分析原始擋.Range["AR47"].DisplayText;
        KynurenineTyptophan軌跡.Kynurenine.Baseline = worksheetCT代謝体分析原始擋.Range["AR33"].DisplayText;
        KynurenineTyptophan軌跡.Kynurenine.Interval15Min = worksheetCT代謝体分析原始擋.Range["AR34"].DisplayText;
        KynurenineTyptophan軌跡.Kynurenine.Interval30Min = worksheetCT代謝体分析原始擋.Range["AR35"].DisplayText;

        var KynurenineSlashgTyptophan軌跡 = 數值縱向軌跡發炎反應.KynurenineSlashgTyptophan軌跡;
        KynurenineSlashgTyptophan軌跡.KynTrp職棒平均.Baseline = worksheetCT代謝体分析原始擋.Range["AQ45"].DisplayText;
        KynurenineSlashgTyptophan軌跡.KynTrp職棒平均.Interval15Min = worksheetCT代謝体分析原始擋.Range["AQ46"].DisplayText;
        KynurenineSlashgTyptophan軌跡.KynTrp職棒平均.Interval30Min = worksheetCT代謝体分析原始擋.Range["AQ47"].DisplayText;
        KynurenineSlashgTyptophan軌跡.KynTrp.Baseline = worksheetCT代謝体分析原始擋.Range["AQ33"].DisplayText;
        KynurenineSlashgTyptophan軌跡.KynTrp.Interval15Min = worksheetCT代謝体分析原始擋.Range["AQ34"].DisplayText;
        KynurenineSlashgTyptophan軌跡.KynTrp.Interval30Min = worksheetCT代謝体分析原始擋.Range["AQ35"].DisplayText;

        #endregion

        #region 數值縱向軌跡甲基化
        var 甲基化胺基酸軌跡 = 數值縱向軌跡甲基化.甲基化胺基酸軌跡;
        甲基化胺基酸軌跡.TMAO職棒平均.Baseline = worksheetCT代謝体分析原始擋.Range["BP45"].DisplayText;
        甲基化胺基酸軌跡.TMAO職棒平均.Interval15Min = worksheetCT代謝体分析原始擋.Range["BP46"].DisplayText;
        甲基化胺基酸軌跡.TMAO職棒平均.Interval30Min = worksheetCT代謝体分析原始擋.Range["BP47"].DisplayText;
        甲基化胺基酸軌跡.TMAO.Baseline = worksheetCT代謝体分析原始擋.Range["BP33"].DisplayText;
        甲基化胺基酸軌跡.TMAO.Interval15Min = worksheetCT代謝体分析原始擋.Range["BP34"].DisplayText;
        甲基化胺基酸軌跡.TMAO.Interval30Min = worksheetCT代謝体分析原始擋.Range["BP35"].DisplayText;

        甲基化胺基酸軌跡.Serine職棒平均.Baseline = worksheetCT代謝体分析原始擋.Range["AS45"].DisplayText;
        甲基化胺基酸軌跡.Serine職棒平均.Interval15Min = worksheetCT代謝体分析原始擋.Range["AS46"].DisplayText;
        甲基化胺基酸軌跡.Serine職棒平均.Interval30Min = worksheetCT代謝体分析原始擋.Range["AS47"].DisplayText;
        甲基化胺基酸軌跡.Serine.Baseline = worksheetCT代謝体分析原始擋.Range["AS33"].DisplayText;
        甲基化胺基酸軌跡.Serine.Interval15Min = worksheetCT代謝体分析原始擋.Range["AS34"].DisplayText;
        甲基化胺基酸軌跡.Serine.Interval30Min = worksheetCT代謝体分析原始擋.Range["AS35"].DisplayText;

        甲基化胺基酸軌跡.Glycine職棒平均.Baseline = worksheetCT代謝体分析原始擋.Range["U45"].DisplayText;
        甲基化胺基酸軌跡.Glycine職棒平均.Interval15Min = worksheetCT代謝体分析原始擋.Range["U46"].DisplayText;
        甲基化胺基酸軌跡.Glycine職棒平均.Interval30Min = worksheetCT代謝体分析原始擋.Range["U47"].DisplayText;
        甲基化胺基酸軌跡.Glycine.Baseline = worksheetCT代謝体分析原始擋.Range["U33"].DisplayText;
        甲基化胺基酸軌跡.Glycine.Interval15Min = worksheetCT代謝体分析原始擋.Range["U34"].DisplayText;
        甲基化胺基酸軌跡.Glycine.Interval30Min = worksheetCT代謝体分析原始擋.Range["U35"].DisplayText;

        甲基化胺基酸軌跡.Sarcosine職棒平均.Baseline = worksheetCT代謝体分析原始擋.Range["S45"].DisplayText;
        甲基化胺基酸軌跡.Sarcosine職棒平均.Interval15Min = worksheetCT代謝体分析原始擋.Range["S46"].DisplayText;
        甲基化胺基酸軌跡.Sarcosine職棒平均.Interval30Min = worksheetCT代謝体分析原始擋.Range["S47"].DisplayText;
        甲基化胺基酸軌跡.Sarcosine.Baseline = worksheetCT代謝体分析原始擋.Range["S33"].DisplayText;
        甲基化胺基酸軌跡.Sarcosine.Interval15Min = worksheetCT代謝体分析原始擋.Range["S34"].DisplayText;
        甲基化胺基酸軌跡.Sarcosine.Interval30Min = worksheetCT代謝体分析原始擋.Range["S35"].DisplayText;

        甲基化胺基酸軌跡.Choline職棒平均.Baseline = worksheetCT代謝体分析原始擋.Range["T45"].DisplayText;
        甲基化胺基酸軌跡.Choline職棒平均.Interval15Min = worksheetCT代謝体分析原始擋.Range["T46"].DisplayText;
        甲基化胺基酸軌跡.Choline職棒平均.Interval30Min = worksheetCT代謝体分析原始擋.Range["T47"].DisplayText;
        甲基化胺基酸軌跡.Choline.Baseline = worksheetCT代謝体分析原始擋.Range["T33"].DisplayText;
        甲基化胺基酸軌跡.Choline.Interval15Min = worksheetCT代謝体分析原始擋.Range["T34"].DisplayText;
        甲基化胺基酸軌跡.Choline.Interval30Min = worksheetCT代謝体分析原始擋.Range["T35"].DisplayText;
        #endregion
    }

    private void Collect抽血檢驗_血液20241212FromExcel(NextGenerationSportsCTMSModel CTMSModel, IWorkbook workbook)
    {
        IWorksheet worksheet抽血檢驗_血液 = workbook
            .Worksheets[MagicObjectHelper.抽血檢驗_血液原始檔20241212WorkSheetName];

        var 抽血檢驗_血液 = CTMSModel.抽血檢驗_血液;

        抽血檢驗_血液.白血球計數_WBC = worksheet抽血檢驗_血液.Range["B1"].Value;
        抽血檢驗_血液.紅血球計數_RHC = worksheet抽血檢驗_血液.Range["B2"].Value;
        抽血檢驗_血液.血色素_Hb = worksheet抽血檢驗_血液.Range["B3"].Value;
        抽血檢驗_血液.血比容_Hct = worksheet抽血檢驗_血液.Range["B4"].Value;
        抽血檢驗_血液.平均紅血球容積_MCV = worksheet抽血檢驗_血液.Range["B5"].Value;
        抽血檢驗_血液.紅血球血紅素量_MCH = worksheet抽血檢驗_血液.Range["B6"].Value;
        抽血檢驗_血液.平均紅血球血紅素濃度_MCHC = worksheet抽血檢驗_血液.Range["B7"].Value;
        抽血檢驗_血液.網狀紅血球_RDW = worksheet抽血檢驗_血液.Range["B8"].Value;
        抽血檢驗_血液.血小板_Plt = worksheet抽血檢驗_血液.Range["B9"].Value;
        抽血檢驗_血液.平均血小板容積_MPV = worksheet抽血檢驗_血液.Range["B10"].Value;
        抽血檢驗_血液.嗜中性白血球_Seg = worksheet抽血檢驗_血液.Range["B12"].Value;
        抽血檢驗_血液.嗜酸性白血球_Eos = worksheet抽血檢驗_血液.Range["B13"].Value;
        抽血檢驗_血液.嗜鹼性白血球_Baso = worksheet抽血檢驗_血液.Range["B14"].Value;
        抽血檢驗_血液.單核球_Mono = worksheet抽血檢驗_血液.Range["B15"].Value;
        抽血檢驗_血液.淋巴球_Lymph = worksheet抽血檢驗_血液.Range["B16"].Value;

        #region 新版 抽血檢驗_血液生化特殊Model
        var 抽血檢驗_血液Items = CTMSModel.抽血檢驗_血液Items;
        抽血檢驗_血液Items.Clear();
        抽血檢驗_血液Items.Add(new 抽血檢驗_血液生化特殊Model()
        {
            檢驗項目 = "白血球計數 (WBC)",
            參考區間 = "3.4-9.5",
            參考區間開始 = 3.4,
            參考區間結束 = 9.5,
            參考區間類型 = MagicObjectHelper.參考區間類型_區間,
            檢驗數據 = worksheet抽血檢驗_血液.Range["B1"].Value
        });
        抽血檢驗_血液Items.Add(new 抽血檢驗_血液生化特殊Model()
        {
            檢驗項目 = "紅血球計數 (RBC)",
            參考區間 = "4.26-5.78",
            參考區間開始 = 4.26,
            參考區間結束 = 5.78,
            參考區間類型 = MagicObjectHelper.參考區間類型_區間,
            檢驗數據 = worksheet抽血檢驗_血液.Range["B2"].Value
        });
        抽血檢驗_血液Items.Add(new 抽血檢驗_血液生化特殊Model()
        {
            檢驗項目 = "血色素 (Hb)",
            參考區間 = "13.3-17.2",
            參考區間開始 = 13.3,
            參考區間結束 = 17.2,
            參考區間類型 = MagicObjectHelper.參考區間類型_區間,
            檢驗數據 = worksheet抽血檢驗_血液.Range["B3"].Value
        });
        抽血檢驗_血液Items.Add(new 抽血檢驗_血液生化特殊Model()
        {
            檢驗項目 = "血比容 (Hct)",
            參考區間 = "39.1-50.2",
            參考區間開始 = 39.1,
            參考區間結束 = 50.2,
            參考區間類型 = MagicObjectHelper.參考區間類型_區間,
            檢驗數據 = worksheet抽血檢驗_血液.Range["B4"].Value
        });
        抽血檢驗_血液Items.Add(new 抽血檢驗_血液生化特殊Model()
        {
            檢驗項目 = "平均紅血球容積 (MCV)",
            參考區間 = "81.9-98.4",
            參考區間開始 = 81.9,
            參考區間結束 = 98.4,
            參考區間類型 = MagicObjectHelper.參考區間類型_區間,
            檢驗數據 = worksheet抽血檢驗_血液.Range["B5"].Value
        });
        抽血檢驗_血液Items.Add(new 抽血檢驗_血液生化特殊Model()
        {
            檢驗項目 = "紅血球血紅素量 (MCH)",
            參考區間 = "27.5-33.7",
            參考區間開始 = 27.5,
            參考區間結束 = 33.7,
            參考區間類型 = MagicObjectHelper.參考區間類型_區間,
            檢驗數據 = worksheet抽血檢驗_血液.Range["B6"].Value
        });
        抽血檢驗_血液Items.Add(new 抽血檢驗_血液生化特殊Model()
        {
            檢驗項目 = "平均紅血球血紅素濃度 (MCHC)",
            參考區間 = "32.8-35.4",
            參考區間開始 = 32.8,
            參考區間結束 = 35.4,
            參考區間類型 = MagicObjectHelper.參考區間類型_區間,
            檢驗數據 = worksheet抽血檢驗_血液.Range["B7"].Value
        });
        抽血檢驗_血液Items.Add(new 抽血檢驗_血液生化特殊Model()
        {
            檢驗項目 = "紅血球分布寬度 (RDW)",
            參考區間 = "12.0-14.6",
            參考區間開始 = 12.0,
            參考區間結束 = 14.6,
            參考區間類型 = MagicObjectHelper.參考區間類型_區間,
            檢驗數據 = worksheet抽血檢驗_血液.Range["B8"].Value
        });
        抽血檢驗_血液Items.Add(new 抽血檢驗_血液生化特殊Model()
        {
            檢驗項目 = "血小板 (Plt)",
            參考區間 = "143-349",
            參考區間開始 = 143,
            參考區間結束 = 349,
            參考區間類型 = MagicObjectHelper.參考區間類型_區間,
            檢驗數據 = worksheet抽血檢驗_血液.Range["B9"].Value
        });
        抽血檢驗_血液Items.Add(new 抽血檢驗_血液生化特殊Model()
        {
            檢驗項目 = "平均血小板容積 (MPV)",
            參考區間 = "6.8-10.2",
            參考區間開始 = 6.8,
            參考區間結束 = 10.2,
            參考區間類型 = MagicObjectHelper.參考區間類型_區間,
            檢驗數據 = worksheet抽血檢驗_血液.Range["B10"].Value
        });
        抽血檢驗_血液Items.Add(new 抽血檢驗_血液生化特殊Model()
        {
            檢驗項目 = "嗜中性白血球 (Seg)",
            參考區間 = "40.8-76.6",
            參考區間開始 = 40.8,
            參考區間結束 = 76.6,
            參考區間類型 = MagicObjectHelper.參考區間類型_區間,
            檢驗數據 = worksheet抽血檢驗_血液.Range["B12"].Value
        });
        抽血檢驗_血液Items.Add(new 抽血檢驗_血液生化特殊Model()
        {
            檢驗項目 = "嗜酸性白血球 (Eos)",
            參考區間 = "0.4-7.5",
            參考區間開始 = 0.4,
            參考區間結束 = 7.5,
            參考區間類型 = MagicObjectHelper.參考區間類型_區間,
            檢驗數據 = worksheet抽血檢驗_血液.Range["B13"].Value
        });
        抽血檢驗_血液Items.Add(new 抽血檢驗_血液生化特殊Model()
        {
            檢驗項目 = "嗜鹼性白血球 (Baso)",
            參考區間 = "0.2-1.7",
            參考區間開始 = 0.2,
            參考區間結束 = 1.7,
            參考區間類型 = MagicObjectHelper.參考區間類型_區間,
            檢驗數據 = worksheet抽血檢驗_血液.Range["B14"].Value
        });
        抽血檢驗_血液Items.Add(new 抽血檢驗_血液生化特殊Model()
        {
            檢驗項目 = "單核球 (Mono)",
            參考區間 = "4.4-11.8",
            參考區間開始 = 4.4,
            參考區間結束 = 11.8,
            參考區間類型 = MagicObjectHelper.參考區間類型_區間,
            檢驗數據 = worksheet抽血檢驗_血液.Range["B15"].Value
        });
        抽血檢驗_血液Items.Add(new 抽血檢驗_血液生化特殊Model()
        {
            檢驗項目 = "淋巴球 (Lymph)",
            參考區間 = "15.4-47.0",
            參考區間開始 = 15.4,
            參考區間結束 = 47.0,
            參考區間類型 = MagicObjectHelper.參考區間類型_區間,
            檢驗數據 = worksheet抽血檢驗_血液.Range["B16"].Value
        });
        計算血液檢查的異常(抽血檢驗_血液Items);
        #endregion

    }
    void 計算血液檢查的異常(List<抽血檢驗_血液生化特殊Model> Items)
    {
        foreach (var item in Items)
        {
            if (string.IsNullOrEmpty(item.參考區間))
            {
                continue;
            }
            if (item.參考區間類型 == MagicObjectHelper.參考區間類型_區間)
            {
                if (string.IsNullOrEmpty(item.檢驗數據) == false &&
                    (item.檢驗數據.ToBloodDouble() < item.參考區間開始 ||
                    item.檢驗數據.ToDouble() > item.參考區間結束))
                {
                    item.TextClassName = MagicObjectHelper.異常檢驗數計類別;
                }
            }
            else if (item.參考區間類型 == MagicObjectHelper.參考區間類型_大於)
            {
                if (string.IsNullOrEmpty(item.檢驗數據) == false &&
                    (item.檢驗數據.ToBloodDouble() <= item.參考區間結束))
                {
                    item.TextClassName = MagicObjectHelper.異常檢驗數計類別;
                }
            }
            else if (item.參考區間類型 == MagicObjectHelper.參考區間類型_大於等於)
            {
                if (string.IsNullOrEmpty(item.檢驗數據) == false &&
                    (item.檢驗數據.ToBloodDouble() < item.參考區間結束))
                {
                    item.TextClassName = MagicObjectHelper.異常檢驗數計類別;
                }
            }
            else if (item.參考區間類型 == MagicObjectHelper.參考區間類型_小於)
            {
                if (string.IsNullOrEmpty(item.檢驗數據) == false &&
                    (item.檢驗數據.ToBloodDouble() >= item.參考區間開始))
                {
                    item.TextClassName = MagicObjectHelper.異常檢驗數計類別;
                }
            }
            else if (item.參考區間類型 == MagicObjectHelper.參考區間類型_小於等於)
            {
                if (string.IsNullOrEmpty(item.檢驗數據) == false &&
                    (item.檢驗數據.ToBloodDouble() > item.參考區間開始))
                {
                    item.TextClassName = MagicObjectHelper.異常檢驗數計類別;
                }
            }
        }
    }

    void 計算血液特殊檢查的異常(List<抽血檢驗_血液生化特殊Model> Items)
    {
        foreach (var item in Items)
        {
            if (item.檢驗項目 == "醣化血紅素(HbA1C)")
            {
                int foo = 2;
            }
            if (string.IsNullOrEmpty(item.參考區間))
            {
                continue;
            }
            if (item.參考區間類型 == MagicObjectHelper.參考區間類型_區間)
            {
                if (string.IsNullOrEmpty(item.運動前) == false &&
                    (item.運動前.ToBloodDouble() < item.參考區間開始 ||
                    item.運動前.ToBloodDouble() > item.參考區間結束))
                {
                    item.TextClassName = MagicObjectHelper.異常檢驗數計類別;
                    item.TextClassName運動前 = MagicObjectHelper.異常檢驗數計類別;
                }
                if (string.IsNullOrEmpty(item.運動後15分鐘) == false &&
                    (item.運動後15分鐘.ToBloodDouble() < item.參考區間開始 ||
                    item.運動後15分鐘.ToBloodDouble() > item.參考區間結束))
                {
                    item.TextClassName = MagicObjectHelper.異常檢驗數計類別;
                    item.TextClassName運動後15分鐘 = MagicObjectHelper.異常檢驗數計類別;
                }
                if (string.IsNullOrEmpty(item.運動後30分鐘) == false &&
                    (item.運動後30分鐘.ToBloodDouble() < item.參考區間開始 ||
                    item.運動後30分鐘.ToBloodDouble() > item.參考區間結束))
                {
                    item.TextClassName = MagicObjectHelper.異常檢驗數計類別;
                    item.TextClassName運動後30分鐘 = MagicObjectHelper.異常檢驗數計類別;
                }
            }
            else if (item.參考區間類型 == MagicObjectHelper.參考區間類型_大於)
            {
                if (string.IsNullOrEmpty(item.運動前) == false &&
                    (item.運動前.ToBloodDouble() <= item.參考區間結束))
                {
                    item.TextClassName = MagicObjectHelper.異常檢驗數計類別;
                    item.TextClassName運動前 = MagicObjectHelper.異常檢驗數計類別;
                }
                if (string.IsNullOrEmpty(item.運動後15分鐘) == false &&
                    (item.運動後15分鐘.ToBloodDouble() <= item.參考區間結束))
                {
                    item.TextClassName = MagicObjectHelper.異常檢驗數計類別;
                    item.TextClassName運動後15分鐘 = MagicObjectHelper.異常檢驗數計類別;
                }
                if (string.IsNullOrEmpty(item.運動後30分鐘) == false &&
                    (item.運動後30分鐘.ToBloodDouble() <= item.參考區間結束))
                {
                    item.TextClassName = MagicObjectHelper.異常檢驗數計類別;
                    item.TextClassName運動後30分鐘 = MagicObjectHelper.異常檢驗數計類別;
                }
            }
            else if (item.參考區間類型 == MagicObjectHelper.參考區間類型_大於等於)
            {
                if (string.IsNullOrEmpty(item.運動前) == false &&
                    (item.運動前.ToBloodDouble() < item.參考區間結束))
                {
                    item.TextClassName = MagicObjectHelper.異常檢驗數計類別;
                    item.TextClassName運動前 = MagicObjectHelper.異常檢驗數計類別;
                }
                if (string.IsNullOrEmpty(item.運動後15分鐘) == false &&
                    (item.運動後15分鐘.ToBloodDouble() < item.參考區間結束))
                {
                    item.TextClassName = MagicObjectHelper.異常檢驗數計類別;
                    item.TextClassName運動後15分鐘 = MagicObjectHelper.異常檢驗數計類別;
                }
                if (string.IsNullOrEmpty(item.運動後30分鐘) == false &&
                    (item.運動後30分鐘.ToBloodDouble() < item.參考區間結束))
                {
                    item.TextClassName = MagicObjectHelper.異常檢驗數計類別;
                    item.TextClassName運動後30分鐘 = MagicObjectHelper.異常檢驗數計類別;
                }
            }
            else if (item.參考區間類型 == MagicObjectHelper.參考區間類型_小於)
            {
                if (string.IsNullOrEmpty(item.運動前) == false &&
                    (item.運動前.ToBloodDouble() >= item.參考區間開始))
                {
                    item.TextClassName = MagicObjectHelper.異常檢驗數計類別;
                    item.TextClassName運動前 = MagicObjectHelper.異常檢驗數計類別;
                }
                if (string.IsNullOrEmpty(item.運動後15分鐘) == false &&
                    (item.運動後15分鐘.ToBloodDouble() >= item.參考區間開始))
                {
                    item.TextClassName = MagicObjectHelper.異常檢驗數計類別;
                    item.TextClassName運動後15分鐘 = MagicObjectHelper.異常檢驗數計類別;
                }
                if (string.IsNullOrEmpty(item.運動後30分鐘) == false &&
                    (item.運動後30分鐘.ToBloodDouble() >= item.參考區間開始))
                {
                    item.TextClassName = MagicObjectHelper.異常檢驗數計類別;
                    item.TextClassName運動後30分鐘 = MagicObjectHelper.異常檢驗數計類別;
                }
            }
            else if (item.參考區間類型 == MagicObjectHelper.參考區間類型_小於等於)
            {
                if (string.IsNullOrEmpty(item.運動前) == false &&
                    (item.運動前.ToBloodDouble() > item.參考區間開始))
                {
                    item.TextClassName = MagicObjectHelper.異常檢驗數計類別;
                    item.TextClassName運動前 = MagicObjectHelper.異常檢驗數計類別;
                }
                if (string.IsNullOrEmpty(item.運動後15分鐘) == false &&
                    (item.運動後15分鐘.ToBloodDouble() > item.參考區間開始))
                {
                    item.TextClassName = MagicObjectHelper.異常檢驗數計類別;
                    item.TextClassName運動後15分鐘 = MagicObjectHelper.異常檢驗數計類別;
                }
                if (string.IsNullOrEmpty(item.運動後30分鐘) == false &&
                    (item.運動後30分鐘.ToBloodDouble() > item.參考區間開始))
                {
                    item.TextClassName = MagicObjectHelper.異常檢驗數計類別;
                    item.TextClassName運動後30分鐘 = MagicObjectHelper.異常檢驗數計類別;
                }
            }
        }
    }
    private void Collect抽血檢驗_生化20241212FromExcel(NextGenerationSportsCTMSModel CTMSModel, IWorkbook workbook)
    {
        IWorksheet worksheet抽血檢驗_生化 = workbook
            .Worksheets[MagicObjectHelper.抽血檢驗_生化原始檔20241212WorkSheetName];

        var 抽血檢驗_生化 = CTMSModel.抽血檢驗_生化;

        抽血檢驗_生化.血中尿素氮_BUN = worksheet抽血檢驗_生化.Range["B16"].Value;
        抽血檢驗_生化.天門冬胺酸轉胺酶_AST = worksheet抽血檢驗_生化.Range["B2"].Value;
        抽血檢驗_生化.丙胺酸轉胺酶_ALT = worksheet抽血檢驗_生化.Range["B3"].Value;
        抽血檢驗_生化.膽紅素總量_Total_Bilirubin = worksheet抽血檢驗_生化.Range["B4"].Value;
        抽血檢驗_生化.白蛋白_球蛋白_AG = worksheet抽血檢驗_生化.Range["B5"].Value;
        抽血檢驗_生化.總膽固醇_CHOL = worksheet抽血檢驗_生化.Range["B6"].Value;
        抽血檢驗_生化.高密度脂蛋白膽固醇_HDL = worksheet抽血檢驗_生化.Range["B7"].Value;
        抽血檢驗_生化.低密度脂蛋白膽固醇_LDL = worksheet抽血檢驗_生化.Range["B8"].Value;
        抽血檢驗_生化.三酸甘油脂_TG = worksheet抽血檢驗_生化.Range["B9"].Value;
        抽血檢驗_生化.醣化血紅素_HbA1C = worksheet抽血檢驗_生化.Range["B1"].Value;

        #region 新版 抽血檢驗_血液生化特殊Model
        var 抽血檢驗_生化Items = CTMSModel.抽血檢驗_生化Items;
        抽血檢驗_生化Items.Clear();
        抽血檢驗_生化Items.Add(new 抽血檢驗_血液生化特殊Model()
        {
            檢驗項目 = "血中尿素氮(BUN)",
            參考區間 = "6-20",
            參考區間開始 = 6,
            參考區間結束 = 20,
            參考區間類型 = MagicObjectHelper.參考區間類型_區間,
            檢驗數據 = worksheet抽血檢驗_生化.Range["B16"].Value
        });
        抽血檢驗_生化Items.Add(new 抽血檢驗_血液生化特殊Model()
        {
            檢驗項目 = "天門冬胺酸轉胺酶(AST)",
            參考區間 = "10-50",
            參考區間開始 = 10,
            參考區間結束 = 50,
            參考區間類型 = MagicObjectHelper.參考區間類型_區間,
            檢驗數據 = worksheet抽血檢驗_生化.Range["B2"].Value
        });
        抽血檢驗_生化Items.Add(new 抽血檢驗_血液生化特殊Model()
        {
            檢驗項目 = "丙胺酸轉胺酶(ALT)",
            參考區間 = "≦50",
            參考區間開始 = 50,
            參考區間結束 = 50,
            參考區間類型 = MagicObjectHelper.參考區間類型_小於等於,
            檢驗數據 = worksheet抽血檢驗_生化.Range["B3"].Value
        });
        抽血檢驗_生化Items.Add(new 抽血檢驗_血液生化特殊Model()
        {
            檢驗項目 = "膽紅素總量(Total Bilirubin)",
            參考區間 = "≦1.2",
            參考區間開始 = 1.2,
            參考區間結束 = 1.2,
            參考區間類型 = MagicObjectHelper.參考區間類型_小於等於,
            檢驗數據 = worksheet抽血檢驗_生化.Range["B4"].Value
        });
        抽血檢驗_生化Items.Add(new 抽血檢驗_血液生化特殊Model()
        {
            檢驗項目 = "白蛋白/球蛋白 (A/G)",
            參考區間 = "",
            參考區間開始 = 0,
            參考區間結束 = 0,
            參考區間類型 = MagicObjectHelper.參考區間類型_區間,
            檢驗數據 = worksheet抽血檢驗_生化.Range["B5"].Value
        });
        抽血檢驗_生化Items.Add(new 抽血檢驗_血液生化特殊Model()
        {
            檢驗項目 = "總膽固醇(CHOL)",
            參考區間 = "<200",
            參考區間開始 = 200,
            參考區間結束 = 200,
            參考區間類型 = MagicObjectHelper.參考區間類型_小於,
            檢驗數據 = worksheet抽血檢驗_生化.Range["B6"].Value
        });
        抽血檢驗_生化Items.Add(new 抽血檢驗_血液生化特殊Model()
        {
            檢驗項目 = "高密度脂蛋白膽固醇(HDL)",
            參考區間 = ">40",
            參考區間開始 = 40,
            參考區間結束 = 40,
            參考區間類型 = MagicObjectHelper.參考區間類型_大於,
            檢驗數據 = worksheet抽血檢驗_生化.Range["B7"].Value
        });
        抽血檢驗_生化Items.Add(new 抽血檢驗_血液生化特殊Model()
        {
            檢驗項目 = "低密度脂蛋白膽固醇(LDL)",
            參考區間 = "<100",
            參考區間開始 = 100,
            參考區間結束 = 100,
            參考區間類型 = MagicObjectHelper.參考區間類型_小於,
            檢驗數據 = worksheet抽血檢驗_生化.Range["B8"].Value
        });
        抽血檢驗_生化Items.Add(new 抽血檢驗_血液生化特殊Model()
        {
            檢驗項目 = "三酸甘油脂(TG)",
            參考區間 = "<150",
            參考區間開始 = 150,
            參考區間結束 = 150,
            參考區間類型 = MagicObjectHelper.參考區間類型_小於,
            檢驗數據 = worksheet抽血檢驗_生化.Range["B9"].Value
        });
        抽血檢驗_生化Items.Add(new 抽血檢驗_血液生化特殊Model()
        {
            檢驗項目 = "醣化血紅素(HbA1C)",
            參考區間 = "4.8-5.9",
            參考區間開始 = 4.8,
            參考區間結束 = 5.9,
            參考區間類型 = MagicObjectHelper.參考區間類型_區間,
            檢驗數據 = worksheet抽血檢驗_生化.Range["B1"].Value
        });
        計算血液檢查的異常(抽血檢驗_生化Items);
        #endregion
    }

    private void Collect抽血檢驗_特殊20241212FromExcel(NextGenerationSportsCTMSModel CTMSModel, IWorkbook workbook)
    {
        IWorksheet worksheet抽血檢驗_特殊 = workbook
            .Worksheets[MagicObjectHelper.抽血檢驗_特殊原始檔20241212WorkSheetName];

        var 抽血檢驗_特殊 = CTMSModel.抽血檢驗_特殊;

        #region 運動前
        var item = 抽血檢驗_特殊.運動前;
        item.肌酸酐_CR = worksheet抽血檢驗_特殊.Range["B1"].Value;
        item.乳酸脫氫酶_LDH = worksheet抽血檢驗_特殊.Range["B2"].Value;
        item.葡萄糖_Glucose = worksheet抽血檢驗_特殊.Range["B3"].Value;
        item.鹼性磷酸酶_ALKP = worksheet抽血檢驗_特殊.Range["B4"].Value;
        item.醣化血紅素_HbA1C = worksheet抽血檢驗_特殊.Range["B5"].Value;
        item.高敏感C反應性蛋白質_HS_CRP = worksheet抽血檢驗_特殊.Range["B6"].Value;
        item.結合蛋白_Haptoglobin = worksheet抽血檢驗_特殊.Range["B7"].Value;
        item.血清前白蛋白_Prealbumin = worksheet抽血檢驗_特殊.Range["B8"].Value;
        item.胰島素_Insulin = worksheet抽血檢驗_特殊.Range["B9"].Value;
        item.類胰島素成長因子_IGF_1 = worksheet抽血檢驗_特殊.Range["B10"].Value;
        item.C_胜鏈胰島素_C_peptide = worksheet抽血檢驗_特殊.Range["B11"].Value;
        item.介白素_6_IL_6 = worksheet抽血檢驗_特殊.Range["B12"].Value;
        item.皮質醇_Cortisol = worksheet抽血檢驗_特殊.Range["B13"].Value;
        #endregion

        #region 運動後15分鐘
        item = 抽血檢驗_特殊.運動後15分鐘;
        item.肌酸酐_CR = worksheet抽血檢驗_特殊.Range["C1"].Value;
        item.乳酸脫氫酶_LDH = worksheet抽血檢驗_特殊.Range["C2"].Value;
        item.葡萄糖_Glucose = worksheet抽血檢驗_特殊.Range["C3"].Value;
        item.鹼性磷酸酶_ALKP = worksheet抽血檢驗_特殊.Range["C4"].Value;
        item.醣化血紅素_HbA1C = worksheet抽血檢驗_特殊.Range["C5"].Value;
        item.高敏感C反應性蛋白質_HS_CRP = worksheet抽血檢驗_特殊.Range["C6"].Value;
        item.結合蛋白_Haptoglobin = worksheet抽血檢驗_特殊.Range["C7"].Value;
        item.血清前白蛋白_Prealbumin = worksheet抽血檢驗_特殊.Range["C8"].Value;
        item.胰島素_Insulin = worksheet抽血檢驗_特殊.Range["C9"].Value;
        item.類胰島素成長因子_IGF_1 = worksheet抽血檢驗_特殊.Range["C10"].Value;
        item.C_胜鏈胰島素_C_peptide = worksheet抽血檢驗_特殊.Range["C11"].Value;
        item.介白素_6_IL_6 = worksheet抽血檢驗_特殊.Range["C12"].Value;
        item.皮質醇_Cortisol = worksheet抽血檢驗_特殊.Range["C13"].Value;
        #endregion

        #region 運動後30分鐘
        item = 抽血檢驗_特殊.運動後30分鐘;
        item.肌酸酐_CR = worksheet抽血檢驗_特殊.Range["D1"].Value;
        item.乳酸脫氫酶_LDH = worksheet抽血檢驗_特殊.Range["D2"].Value;
        item.葡萄糖_Glucose = worksheet抽血檢驗_特殊.Range["D3"].Value;
        item.鹼性磷酸酶_ALKP = worksheet抽血檢驗_特殊.Range["D4"].Value;
        item.醣化血紅素_HbA1C = worksheet抽血檢驗_特殊.Range["D5"].Value;
        item.高敏感C反應性蛋白質_HS_CRP = worksheet抽血檢驗_特殊.Range["D6"].Value;
        item.結合蛋白_Haptoglobin = worksheet抽血檢驗_特殊.Range["D7"].Value;
        item.血清前白蛋白_Prealbumin = worksheet抽血檢驗_特殊.Range["D8"].Value;
        item.胰島素_Insulin = worksheet抽血檢驗_特殊.Range["D9"].Value;
        item.類胰島素成長因子_IGF_1 = worksheet抽血檢驗_特殊.Range["D10"].Value;
        item.C_胜鏈胰島素_C_peptide = worksheet抽血檢驗_特殊.Range["D11"].Value;
        item.介白素_6_IL_6 = worksheet抽血檢驗_特殊.Range["D12"].Value;
        item.皮質醇_Cortisol = worksheet抽血檢驗_特殊.Range["D13"].Value;
        #endregion

        #region 新版 抽血檢驗_血液生化特殊Model
        var 抽血檢驗_特殊Items = CTMSModel.抽血檢驗_特殊Items;
        抽血檢驗_特殊Items.Clear();
        抽血檢驗_特殊Items.Add(new 抽血檢驗_血液生化特殊Model()
        {
            檢驗項目 = "肌酸酐(Creatinine)",
            參考區間 = "0.7-1.2",
            參考區間開始 = 0.7,
            參考區間結束 = 1.2,
            參考區間類型 = MagicObjectHelper.參考區間類型_區間,
            運動前 = worksheet抽血檢驗_特殊.Range["B1"].Value,
            運動後15分鐘 = worksheet抽血檢驗_特殊.Range["C1"].Value,
            運動後30分鐘 = worksheet抽血檢驗_特殊.Range["D1"].Value
        });
        抽血檢驗_特殊Items.Add(new 抽血檢驗_血液生化特殊Model()
        {
            檢驗項目 = "腎絲球過濾率(eGFR)",
            參考區間 = ">60",
            參考區間開始 = 60,
            參考區間結束 = 60,
            參考區間類型 = MagicObjectHelper.參考區間類型_大於,
            運動前 = worksheet抽血檢驗_特殊.Range["B2"].Value,
            運動後15分鐘 = worksheet抽血檢驗_特殊.Range["C2"].Value,
            運動後30分鐘 = worksheet抽血檢驗_特殊.Range["D2"].Value
        });
        抽血檢驗_特殊Items.Add(new 抽血檢驗_血液生化特殊Model()
        {
            檢驗項目 = "乳酸脫氫酶(LDH)",
            參考區間 = "135-225",
            參考區間開始 = 135,
            參考區間結束 = 225,
            參考區間類型 = MagicObjectHelper.參考區間類型_區間,
            運動前 = worksheet抽血檢驗_特殊.Range["B3"].Value,
            運動後15分鐘 = worksheet抽血檢驗_特殊.Range["C3"].Value,
            運動後30分鐘 = worksheet抽血檢驗_特殊.Range["D3"].Value
        });
        抽血檢驗_特殊Items.Add(new 抽血檢驗_血液生化特殊Model()
        {
            檢驗項目 = "葡萄糖(Glucose)",
            參考區間 = "60-99",
            參考區間開始 = 60,
            參考區間結束 = 99,
            參考區間類型 = MagicObjectHelper.參考區間類型_區間,
            運動前 = worksheet抽血檢驗_特殊.Range["B4"].Value,
            運動後15分鐘 = worksheet抽血檢驗_特殊.Range["C4"].Value,
            運動後30分鐘 = worksheet抽血檢驗_特殊.Range["D4"].Value
        });
        抽血檢驗_特殊Items.Add(new 抽血檢驗_血液生化特殊Model()
        {
            檢驗項目 = "鹼性磷酸酶(ALKP)",
            參考區間 = "40-129",
            參考區間開始 = 40,
            參考區間結束 = 129,
            參考區間類型 = MagicObjectHelper.參考區間類型_區間,
            運動前 = worksheet抽血檢驗_特殊.Range["B5"].Value,
            運動後15分鐘 = worksheet抽血檢驗_特殊.Range["C5"].Value,
            運動後30分鐘 = worksheet抽血檢驗_特殊.Range["D5"].Value
        });
        //抽血檢驗_特殊Items.Add(new 抽血檢驗_血液生化特殊Model()
        //{
        //    檢驗項目 = "醣化血紅素(HbA1C)",
        //    參考區間 = "4.8-5.9",
        //    參考區間開始 = 4.8,
        //    參考區間結束 = 5.9,
        //    參考區間類型 = MagicObjectHelper.參考區間類型_區間,
        //    運動前 = worksheet抽血檢驗_特殊.Range["B5"].Value,
        //    運動後15分鐘 = worksheet抽血檢驗_特殊.Range["C5"].Value,
        //    運動後30分鐘 = worksheet抽血檢驗_特殊.Range["D5"].Value
        //});
        抽血檢驗_特殊Items.Add(new 抽血檢驗_血液生化特殊Model()
        {
            檢驗項目 = "高敏感C反應性蛋白質(HS-CRP)",
            參考區間 = "<7.4",
            參考區間開始 = 7.4,
            參考區間結束 = 7.4,
            參考區間類型 = MagicObjectHelper.參考區間類型_小於,
            運動前 = worksheet抽血檢驗_特殊.Range["B7"].Value,
            運動後15分鐘 = worksheet抽血檢驗_特殊.Range["C7"].Value,
            運動後30分鐘 = worksheet抽血檢驗_特殊.Range["D7"].Value
        });
        抽血檢驗_特殊Items.Add(new 抽血檢驗_血液生化特殊Model()
        {
            檢驗項目 = "結合蛋白(Haptoglobin)",
            參考區間 = "36-195",
            參考區間開始 = 36,
            參考區間結束 = 195,
            參考區間類型 = MagicObjectHelper.參考區間類型_區間,
            運動前 = worksheet抽血檢驗_特殊.Range["B8"].Value,
            運動後15分鐘 = worksheet抽血檢驗_特殊.Range["C8"].Value,
            運動後30分鐘 = worksheet抽血檢驗_特殊.Range["D8"].Value
        });
        抽血檢驗_特殊Items.Add(new 抽血檢驗_血液生化特殊Model()
        {
            檢驗項目 = "血清前白蛋白(Prealbumin)",
            參考區間 = "18-38",
            參考區間開始 = 18,
            參考區間結束 = 38,
            參考區間類型 = MagicObjectHelper.參考區間類型_區間,
            運動前 = worksheet抽血檢驗_特殊.Range["B9"].Value,
            運動後15分鐘 = worksheet抽血檢驗_特殊.Range["C9"].Value,
            運動後30分鐘 = worksheet抽血檢驗_特殊.Range["D9"].Value
        });
        抽血檢驗_特殊Items.Add(new 抽血檢驗_血液生化特殊Model()
        {
            檢驗項目 = "胰島素(Insulin)",
            參考區間 = "1.21-14.5",
            參考區間開始 = 1.21,
            參考區間結束 = 14.5,
            參考區間類型 = MagicObjectHelper.參考區間類型_區間,
            運動前 = worksheet抽血檢驗_特殊.Range["B10"].Value,
            運動後15分鐘 = worksheet抽血檢驗_特殊.Range["C10"].Value,
            運動後30分鐘 = worksheet抽血檢驗_特殊.Range["D10"].Value
        });
        抽血檢驗_特殊Items.Add(new 抽血檢驗_血液生化特殊Model()
        {
            檢驗項目 = "類胰島素成長因子(IGF-1)",
            參考區間 = "135-328",
            參考區間開始 = 135,
            參考區間結束 = 328,
            參考區間類型 = MagicObjectHelper.參考區間類型_區間,
            運動前 = worksheet抽血檢驗_特殊.Range["B11"].Value,
            運動後15分鐘 = worksheet抽血檢驗_特殊.Range["C11"].Value,
            運動後30分鐘 = worksheet抽血檢驗_特殊.Range["D11"].Value
        });
        抽血檢驗_特殊Items.Add(new 抽血檢驗_血液生化特殊Model()
        {
            檢驗項目 = "C-胜鏈胰島素(C-peptide)",
            參考區間 = "1.1-4.4",
            參考區間開始 = 1.1,
            參考區間結束 = 4.4,
            參考區間類型 = MagicObjectHelper.參考區間類型_區間,
            運動前 = worksheet抽血檢驗_特殊.Range["B12"].Value,
            運動後15分鐘 = worksheet抽血檢驗_特殊.Range["C12"].Value,
            運動後30分鐘 = worksheet抽血檢驗_特殊.Range["D12"].Value
        });
        抽血檢驗_特殊Items.Add(new 抽血檢驗_血液生化特殊Model()
        {
            檢驗項目 = "介白素-6(IL-6)",
            參考區間 = "<7.0",
            參考區間開始 = 7.0,
            參考區間結束 = 7.0,
            參考區間類型 = MagicObjectHelper.參考區間類型_小於,
            運動前 = worksheet抽血檢驗_特殊.Range["B13"].Value,
            運動後15分鐘 = worksheet抽血檢驗_特殊.Range["C13"].Value,
            運動後30分鐘 = worksheet抽血檢驗_特殊.Range["D13"].Value
        });
        抽血檢驗_特殊Items.Add(new 抽血檢驗_血液生化特殊Model()
        {
            檢驗項目 = "皮質醇(Cortisol)",
            參考區間 = "4.82-19.5",
            參考區間開始 = 4.82,
            參考區間結束 = 19.5,
            參考區間類型 = MagicObjectHelper.參考區間類型_區間,
            運動前 = worksheet抽血檢驗_特殊.Range["B14"].Value,
            運動後15分鐘 = worksheet抽血檢驗_特殊.Range["C14"].Value,
            運動後30分鐘 = worksheet抽血檢驗_特殊.Range["D14"].Value
        });
        計算血液特殊檢查的異常(抽血檢驗_特殊Items);

        #endregion
    }
    private void Collect綜合評估建議20241212FromExcel(NextGenerationSportsCTMSModel CTMSModel, IWorkbook workbook)
    {
        IWorksheet worksheetCT身體組成 = workbook
            .Worksheets[MagicObjectHelper.CT身體組成原始擋20241212WorkSheetName];
        IWorksheet worksheet心肺功能 = workbook
            .Worksheets[MagicObjectHelper.心肺功能原始擋20241212WorkSheetName];

        var Recommendation綜合評估建議 = CTMSModel.ComprehensiveAssessmentRecommendation綜合評估建議Model;

        Recommendation綜合評估建議.VisceralFatPercentile內臟脂肪百分位 =
            worksheetCT身體組成.Range["C36"].Value;
        Recommendation綜合評估建議.SubcutaneousFatPercentile皮下脂肪百分位 =
            worksheetCT身體組成.Range["H36"].Value;
        Recommendation綜合評估建議.MetabolicDisorderRisk代謝失調風險 =
            worksheetCT身體組成.Range["Y22"].Value;
        Recommendation綜合評估建議.WaistCircumferencePercentile腰圍百分位 =
            worksheetCT身體組成.Range["B41"].Value;
        double AB22 = worksheetCT身體組成.Range["AB22"].Value.ToDouble();
        Recommendation綜合評估建議.MuscleHealth肌肉健康度 = (100 - AB22).ToString("F0");
        Recommendation綜合評估建議.核心肌群校正肌力 =
            worksheetCT身體組成.Range["F22"].Value;
        Recommendation綜合評估建議.心肺能力 =
            worksheet心肺功能.Range["B15"].Value;

        #region 核心均衡力
        // 這六個數值平均
        // F18*80%+F20*10%+F21*10%
        // F17*20%+F18*20%+F19*20%+F20*10%+F21*30%
        // F17*60%+F18*40%
        // F20*10%+F21*90%
        // F18*50%+F20*50%
        // F19*100%

        double F17 = worksheetCT身體組成.Range["F17"].Value.ToDouble();
        double F18 = worksheetCT身體組成.Range["F18"].Value.ToDouble();
        double F19 = worksheetCT身體組成.Range["F19"].Value.ToDouble();
        double F20 = worksheetCT身體組成.Range["F20"].Value.ToDouble();
        double F21 = worksheetCT身體組成.Range["F21"].Value.ToDouble();
        var v1 = (F18 * 0.8 + F20 * 0.1 + F21 * 0.1);
        var v2 = (F17 * 0.2 + F18 * 0.2 + F19 * 0.2 + F20 * 0.1 + F21 * 0.3);
        var v3 = (F17 * 0.6 + F18 * 0.4);
        var v4 = (F20 * 0.1 + F21 * 0.9);
        var v5 = (F18 * 0.5 + F20 * 0.5);
        var v6 = (F19 * 1.0);
        var avg = (v1 + v2 + v3 + v4 + v5 + v6) / 6.0;
        string avgStr = avg.ToString("F0");
        Recommendation綜合評估建議.CoreBalance核心均衡力 = avgStr;

        #endregion
    }

    #endregion

    #region 第一版的需求
    private void Collect首頁FromExcel(NextGenerationSportsCTMSModel CTMSModel, IWorkbook workbook)
    {
        IWorksheet worksheetCT身體組成 = workbook
            .Worksheets[MagicObjectHelper.CT身體組成WorkSheetName];
        IWorksheet worksheetBIA身體組成 = workbook
            .Worksheets[MagicObjectHelper.BIA身體組成WorkSheetName];

        CTMSModel.HomePageModel.姓名 = worksheetCT身體組成.Range["B2"].Value;
        CTMSModel.HomePageModel.性別 = worksheetCT身體組成.Range["E2"].Value;
        CTMSModel.HomePageModel.年齡 = worksheetCT身體組成.Range["D2"].Value;
        CTMSModel.HomePageModel.身高 = worksheetBIA身體組成.Range["C2"].Value;
        CTMSModel.HomePageModel.體重 = worksheetBIA身體組成.Range["C3"].Value;
        CTMSModel.HomePageModel.BMI = worksheetBIA身體組成.Range["C5"].Value;
    }

    private void Collect身體組成_肌肉質量FromExcel(NextGenerationSportsCTMSModel CTMSModel, IWorkbook workbook)
    {
        IWorksheet worksheetCT身體組成 = workbook
            .Worksheets[MagicObjectHelper.CT身體組成WorkSheetName];
        IWorksheet worksheetBIA身體組成 = workbook
            .Worksheets[MagicObjectHelper.BIA身體組成WorkSheetName];

        var MuscleMassModel肌肉質量 = CTMSModel.BodyMuscleMassModel肌肉質量Model;

        MuscleMassModel肌肉質量.Waistline腰圍 = worksheetCT身體組成.Range["B40"].Value.ToDouble().ToString("F1");
        MuscleMassModel肌肉質量.VertebralBody椎體 = worksheetCT身體組成.Range["B45"].Value
            .ToDouble().ToString("F1");

        #region 計算Skeleton骨架
        var Skeleton骨架 = worksheetCT身體組成.Range["B46"].Value;
        int Skeleton骨架Int = 0;
        int.TryParse(Skeleton骨架, out Skeleton骨架Int);
        if (Skeleton骨架Int <= 34)
        {
            MuscleMassModel肌肉質量.Skeleton骨架 = "小";
        }
        else if (Skeleton骨架Int > 34 && Skeleton骨架Int <= 67)
        {
            MuscleMassModel肌肉質量.Skeleton骨架 = "中";
        }
        else
        {
            MuscleMassModel肌肉質量.Skeleton骨架 = "大";
        }
        #endregion

        MuscleMassModel肌肉質量.Area面積 = worksheetCT身體組成.Range["B13"].Value.ToDouble().ToString("F1");
        MuscleMassModel肌肉質量.Density密度 = worksheetCT身體組成.Range["C13"].Value.ToDouble().ToString("F1");
        MuscleMassModel肌肉質量.Index指標 = worksheetCT身體組成.Range["E13"].Value.ToDouble().ToString("F1");
        MuscleMassModel肌肉質量.CoreMuscleMass核心肌群肌肉量 =
            worksheetCT身體組成.Range["B22"].Value;
        MuscleMassModel肌肉質量.CoreMuscleEndurance核心肌群肌肉品質 =
            worksheetCT身體組成.Range["C22"].Value;
        MuscleMassModel肌肉質量.CorrectCoreMuscleStrength校正肌力 =
            worksheetCT身體組成.Range["D22"].Value;

        #region 雷達圖的數據
        var CoreMuscleMass核心肌群肌肉量Radar = MuscleMassModel肌肉質量.CoreMuscleMass核心肌群肌肉量RadarChartModel;
        CoreMuscleMass核心肌群肌肉量Radar.RectusAbdominis腹直肌 =
            worksheetCT身體組成.Range["B17"].Value.ToDouble().ToString("F1");
        CoreMuscleMass核心肌群肌肉量Radar.InternalObliqueExternalObliqueTransversusAbdominis內斜肌_外斜肌_腹橫肌 =
            worksheetCT身體組成.Range["B18"].Value.ToDouble().ToString("F1");
        CoreMuscleMass核心肌群肌肉量Radar.PsoasMuscle腰肌 =
            worksheetCT身體組成.Range["B19"].Value.ToDouble().ToString("F1");
        CoreMuscleMass核心肌群肌肉量Radar.QuadratusLumborum腰方肌 =
            worksheetCT身體組成.Range["B20"].Value.ToDouble().ToString("F1");
        CoreMuscleMass核心肌群肌肉量Radar.ErectorSpinaeMultifidus豎脊肌_多裂肌 =
            worksheetCT身體組成.Range["B21"].Value.ToDouble().ToString("F1");

        var CoreMuscleEndurance核心肌群肌耐力Radar = MuscleMassModel肌肉質量.CoreMuscleEndurance核心肌群肌耐力RadarChartModel;
        CoreMuscleEndurance核心肌群肌耐力Radar.RectusAbdominis腹直肌 =
            worksheetCT身體組成.Range["C17"].Value.ToDouble().ToString("F1");
        CoreMuscleEndurance核心肌群肌耐力Radar.InternalObliqueExternalObliqueTransversusAbdominis內斜肌_外斜肌_腹橫肌 =
            worksheetCT身體組成.Range["C18"].Value.ToDouble().ToString("F1");
        CoreMuscleEndurance核心肌群肌耐力Radar.PsoasMuscle腰肌 =
            worksheetCT身體組成.Range["C19"].Value.ToDouble().ToString("F1");
        CoreMuscleEndurance核心肌群肌耐力Radar.QuadratusLumborum腰方肌 =
            worksheetCT身體組成.Range["C20"].Value.ToDouble().ToString("F1");
        CoreMuscleEndurance核心肌群肌耐力Radar.ErectorSpinaeMultifidus豎脊肌_多裂肌 =
            worksheetCT身體組成.Range["C21"].Value.ToDouble().ToString("F1");

        var CorrectCoreMuscleStrength核心肌群校正肌力強度Radar = MuscleMassModel肌肉質量.CorrectCoreMuscleStrength核心肌群校正肌力強度RadarChartModel;
        CorrectCoreMuscleStrength核心肌群校正肌力強度Radar.RectusAbdominis腹直肌 =
            worksheetCT身體組成.Range["D17"].Value.ToDouble().ToString("F1");
        CorrectCoreMuscleStrength核心肌群校正肌力強度Radar.InternalObliqueExternalObliqueTransversusAbdominis內斜肌_外斜肌_腹橫肌 =
            worksheetCT身體組成.Range["D18"].Value.ToDouble().ToString("F1");
        CorrectCoreMuscleStrength核心肌群校正肌力強度Radar.PsoasMuscle腰肌 =
            worksheetCT身體組成.Range["D19"].Value.ToDouble().ToString("F1");
        CorrectCoreMuscleStrength核心肌群校正肌力強度Radar.QuadratusLumborum腰方肌 =
            worksheetCT身體組成.Range["D20"].Value.ToDouble().ToString("F1");
        CorrectCoreMuscleStrength核心肌群校正肌力強度Radar.ErectorSpinaeMultifidus豎脊肌_多裂肌 =
            worksheetCT身體組成.Range["D21"].Value.ToDouble().ToString("F1");

        #endregion
    }

    private void Collect身體組成_動作分析FromExcel(NextGenerationSportsCTMSModel CTMSModel, IWorkbook workbook)
    {
        IWorksheet worksheetCT身體組成 = workbook
            .Worksheets[MagicObjectHelper.CT身體組成WorkSheetName];
        IWorksheet worksheetBIA身體組成 = workbook
            .Worksheets[MagicObjectHelper.BIA身體組成WorkSheetName];

        var MotionAnalysis動作分析 = CTMSModel.BodyMotionAnalysis動作分析Model;
        double F18 = worksheetCT身體組成.Range["F18"].Value.ToDouble();
        double D18 = worksheetCT身體組成.Range["D18"].Value.ToDouble();
        double C18 = worksheetCT身體組成.Range["C18"].Value.ToDouble();
        // F18*100%
        MotionAnalysis動作分析.TorsoRotation軀幹旋轉 = (F18 * 1.0).ToString("F0");

        double F17 = worksheetCT身體組成.Range["F17"].Value.ToDouble();
        double F20 = worksheetCT身體組成.Range["F20"].Value.ToDouble();
        double F21 = worksheetCT身體組成.Range["F21"].Value.ToDouble();
        double D20 = worksheetCT身體組成.Range["D20"].Value.ToDouble();
        double D21 = worksheetCT身體組成.Range["D21"].Value.ToDouble();
        double C17 = worksheetCT身體組成.Range["C17"].Value.ToDouble();
        double D17 = worksheetCT身體組成.Range["D17"].Value.ToDouble();
        double C20 = worksheetCT身體組成.Range["C20"].Value.ToDouble();
        double C21 = worksheetCT身體組成.Range["C21"].Value.ToDouble();
        //F17*20%+F18*20%+F20*20%+F21*40%
        MotionAnalysis動作分析.TrunkStability軀幹穩定 =
            (F17 * 0.2 + F18 * 0.2 + F20 * 0.2 + F21 * 0.4).ToString("F0");

        //F17*60%+F18*40%
        MotionAnalysis動作分析.ForwardBendOfTrunk軀幹前彎 =
            (F17 * 0.6 + F18 * 0.4).ToString("F0");

        //F21*100%
        MotionAnalysis動作分析.TrunkStretch軀幹伸展 =
            (F21 * 1.0).ToString("F0");

        //F18*66%+F20*34%
        MotionAnalysis動作分析.SideCurvatureOfTrunk軀幹側彎 =
            (F18 * 0.66 + F20 * 0.34).ToString("F0");

        #region 肌力強度 雷達圖
        var MuscleStrength肌力強 = MotionAnalysis動作分析.MuscleStrength肌力強度RadarChartModel;
        //D18*100%	
        MuscleStrength肌力強.Rotate旋轉 = (D18 * 1).ToString("F1");
        //D17*20%+D18*20%+D20*20%+D21*40%	
        MuscleStrength肌力強.Stablize穩定 =
            (D17 * 0.2 + D18 * 0.2 + D20 * 0.2 + D21 * 0.4).ToString("F1");
        //D17*60%+D18*40%	
        MuscleStrength肌力強.BendForward前彎 = (D17 * 0.6 + D18 * 0.4).ToString("F1");
        //D21*100%
        MuscleStrength肌力強.Stretch伸展 = (D21 * 1).ToString("F1");
        //D18*66%+D20*34%
        MuscleStrength肌力強.SideBend側彎 = (D18 * 0.66 + D20 * 0.34).ToString("F1");
        #endregion

        #region 肌耐力 雷達圖
        var MuscularEndurance肌耐力 = MotionAnalysis動作分析.MuscularEndurance肌耐力RadarChartModel;
        //C18*100% 
        MuscularEndurance肌耐力.Rotate旋轉 = (C18 * 1.0).ToString("F1");
        //C17*20%+C18*20%+C20*20%+C21*40% 
        MuscularEndurance肌耐力.Stablize穩定 = (C17 * 0.2 + C18 * 0.2 + C20 * 0.2 + C21 * 0.4).ToString("F1");
        //C17*60%+C18*40% 
        MuscularEndurance肌耐力.BendForward前彎 = (C17 * 0.6 + C18 * 0.4).ToString("F1");
        //C21*100%
        MuscularEndurance肌耐力.Stretch伸展 = (C21 * 1.0).ToString("F1");
        //C18*66%+C20*34%	
        MuscularEndurance肌耐力.SideBend側彎 = (C18 * 0.66 + C20 * 0.34).ToString("F1");
        #endregion

        #region 肌肉綜合表現 雷達圖
        var OverallMusclePerformance肌肉綜合表現 = MotionAnalysis動作分析.OverallMusclePerformance肌肉綜合表現RadarChartModel;
        //F18*100%
        OverallMusclePerformance肌肉綜合表現.Rotate旋轉 = Math.Sqrt((D18 * 1.0) * (C18 * 1.0)).ToString("F1");
        //F17*20%+F18*20%+F20*20%+F21*40%
        OverallMusclePerformance肌肉綜合表現.Stablize穩定 =
                    (F17 * 0.2 + F18 * 0.2 + F20 * 0.2 + F21 * 0.4).ToString("F1");
        //F17*60%+F18*40% 
        OverallMusclePerformance肌肉綜合表現.BendForward前彎 =
            (F17 * 0.6 + F18 * 0.4).ToString("F1");
        //F21*100%
        OverallMusclePerformance肌肉綜合表現.Stretch伸展 = (F21 * 1.0).ToString("F1");
        //F18*66%+F20*34%
        OverallMusclePerformance肌肉綜合表現.SideBend側彎 =
            (F18 * 0.66 + F20 * 0.34).ToString("F1");
        #endregion
    }

    private void Collect身體組成_肌肉品質FromExcel(NextGenerationSportsCTMSModel CTMSModel, IWorkbook workbook)
    {
        IWorksheet worksheetCT身體組成 = workbook
            .Worksheets[MagicObjectHelper.CT身體組成WorkSheetName];
        IWorksheet worksheetBIA身體組成 = workbook
            .Worksheets[MagicObjectHelper.BIA身體組成WorkSheetName];
        var BodyMuscleQuality肌肉品質 = CTMSModel.BodyMuscleQuality肌肉品質Model;
        BodyMuscleQuality肌肉品質.Skewness偏度 = worksheetCT身體組成.Range["B31"].Value.ToDouble().ToString("F1"); ;
        BodyMuscleQuality肌肉品質.Kurtosis峰度 = worksheetCT身體組成.Range["C31"].Value.ToDouble().ToString("F1"); ;
        #region 健康肌肉比
        double S13 = worksheetCT身體組成.Range["S13"].Value.ToDouble();
        double I13 = worksheetCT身體組成.Range["I13"].Value.ToDouble();
        double N13 = worksheetCT身體組成.Range["N13"].Value.ToDouble();
        //S13/(I13+N13+S13)
        BodyMuscleQuality肌肉品質.HealthyMuscleRatio健康肌肉比 = (S13 / (I13 + N13 + S13) * 100.0).ToString("F1") + "%";
        #endregion
        BodyMuscleQuality肌肉品質.HighQualityMuscleHealthScore高品質肌肉健康度 =
            worksheetCT身體組成.Range["AC22"].Value;

        #region 肌肉健康度
        double AB22 = worksheetCT身體組成.Range["AB22"].Value.ToDouble();
        // 100-AB22
        BodyMuscleQuality肌肉品質.MuscleHealthScore肌肉健康度 = (100.0 - AB22).ToString("F0");
        #endregion

        #region 肌肉健康度 雷達圖
        var MuscleHealthScore肌肉健康度 = BodyMuscleQuality肌肉品質.MuscleHealthScore肌肉健康度RadarChartModel;
        double AB17 = worksheetCT身體組成.Range["AB17"].Value.ToDouble();
        double AB18 = worksheetCT身體組成.Range["AB18"].Value.ToDouble();
        double AB19 = worksheetCT身體組成.Range["AB19"].Value.ToDouble();
        double AB20 = worksheetCT身體組成.Range["AB20"].Value.ToDouble();
        double AB21 = worksheetCT身體組成.Range["AB21"].Value.ToDouble();
        //100 - AB17
        MuscleHealthScore肌肉健康度.RectusAbdominis腹直肌 = (100 - AB17).ToString("F1");
        //100 - AB18
        MuscleHealthScore肌肉健康度.InternalObliqueExternalObliqueTransversusAbdominis內斜肌_外斜肌_腹橫肌 =
            (100 - AB18).ToString("F1");
        //100 - AB19
        MuscleHealthScore肌肉健康度.PsoasMuscle腰肌 = (100 - AB19).ToString("F1");
        //100 - AB20
        MuscleHealthScore肌肉健康度.QuadratusLumborum腰方肌 = (100 - AB20).ToString("F1");
        //100 - AB21
        MuscleHealthScore肌肉健康度.ErectorSpinaeMultifidus豎脊肌_多裂肌 = (100 - AB21).ToString("F1");
        #endregion

        #region 高品質肌肉健康度 雷達圖
        var HighQualityMuscleHealthScore高品質肌肉健康度 = BodyMuscleQuality肌肉品質.HighQualityMuscleHealthScore高品質肌肉健康度RadarChartModel;
        double AC17 = worksheetCT身體組成.Range["AC17"].Value.ToDouble();
        double AC18 = worksheetCT身體組成.Range["AC18"].Value.ToDouble();
        double AC19 = worksheetCT身體組成.Range["AC19"].Value.ToDouble();
        double AC20 = worksheetCT身體組成.Range["AC20"].Value.ToDouble();
        double AC21 = worksheetCT身體組成.Range["AC21"].Value.ToDouble();
        //AC17
        HighQualityMuscleHealthScore高品質肌肉健康度.RectusAbdominis腹直肌 = AC17.ToString("F1");
        //AC18
        HighQualityMuscleHealthScore高品質肌肉健康度.InternalObliqueExternalObliqueTransversusAbdominis內斜肌_外斜肌_腹橫肌 =
            AC18.ToString("F1");
        //AC19
        HighQualityMuscleHealthScore高品質肌肉健康度.PsoasMuscle腰肌 = AC19.ToString("F1");
        //AC20
        HighQualityMuscleHealthScore高品質肌肉健康度.QuadratusLumborum腰方肌 = AC20.ToString("F1");
        //AC21
        HighQualityMuscleHealthScore高品質肌肉健康度.ErectorSpinaeMultifidus豎脊肌_多裂肌 =
            AC21.ToString("F1");
        #endregion
    }

    private void Collect身體組成_脂肪分析FromExcel(NextGenerationSportsCTMSModel CTMSModel, IWorkbook workbook)
    {
        IWorksheet worksheetCT身體組成 = workbook
            .Worksheets[MagicObjectHelper.CT身體組成WorkSheetName];
        IWorksheet worksheetBIA身體組成 = workbook
            .Worksheets[MagicObjectHelper.BIA身體組成WorkSheetName];
        var BodyFatAnalysis脂肪分析 = CTMSModel.BodyFatAnalysis脂肪分析Model;

        BodyFatAnalysis脂肪分析.VisceralFatPercentile內臟脂肪百分位 =
            worksheetCT身體組成.Range["C36"].Value;
        BodyFatAnalysis脂肪分析.SubcutaneousFatPercentile皮下脂肪百分位 =
            worksheetCT身體組成.Range["H36"].Value;
        BodyFatAnalysis脂肪分析.WaistCircumferencePercentile腰圍百分位 =
            worksheetCT身體組成.Range["B41"].Value.ToDouble().ToString("F0");
        BodyFatAnalysis脂肪分析.MetabolicSyndromeRisk代謝失調風險 =
            worksheetCT身體組成.Range["Y22"].Value;
        BodyFatAnalysis脂肪分析.SpineSkeleton脊椎骨架 =
            worksheetCT身體組成.Range["B46"].Value;
        BodyFatAnalysis脂肪分析.Height身高 = worksheetBIA身體組成.Range["C2"].Value;
        BodyFatAnalysis脂肪分析.Weight體重 = worksheetBIA身體組成.Range["C3"].Value;
        BodyFatAnalysis脂肪分析.BMI = worksheetBIA身體組成.Range["C5"].Value;
        BodyFatAnalysis脂肪分析.BodyFatPercentage體脂率 = worksheetBIA身體組成.Range["C7"].Value;
        BodyFatAnalysis脂肪分析.BasalMetabolicRate基礎代謝率 =
            worksheetBIA身體組成.Range["C8"].Value;
        BodyFatAnalysis脂肪分析.TotalDailyEnergyExpenditure每日消耗總熱量 =
                        worksheetBIA身體組成.Range["C9"].Value;
        BodyFatAnalysis脂肪分析.BodyWater身體水分 = worksheetBIA身體組成.Range["C10"].Value;

        #region 各肌群脂肪變性百分位 雷達圖
        var FatDegenerationPercentileInMuscleGroups各肌群脂肪變性百分位 = BodyFatAnalysis脂肪分析.FatDegenerationPercentileInMuscleGroups各肌群脂肪變性百分位RadarChartModel;
        FatDegenerationPercentileInMuscleGroups各肌群脂肪變性百分位.RectusAbdominis腹直肌 =
            worksheetCT身體組成.Range["Y17"].Value;
        FatDegenerationPercentileInMuscleGroups各肌群脂肪變性百分位.InternalObliqueExternalObliqueTransversusAbdominis內斜肌_外斜肌_腹橫肌 =
            worksheetCT身體組成.Range["Y18"].Value;
        FatDegenerationPercentileInMuscleGroups各肌群脂肪變性百分位.PsoasMuscle腰肌 =
            worksheetCT身體組成.Range["Y19"].Value;
        FatDegenerationPercentileInMuscleGroups各肌群脂肪變性百分位.QuadratusLumborum腰方肌 =
            worksheetCT身體組成.Range["Y20"].Value;
        FatDegenerationPercentileInMuscleGroups各肌群脂肪變性百分位.ErectorSpinaeMultifidus豎脊肌_多裂肌 =
            worksheetCT身體組成.Range["Y21"].Value;
        #endregion
    }

    private void Collect綜合評估建議FromExcel(NextGenerationSportsCTMSModel CTMSModel, IWorkbook workbook)
    {
        IWorksheet worksheetCT身體組成 = workbook
            .Worksheets[MagicObjectHelper.CT身體組成WorkSheetName];
        IWorksheet worksheetBIA身體組成 = workbook
            .Worksheets[MagicObjectHelper.BIA身體組成WorkSheetName];
        var Recommendation綜合評估建議 = CTMSModel.ComprehensiveAssessmentRecommendation綜合評估建議Model;
        Recommendation綜合評估建議.VisceralFatPercentile內臟脂肪百分位 =
            worksheetCT身體組成.Range["C36"].Value;
        Recommendation綜合評估建議.SubcutaneousFatPercentile皮下脂肪百分位 =
            worksheetCT身體組成.Range["H36"].Value;
        Recommendation綜合評估建議.MetabolicDisorderRisk代謝失調風險 =
            worksheetCT身體組成.Range["Y22"].Value;
        Recommendation綜合評估建議.WaistCircumferencePercentile腰圍百分位 =
            worksheetCT身體組成.Range["B41"].Value;
        double AB22 = worksheetCT身體組成.Range["AB22"].Value.ToDouble();
        Recommendation綜合評估建議.MuscleHealth肌肉健康度 = (100 - AB22).ToString("F0");
        Recommendation綜合評估建議.MuscleStrength肌力 =
            worksheetCT身體組成.Range["D22"].Value;
        Recommendation綜合評估建議.MuscleEndurance肌耐力 =
            worksheetCT身體組成.Range["C22"].Value;

        #region 核心均衡力
        double D18 = worksheetCT身體組成.Range["D18"].Value.ToDouble();
        double C18 = worksheetCT身體組成.Range["C18"].Value.ToDouble();
        double D17 = worksheetCT身體組成.Range["D17"].Value.ToDouble();
        double D20 = worksheetCT身體組成.Range["D20"].Value.ToDouble();
        double D21 = worksheetCT身體組成.Range["D21"].Value.ToDouble();
        double C17 = worksheetCT身體組成.Range["C17"].Value.ToDouble();
        double C20 = worksheetCT身體組成.Range["C20"].Value.ToDouble();
        double C21 = worksheetCT身體組成.Range["C21"].Value.ToDouble();
        double F18 = worksheetCT身體組成.Range["F18"].Value.ToDouble();
        double F17 = worksheetCT身體組成.Range["F17"].Value.ToDouble();
        double F20 = worksheetCT身體組成.Range["F20"].Value.ToDouble();
        double F21 = worksheetCT身體組成.Range["F21"].Value.ToDouble();

        //(F18*100%+
        //(F17*20%+F18*20%+F20*20%+F21*40%)+
        //(F17*60%+F18*40%)+
        //F21*100%+
        //(F18*66%+F20*34%))/5
        Recommendation綜合評估建議.CoreBalance核心均衡力 =
            (((F18 * 1.0) +
            (F17 * 0.2 + F18 * 0.2 + F20 * 0.2 + F21 * 0.4) +
            (F17 * 0.6 + F18 * 0.4) +
            (F21 * 1.0) +
            (F18 * 0.66 + F20 * 0.34))
            / 5.0).ToString("F0");
        #endregion
    }
    #endregion

    public void ShowInformation(NextGenerationSportsCTMSModel CTMSModel)
    {
        Console.WriteLine($"性別 {CTMSModel.HomePageModel.性別}");
        Console.WriteLine($"年齡 {CTMSModel.HomePageModel.年齡}");
        Console.WriteLine($"身高 {CTMSModel.HomePageModel.身高}");
        Console.WriteLine($"體重 {CTMSModel.HomePageModel.體重}");
        Console.WriteLine($"BMI {CTMSModel.HomePageModel.BMI}");
    }
}
