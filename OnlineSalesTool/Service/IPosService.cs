using OnlineSalesTool.ApiParameter;
using OnlineSalesTool.DTO;
using OnlineSalesTool.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineSalesTool.Const
{
    public interface IPosService : IDisposable
    {
        Task<PosListingVM> Get(ListingParams param);
        Task<int> Create(PosDTO pos);
        Task Update(PosDTO pos);
        Task<int> CheckCode(string posCode);
    }
}
