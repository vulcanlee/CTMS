using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTMS.DataModel.Models
{
    public class RolePermission
    {
        public List<RolePermissionNode> Permissions { get; set; } = new();
    }
    public class RolePermissionNode
    {
        public string Name { get; set; }
        public bool Enable { get; set; }=false;
    }
}
