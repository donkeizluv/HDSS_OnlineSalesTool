using System;
using System.Threading.Tasks;
using NLog;
using OnlineSalesTool.ApiParameter;
using OnlineSalesTool.EFModel;
using OnlineSalesTool.DTO;
using OnlineSalesTool.Query;
using OnlineSalesTool.ViewModels;
using OnlineSalesTool.CustomException;
using System.Linq;
using OnlineSalesTool.Cache;
using OnlineSalesTool.AppEnum;
using MoreLinq;
using Microsoft.EntityFrameworkCore;

namespace OnlineSalesTool.Service
{
    public class PosService : ServiceBase, IPosService
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly ListQuery<Pos, PosDTO> _query;
        private readonly IRoleCache _roleCache;

        public PosService(OnlineSalesContext context,
            IUserResolver userResolver,
            IRoleCache roleCache,
            ListQuery<Pos, PosDTO> q)
            : base(userResolver.GetPrincipal(), context)
        {
            _query = q;
            _roleCache = roleCache;
        }

        public async Task<PosListingVM> Get(ListingParams param)
        {
            var vm = new PosListingVM(param ?? throw new ArgumentNullException());
            var q = _query.CreateBaseQuery(Role.ToString());
            (var items, int total) = await _query.ApplyParameters(q, param);
            vm.SetItems(items, param.ItemPerPage, total);
            //Availabe shifts
            vm.Shifts = await DbContext.Shift.Select(s => new ShiftDTO(s)).ToListAsync();
            return vm;
        }
        
        public async Task<int> Create(PosDTO pos)
        {
            BasicCheck(pos ?? throw new ArgumentNullException());
            //Check Manager
            await CheckUser(pos.BDS?.UserId, RoleEnum.BDS, true);
            var newPos = new Pos()
            {
                PosCode = pos.PosCode,
                PosName = pos.PosName,
                Address = pos.Address,
                UserId = pos.BDS.UserId,
                Phone = pos.Phone
            };
            pos.Shifts.ForEach(s => newPos.PosShift.Add(new PosShift() { ShiftId = s.ShiftId }));
            await DbContext.Pos.AddAsync(newPos);
            await DbContext.SaveChangesAsync();
            return newPos.PosId;
        }

        public async Task Update(PosDTO pos)
        {
            BasicCheck(pos ?? throw new ArgumentNullException());
            var updatePos = await DbContext.Pos.SingleOrDefaultAsync(p => p.PosId == pos.PosId);
            if (updatePos == null) throw new BussinessException($"Cant find POS: {pos.PosId}");
            //Check Manager
            await CheckUser(pos.BDS?.UserId, RoleEnum.BDS, true);

            updatePos.PosCode = pos.PosCode;
            updatePos.PosName = pos.PosName;
            updatePos.Address = pos.Address;
            updatePos.UserId = pos.BDS.UserId;
            updatePos.Phone = pos.Phone;
            //Replace with updated shifts
            updatePos.PosShift.Clear();
            pos.Shifts.ForEach(s => updatePos.PosShift.Add(new PosShift() { ShiftId = s.ShiftId }));
            await DbContext.SaveChangesAsync();
        }

        private void BasicCheck(PosDTO pos)
        {
            if (pos == null) throw new ArgumentNullException();
            //Check for null
            if (string.IsNullOrEmpty(pos.PosName)) throw new BussinessException($"Missing value: {nameof(Pos.PosName)}");
            if (string.IsNullOrEmpty(pos.PosCode)) throw new BussinessException($"Missing value: {nameof(Pos.PosCode)}");
            if (pos.Shifts == null || !pos.Shifts.Any()) throw new BussinessException("POS must have at least 1 shift");
            if (string.IsNullOrEmpty(pos.Address)) throw new BussinessException($"Missing value: {nameof(Pos.Address)}");
            if (string.IsNullOrEmpty(pos.Phone)) throw new BussinessException($"Missing value: {nameof(Pos.Phone)}");
            if (pos.BDS == null) throw new BussinessException("Pos must have BDS");
        }
    }
}
