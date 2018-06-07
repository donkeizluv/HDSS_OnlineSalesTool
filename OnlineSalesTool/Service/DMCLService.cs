using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OnlineSalesTool.ApiParameter;
using OnlineSalesTool.AppEnum;
using OnlineSalesTool.CustomException;
using OnlineSalesTool.DTO;
using OnlineSalesTool.EFModel;
using OnlineSalesTool.Logic;
using OnlineSalesTool.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace OnlineSalesTool.Service
{
    public class DMCLService : IDMCLService
    {
        private readonly ILogger<DMCLService> _logger;
        private readonly OnlineSalesContext _context;
        private readonly IScheduleMatcher _matcher;
        private readonly IAPIAuth _apiAuth;
        
        public DMCLService(OnlineSalesContext context,
        IScheduleMatcher matcher,
        IAPIAuth apiAuth,
        ILogger<DMCLService> logger)
        {
            _context = context;
            _matcher = matcher;
            _apiAuth = apiAuth;
            _logger = logger;
        }
        public async Task<OrderDTO> GetStatus(string guid)
        {
            if (string.IsNullOrEmpty(guid))
                throw new ArgumentException("message", nameof(guid));
            var order = await _context.OnlineOrder.SingleOrDefaultAsync(o => o.OrderGuid == guid);
            if(order == null)
                throw new BussinessException($"Cant find order of {guid}");
            return new OrderDTO() {
                Guid = order.OrderGuid,
                FullName = order.Name,
                NatId = order.NatId,
                Phone = order.Phone,
                Address = order.Address,
                PosCode = order.PosCode,
                Product = order.Product,
                Amount = order.Amount,
                Paid = order.Paid,
                LoanAmount = order.LoanAmount,
                Term = order.Term,
                Received = order.Received,
                Status = DMCL_StageTranslate(order.Stage.ToString()).ToString(),
                ContractNumber = order.Induscontract,
                Signature = _apiAuth.Forge(order.OrderGuid)
            };      
        }
        private static DMCLEnum DMCL_StageTranslate(string stageName)
        {
            if(!Enum.TryParse<StageEnum>(stageName, out var stage))
                throw new ArgumentException($"Invalid stage name of {stageName}");
            switch (stage)
            {
                case StageEnum.Approved:
                    return DMCLEnum.APPROVED;
                case StageEnum.CustomerConfirm:
                    return DMCLEnum.PROCESSING;
                case StageEnum.EnterContractNumber:
                    return DMCLEnum.PROCESSING;
                case StageEnum.NotAssignable:
                    return DMCLEnum.PROCESSING;
                case StageEnum.NotAssigned:
                    return DMCLEnum.PROCESSING;
                case StageEnum.WaitForOnlineBill:
                    return DMCLEnum.PROCESSING;
                case StageEnum.WaitForFinalStatus:
                    return DMCLEnum.PROCESSING;
                case StageEnum.Reject:
                    return DMCLEnum.REJECT;
                case StageEnum.CustomerReject:
                    return DMCLEnum.CUSTOMER_REJECT;
                default:
                    return DMCLEnum.PROCESSING;
            }
        }
        public async Task Push(IEnumerable<OrderDTO> orders)
        {
            if(orders == null) throw new ArgumentNullException();
            //Map to app orders
            var appOrders = orders.Select(o => new OnlineOrder(){
                OrderGuid = o.Guid,
                Name = o.FullName,
                NatId = o.NatId,
                Phone = o.Phone,
                Address = o.Address,
                PosCode = o.PosCode,
                Product = o.Product,
                Amount = o.Amount,
                Paid = o.Paid,
                LoanAmount = o.LoanAmount,
                Term = o.Term,
                Received = DateTime.Now,
                StageId = (int)StageEnum.NotAssigned
            });
            //Try to assign
            var currentTime = DateTime.Now;
            foreach (var item in appOrders)
            {
                (var matchFound, var ids, var reason) = await _matcher.GetUserMatchedSchedule(item.PosCode, currentTime);
                if(matchFound)
                {
                    item.StageId = (int)StageEnum.WaitForOnlineBill;
                    //In case of additonal logic related to multiple matchers
                    //Goes here
                    item.AssignUserId = ids.First();
                }
                else
                {
                    item.StageId = (int)StageEnum.NotAssignable;
                    _logger.LogDebug($"No user matched for order guid {item.OrderGuid} at {currentTime.ToString()}");
                }
            }
            await _context.OnlineOrder.AddRangeAsync(appOrders);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateBill(OnlineBillDTO onlineBill)
        {
            if (onlineBill == null)
            {
                throw new ArgumentNullException(nameof(onlineBill));
            }
            var order = _context.OnlineOrder.SingleOrDefault(o => o.OrderGuid == onlineBill.Guid);
            if(order == null) throw new BussinessException($"Cant find order of {onlineBill.Guid}");
            order.OrderNumber = onlineBill.OnlineBill;
            order.StageId = (int)StageEnum.Completed;
            await _context.SaveChangesAsync();
        }
    }
}
