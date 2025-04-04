﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTMS.DataModel.Models;

public struct VerifyRecordResult
{
    public bool Success { get; set; }
    public Exception Exception { get; set; }
    public string Message { get; set; }
}
