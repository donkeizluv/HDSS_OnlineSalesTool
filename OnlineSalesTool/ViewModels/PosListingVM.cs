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
    public class PosListingVM : ListingViewModel<PosDTO>
    {
        public IEnumerable<ShiftDTO> Shifts { get; set; }
        public PosListingVM(ListingParams param) : base(param)
        {
        }
    }
}
