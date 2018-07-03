using System;
using System.ComponentModel.DataAnnotations;

namespace OnlineSalesCore.DTO
{
    public class CaseAssignDTO
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }
    }
}
