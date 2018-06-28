using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OnlineSalesCore.DTO;
using OnlineSalesCore.EFModel;
using OnlineSalesCore.Exceptions;
using OnlineSalesCore.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineSalesCore.Service
{
    public class ScheduleService : ContextAwareService, IScheduleService
    {
        //Number of prev sche to return in VM
        public const int NearestMonthScheduleTake = 5;
        private readonly ILogger<ScheduleService> _logger;

        public ScheduleService(OnlineSalesContext context, IHttpContextAccessor httpContext, ILogger<ScheduleService> logger)
            : base(httpContext, context) 
        { 
            _logger = logger;
        }

        public async Task<ShiftAssignerVM> Get()
        {
            switch (Role)
            {
                case Const.RoleEnum.CA:
                    return await CreateVM_CA();
                case Const.RoleEnum.BDS:
                    return await CreateVM_BDS();
                case Const.RoleEnum.ASM:
                    throw new NotImplementedException();
                case Const.RoleEnum.ADMIN: //See all
                    return await CreateVM_BDS();
                default:
                    throw new InvalidOperationException();
            }
        }
        #region Create VM
        private async Task<ShiftAssignerVM> CreateVM_CA()
        {
            //CA can see his manager POSs' schedules
            var now = DateTime.Now;
            var manId = await DbContext.AppUser
                .Where(u => u.UserId == UserId)
                .Select(u => u.ManagerId ?? -1).FirstOrDefaultAsync();
            //manId == 1 results in empty POSs, prev schedules vm
            var vm = new ShiftAssignerVM()
            {
                //CA cant have anyone under his management
                Users = new AppUserDTO[] { },
                //Get all POS under management
                POSs = await DbContext.Pos.Where(u => u.UserId == manId)
                .Select(p => new PosDTO(p) {
                    Shifts = p.PosShift
                            .Select(ps => ps.Shift)
                            .Select(s => new ShiftDTO(s)),
                    PreviousMonthSchedules = p.PosSchedule
                    .OrderByDescending(g => g.MonthYear.Year)
                    .ThenByDescending(g => g.MonthYear.Month)
                    .Take(NearestMonthScheduleTake)
                    .Select(g => new PosScheduleDTO() {
                        PosScheduleId = g.PosScheduleId,
                        MonthYear = g.MonthYear
                    }),
                    HasCurrentMonthSchedule = p.PosSchedule.Any(ps => ps.MonthYear.Year == now.Year 
                                                && ps.MonthYear.Month == now.Month)
                }).ToListAsync(),
                //Current SYS Month/Year
                SystemMonthYear = now
            };
            return vm;
        }
        private async Task<ShiftAssignerVM> CreateVM_BDS()
        {
            //BDS can see his POSs' schedules
            var now = DateTime.Now;
            var vm = new ShiftAssignerVM()
            {
                //Get all ids under management
                Users = await DbContext.AppUser
                .Where(u => u.ManagerId == UserId)
                .Select(u => new AppUserDTO(u)).ToListAsync(),
                //Get all POS under management
                POSs = await DbContext.Pos.Where(p => p.UserId == UserId)
                .Select(p => new PosDTO(p) {
                    Shifts = p.PosShift
                            .Select(ps => ps.Shift)
                            .Select(s => new ShiftDTO(s)),
                    PreviousMonthSchedules = p.PosSchedule
                    .OrderByDescending(g => g.MonthYear.Year)
                    .ThenByDescending(g => g.MonthYear.Month)
                    .Take(NearestMonthScheduleTake)
                     .Select(g => new PosScheduleDTO() {
                         PosScheduleId = g.PosScheduleId,
                         MonthYear = g.MonthYear
                     }),
                    HasCurrentMonthSchedule = p.PosSchedule.Any(ps => ps.MonthYear.Year == now.Year && ps.MonthYear.Month == now.Month)
                }).AsNoTracking().ToListAsync(),
                //Current SYS Month/Year
                SystemMonthYear = now
            };
            return vm;
        }
        #endregion
        /// <summary>
        /// Check if schedule sastisfies bussines logic
        /// </summary>
        /// <param name="scheduleContainer"></param>
        /// <returns></returns>
        private async Task ThrowIfCheckFail(ScheduleContainer scheduleContainer)
        {
            var userId = UserId;
            if (scheduleContainer == null) throw new ArgumentNullException($"Param: {nameof(scheduleContainer)} is null.");
            var emptyDefine = scheduleContainer.Schedules.FirstOrDefault(s => s.Shift == null || s.User == null);
            if(emptyDefine != null)
            {
                throw new BussinessException($"Some shift details missing shift/user definition");
            }
            //Check format valid
            if (!scheduleContainer.CheckBasicFormat(out string formatReason))
            {
                throw new BussinessException(formatReason);
            }
            //Every day of month must have shifts defined
            var missing = scheduleContainer.DaysInMonthRange.Select(d => d.Day).Except(scheduleContainer.Schedules.Select(s => s.Day));
            if (missing.Any())
            {
                throw new BussinessException($"Missing shift for days: {string.Concat(missing.Select(s => s.ToString("dd") + " "))}");
            }
            //Check logic/bussiness valid
            //Find any dupicate of same ShiftDate-ShiftId-UserId
            var dupFound =
                from s in scheduleContainer.Schedules
                group s by new {
                    s.Day,
                    s.Shift.ShiftId,
                    s.User.UserId
                } into g
                select g;
            int dupCount = dupFound.Count(g => g.Count() > 1);
            if (dupCount > 0)
            {
                throw new BussinessException($"Duplicate of exact same detail count: {dupCount}");
            }
            //Check if this POS is manage by this user
            if(!DbContext.Pos.Any(p => p.PosId == scheduleContainer.TargetPos))
            {
                throw new BussinessException($"Target pos: {scheduleContainer.TargetPos} is not managed by user id: {userId}");
            }
            //Get all shift of POS
            var shiftsOfPos = await DbContext.PosShift
                .Where(ps => ps.PosId == scheduleContainer.TargetPos)
                .Select(ps => ps.Shift)
                .AsNoTracking()
                .ToListAsync();
            if (!shiftsOfPos.Any())
            {
                throw new BussinessException($"Cant find any shifts of target pos: {scheduleContainer.TargetPos}");
            }
            var groupByDate = scheduleContainer.Schedules.GroupBy(s => s.Day);
            //Every day must have number shift defined equals to number of shift the POS has
            //For ex: POS123 have 4 shift, every day of schedule must have 4 shift schedule defined
            //Also if detail contains ShiftId that are not defined in PosShift of the POS then -> not valid
            //All shift ids of POS must have 1 match
            //ShiftId of POS except shiftid of day if any then not all are defined
            if (groupByDate.Any(g => shiftsOfPos
                .Select(shiftId2 => shiftId2.ShiftId)
                .Except(g.Select(s => s.Shift.ShiftId))
                .Any()))
            {
                throw new BussinessException($"Some shift ids are not valid or POS does not have this shift");
            }
            //Check all user id of shift detail are managed by current user
            //Get all user under this user management
            var managedUserIds = await DbContext.AppUser
                .Where(u => u.ManagerId == userId)
                .Select(u => u.UserId)
                .ToListAsync();
            if(!managedUserIds.Any())
            {
                throw new BussinessException($"UserId: {userId} have no users under management");
            }
            //Group by all user ids of schedule
            var groupByUserId = scheduleContainer.Schedules
                                .GroupBy(s => s.User.UserId)
                                .Select(u => u.First().User.UserId);
            var notUnderManaged = groupByUserId.Except(managedUserIds);
            if(notUnderManaged.Any())
            {
                throw new BussinessException($"User ids: {string.Concat(notUnderManaged.Select(u => u + " "))} are not managed by: {userId}");
            }
            //Check if this specific schedule Month/Year has been defined
            if (DbContext.PosSchedule.Any(
                s => s.Pos.PosId == scheduleContainer.TargetPos &&
                s.MonthYear.Month == scheduleContainer.MonthYear.Month &&
                s.MonthYear.Year == scheduleContainer.MonthYear.Year))
            {
                throw new BussinessException($"Schedule: {scheduleContainer.MonthYear.ToString("MM/yyyy")} of POS: {scheduleContainer.TargetPos} has already been defined");
            }
            //Clean
        }

        public async Task<int> Create(ScheduleContainer schedule)
        {
            if (schedule == null) throw new ArgumentNullException();
            await ThrowIfCheckFail(schedule);
            var posSchedule = schedule.ToPosSchedule();
            //User submited schedule
            posSchedule.AutoFill = false;
            posSchedule.SubmitTime = DateTime.Now;
            await DbContext.PosSchedule.AddAsync(posSchedule);
            await DbContext.SaveChangesAsync();
            return posSchedule.PosScheduleId;
        }

        public async Task<IEnumerable<ScheduleDetailDTO>> GetDetail(int posScheduleId)
        {
            var posSchedule = await (DbContext.PosSchedule
                .Where(ps => ps.PosScheduleId == posScheduleId)
                .Include(ps => ps.ScheduleDetail)
                    .ThenInclude(sd => sd.Shift)
                .Include(ps => ps.ScheduleDetail)
                    .ThenInclude(ps => ps.User)
                .SingleOrDefaultAsync());
            return posSchedule.ScheduleDetail
                .OrderBy(sd => sd.Shift.DisplayOrder)
                .Select(sd => new ScheduleDetailDTO() {
                    Day = sd.Day,
                    Shift = new ShiftDTO(sd.Shift),
                    User = new AppUserDTO(sd.User)
                }
            );
        }
    }
}
