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
        Task Create(AppUserPOCO pos);
        Task Update(AppUserPOCO pos);
        Task<IEnumerable<SelectOptionPOCO>> SearchSuggest(RoleEnum role, string q);
    }
}
