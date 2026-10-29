using CTMS.Share.Helpers;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTMS.Business.Helpers;

public class TestingServerHelper
{
    public bool IsTestingServer(NavigationManager navigationManager)
    {
        bool isTesting = false;
        if (navigationManager != null)
        {
            var uri = navigationManager.ToAbsoluteUri(navigationManager.Uri);
            if (!string.IsNullOrEmpty(uri.ToString()) &&
            (uri.ToString().Contains(":8080/") || uri.ToString().Contains(":5272/")))
            {
                isTesting = true;
            }
        }
        else
        {
            isTesting = true;
        }
        return isTesting;
    }

    /// <summary>
    /// 依站台 port 決定系統標題：1111（正式）與 2222（測試）站台為 GLP-1，其餘維持預設
    /// </summary>
    public string GetSystemTitle(NavigationManager navigationManager)
    {
        if (navigationManager != null)
        {
            var uri = navigationManager.ToAbsoluteUri(navigationManager.Uri);
            if (uri.ToString().Contains(":1111/") || uri.ToString().Contains(":2222/"))
            {
                return MagicObjectHelper.SystemTitleGLP1;
            }
        }
        return MagicObjectHelper.SystemTitleDefault;
    }
}
