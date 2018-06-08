using System;
using System.Collections.Generic;

namespace OnlineSalesCore.EFModel
{
    public partial class Pos
    {
        public Pos()
        {
            PosSchedule = new HashSet<PosSchedule>();
            PosShift = new HashSet<PosShift>();
        }

        public int PosId { get; set; }
        public string PosName { get; set; }
        public string PosCode { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public int UserId { get; set; }

        public AppUser User { get; set; }
        public ICollection<PosSchedule> PosSchedule { get; set; }
        public ICollection<PosShift> PosShift { get; set; }
    }
}
