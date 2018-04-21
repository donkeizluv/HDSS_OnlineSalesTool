using OnlineSalesTool.Logic;
using NLog;
using OnlineSalesTool.EFModel;
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace OnlineSalesTool.Logic.Impl
{
    //TODO: Test
    public class SimpleScheduleMatcher : IScheduleMatcher
    {
        //Not needed yet
        //private static Func<OnlineSalesContext, DateTime, string, IEnumerable<ShiftSchedule>> _getShiftSchedule =
        //   EF.CompileQuery((OnlineSalesContext context, DateTime day, string posCode) =>
        //       context.ShiftSchedule
        //        .Where(p => p.ShiftDate.Date == day.Date)
        //        .Include(p => p.Pos)
        //        .Where(p => p.Pos.PosCode == posCode)
        //        .Include(shift => shift.Shift)
        //            .ThenInclude(detail => detail.ShiftDetail)
        //        .Where(s => s.Shift.ShiftDetail.Any(d => day.TimeOfDay >= d.StartAt && day.TimeOfDay <= d.EndAt)));

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly OnlineSalesContext _context;

        public SimpleScheduleMatcher(OnlineSalesContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Try to assign this order to CA of pre-scheduled shift, base on this order's PosCode
        /// </summary>
        /// <param name="order"></param>
        /// <param name="user"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public bool GetUserMatchedSchedule(string posCode, out IEnumerable<int> matchUserId, out string reason)
        {
            reason = string.Empty;
            matchUserId = null;
            var today = DateTime.Now;
            var timeOfday = today.TimeOfDay;

            if (string.IsNullOrEmpty(posCode)) throw new ArgumentNullException();
            _logger.Trace($"Try assigning for {nameof(posCode)}: {posCode} at: {timeOfday}");
            //Find shift schedule of this POS that match current system time
            //var shiftSchedules = _getShiftSchedule.Invoke(_context, today, posCode);

            //No need to include if no ref to those entity need at the end of this db context
            var shiftSchedules = _context.ShiftSchedule
                .Where(p => p.ShiftDate.Date == today.Date)
                //.Include(p => p.Pos)
                .Where(p => p.Pos.PosCode == posCode)
                //.Include(shift => shift.Shift)
                //    .ThenInclude(detail => detail.ShiftDetail)
                .Where(s => s.Shift.ShiftDetail.Any(d => timeOfday >= d.StartAt && timeOfday <= d.EndAt)).AsNoTracking().ToList();

            if (shiftSchedules.Count() < 1)
            {
                reason = $"Cant find any {nameof(ShiftSchedule)} for {nameof(posCode)}: {posCode}, on: {today}";
                return false;
            }
            _logger.Trace($"Matched schedule found: {shiftSchedules.Count()}");
            matchUserId = shiftSchedules.Select(s => s.UserId).Distinct();
            return true;
        }
    }
}
