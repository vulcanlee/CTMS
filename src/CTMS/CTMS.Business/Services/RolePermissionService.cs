using CTMS.DataModel.Models;
using CTMS.Share.Helpers;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTMS.Business.Services
{
    public class RolePermissionService
    {
        public List<string> GetRolePermissionAllName()
        {
            return new List<string>
            {
                MagicObjectHelper.ROLE瀏覽,
                MagicObjectHelper.ROLE新增病患,
                MagicObjectHelper.ROLE臨床資訊,
                MagicObjectHelper.ROLE臨床資料,
                MagicObjectHelper.ROLE抽血資料,
                MagicObjectHelper.ROLE副作用,
                MagicObjectHelper.ROLE問卷,
                MagicObjectHelper.ROLE追蹤資料,
                MagicObjectHelper.ROLE風險評估,
                MagicObjectHelper.ROLE通知放射科醫師,
                MagicObjectHelper.ROLE風險評估影像確認,
                MagicObjectHelper.ROLE風險評估結果確認,
                MagicObjectHelper.ROLE風險評估確認歷程,
                MagicObjectHelper.ROLEAI操作,
                MagicObjectHelper.ROLE備份還原
            };
        }

        public List<string> GetGet預設新建帳號角色ToJsonPermissionAllName()
        {
            return new List<string>
            {
                MagicObjectHelper.ROLE瀏覽,
                MagicObjectHelper.ROLE新增病患,
                MagicObjectHelper.ROLE臨床資訊,
                MagicObjectHelper.ROLE臨床資料,
                MagicObjectHelper.ROLE抽血資料,
                MagicObjectHelper.ROLE副作用,
                MagicObjectHelper.ROLE問卷,
                MagicObjectHelper.ROLE追蹤資料,
                MagicObjectHelper.ROLE風險評估,
            };
        }
        public string GetRolePermissionAllNameToJson()
        {
            var items = GetRolePermissionAllName();
            return Newtonsoft.Json.JsonConvert.SerializeObject(items);
        }

        public string Get預設新建帳號角色ToJson()
        {
            var items = GetGet預設新建帳號角色ToJsonPermissionAllName();
            return Newtonsoft.Json.JsonConvert.SerializeObject(items);
        }

        public RolePermission InitializePermissionSetting()
        {
            var allPermisssionName = GetRolePermissionAllName();
            var result = new RolePermission();
            foreach (var item in allPermisssionName)
            {
                result.Permissions.Add(new RolePermissionNode
                {
                    Name = item,
                    Enable = false,
                });
            }
            return result;
        }

        public void SetPermissionInput(RolePermission rolePermission, List<string> permissions)
        {
            foreach (var item in rolePermission.Permissions)
            {
                item.Enable = permissions.Contains(item.Name);
            }
        }

        public List<string> GetPermissionInput(RolePermission rolePermission)
        {
            return rolePermission.Permissions.Where(x => x.Enable).Select(x => x.Name).ToList();
        }

        public string GetPermissionInputToJson(RolePermission rolePermission)
        {
            var items = GetPermissionInput(rolePermission);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(items);
            return json;
        }
    }
}
