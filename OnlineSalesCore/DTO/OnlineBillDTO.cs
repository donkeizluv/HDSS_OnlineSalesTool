using System.ComponentModel.DataAnnotations;

namespace OnlineSalesCore.DTO
{
    public class OnlineBillDTO
    {
        public OnlineBillDTO()
        {

        }
        
        [Required]
        [StringLength(32)]
        public string Guid { get; set; }
        [Required]
        [StringLength(10)]
        public string OnlineBill { get; set; }
        [Required]
        public string Signature { get; set; }
    }
}
