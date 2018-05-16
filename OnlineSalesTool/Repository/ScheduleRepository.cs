using Microsoft.EntityFrameworkCore;
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

namespace OnlineSalesTool.Repository
{
    public class ScheduleRepository : BaseRepo, IScheduleRepository
    {
        //Number of prev sche to return in VM
        public const int NearestMonthScheduleTake = 3;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public ScheduleRepository(OnlineSalesContext context, IUserResolver userResolver)
            : base(userResolver.GetPrincipal(), context) { }

        public async Task<ShiftAssignerViewModel> CreateAssignerVM()
        {
            var now = DateTime.Now;
            var vm = new ShiftAssignerViewModel()
            {
                //Get all ids under management
                Users = await DbContext.AppUser
                .Where(u => u.ManagerId == PrincipalUserId)
                .Select(u => new AppUserPOCO() {
                    UserId = u.UserId,
                    DisplayName = $"{u.Name} - {u.Hr}"
                }).AsNoTracking().ToListAsync(),
                //Get all POS under management
                POSs = await DbContext.Pos.Where(p => p.UserId == PrincipalUserId)
                .Select(p => new PosPOCO() {
                    PosCode = p.PosCode,
                    PosId = p.PosId,
                    PosName = p.PosName,
                    //Get all shifts of this POS
                    Shifts = p.PosShift
                    .Select(ps => new ShiftPOCO() {
                        Name = ps.Shift.Name,
                        ShiftId = ps.ShiftId,
                        //Get all shift details of this Shift
                        //ShiftDetails = ps.Shift.ShiftDetail
                        //.Select(d => new ShiftDetailPOCO() {
                        //    StartAt = d.StartAt, EndAt = d.EndAt
                        //})
                    }),
                    PreviousMonthSchedules = p.PosSchedule
                    .OrderByDescending(g => g.MonthYear.Year)
                    .ThenByDescending(g => g.MonthYear.Month)
                    .Take(NearestMonthScheduleTake)
                    .Select(g => new PosSchedulePOCO(
                            g.ScheduleDetail.Select(gg => new ScheduleDetailPOCO(){
                                        Day = gg.Day,
                                        Shift = new ShiftPOCO() { Name = gg.Shift.Name, ShiftId = gg.ShiftId },
                                        User = new AppUserPOCO()
                                        {
                                            UserId = gg.User.UserId,
                                            DisplayName = $"{gg.User.Name} - {gg.User.Hr}"
                                        }}), new DateTime(g.MonthYear.Year, g.MonthYear.Month, 1))),
                    HasCurrentMonthSchedule = p.PosSchedule.Any(ps => ps.MonthYear.Year == now.Year && ps.MonthYear.Month == now.Month)
                }).AsNoTracking().ToListAsync(),
                //Current SYS Month/Year
                SystemMonthYear = now
            };
            return vm;
        }

        /// <summary>
        /// Check if schedule sastisfies bussines logic
        /// </summary>
        /// <param name="scheduleContainer"></param>
        /// <returns></returns>
        private async Task ThrowIfCheckFail(ScheduleContainer scheduleContainer)
        {
            var userId = PrincipalUserId;
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

        public async Task SaveSchedule(ScheduleContainer schedule)
        {
            if (schedule == null) throw new ArgumentNullException();
            await ThrowIfCheckFail(schedule);
            await DbContext.PosSchedule.AddAsync(schedule.ToPosSchedule());
            await DbContext.SaveChangesAsync();
        }
    }
}
