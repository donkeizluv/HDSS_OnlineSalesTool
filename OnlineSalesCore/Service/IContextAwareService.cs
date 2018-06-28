using OnlineSalesCore.Const;
using OnlineSalesCore.EFModel;
using System;
using System.Security.Claims;

namespace OnlineSalesCore.Service
{
    public interface IContextAwareService : IDisposable
    {
        RoleEnum Role { get; }
        string Username { get; }
        int UserId { get; }
        ClaimsPrincipal UserPrincipal { get; }
        OnlineSalesContext DbContext { get; }

    }
}
