﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTMS.DataModel.Models.ClinicalInformation
{
    public class BasicClinicalPresentation_臨床資訊
    {
        // 左側欄位
        /// <summary>
        /// 病人編號
        /// </summary>
        public string SubjectNo { get; set; }
        /// <summary>
        /// 臨床資訊 癌別 EC or OCC(自填)
        /// </summary>
        public string ECOrOCC { get; set; }  
        /// <summary>
        /// 年齡 (Age) 20歲-80歲
        /// </summary>
        public int Age { get; set; }   
        /// <summary>
        /// 月經狀態 (0停經, 1未停經)
        /// </summary>
        public int MenstrualStatus { get; set; }   
        /// <summary>
        /// 身高(cm) 140cm-180cm
        /// </summary>
        public double Height { get; set; }     
        /// <summary>
        /// 體重(kg) 30kg-120kg
        /// </summary>
        public double Weight { get; set; }
        /// <summary>
        /// BMI (Kg/m²)
        /// </summary>
        public double BMI { get; set; }
        /// <summary>
        /// 體表面積(BSA) m²
        /// </summary>
        public double BSA { get; set; }
        /// <summary>
        /// 腰圍 (AC) cm
        /// </summary>
        public double AbdominalCircumference { get; set; }  
        /// <summary>
        /// 日常體能狀態(PS) 0, 1, 2
        /// </summary>
        public int PerformanceStatus { get; set; }
        // 右側欄位

        /// <summary>
        /// 癌症狀態 癌別 卵巢/子宮
        /// </summary>
        public string CancerType { get; set; }
        /// <summary>
        /// 癌症分期(2023 FIGO)
        /// </summary>
        public string FIGOStaging { get; set; }
        /// <summary>
        /// AJCC c stage
        /// </summary>
        public string AJCCClinicalStage { get; set; }
        /// <summary>
        /// AJCC p stage
        /// </summary>
        public string AJCCPathologicalStage { get; set; }  
        /// <summary>
        /// 組織型態
        /// </summary>
        public string HistologicalType { get; set; }
        /// <summary>
        /// MMR protein
        /// </summary>
        public string MMRProtein { get; set; }
        /// <summary>
        /// p53
        /// </summary>
        public string P53 { get; set; }
        /// <summary>
        /// Hormon status
        /// </summary>
        public string HormonStatus { get; set; }

        // 計算BMI和BSA的方法
        public void CalculateBMI()
        {
            if (Height > 0 && Weight > 0)
            {
                // BMI = 體重(kg) / 身高(m)²
                BMI = Weight / Math.Pow(Height / 100, 2);
            }
        }

        public void CalculateBSA()
        {
            if (Height > 0 && Weight > 0)
            {
                // BSA = (體重(kg) × 身高(cm)) ÷ 3600 的開根號
                BSA = Math.Sqrt((Weight * Height) / 3600);
            }
        }
    }
}
