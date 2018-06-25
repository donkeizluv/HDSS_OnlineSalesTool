using System;
using System.Collections.Generic;

namespace OnlineSalesCore.EFModel
{
    public partial class AmendedContracts
    {
        public long Id { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ContractNumber { get; set; }
        public string NewContractNumber { get; set; }
        public string ParentSimulation { get; set; }
        public byte Status { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Comment { get; set; }
    }
}
