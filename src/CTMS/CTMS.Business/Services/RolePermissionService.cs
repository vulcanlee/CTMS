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
                MagicObjectHelper.ROLE主頁,
                MagicObjectHelper.ROLE動作能力,
                MagicObjectHelper.ROLE心肺功能,
                MagicObjectHelper.ROLE心理韌性,
                MagicObjectHelper.ROLE身體組成,
                MagicObjectHelper.ROLE基因體分析,
                MagicObjectHelper.ROLE代謝體分析,
                MagicObjectHelper.ROLE抽血檢驗,
                MagicObjectHelper.ROLE綜合評估建議,
                MagicObjectHelper.ROLE綜合評估建議編輯,
                MagicObjectHelper.ROLE報告摘要輸入,
                MagicObjectHelper.ROLE下載PDF,
                MagicObjectHelper.ROLE上傳資料,
                MagicObjectHelper.ROLE備份還原
            };
        }

        public string GetRolePermissionAllNameToJson()
        {
            var items = GetRolePermissionAllName();
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
