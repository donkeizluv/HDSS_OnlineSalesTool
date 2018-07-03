using System;
using System.Collections.Generic;

namespace OnlineSalesCore.Models
{
    public partial class FollowingContracts
    {
        public long Id { get; set; }
        public string ContractNumber { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public string ContractStatus { get; set; }
    }
}
