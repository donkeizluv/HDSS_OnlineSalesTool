using OnlineSalesTool.EFModel;
using OnlineSalesTool.Const;
using System.Collections.Generic;
using System.Linq;

namespace OnlineSalesTool.DTO
{
    public class OnlineBillDTO
    {
        public OnlineBillDTO()
        {

        }
        
        public string Guid { get; set; }
        public string OnlineBill { get; set; }
        public string Signature { get; set; }
    }
}
