using CTMS.DataModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTMS.Business.Services
{
    public class CurrentUserService
    {
        public CurrentUser CurrentUser { get; set; } = new();
    }
}
