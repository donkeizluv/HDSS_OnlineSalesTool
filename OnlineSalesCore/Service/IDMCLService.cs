using OnlineSalesCore.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineSalesCore.Service
{
    public interface IDMCLService : IDisposable
    {
        Task Push(IEnumerable<OrderDTO> orders);
        Task<OrderDTO> GetStatus(string guid);
        Task UpdateBill(OnlineBillDTO onlineBill);

    }
}
