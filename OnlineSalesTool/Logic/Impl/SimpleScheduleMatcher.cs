using NLog;
using OnlineSalesTool.EFModel;
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

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

        public async Task<(bool, List<int>, string)> GetUserMatchedSchedule(string posCode, DateTime date)
        {
            if (string.IsNullOrEmpty(posCode)) throw new ArgumentNullException();
            if (date == default(DateTime)) throw new ArgumentNullException();

            var timeOfDate = date.TimeOfDay;
            _logger.Trace($"Try assigning for {nameof(posCode)}: {posCode} at: {timeOfDate}");
            //BUG: Cant merge this query and with out using ToList
            //Will cause "the multi-part identifier could not be bound"
            //Workaround is to include shiftdetail then filter on that list
            var detail = await _context.PosSchedule
                .Where(s => s.Pos.PosCode == posCode)
                .Where(s => s.MonthYear.Date == new DateTime(date.Year, date.Month, 1))
                .SelectMany(s => s.ScheduleDetail.Where(d => d.Day == date.Day))
                .Include(s => s.Shift)
                    .ThenInclude(sd => sd.ShiftDetail).ToListAsync();
            _logger.Trace($"Found {detail.Count()} {nameof(ScheduleDetail)} of {date.Date.ToString("MM/yyyy")}");
            //Match specific time
            var matched = detail.Where(s => s.Shift.ShiftDetail.Any(d => timeOfDate >= d.StartAt && timeOfDate <= d.EndAt));

            if (!matched.Any())
            {
                var reason = $"Cant find any {nameof(ScheduleDetail)} for {nameof(posCode)}: {posCode}, on: {date}";
                return (false, null, reason);
            }
            _logger.Trace($"Detail matched specific time {timeOfDate}: {matched.Count()}");
            return (true, matched.Select(m => m.UserId).Distinct().ToList(), string.Empty);
        }
    }
}
