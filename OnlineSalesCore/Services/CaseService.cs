using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OnlineSalesCore.Helper;
using OnlineSalesCore.Cache;
using OnlineSalesCore.Const;
using OnlineSalesCore.DTO;
using OnlineSalesCore.Models;
using OnlineSalesCore.Exceptions;
using OnlineSalesCore.Extentions;
using OnlineSalesCore.Queries;
using OnlineSalesCore.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineSalesCore.Services
{
    public class CaseService : ContextAwareService, ICaseService
    {
        private readonly ILogger _logger;
        private readonly ListQuery<OnlineOrder, CaseDTO> _query;
        private readonly IRoleCache _roleCache;
        private readonly IIndusService _indus;
        private readonly IMailerService _mail;
        public CaseService(OnlineSalesContext context,
            IHttpContextAccessor httpContext,
            IRoleCache roleCache,
            ILogger<CaseService> logger,
            ListQuery<OnlineOrder, CaseDTO> q,
            IIndusService indus,
            IMailerService mail)
            : base(httpContext, context)
        {
            _logger = logger;
            _query = q;
            _roleCache = roleCache;
            _indus = indus;
            _mail = mail;
        }
        public async Task UpdateContract(UpdateContractDTO dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            //Must be the assigned CA & correct stage to use this func
            var order = await DbContext.OnlineOrder
                .Where(o => o.OrderId == dto.Id)
                .Include(o => o.AssignUser)
                .Include(o => o.Stage)
                .SingleOrDefaultAsync() ?? 
                    throw new BussinessException($"Order: {dto.Id} is not exist");;
            if (order.AssignUser.UserId != UserId)
            {
                throw new BussinessException($"Order: {dto.Id} is not assigned to this user");
            }
            //Maybe allow updating contract at multiple stages in the future?
            if (order.StageId != (int)StageEnum.EnterContractNumber)
            {
                throw new BussinessException($"Order: {dto.Id} current stage: {order.Stage.Stage} is not updatable");
            }
            //Check contract already related to any order
            if (await DbContext.OnlineOrder.AnyAsync(o => o.Induscontract == dto.Contract))
            {
                throw new BussinessException($"Contract: {dto.Contract} is already related to another order");
            }
            // if (!await DbContext.FollowingContracts.AnyAsync(f => f.ContractNumber == dto.Contract))
            // {

            // }
            //Check INDUS contract exists
            //Check INDUS contract NatId == case NatId
            var indusContract = await _indus.GetContract(dto.Contract) ??
                throw new BussinessException($"Contract: {dto.Contract} is not exist");
            if (string.Compare(indusContract.NatId, order.NatId, true) != 0)
            {
                throw new BussinessException($"Contract: {dto.Contract} customer nat id is not the same as order");
            }
            //Proceed
            //Update order's contract number & stage
            order.Induscontract = dto.Contract;
            order.StageId = (int)StageEnum.WaitForFinalStatus;
            await DbContext.FollowingContracts.AddAsync(new FollowingContracts()
            {
                StartDate = DateTime.Now,
                ContractNumber = dto.Contract
            });
            await DbContext.SaveChangesAsync();
        }
        public async Task Assign(CaseAssignDTO dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }
            //Assignee must be CA
            var order = await DbContext.OnlineOrder.Where(o => o.OrderId == dto.Id).SingleOrDefaultAsync() ??
                throw new BussinessException($"Order: {dto.Id} is not exists");
            var assignee = await DbContext.AppUser.Where(u => u.UserId == dto.UserId).SingleOrDefaultAsync() ??
                throw new BussinessException($"User: {dto.UserId} is not exists");
            //Is stage valid?
            if (order.StageId != (int)StageEnum.NotAssignable)
            {
                throw new BussinessException($"Order: {dto.Id} current stage does not allow assigning");
            }
            //BDS: Is assignee under this BDS?
            if (Role == RoleEnum.BDS)
            {
                if (assignee.ManagerId != UserId)
                    throw new BussinessException($"User: {dto.UserId} is not under current user management");
            }
            //Proceed
            order.AssignUserId = assignee.UserId;
            order.StageId = (int)StageEnum.CustomerConfirm;
            await DbContext.SaveChangesAsync();
            _mail.MailNewAssign(order, assignee.Email, null);

        }
        public async Task Confirm(CustomerConfirmDTO dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }
            var order = await DbContext.OnlineOrder.Where(o => o.OrderId == dto.Id).SingleOrDefaultAsync() ??
                throw new BussinessException($"Order: {dto.Id} is not exists");
            //Stage check
            if(order.StageId != (int)StageEnum.CustomerConfirm)
            {
                throw new BussinessException($"Order: {dto.Id} stage is not valid for comfirming");
            }
            //Must be the assignee of case to confirm                
            if(order.AssignUserId != UserId)
            {
                throw new BussinessException($"Order: {dto.Id} is not assigned to current user");
            }
            //Proceed
            if(dto.Confirm)
            {
                order.StageId = (int)StageEnum.EnterContractNumber;
            }
            else
            {
                order.StageId = (int)StageEnum.CustomerReject;
                _mail.MailStageChanged(order, StageEnum.CustomerReject.ToString(), Email, null);
            }
            await DbContext.SaveChangesAsync();

        }
        public async Task<CaseListingVM> Get(Params param)
        {
            var vm = new CaseListingVM(param ?? throw new ArgumentNullException());
            var q = _query.CreateBaseQuery(Role.ToString());
            (var items, int total) = await _query.ApplyParameters(q, param);
            vm.SetItems(items, param.ItemPerPage, total);
            //Availabe shifts
            return vm;
        }
    }
}
