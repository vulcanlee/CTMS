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
}
