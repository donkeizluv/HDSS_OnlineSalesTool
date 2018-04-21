using OnlineSalesTool.ApiParameter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineSalesTool.ViewModels
{
    //Generic this shit
    interface IListingViewModelFactory<T> where T : IListingViewModel
    {
        Task<T> Create(ListingParams apiParam);
    }
}
