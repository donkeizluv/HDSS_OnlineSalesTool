using System;
using System.Threading.Tasks;
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
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace OnlineSalesTool.Service
{
    public class PosService : ServiceBase, IPosService
    {
        private readonly ILogger<PosService> _logger;
        private readonly ListQuery<Pos, PosDTO> _query;
        private readonly IRoleCache _roleCache;

        public PosService(OnlineSalesContext context,
            IHttpContextAccessor httpContext,
            IRoleCache roleCache,
            ILogger<PosService> logger,
            ListQuery<Pos, PosDTO> q)
            : base(httpContext, context)
        {
            _logger = logger;
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
            vm.Shifts = await DbContext.Shift
                .OrderBy(s => s.DisplayOrder)
                .Select(s => new ShiftDTO(s)).ToListAsync();
            return vm;
        }
        
        public async Task<int> Create(PosDTO pos)
        {
            BasicCheck(pos ?? throw new ArgumentNullException());
            //Check Manager
            await CheckUser(pos.BDS?.UserId, RoleEnum.BDS, true);
            var newPos = new Pos()
            {
                PosCode = pos.PosCode.ToUpper(),
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

        public async Task Update(PosDTO posDto)
        {
            BasicCheck(posDto ?? throw new ArgumentNullException());
            var updatePos = await DbContext.Pos
                .Where(p => p.PosId == posDto.PosId)
                .Include(p => p.PosShift)
                .SingleOrDefaultAsync();
            if (updatePos == null) throw new BussinessException($"Cant find POS: {posDto.PosId}");
            //If changes BDS then check BDS
            if(posDto.BDS.UserId != updatePos.UserId)
                await CheckUser(posDto.BDS?.UserId, RoleEnum.BDS, true);
            updatePos.PosCode = posDto.PosCode;
            updatePos.PosName = posDto.PosName;
            updatePos.Address = posDto.Address;
            updatePos.UserId = posDto.BDS.UserId;
            updatePos.Phone = posDto.Phone;
            //Replace with updated shifts
            updatePos.PosShift.Clear();
            posDto.Shifts.ForEach(s => updatePos.PosShift.Add(new PosShift() { ShiftId = s.ShiftId }));
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
        public async Task<int> CheckCode(string posCode)
        {
            var pos = await DbContext.Pos.FirstOrDefaultAsync(p => p.PosCode == posCode);
            if(pos == null) return -1;
            return pos.PosId;
        }
    }
}
