using OnlineSalesCore.ApiParameter;
using OnlineSalesCore.DTO;
using OnlineSalesCore.ViewModels;
using System;
using System.Threading.Tasks;

namespace OnlineSalesCore.Service
{
    public interface IPosService : IDisposable
    {
        Task<PosListingVM> Get(ListingParams param);
        Task<int> Create(PosDTO pos);
        Task Update(PosDTO pos);
        Task<int> CheckCode(string posCode);
    }
}
