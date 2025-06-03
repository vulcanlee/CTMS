using CTMS.Share.Extensions;
using System;
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
        public string ECorOC { get; set; }  
        /// <summary>
        /// 年齡 (Age) 20歲-80歲
        /// </summary>
        public string Age { get; set; }   
        /// <summary>
        /// 月經狀態 (0停經, 1未停經)
        /// </summary>
        public string MenstrualStatus { get; set; }   
        /// <summary>
        /// 身高(cm) 140cm-180cm
        /// </summary>
        public string Height { get; set; }     
        /// <summary>
        /// 體重(kg) 30kg-120kg
        /// </summary>
        public string Weight { get; set; }
        /// <summary>
        /// BMI (Kg/m²)
        /// </summary>
        public string BMI { get; set; }
        /// <summary>
        /// 體表面積(BSA) m²
        /// </summary>
        public string BSA { get; set; }
        /// <summary>
        /// 腰圍 (AC) cm
        /// </summary>
        public string AbdominalCircumference { get; set; }  
        /// <summary>
        /// 日常體能狀態(PS) 0, 1, 2
        /// </summary>
        public string PerformanceStatus { get; set; }
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

        public void CalculateCancerType()
        {
            if (ECorOC == "EC")
            {
                CancerType = "子宮內膜癌";
            }
            else if (ECorOC == "OC")
            {
                CancerType = "卵巢癌";
            }
            else
            {
                CancerType = "未知";
            }
        }
        // 計算BMI和BSA的方法
        public void CalculateBMI()
        {
            if (Height.ToDouble() > 0 && Weight.ToDouble() > 0)
            {
                // BMI = 體重(kg) / 身高(m)²
                BMI = (Weight.ToDouble() / Math.Pow(Height.ToDouble() / 100, 2)).ToString("F2");
            }
        }

        public void CalculateBSA()
        {
            if (Height.ToDouble() > 0 && Weight.ToDouble() > 0)
            {
                // BSA = (體重(kg) × 身高(cm)) ÷ 3600 的開根號
                BSA = Math.Sqrt((Weight.ToDouble() * Height.ToDouble()) / 3600).ToString("F2");
            }
        }
        public void CalculateBMIAndBSA()
        {
            CalculateBMI();
            CalculateBSA();
        }
    }
}
