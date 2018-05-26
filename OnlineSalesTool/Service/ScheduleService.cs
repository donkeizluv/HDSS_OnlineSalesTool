﻿using Microsoft.EntityFrameworkCore;
using NLog;
using OnlineSalesTool.CustomException;
using OnlineSalesTool.EFModel;
using OnlineSalesTool.Logic;
using OnlineSalesTool.POCO;
using OnlineSalesTool.Service;
using OnlineSalesTool.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineSalesTool.Service
{
    public class ScheduleService : ServiceBase, IScheduleService
    {
        //Number of prev sche to return in VM
        public const int NearestMonthScheduleTake = 3;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public ScheduleService(OnlineSalesContext context, IUserResolver userResolver)
            : base(userResolver.GetPrincipal(), context) { }

        public async Task<ShiftAssignerViewModel> Get()
        {
            switch (Role)
            {
                case AppEnum.RoleEnum.CA:
                    return await CreateVM_CA();
                case AppEnum.RoleEnum.BDS:
                    return await CreateVM_BDS();
                case AppEnum.RoleEnum.ASM:
                    throw new NotImplementedException();
                case AppEnum.RoleEnum.ADMIN: //See all
                    return await CreateVM_BDS();
                default:
                    throw new InvalidOperationException();
            }
        }
        #region Create VM
        private async Task<ShiftAssignerViewModel> CreateVM_CA()
        {
            //CA can see his manager POSs' schedules
            var now = DateTime.Now;
            var manId = await DbContext.AppUser
                .Where(u => u.UserId == UserId)
                .Select(u => u.ManagerId ?? -1).FirstOrDefaultAsync();
            //manId == 1 results in empty POSs, prev schedules vm
            var vm = new ShiftAssignerViewModel()
            {
                //CA cant have anyone under his management
                Users = new AppUserPOCO[] { },
                //Get all POS under management
                POSs = await DbContext.Pos.Where(u => u.UserId == manId)
                .Select(p => new PosPOCO(p) {
                    Shifts = p.PosShift
                            .Select(ps => ps.Shift)
                            .Select(s => new ShiftPOCO() {
                                ShiftId = s.ShiftId,
                                Name = s.Name
                            }),
                    PreviousMonthSchedules = p.PosSchedule
                    .OrderByDescending(g => g.MonthYear.Year)
                    .ThenByDescending(g => g.MonthYear.Month)
                    .Take(NearestMonthScheduleTake)
                    .Select(g => new PosSchedulePOCO(
                            g.ScheduleDetail.Select(gg => new ScheduleDetailPOCO()
                            {
                                Day = gg.Day,
                                Shift = new ShiftPOCO() { Name = gg.Shift.Name, ShiftId = gg.ShiftId },
                                User = new AppUserPOCO(gg.User)
                            }), new DateTime(g.MonthYear.Year, g.MonthYear.Month, 1))),
                    HasCurrentMonthSchedule = p.PosSchedule.Any(ps => ps.MonthYear.Year == now.Year && ps.MonthYear.Month == now.Month)
                }).ToListAsync(),
                //Current SYS Month/Year
                SystemMonthYear = now
            };
            return vm;
        }
        private async Task<ShiftAssignerViewModel> CreateVM_BDS()
        {
            //BDS can see his POSs' schedules
            var now = DateTime.Now;
            var vm = new ShiftAssignerViewModel()
            {
                //Get all ids under management
                Users = await DbContext.AppUser
                .Where(u => u.ManagerId == UserId)
                .Select(u => new AppUserPOCO(u)).ToListAsync(),
                //Get all POS under management
                POSs = await DbContext.Pos.Where(p => p.UserId == UserId)
                .Select(p => new PosPOCO(p) {
                    Shifts = p.PosShift
                            .Select(ps => ps.Shift)
                            .Select(s => new ShiftPOCO() {
                                ShiftId = s.ShiftId,
                                Name = s.Name
                            }),
                    PreviousMonthSchedules = p.PosSchedule
                    .OrderByDescending(g => g.MonthYear.Year)
                    .ThenByDescending(g => g.MonthYear.Month)
                    .Take(NearestMonthScheduleTake)
                    .Select(g => new PosSchedulePOCO(
                            g.ScheduleDetail.Select(gg => new ScheduleDetailPOCO()
                            {
                                Day = gg.Day,
                                Shift = new ShiftPOCO() { Name = gg.Shift.Name, ShiftId = gg.ShiftId },
                                User = new AppUserPOCO(gg.User)
                            }), new DateTime(g.MonthYear.Year, g.MonthYear.Month, 1))),
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
            if (DbContext.PosSchedule.Any(s => s.Pos.PosId == scheduleContainer.TargetPos &&
            s.MonthYear.Month == scheduleContainer.MonthYear.Month &&
            s.MonthYear.Year == scheduleContainer.MonthYear.Year))
            {
                throw new BussinessException($"Schedule: {scheduleContainer.MonthYear.ToString("MM/yyyy")} of POS: {scheduleContainer.TargetPos} has already been defined");
            }
            //Clean
        }

        public async Task Create(ScheduleContainer schedule)
        {
            if (schedule == null) throw new ArgumentNullException();
            await ThrowIfCheckFail(schedule);
            await DbContext.PosSchedule.AddAsync(schedule.ToPosSchedule());
            await DbContext.SaveChangesAsync();
        }
    }
}