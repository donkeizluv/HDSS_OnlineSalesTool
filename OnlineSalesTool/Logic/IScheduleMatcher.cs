using OnlineSalesTool.EFModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineSalesTool.Logic
{
    public interface IScheduleMatcher
    {
        /// <summary>
        /// Try to assign this case to an user according to scheluded shift & System date
        /// </summary>
        /// <param name="order"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        bool GetUserMatchedSchedule(string posCode, out IEnumerable<int> matchUserId, out string reason);
    }
}
