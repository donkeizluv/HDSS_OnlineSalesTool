using OnlineSalesTool.ApiParameter;
using OnlineSalesTool.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineSalesTool.ViewModels
{
    /// <summary>
    /// Generic listing of POSs
    /// </summary>
    public class UserListingVM : ListingViewModel<AppUserDTO>
    {
        public UserListingVM(ListingParams param) : base(param)
        {
        }
    }
}
