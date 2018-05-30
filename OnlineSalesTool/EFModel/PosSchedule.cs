using System;
using System.Collections.Generic;

namespace OnlineSalesTool.EFModel
{
    public partial class PosSchedule
    {
        public PosSchedule()
        {
            ScheduleDetail = new HashSet<ScheduleDetail>();
        }

        public int PosScheduleId { get; set; }
        public DateTime MonthYear { get; set; }
        public int PosId { get; set; }
        public DateTime SubmitTime { get; set; }
        public bool AutoFill { get; set; }

        public Pos Pos { get; set; }
        public ICollection<ScheduleDetail> ScheduleDetail { get; set; }
    }
}
