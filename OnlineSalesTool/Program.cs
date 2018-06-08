using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using NLog.Web;
using System;
using System.IO;
using System.Reflection;

namespace OnlineSalesTool
{
    public class Program
    {
        public static string ExeDir
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                var uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }
        public static void Main(string[] args)
        {
            BuildWebHost(args).Build().Run();
        }
        public static IWebHostBuilder BuildWebHost(string[] args)
        {
            //Start webserver
            return WebHost.CreateDefaultBuilder(args)
                .UseNLog()
                .UseStartup<Startup>();
        }
    }
}
