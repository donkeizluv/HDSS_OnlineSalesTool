using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineSalesTool.Exceptions
{
    /// <summary>
    /// Bussiness check, logic related problem, catch this at controller's actions
    /// </summary>
    public class BussinessException : Exception
    {
        public BussinessException(string mess) : base(mess)
        {

        }
    }
}
