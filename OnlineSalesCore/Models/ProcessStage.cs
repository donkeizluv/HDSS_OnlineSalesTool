using System;
using System.Collections.Generic;

namespace OnlineSalesCore.Models
{
    public partial class ProcessStage
    {
        public ProcessStage()
        {
            OnlineOrder = new HashSet<OnlineOrder>();
        }

        public int StageId { get; set; }
        public string Stage { get; set; }
        public int StageNumber { get; set; }
        public string Description { get; set; }

        public ICollection<OnlineOrder> OnlineOrder { get; set; }
    }
}
