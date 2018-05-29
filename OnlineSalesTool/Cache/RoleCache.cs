using OnlineSalesTool.AppEnum;
using OnlineSalesTool.CustomException;
using OnlineSalesTool.EFModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineSalesTool.Cache
{
    public class RoleCache : IRoleCache
    {
        private static IDictionary<string, int> _roleDict;

        public RoleCache(OnlineSalesContext context)
        {
            if (context == null) throw new ArgumentNullException();
            if (_roleDict != null) return;
            _roleDict = context.UserRole.Select(u => new { u.RoleId, u.Name }).
                ToDictionary(key => key.Name, id => id.RoleId);
        }

        public void GetRoleId(string role, out int roleId, out RoleEnum appRole)
        {
            if (string.IsNullOrEmpty(role))
                throw new BussinessException($"No role is specified");
            if (!_roleDict.ContainsKey(role))
                throw new BussinessException($"No such role found in database: {role}");
            if(!Enum.TryParse(role, true, out appRole))
                throw new BussinessException($"Invalid role: {role}");
            roleId = _roleDict[role];
        }
    }
}
