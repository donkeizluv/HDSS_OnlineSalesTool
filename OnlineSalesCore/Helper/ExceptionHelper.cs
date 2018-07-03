using System;
using Microsoft.Extensions.Logging;
using NLog;

namespace OnlineSalesCore.Helper
{
    public static class ExceptionHelper
    {
        //private static Logger _logger = LogManager.GetCurrentClassLogger();
        public static void LogException(Exception ex, Microsoft.Extensions.Logging.ILogger logger)
        {
            if(ex == null)  throw new ArgumentNullException();
            if(logger == null) throw new ArgumentNullException();
            logger.LogError(ex.GetType().ToString());
            logger.LogError(ex.Message);
            logger.LogError(ex.StackTrace);
            if (ex.InnerException != null)
            {
                logger.LogError("+++++++++Inner Ex: +++++++++");
                LogException(ex.InnerException, logger);
            }
        }
        public static void NLogException(Exception ex, Logger logger)
        {
            if(ex == null)  throw new ArgumentNullException();
            if(logger == null) throw new ArgumentNullException();
            logger.Error(ex.GetType().ToString());
            logger.Error(ex.Message);
            logger.Error(ex.StackTrace);
            if (ex.InnerException != null)
            {
                logger.Error("+++++++++Inner Ex: +++++++++");
                NLogException(ex.InnerException, logger);
            }
        }
    }
}
