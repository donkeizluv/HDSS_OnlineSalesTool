﻿using OnlineSalesTool.ApiParameter;
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
    public class PosListingVM : ListingViewModel<PosPOCO>
    {
        public PosListingVM(ListingParams param) : base(param)
        {
        }
    }
}
