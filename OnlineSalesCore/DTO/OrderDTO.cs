using OnlineSalesCore.EFModel;
using System;
using System.ComponentModel.DataAnnotations;

namespace OnlineSalesCore.DTO
{
    public class OrderDTO
    {
        public OrderDTO()
        {

        }
        public int OrderId { get; set; }
        [Required]
        [StringLength(32)]
        public string Guid { get; set; }
        [Required]
        [StringLength(50)]
        public string FullName { get; set; }
        [Required]
        [StringLength(20)]
        public string NatId { get; set; }
        [Required]
        [StringLength(20)]
        public string Phone { get; set; }
        [Required]
        [StringLength(70)]
        public string Address { get; set; }
        [Required]
        [StringLength(10)]
        public string PosCode { get; set; }
        [Required]
        [StringLength(50)]
        public string Product { get; set; }
        [Required]
        public int Amount { get; set; }
        [Required]
        public int Paid { get; set; }
        [Required]
        public int LoanAmount { get; set; }
        [Required]
        public int Term { get; set; }
        public DateTime Received { get; set; }
        public string ContractNumber { get; set; }
        public int StageId { get; set; }
        public string OrderNumber { get; set; }
        public AppUser AssignUser { get; set; }
        public string Stage { get; set; }
        public string Status { get; set; }
        [Required]
        public string Signature { get; set; }
        
        
    }
}
