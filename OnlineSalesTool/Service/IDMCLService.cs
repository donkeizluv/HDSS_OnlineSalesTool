using OnlineSalesTool.ApiParameter;
using OnlineSalesTool.DTO;
using OnlineSalesTool.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineSalesTool.Const
{
    public interface IDMCLService : IDisposable
    {
        Task Push(IEnumerable<OrderDTO> orders);
        Task<OrderDTO> GetStatus(string guid);
        Task UpdateBill(OnlineBillDTO onlineBill);

    }
}
