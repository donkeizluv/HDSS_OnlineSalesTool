using OnlineSalesCore.Const;

namespace OnlineSalesCore.Cache
{
    public interface IRoleCache
    {
        void GetRoleId(string role, out int roleId, out RoleEnum appRole);
    }
}
