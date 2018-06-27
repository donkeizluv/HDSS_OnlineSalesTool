using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OnlineSalesCore.EFModel;

namespace OnlineSalesCore.Logic
{
    public interface IScheduleMatcher
    {
        // Try to assign this case to an user according to scheluded shift & System date
        Task<(bool, List<AppUser>, string)> GetUserMatchedSchedule(string posCode, DateTime date);
    }
}
