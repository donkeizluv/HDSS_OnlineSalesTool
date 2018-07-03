using OnlineSalesCore.Helper;
using OnlineSalesCore.DTO;
using OnlineSalesCore.ViewModels;
using System;
using System.Threading.Tasks;

namespace OnlineSalesCore.Services
{
    public interface IPosService : IDisposable
    {
        Task<PosListingVM> Get(Params param);
        Task<int> Create(PosDTO pos);
        Task Update(PosDTO pos);
        Task<int> CheckCode(string posCode);
    }
}
