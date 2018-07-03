using OnlineSalesCore.Helper;
using OnlineSalesCore.DTO;
using System.Collections.Generic;

namespace OnlineSalesCore.ViewModels
{
    /// <summary>
    /// Generic listing of POSs
    /// </summary>
    public class PosListingVM : ListingViewModel<PosDTO>
    {
        public IEnumerable<ShiftDTO> Shifts { get; set; }
        public PosListingVM(Params param) : base(param)
        {
        }
    }
}
