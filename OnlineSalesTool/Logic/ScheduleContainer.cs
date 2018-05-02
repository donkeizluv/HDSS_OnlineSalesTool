using Newtonsoft.Json;
using OnlineSalesTool.EFModel;
using OnlineSalesTool.POCO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineSalesTool.Logic
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
        public IEnumerable<ShiftSchedulePOCO> Schedules { get; }
        public int TargetPos { get; private set; }

        [JsonConstructor]
        public ScheduleContainer(int targetPos, DateTime targetMonthYear, IEnumerable<ShiftSchedulePOCO> schedules)
        {
            MonthYear = targetMonthYear;
            Schedules = schedules;
            TargetPos = targetPos;
        }

        public IEnumerable<ShiftSchedule> ToShiftSchedules()
        {
            return Schedules.Select(p => new ShiftSchedule()
            {
                ShiftDate = p.ShiftDate,
                PosId = TargetPos,
                ShiftId = p.Shift.ShiftId,
                UserId = p.User.UserId
            });
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
            //All shift dates must be same month / year with MonthYear
            if (Schedules.Any(s => s.ShiftDate.Month != MonthYear.Month || s.ShiftDate.Year != MonthYear.Year))
            {
                reason = $"Month/Year of some shift details are different from MonthYear of this schedule";
                return false;
            }
            return true;
        }
    }
}
