using System;
using System.Collections.Generic;

namespace OnlineSalesTool.EFModel
{
    public partial class PosShift
    {
        public int PosId { get; set; }
        public int ShiftId { get; set; }

        public Pos Pos { get; set; }
        public Shift Shift { get; set; }
    }
}
