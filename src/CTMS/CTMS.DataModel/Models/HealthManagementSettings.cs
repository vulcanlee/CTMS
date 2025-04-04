using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTMS.DataModel.Models
{
    public class CTMSSettings
    {
        public string SyncfusionLicenseKey { get; set; }
        public ConnectionStrings ConnectionStrings { get; set; }
    }

    public class ConnectionStrings
    {
        public string DefaultConnection { get; set; }
        public string SQLiteDefaultConnection { get; set; }

    }

}
