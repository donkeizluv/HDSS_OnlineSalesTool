using OnlineSalesTool.AppEnum;
using OnlineSalesTool.EFModel;
using System.Security.Claims;

namespace OnlineSalesTool.Service
{
    public interface IService
    {
        RoleEnum Role { get; }
        string Username { get; }
        int UserId { get; }
        ClaimsPrincipal UserPrincipal { get; }
        OnlineSalesContext DbContext { get; }

    }
}
