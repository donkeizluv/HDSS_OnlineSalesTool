using Microsoft.AspNetCore.Http;
using NLog;
using OnlineSalesTool.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace OnlineSalesTool.Service
{
    public static class Utility
    {
        //private static Logger _logger = LogManager.GetCurrentClassLogger();
        public static void LogException(Exception ex, Logger logger)
        {
            if(ex == null)  throw new ArgumentNullException();
            if(logger == null) throw new ArgumentNullException();
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
        public static string RandomString(int length, Random random)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
