using OnlineSalesTool.ApiParameter;
using OnlineSalesTool.AppEnum;
using OnlineSalesTool.DTO;
using OnlineSalesTool.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineSalesTool.Service
{
    public interface IUserService
    {
        Task<UserListingVM> Get(ListingParams param);
        Task<int> Create(AppUserDTO user);
        Task Update(AppUserDTO user);
        Task<IEnumerable<SelectOptionDTO>> SearchSuggest(RoleEnum role, string q);
    }
}
