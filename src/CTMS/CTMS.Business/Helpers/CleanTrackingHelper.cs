using CTMS.EntityModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTMS.Business.Helpers;

public class CleanTrackingHelper
{
    public static void Clean<T>(BackendDBContext context) where T : class
    {
        foreach (var fooXItem in context.Set<T>().Local)
        {
            context.Entry(fooXItem).State = EntityState.Detached;
        }
    }
}
