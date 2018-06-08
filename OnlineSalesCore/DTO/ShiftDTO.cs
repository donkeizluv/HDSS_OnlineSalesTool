using OnlineSalesCore.EFModel;

namespace OnlineSalesCore.DTO
{
    public class ShiftDTO
    {
        public ShiftDTO()
        {

        }
        public ShiftDTO(Shift shift)
        {
            Name = shift.Name;
            ExtName = shift.ExtName;
            ShiftId = shift.ShiftId;
        }
        public string Name { get; set; }
        public string ExtName { get; set; }
        public int ShiftId { get; set; }
        //public IEnumerable<ShiftDetailPOCO> ShiftDetails { get; set; }
    }
}