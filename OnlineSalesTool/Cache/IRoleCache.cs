using OnlineSalesTool.AppEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineSalesTool.Cache
{
    public interface IRoleCache
    {
        void GetRoleId(string role, out int roleId, out RoleEnum appRole);
    }
}
