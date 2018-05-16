using System;
using System.Collections.Generic;

namespace SyncEngine.EFModel
{
    public partial class ShiftSchedule
    {
        public DateTime ShiftDate { get; set; }
        public int PosId { get; set; }
        public int UserId { get; set; }
        public int ShiftId { get; set; }

        public Pos Pos { get; set; }
        public Shift Shift { get; set; }
        public AppUser User { get; set; }
    }
}
