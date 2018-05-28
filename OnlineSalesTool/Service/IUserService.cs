using OnlineSalesTool.ApiParameter;
using OnlineSalesTool.AppEnum;
using OnlineSalesTool.POCO;
using OnlineSalesTool.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineSalesTool.Service
{
    public interface IUserService
    {
        Task<UserListingVM> Get(ListingParams param);
        Task<int> Create(AppUserPOCO user);
        Task Update(AppUserPOCO user);
        Task<IEnumerable<SelectOptionPOCO>> SearchSuggest(RoleEnum role, string q);
    }
}
