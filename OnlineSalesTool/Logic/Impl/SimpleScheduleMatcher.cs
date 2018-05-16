using NLog;
using OnlineSalesTool.EFModel;
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace OnlineSalesTool.Logic.Impl
{
    public class SimpleScheduleMatcher : IScheduleMatcher
    {
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
        public bool GetUserMatchedSchedule(string posCode, DateTime date, out IEnumerable<int> matchUserId, out string reason)
        {
            if (string.IsNullOrEmpty(posCode)) throw new ArgumentNullException();
            if(date == default(DateTime)) throw new ArgumentNullException();
            //Init vars
            reason = string.Empty;
            matchUserId = null;
            var timeOfDate = date.TimeOfDay;

            _logger.Trace($"Try assigning for {nameof(posCode)}: {posCode} at: {timeOfDate}");
            //Find shift schedule of this POS that match current system time
            //var shiftSchedules = _getShiftSchedule.Invoke(_context, today, posCode);

            //BUG: Cant merge this query and with out using ToList
            //Will cause "the multi-part identifier could not be bound"
            //Workaround is to include shiftdetail then filter on that list
            var detail = _context.PosSchedule
                .Where(s => s.Pos.PosCode == posCode)
                .Where(s => s.MonthYear.Date == new DateTime(date.Year, date.Month, 1))
                .SelectMany(s => s.ScheduleDetail.Where(d => d.Day == date.Day))
                .Include(s => s.Shift)
                    .ThenInclude(sd => sd.ShiftDetail).ToList();
            _logger.Trace($"Found {detail.Count()} {nameof(ScheduleDetail)} of {date.Date.ToString("MM/yyyy")}");
            //Match specific time
            var matched = detail.Where(s => s.Shift.ShiftDetail.Any(d => timeOfDate >= d.StartAt && timeOfDate <= d.EndAt));

            if (!matched.Any())
            {
                reason = $"Cant find any {nameof(ScheduleDetail)} for {nameof(posCode)}: {posCode}, on: {date}";
                return false;
            }
            _logger.Trace($"Detail matched specific time {timeOfDate}: {matched.Count()}");
            matchUserId = matched.Select(m => m.UserId).Distinct();
            return true;
        }
    }
}
