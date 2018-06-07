using OnlineSalesTool.EFModel;
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace OnlineSalesTool.Logic.Impl
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
            _logger.LogTrace($"Try assigning for {nameof(posCode)}: {posCode} at: {timeOfDate}");
            var detail = await _context.PosSchedule
                .Where(s => s.Pos.PosCode == posCode && s.MonthYear.Date == new DateTime(date.Year, date.Month, 1))
                .SelectMany(s => s.ScheduleDetail.Where(d => d.Day == date.Day))
                .Include(s => s.Shift)
                    .ThenInclude(sd => sd.ShiftDetail).ToListAsync();
            _logger.LogTrace($"Found {detail.Count()} {nameof(ScheduleDetail)} of {date.Date.ToString("MM/yyyy")}");
            //Match specific time
            var matched = detail.Where(s => s.Shift.ShiftDetail.Any(d => timeOfDate >= d.StartAt && timeOfDate <= d.EndAt));

            if (!matched.Any())
            {
                var reason = $"Cant find any {nameof(ScheduleDetail)} for {nameof(posCode)}: {posCode}, on: {date}";
                return (false, null, reason);
            }
            _logger.LogTrace($"Detail matched specific time {timeOfDate}: {matched.Count()}");
            return (true, matched.Select(m => m.UserId).Distinct().ToList(), string.Empty);
        }
    }
}
