using Microsoft.AspNetCore.Mvc.Filters;
using NLog;
using OnlineSalesTool.Service;

namespace OnlineSalesTool.Filter
{
    /// <summary>
    /// Log unhandled exceptions
    /// </summary>
    public class LogExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            var logger = LogManager.GetLogger(context.ActionDescriptor.DisplayName);
            Utility.LogException(context.Exception, logger);
        }
    }
}
