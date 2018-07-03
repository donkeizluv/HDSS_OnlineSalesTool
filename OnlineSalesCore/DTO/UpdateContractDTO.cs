using System;
using System.ComponentModel.DataAnnotations;

namespace OnlineSalesCore.DTO
{
    public class UpdateContractDTO
    {
        private string _contract;

        [Required]
        public int Id { get; set; }
        [Required]
        public string Contract { get => _contract.ToUpper(); set => _contract = value; }
    }
}
