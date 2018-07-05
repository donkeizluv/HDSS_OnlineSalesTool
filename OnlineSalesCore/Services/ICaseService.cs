using OnlineSalesCore.Helper;
using OnlineSalesCore.DTO;
using OnlineSalesCore.ViewModels;
using System;
using System.Threading.Tasks;

namespace OnlineSalesCore.Services
{
    public interface ICaseService : IDisposable
    {
        Task<CaseListingVM> Get(Params param);
        Task UpdateContract(UpdateContractDTO dto);
        Task Assign(CaseAssignDTO dto);
        Task Confirm(CustomerConfirmDTO dto);
        Task DocumentConfirm(CustomerConfirmDTO dto);
    }
}
