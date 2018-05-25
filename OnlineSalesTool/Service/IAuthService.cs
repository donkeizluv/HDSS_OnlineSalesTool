using OnlineSalesTool.EFModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static OnlineSalesTool.Controllers.AccountController;

namespace OnlineSalesTool.Service
{
    public interface IAuthService
    {
        Task<(LoginResult, AppUser)> Authenticate(string userName, string pwd);
        Task<AppUser> GetUser(string userName);
    }
}
