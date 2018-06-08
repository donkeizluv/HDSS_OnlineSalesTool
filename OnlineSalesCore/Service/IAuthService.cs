using OnlineSalesCore.Const;
using OnlineSalesCore.EFModel;
using System;
using System.Threading.Tasks;

namespace OnlineSalesCore.Service
{
    public interface IAuthService : IDisposable
    {
        Task<(LoginResult, AppUser)> Authenticate(string userName, string pwd);
        Task<AppUser> GetUser(string userName);
    }
}
