using Newtonsoft.Json;
using NLog;
using Quartz;
using SyncEngine.Config;
using SyncEngine.Job;
using SyncService.DI;
using SyncService.Job;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace SyncService
{
    partial class Program
    {
        //Read same appsettings of site
        private static readonly string ConfigFileName = "appsettings.json";
        private static Config ServiceConfig;
        private static Logger _logger = LogManager.GetCurrentClassLogger();

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

        static void Main(string[] args)
        {
            _logger.Info("Service start....");
            ReadConfig();
            StartEngine().GetAwaiter().GetResult();
            Console.ReadLine();
        }
        static async Task StartEngine()
        {
            //Replace module here!
            var engine = new ServiceEngine(ServiceConfig, GetJobPlans());
            await engine.Start();
        }
        /// <summary>
        /// Add new jobs here
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<JobPlan> GetJobPlans()
        {
            var job = JobBuilder.Create<CLSyncJob>()
                .WithIdentity("clsync", "group1")
                .Build();
            //Key must be same with key in IJob impl to get the correct data in map
            var plan = new JobPlan(new CLTestModule(ServiceConfig.ConnectionStrings), job, CLSyncJob.MODULE_KEY);
            return new List<JobPlan>() { plan };
        }
        private static void ReadConfig()
        {
            try
            {
                _logger.Debug("hello!");
                var dynConfig = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText($"{Program.ExeDir}\\{ConfigFileName}"));
                var config = new Config()
                {
                    ConnectionStrings = dynConfig.ConnectionStrings.Default,
                    SyncInterval = dynConfig.SynService.SyncInterval
                };
                ServiceConfig = config;
            }
            catch (Exception ex)
            {
                _logger.Error(ex,"Reading config failed!");
                throw;
            }
        }
    }
}
