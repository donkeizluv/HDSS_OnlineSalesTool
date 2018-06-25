using OnlineSalesCore.ApiParameter;
using OnlineSalesCore.DTO;
using OnlineSalesCore.ViewModels;
using System;
using System.Threading.Tasks;

namespace OnlineSalesCore.Service
{
    public interface ICaseService : IDisposable
    {
        Task<CaseListingVM> Get(ListingParams param);
    }
}
