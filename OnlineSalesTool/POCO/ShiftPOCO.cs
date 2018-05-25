using OnlineSalesTool.EFModel;
using System.Collections.Generic;

namespace OnlineSalesTool.POCO
{
    public class ShiftPOCO
    {
        public ShiftPOCO()
        {

        }
        //public ShiftPOCO(ScheduleDetail detail)
        //{
        //    ShiftId = detail.ShiftId;
        //    Name = detail.Shift.Name;
        //}
        public string Name { get; set; }
        public int ShiftId { get; set; }
        //public IEnumerable<ShiftDetailPOCO> ShiftDetails { get; set; }
    }
}
