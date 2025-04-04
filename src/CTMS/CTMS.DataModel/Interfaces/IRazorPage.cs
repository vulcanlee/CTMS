﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTMS.DataModel.Interfaces;

public interface IRazorPage
{
    /// <summary>
    /// 呼叫元件的 StateHasChanged()
    /// </summary>
    Task NeedRefreshAsync();
    /// <summary>
    /// 在 UI 執行緒下執行委派方法
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    Task NeedInvokeAsync(Action action);
}
