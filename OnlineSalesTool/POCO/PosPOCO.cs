using OnlineSalesTool.Logic;
using System.Collections.Generic;

namespace OnlineSalesTool.POCO
{
    public class PosPOCO
    {
        public int PosId { get; set; }
        public string PosCode { get; set; }
        public string PosName { get; set; }
        public IEnumerable<ShiftPOCO> Shifts { get; set; }
        //Limit this set to x nearest month
        public IEnumerable<ScheduleContainer> PreviousMonthSchedule { get; set; }

        public bool HasCurrentMonthSchedule { get; set; }
    }
}
