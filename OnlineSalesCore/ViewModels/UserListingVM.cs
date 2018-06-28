using OnlineSalesCore.ApiParameter;
using OnlineSalesCore.DTO;

namespace OnlineSalesCore.ViewModels
{
    /// <summary>
    /// Generic listing of users
    /// </summary>
    public class UserListingVM : ListingViewModel<AppUserDTO>
    {
        public UserListingVM(ListingParams param) : base(param)
        {
        }
    }
}
