using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineSalesTool.Options
{
    public class WindowsAuthOptions
    {
        public bool NoPwdCheck { get; set; }
        public string Domain { get; set; }
        public string Issuer { get; set; }
    }
}
