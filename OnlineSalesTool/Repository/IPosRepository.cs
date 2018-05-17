using OnlineSalesTool.ApiParameter;
using OnlineSalesTool.POCO;
using OnlineSalesTool.ViewModels;
using System.Threading.Tasks;

namespace OnlineSalesTool.Repository
{
    public interface IPosRepository
    {
        Task<PosListingVM> Get(ListingParams param);
        Task Create(PosPOCO pos);
        Task Update(PosPOCO pos);
    }
}
