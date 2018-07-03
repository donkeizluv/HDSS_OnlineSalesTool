using Newtonsoft.Json;
using OnlineSalesCore.DTO;
using OnlineSalesCore.Models;
using OnlineSalesCore.Extentions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OnlineSalesCore.ViewModels
{
    /// <summary>
    /// Container for all daily shift schedule of specific month. Use for create new schedule
    /// </summary>
    public class ScheduleContainer
    {
        //public bool IsValid { get => CheckBasicFormat(out var r); }

        public int TotalDaysOfMonth { get => DateTime.DaysInMonth(MonthYear.Year, MonthYear.Month); }

        public DateTime FirstDate { get => new DateTime(MonthYear.Year, MonthYear.Month, 1); }

        public DateTime LastDate { get => new DateTime(MonthYear.Year, MonthYear.Month, TotalDaysOfMonth); }

        [JsonIgnore]
        public IEnumerable<DateTime> DaysInMonthRange => Enumerable.Range(0, (int)(LastDate - FirstDate).TotalDays + 1)
                                                            .Select(i => FirstDate.AddDays(i));
        public DateTime MonthYear { get; private set; }
        public IEnumerable<ScheduleDetailDTO> Schedules { get; }
        public int TargetPos { get; private set; }

        [JsonConstructor]
        public ScheduleContainer(int targetPos, DateTime targetMonthYear, IEnumerable<ScheduleDetailDTO> schedules)
        {
            MonthYear = targetMonthYear;
            Schedules = schedules;
            TargetPos = targetPos;
        }

        public PosSchedule ToPosSchedule()
        {
            var ps =  new PosSchedule()
            {
                MonthYear = MonthYear,
                PosId = TargetPos
            };
            var details = Schedules.Select(d => new ScheduleDetail()
            {
                Day = d.Day,
                UserId = d.User.UserId,
                ShiftId = d.Shift.ShiftId
            });
            ps.ScheduleDetail.AddRange(details);
            return ps;
        }

        public bool CheckBasicFormat(out string reason)
        {
            reason = string.Empty;
            //Month-Year of this container must be set
            if(MonthYear == default(DateTime))
            {
                reason = $"{nameof(MonthYear)} is not set";
                return false;
            }
            //Since we dont care about Day part so Day must be 1
            if (MonthYear.Day != 1)
            {
                reason = $"{nameof(MonthYear)} Day must be 1";
                return false;
            }
            if(Schedules.Any(s => s.Day < 1 || s.Day > 31))
            {
                reason = $"Some Day of detail are not valid within Day range (1-31)";
                return false;
            }
            return true;
        }
    }
}
