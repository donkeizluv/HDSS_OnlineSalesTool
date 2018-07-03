using System;
using System.Collections.Generic;

namespace OnlineSalesCore.Models
{
    public partial class Pos
    {
        public Pos()
        {
            OnlineOrder = new HashSet<OnlineOrder>();
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
        public ICollection<OnlineOrder> OnlineOrder { get; set; }
        public ICollection<PosSchedule> PosSchedule { get; set; }
        public ICollection<PosShift> PosShift { get; set; }
    }
}
