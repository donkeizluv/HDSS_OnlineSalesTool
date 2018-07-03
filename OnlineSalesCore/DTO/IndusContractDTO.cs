using OnlineSalesCore.Models;
using System;

namespace OnlineSalesCore.DTO
{
    public class IndusContractDTO
    {
        public IndusContractDTO()
        {

        }
        public string FullName { get; set; }
        public string NatId { get; set; }
        public string Product { get; set; }
        public int Amount { get; set; }
        public int Paid { get; set; }
        public int LoanAmount { get; set; }
        public int Term { get; set; }
        public string ContractNumber { get; set; }
        
    }
}
