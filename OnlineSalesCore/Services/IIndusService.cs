using OnlineSalesCore.Helper;
using OnlineSalesCore.DTO;
using OnlineSalesCore.ViewModels;
using System;
using System.Threading.Tasks;

namespace OnlineSalesCore.Services
{
    public interface IIndusService : IDisposable
    {
        Task<IndusContractDTO> GetContract(string contractNumber);
    }
}
