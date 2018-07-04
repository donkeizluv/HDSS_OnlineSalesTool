using OnlineSalesCore.Const;
using OnlineSalesCore.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OnlineSalesCore.Services
{
    public interface IAuthService : IDisposable
    {
        Task<(LoginResult, AppUser)> Authenticate(string userName, string pwd);
        Task<AppUser> GetUser(string userName);
        IEnumerable<Claim> CreateClaims(AppUser user);
    }
}
