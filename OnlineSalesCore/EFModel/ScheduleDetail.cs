using System;
using System.Collections.Generic;

namespace OnlineSalesCore.EFModel
{
    public partial class ScheduleDetail
    {
        public int Day { get; set; }
        public int PosScheduleId { get; set; }
        public int UserId { get; set; }
        public int ShiftId { get; set; }

        public PosSchedule PosSchedule { get; set; }
        public Shift Shift { get; set; }
        public AppUser User { get; set; }
    }
}
