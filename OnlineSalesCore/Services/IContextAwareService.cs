using OnlineSalesCore.Const;
using OnlineSalesCore.Models;
using System;
using System.Security.Claims;

namespace OnlineSalesCore.Services
{
    public interface IContextAwareService : IDisposable
    {
        RoleEnum Role { get; }
        string Username { get; }
        string Email { get; }
        int UserId { get; }
        ClaimsPrincipal UserPrincipal { get; }
        OnlineSalesContext DbContext { get; }

    }
}
