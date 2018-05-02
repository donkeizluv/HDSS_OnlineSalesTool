using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineSalesTool.POCO
{
    public class ShiftSchedulePOCO
    {
        private DateTime _shiftDate;
        public int Day => _shiftDate.Day;
        public DateTime ShiftDate { get => _shiftDate.Date; set => _shiftDate = value; }
        //public int UserId { get; set; }
        public AppUserPOCO User { get; set; }
        public ShiftPOCO Shift { get; set; }
        //Since these dont come alone so no need to have PosId individually
        //public int PosId { get; set; }
    }
}
