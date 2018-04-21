using System;
using System.Collections.Generic;

namespace OnlineSalesTool.EFModel
{
    public partial class Pos
    {
        public Pos()
        {
            PosShift = new HashSet<PosShift>();
            ShiftSchedule = new HashSet<ShiftSchedule>();
        }

        public int PosId { get; set; }
        public string PosName { get; set; }
        public string PosCode { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public int UserId { get; set; }

        public AppUser User { get; set; }
        public ICollection<PosShift> PosShift { get; set; }
        public ICollection<ShiftSchedule> ShiftSchedule { get; set; }
    }
}
