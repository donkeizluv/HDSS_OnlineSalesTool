using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineSalesTool.DTO
{
    public class ScheduleDetailDTO
    {
        public int Day { get; set; }
        //public int UserId { get; set; }
        public AppUserDTO User { get; set; }
        public ShiftDTO Shift { get; set; }
        //Since these dont come alone so no need to have PosId individually
        //public int PosId { get; set; }
    }
}
