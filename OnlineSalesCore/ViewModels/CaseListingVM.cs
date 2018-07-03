using OnlineSalesCore.Helper;
using OnlineSalesCore.DTO;
using System.Collections.Generic;

namespace OnlineSalesCore.ViewModels
{
    /// <summary>
    /// Generic listing of POSs
    /// </summary>
    public class CaseListingVM : ListingViewModel<CaseDTO>
    {
        public CaseListingVM(Params param) : base(param)
        {
        }
    }
}
