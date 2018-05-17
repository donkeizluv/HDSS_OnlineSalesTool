using OnlineSalesTool.POCO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineSalesTool.ViewModels
{
    public class PosListingVM : ListingViewModel<PosPOCO>
    {
        public override int ItemPerPage => 10;
    }
}
