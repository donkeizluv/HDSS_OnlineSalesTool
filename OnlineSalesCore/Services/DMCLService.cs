using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OnlineSalesCore.Const;
using OnlineSalesCore.DTO;
using OnlineSalesCore.Models;
using OnlineSalesCore.Exceptions;
using OnlineSalesCore.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OnlineSalesCore.Cache;

namespace OnlineSalesCore.Services
{
    public class DMCLService : ContextAwareService, IDMCLService
    {
        private readonly ILogger<DMCLService> _logger;
        private readonly IScheduleMatcher _matcher;
        private readonly IAPIAuth _apiAuth;
        private readonly IMailerService _mail;
        // private readonly IMailListCache _mailList;
        public DMCLService(OnlineSalesContext context,
            IHttpContextAccessor httpContext,
            IScheduleMatcher matcher,
            IAPIAuth apiAuth,
            IMailerService mail,
            ILogger<DMCLService> logger) : base(httpContext, context)
        {
            _matcher = matcher;
            _apiAuth = apiAuth;
            _logger = logger;
            _mail = mail;
        }
        public async Task<OrderDTO> GetStatus(string guid)
        {
            if (string.IsNullOrEmpty(guid))
                throw new ArgumentException("message", nameof(guid));
            var order = await DbContext.OnlineOrder
                .Where(o => o.OrderGuid == guid)
                .Include(o => o.Stage)
                .FirstOrDefaultAsync();
            if (order == null)
                throw new BussinessException($"Cant find order of {guid}");
            return new OrderDTO()
            {
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
                Status = DMCL_StageTranslate(order.Stage.Stage).ToString(),
                ContractNumber = order.Induscontract,
                Signature = _apiAuth.Forge(order.OrderGuid)
            };
        }
        private DMCLEnum DMCL_StageTranslate(string stageName)
        {
            if (!Enum.TryParse<StageEnum>(stageName, out var stage))
                throw new ArgumentException($"Invalid stage name of {stageName}");
            switch (stage)
            {
                case StageEnum.WaitForDocument:
                    return DMCLEnum.PROCESSING;
                case StageEnum.CustomerConfirm:
                    return DMCLEnum.PROCESSING;
                case StageEnum.EnterContractNumber:
                    return DMCLEnum.PROCESSING;
                case StageEnum.NotAssignable:
                    return DMCLEnum.PROCESSING;
                case StageEnum.NotAssigned:
                    return DMCLEnum.PROCESSING;
                case StageEnum.WaitForOnlineBill:
                    return DMCLEnum.APPROVED; //Final
                case StageEnum.WaitForFinalStatus:
                    return DMCLEnum.PROCESSING;
                case StageEnum.Reject: //Final
                    return DMCLEnum.REJECT;
                case StageEnum.CustomerReject:
                    return DMCLEnum.CUSTOMER_REJECT; //Final
                case StageEnum.Completed:
                    return DMCLEnum.APPROVED; //Final
                default:
                    _logger.LogInformation($"Unexspected stage: [{stage}] -> use default case {DMCLEnum.PROCESSING}");
                    return DMCLEnum.PROCESSING;
            }
        }
        public async Task Push(IEnumerable<OrderDTO> orders)
        {
            if (orders == null) throw new ArgumentNullException();
            //Check duplicate guid
            if (await DbContext.OnlineOrder.AnyAsync(o => orders.ToList().Any(oo => oo.Guid == o.OrderGuid)))
            {
                throw new BussinessException("Duplicates of guid");
            }
            //POS list to ref PosId of cases
            var foundPos = await DbContext.Pos
                .Where(p => orders
                    .Any(o => o.PosCode.ToUpper() == p.PosCode.ToUpper()))
                .Include(p => p.User) //Include BDS for notification
                .Distinct()
                .ToListAsync();
            //Map to app orders
            var appOrders = orders.Select(o => new OnlineOrder()
            {
                OrderGuid = o.Guid,
                Name = o.FullName,
                NatId = o.NatId,
                Phone = o.Phone,
                Address = o.Address,
                PosCode = o.PosCode,
                PosId = foundPos.FirstOrDefault(p => p.PosCode.ToUpper() == o.PosCode)?.PosId,
                Product = o.Product,
                Amount = o.Amount,
                Paid = o.Paid,
                LoanAmount = o.LoanAmount,
                Term = o.Term,
                Received = DateTime.Now,
                StageId = (int)StageEnum.NotAssigned
            }).ToList();
            //Try to assign
            var currentTime = DateTime.Now;
            foreach (var item in appOrders)
            {
                (var matchFound, var users, var reason) = await _matcher.GetUserMatchedSchedule(item.PosCode, currentTime);
                if (matchFound)
                {
                    //Assigned OK -> CA ask customer's comfirmation
                    item.StageId = (int)StageEnum.CustomerConfirm;
                    //In case of additonal logic related to multiple matchers
                    //Goes here
                    var to = users.First();
                    item.AssignUserId = to.UserId;
                    _mail.MailNewAssign(item, to.Email, null);
                }
                else
                {
                    item.StageId = (int)StageEnum.NotAssignable;
                    _logger.LogDebug($"Cant find assignable users for order guid {item.OrderGuid} at {currentTime.ToString()}");
                    //Notify BDS if POS found
                    var bds = foundPos.FirstOrDefault(p => p.PosId == item.PosId)?.User;
                    if (bds != null)
                    {
                        _mail.MailNotAssignable(item, bds.Email, reason, null);
                        // continue;
                    }
                    //Notify all ADMIN when POS not found
                    //TODO: too compicated and floated to cache mailing list
                    //Use mail group instead
                    // _mail.MailInvalidPOS(item, _mailList.AdminEmails.Values.ToArray());
                }
            }
            await DbContext.OnlineOrder.AddRangeAsync(appOrders);
            await DbContext.SaveChangesAsync();
        }

        public async Task UpdateBill(OnlineBillDTO onlineBill)
        {
            if (onlineBill == null)
                throw new ArgumentNullException(nameof(onlineBill));
            var order = await DbContext.OnlineOrder
                .Where(o => o.OrderGuid == onlineBill.Guid)
                .Include(o => o.AssignUser)
                .Include(o => o.Stage)
                .SingleOrDefaultAsync();
            if (order == null) throw new BussinessException($"Cant find order of {onlineBill.Guid}");
            //Only allow when WaitForOnlineBill
            if (order.StageId != (int)StageEnum.WaitForOnlineBill)
                throw new BussinessException($"Order is not ready for bill number, current stage: {order.Stage.Stage}");
            order.OrderNumber = onlineBill.OnlineBill;
            order.StageId = (int)StageEnum.Completed;
            await DbContext.SaveChangesAsync();
            _mail.MailOnlineBillAvailable(order, order.AssignUser.Email, onlineBill.OnlineBill, null);
        }
    }
}
