using OnlineSalesTool.Const;
using OnlineSalesTool.EFModel;
using System;
using System.Security.Claims;

namespace OnlineSalesTool.Const
{
    public interface IService : IDisposable
    {
        RoleEnum Role { get; }
        string Username { get; }
        int UserId { get; }
        ClaimsPrincipal UserPrincipal { get; }
        OnlineSalesContext DbContext { get; }

    }
}
