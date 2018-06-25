using System;
using System.Collections.Generic;

namespace OnlineSalesCore.EFModel
{
    public partial class ContractActivities
    {
        public long Id { get; set; }
        public string ContractNumber { get; set; }
        public string ActivityCode { get; set; }
        public string Description { get; set; }
        public DateTime InitDate { get; set; }
        public DateTime? CompleteDate { get; set; }
        public string Remark { get; set; }
    }
}
