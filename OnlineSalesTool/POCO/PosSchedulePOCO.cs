using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineSalesTool.POCO
{
    /// <summary>
    /// For displaying in assigner
    /// </summary>
    public class PosSchedulePOCO
    {
        public PosSchedulePOCO()
        {

        }
        public PosSchedulePOCO(IEnumerable<ScheduleDetailPOCO> schedules, DateTime monthYear)
        {
            MonthYear = monthYear;
            Schedules = schedules;
        }
        public IEnumerable<ScheduleDetailPOCO> Schedules { get; set; }
        public DateTime MonthYear { get; set; }


        public string DisplayMonthYear => MonthYear.ToString("MM-yyyy");
        public int TotalDaysOfMonth { get => DateTime.DaysInMonth(MonthYear.Year, MonthYear.Month); }
    }
}
