using System;
using System.ComponentModel.DataAnnotations;

namespace OnlineSalesCore.DTO
{
    public class CustomerConfirmDTO
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public bool Confirm { get; set; }
    }
}
