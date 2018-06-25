﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineSalesCore.Logic
{
    public interface IScheduleMatcher
    {
        //bool GetUserMatchedSchedule(string posCode, DateTime date, out IEnumerable<int> matchUserId, out string reason);
        /// <summary>
        /// Try to assign this case to an user according to scheluded shift & System date
        /// </summary>
        /// <param name="order"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<(bool, List<int>, string)> GetUserMatchedSchedule(string posCode, DateTime date);
    }
}