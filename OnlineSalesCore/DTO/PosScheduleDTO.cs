using System;

namespace OnlineSalesCore.DTO
{
    /// <summary>
    /// For displaying in assigner
    /// </summary>
    public class PosScheduleDTO
    {
        public PosScheduleDTO()
        {

        }
        //public PosSchedulePOCO(IEnumerable<ScheduleDetailPOCO> schedules, DateTime monthYear)
        //{
        //    MonthYear = monthYear;
        //    Schedules = schedules;
        //}
        //Lazy load this
        //public IEnumerable<ScheduleDetailPOCO> Schedules { get; set; }
        public int PosScheduleId { get; set; }
        public DateTime MonthYear { get; set; }
        public string DisplayMonthYear => MonthYear.ToString("MM-yyyy");
        public int TotalDaysOfMonth { get => DateTime.DaysInMonth(MonthYear.Year, MonthYear.Month); }
    }
}
