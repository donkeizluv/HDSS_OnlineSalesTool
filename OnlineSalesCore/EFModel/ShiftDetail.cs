using System;
using System.Collections.Generic;

namespace OnlineSalesCore.EFModel
{
    public partial class ShiftDetail
    {
        public int ShiftDetailId { get; set; }
        public int ShiftId { get; set; }
        public TimeSpan StartAt { get; set; }
        public TimeSpan EndAt { get; set; }

        public Shift Shift { get; set; }
    }
}
