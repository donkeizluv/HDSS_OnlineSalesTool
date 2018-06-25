using OnlineSalesCore.ApiParameter;
using OnlineSalesCore.DTO;
using System.Collections.Generic;

namespace OnlineSalesCore.ViewModels
{
    /// <summary>
    /// Generic listing of POSs
    /// </summary>
    public class CaseListingVM : ListingViewModel<CaseDTO>
    {
        public CaseListingVM(ListingParams param) : base(param)
        {
        }
    }
}
