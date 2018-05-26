using OnlineSalesTool.ApiParameter;
using OnlineSalesTool.POCO;
using OnlineSalesTool.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineSalesTool.Service
{
    public interface IPosService
    {
        Task<PosListingVM> Get(ListingParams param);
        Task Create(PosPOCO pos);
        Task Update(PosPOCO pos);
    }
}
