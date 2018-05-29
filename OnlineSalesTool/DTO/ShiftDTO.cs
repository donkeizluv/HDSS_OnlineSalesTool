using OnlineSalesTool.EFModel;
using System.Collections.Generic;

namespace OnlineSalesTool.DTO
{
    public class ShiftDTO
    {
        public ShiftDTO()
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
