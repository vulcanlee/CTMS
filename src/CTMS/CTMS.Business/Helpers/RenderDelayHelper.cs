using CTMS.Share.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTMS.Business.Helpers;

public static class RenderDelayHelper
{
    public static async Task Delay()
    {
        await Task.Delay(MagicObjectHelper.NeedDelayRefresh);
    }
}
