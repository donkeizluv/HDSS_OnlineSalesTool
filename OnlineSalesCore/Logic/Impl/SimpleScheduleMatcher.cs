using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OnlineSalesCore.EFModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineSalesCore.Logic.Impl
{
    public class SimpleScheduleMatcher : IScheduleMatcher
    {
        private readonly ILogger<SimpleScheduleMatcher> _logger;
        private readonly OnlineSalesContext _context;

        public SimpleScheduleMatcher(OnlineSalesContext context, ILogger<SimpleScheduleMatcher> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<(bool, List<int>, string)> GetUserMatchedSchedule(string posCode, DateTime date)
        {
            if (string.IsNullOrEmpty(posCode)) throw new ArgumentNullException();
            if (date == default(DateTime)) throw new ArgumentNullException();
            var timeOfDate = date.TimeOfDay;
            _logger.LogDebug($"Try assigning for {nameof(posCode)}: {posCode} at: {timeOfDate}");
            var monthYear = new DateTime(date.Year, date.Month, 1);
            var schedule = await _context.PosSchedule
                .Where(s => s.Pos.PosCode == posCode && s.MonthYear.Date == monthYear)
                .SelectMany(s => s.ScheduleDetail.Where(d => d.Day == date.Day))
                .Include(s => s.Shift)
                    .ThenInclude(sd => sd.ShiftDetail).ToListAsync();
            if(!schedule.Any())
            {
                var reason = $"Cant find any {nameof(schedule)} for {nameof(posCode)}: {posCode}, on: {date}";
                return (false, null, reason);
            }
            _logger.LogDebug($"Found {schedule.Count()} {nameof(ScheduleDetail)} of {date.Date.ToString("MM/yyyy")}");
            //Match specific time
            var matched = schedule.Where(s => s.Shift.ShiftDetail.Any(d => timeOfDate >= d.StartAt && timeOfDate <= d.EndAt));

            if (!matched.Any())
            {
                var reason = $"Cant find any {nameof(ScheduleDetail)} for {nameof(posCode)}: {posCode}, on: {date}";
                return (false, null, reason);
            }
            _logger.LogDebug($"Detail matched specific time {timeOfDate}: {matched.Count()}");
            return (true, matched.Select(m => m.UserId).Distinct().ToList(), string.Empty);
        }
    }
}
