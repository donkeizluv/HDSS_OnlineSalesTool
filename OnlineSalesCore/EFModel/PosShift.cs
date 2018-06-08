using System;
using System.Collections.Generic;

namespace OnlineSalesCore.EFModel
{
    public partial class PosShift
    {
        public int PosId { get; set; }
        public int ShiftId { get; set; }

        public Pos Pos { get; set; }
        public Shift Shift { get; set; }
    }
}
