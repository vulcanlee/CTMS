﻿using CTMS.DataModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTMS.Business.Factories;

public static class VerifyRecordResultFactory
{
    public static VerifyRecordResult Build(bool success)
    {
        VerifyRecordResult verifyRecordResult = new VerifyRecordResult()
        {
            Success = success,
            Message = "",
            Exception = null,
        };
        return verifyRecordResult;
    }

    /// <summary>
    /// 使用文字內容來回應
    /// </summary>
    /// <param name="success"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public static VerifyRecordResult Build(bool success, string message, Exception exception = null)
    {
        VerifyRecordResult verifyRecordResult = new VerifyRecordResult()
        {
            Success = success,
            Message = message,
            Exception = exception,
        };
        return verifyRecordResult;
    }

}
