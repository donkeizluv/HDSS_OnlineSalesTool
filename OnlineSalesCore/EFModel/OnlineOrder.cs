using System;
using System.Collections.Generic;

namespace OnlineSalesCore.EFModel
{
    public partial class OnlineOrder
    {
        public int OrderId { get; set; }
        public string OrderGuid { get; set; }
        public string Name { get; set; }
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
        public string Induscontract { get; set; }
        public string Indusstatus { get; set; }
        public int StageId { get; set; }
        public int? AssignUserId { get; set; }
        public bool IsDirty { get; set; }
        public string OrderNumber { get; set; }

        public AppUser AssignUser { get; set; }
        public ProcessStage Stage { get; set; }
    }
}
