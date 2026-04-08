# 檢驗檢查報告:
* Method: http://hisweb.hosp.ncku/HISService/LIS/WService/basic/BusinessLogic.svc

* Function: 
  * 數值報告:SYSPOWERGetLabdataByChartNo(病歷號,起日,迄日)
  * 敘述型報告:SYSPOWERGetTextReportByChartNo(病歷號,起日,迄日)

# 病患藥品資料API網址如下
* https://intra.hosp.ncku.edu.tw/IntraApi/opd/pip/get_patient_medications

* 傳入參數:
  1.	病歷號(Chart_No)
  2.	開立藥品生效日期,查詢區間:(Order_Effect_Start_Date)~(Order_Effect_End_Date)
  3.	開立藥品結束日期,查詢區間: (Order_End_Start_Date)~(Order_End_End_Date)

  開立藥品生效日期、開立藥品結束日期,可擇一傳入即可
