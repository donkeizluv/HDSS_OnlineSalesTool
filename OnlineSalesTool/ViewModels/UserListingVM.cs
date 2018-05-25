using OnlineSalesTool.ApiParameter;
using OnlineSalesTool.POCO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineSalesTool.ViewModels
{
    /// <summary>
    /// Generic listing of POSs
    /// </summary>
    public class UserListingVM : ListingViewModel<AppUserPOCO>
    {
        public UserListingVM(ListingParams param) : base(param)
        {
        }
    }
}
