﻿using Microsoft.AspNetCore.Mvc.Filters;
using NLog;
using OnlineSalesTool.Helper;

namespace OnlineSalesTool.Filter
{
    /// <summary>
    /// Log unhandled exceptions
    /// </summary>
    public class LogExceptionFilterAttribute : ExceptionFilterAttribute
    {        private readonly string _name;

        public LogExceptionFilterAttribute(string name)
        {
            _name = name;
        }
        public override void OnException(ExceptionContext context)
        {
            var logger = LogManager.GetLogger(context.ActionDescriptor.DisplayName);
            Utility.LogException(context.Exception, logger);
        }
    }
}
