using Microsoft.AspNetCore.Http;
using NLog;
using OnlineSalesTool.Logic;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace OnlineSalesTool.Service
{
    public static class Utility
    {
        //private static Logger _logger = LogManager.GetCurrentClassLogger();
        public static void LogException(Exception ex, Logger logger)
        {
            logger.Error(ex.GetType().ToString());
            logger.Error(ex.Message);
            logger.Error(ex.StackTrace);
            if (ex.InnerException != null)
            {
                logger.Error("+++++++++Inner Ex: +++++++++");
                LogException(ex.InnerException, logger);
            }
        }
        
        
        public static bool Decode64(string base64, out int decoded)
        {
            decoded = -1;
            if (string.IsNullOrEmpty(base64)) return false;
            try
            {
                var data = Convert.FromBase64String(base64);
                string decodedString = Encoding.UTF8.GetString(data);
                decoded = int.Parse(decodedString);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}
