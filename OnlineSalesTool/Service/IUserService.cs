using OnlineSalesTool.ApiParameter;
using OnlineSalesTool.Const;
using OnlineSalesTool.DTO;
using OnlineSalesTool.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineSalesTool.Const
{
    public interface IUserService
    {
        Task<UserListingVM> Get(ListingParams param);
        Task<int> Create(AppUserDTO user);
        Task Update(AppUserDTO user);
        Task<IEnumerable<AppUserDTO>> SearchSuggest(RoleEnum role, string q);
        Task<int> CheckUsername(string userName);
    }
}
