using OnlineSalesCore.Models;
using System.Collections.Generic;

namespace OnlineSalesCore.DTO
{
    public class PosDTO
    {
        public PosDTO()
        {

        }
        public PosDTO(Pos pos)
        {
            PosId = pos.PosId;
            PosCode = pos.PosCode;
            PosName = pos.PosName;
            Phone = pos.Phone;
            Address = pos.Address;
        }
        public int PosId { get; set; }
        public string PosCode { get; set; }
        public string PosName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public AppUserDTO BDS { get; set; }
        public IEnumerable<ShiftDTO> Shifts { get; set; }

        //Assigner related stuff
        public IEnumerable<PosScheduleDTO> PreviousMonthSchedules { get; set; }
        public bool HasCurrentMonthSchedule { get; set; }
        
    }
}
