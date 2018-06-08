using System;
using System.Collections.Generic;

namespace OnlineSalesCore.EFModel
{
    public partial class Logs
    {
        public int LogId { get; set; }
        public string Class { get; set; }
        public string Method { get; set; }
        public string Message { get; set; }
        public string Trace { get; set; }
        public bool IsSent { get; set; }
        public DateTime Timestamp { get; set; }
        public string Type { get; set; }
    }
}
