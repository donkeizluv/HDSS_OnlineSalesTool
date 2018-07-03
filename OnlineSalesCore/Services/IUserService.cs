using OnlineSalesCore.Helper;
using OnlineSalesCore.Const;
using OnlineSalesCore.DTO;
using OnlineSalesCore.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineSalesCore.Services
{
    public interface IUserService : IDisposable
    {
        Task<UserListingVM> Get(Params param);
        Task<int> Create(AppUserDTO user);
        Task Update(AppUserDTO user);
        Task<IEnumerable<AppUserDTO>> Suggest(RoleEnum role, string q);
        Task<IEnumerable<AppUserDTO>> SuggestAssign(string q);
        Task<int> CheckUsername(string userName);
    }
}
