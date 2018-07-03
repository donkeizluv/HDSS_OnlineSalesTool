using OnlineSalesCore.Helper;
using OnlineSalesCore.DTO;

namespace OnlineSalesCore.ViewModels
{
    /// <summary>
    /// Generic listing of users
    /// </summary>
    public class UserListingVM : ListingViewModel<AppUserDTO>
    {
        public UserListingVM(Params param) : base(param)
        {
        }
    }
}
