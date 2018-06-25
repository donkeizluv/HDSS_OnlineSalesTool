using OnlineSalesCore.ApiParameter;
using OnlineSalesCore.DTO;
using OnlineSalesCore.ViewModels;
using System;
using System.Threading.Tasks;

namespace OnlineSalesCore.Service
{
    public interface IIndusService : IDisposable
    {
        Task<IndusContractDTO> Get(string contractNumber);
    }
}
