using OnlineSalesTool.EFModel;
using OnlineSalesTool.Logic;
using System.Collections.Generic;
using System.Linq;

namespace OnlineSalesTool.POCO
{
    public class PosPOCO
    {
        public PosPOCO()
        {

        }
        public PosPOCO(Pos pos)
        {
            PosId = pos.PosId;
            PosCode = pos.PosCode;
            PosName = pos.PosName;
            Phone = pos.Phone;
            Address = pos.Address;
            //Doesnt work well with IQueryable bc these nav properties are always null
            //if (pos.User != null)
            //    BDS = new AppUserPOCO(pos.User);
            //if(pos.PosShift != null)
            //{
            //    Shifts = pos.PosShift
            //                .Select(ps => ps.Shift)
            //                .Select(s => new ShiftPOCO() {
            //                    ShiftId = s.ShiftId,
            //                    Name = s.Name
            //                });
            //}
        }
        public int PosId { get; set; }
        public string PosCode { get; set; }
        public string PosName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public AppUserPOCO BDS { get; set; }
        public IEnumerable<ShiftPOCO> Shifts { get; set; }

        //Assigner related stuff
        public IEnumerable<PosSchedulePOCO> PreviousMonthSchedules { get; set; }
        public bool HasCurrentMonthSchedule { get; set; }
        
    }
}
