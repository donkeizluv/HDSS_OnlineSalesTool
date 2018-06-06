using OnlineSalesTool.EFModel;
using System;
using System.Collections.Generic;

namespace OnlineSalesTool.DTO
{
    public class OrderDTO
    {
        public OrderDTO()
        {

        }
        public int OrderId { get; set; }
        public string Guid { get; set; }
        public string FullName { get; set; }
        public string NatId { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string PosCode { get; set; }
        public string Product { get; set; }
        public int Amount { get; set; }
        public int Paid { get; set; }
        public int LoanAmount { get; set; }
        public int Term { get; set; }
        public DateTime Received { get; set; }
        public string ContractNumber { get; set; }
        public int StageId { get; set; }
        public string OrderNumber { get; set; }
        public AppUser AssignUser { get; set; }
        public string Stage { get; set; }
        public string Status { get; set; }
        public string Signature { get; set; }
        
        
    }
}
