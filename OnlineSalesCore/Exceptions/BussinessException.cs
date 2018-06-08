using System;

namespace OnlineSalesCore.Exceptions
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
