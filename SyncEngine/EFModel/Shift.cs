using System;
using System.Collections.Generic;

namespace SyncEngine.EFModel
{
    public partial class Shift
    {
        public Shift()
        {
            PosShift = new HashSet<PosShift>();
            ShiftDetail = new HashSet<ShiftDetail>();
            ShiftSchedule = new HashSet<ShiftSchedule>();
        }

        public int ShiftId { get; set; }
        public string Name { get; set; }

        public ICollection<PosShift> PosShift { get; set; }
        public ICollection<ShiftDetail> ShiftDetail { get; set; }
        public ICollection<ShiftSchedule> ShiftSchedule { get; set; }
    }
}
