using System;
using System.Collections.Generic;

namespace OnlineSalesTool.EFModel
{
    public partial class Shift
    {
        public Shift()
        {
            PosShift = new HashSet<PosShift>();
            ScheduleDetail = new HashSet<ScheduleDetail>();
            ShiftDetail = new HashSet<ShiftDetail>();
        }

        public int ShiftId { get; set; }
        public string Name { get; set; }
        public int? DisplayOrder { get; set; }

        public ICollection<PosShift> PosShift { get; set; }
        public ICollection<ScheduleDetail> ScheduleDetail { get; set; }
        public ICollection<ShiftDetail> ShiftDetail { get; set; }
    }
}
