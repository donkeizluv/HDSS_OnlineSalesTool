using Ninject.Modules;
using Quartz;
using Quartz.Impl;
using SyncEngine.Config;
using SyncEngine.Job;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SyncService.Job
{
    //Consider moving this to a new process
    public class ServiceEngine
    {
        public const string DEFAULT_TRIGGER = "default_trigger";
        public const string DEFAULT_GROUP = "default_group";

        private readonly ITrigger _defaultTrigger;
        private readonly Config _config;
        public bool Started { get; private set; } = false;
        public IEnumerable<JobPlan> JobPlans { get; private set; }

        public ServiceEngine(Config config, IEnumerable<JobPlan> jobPlans)
        {
            _config = config ?? throw new ArgumentNullException();
            JobPlans = jobPlans;
            _defaultTrigger = GetDefaultTrigger();
        }

        public async Task Start()
        {
            if (Started) throw new InvalidOperationException("Scheduler is already started.");
            Started = true;
            // Grab the Scheduler instance from the Factory 
            var scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            //Start
            foreach (var plan in JobPlans)
            {
                //Add context data
                scheduler.Context.Put(plan.ModuleKey, plan.Module);
                //Use trigger
                if(plan.Trigger == null)
                    await scheduler.ScheduleJob(plan.Detail, _defaultTrigger);
                else
                    await scheduler.ScheduleJob(plan.Detail, plan.Trigger);
            }
            await scheduler.Start();
        }
        /// <summary>
        /// Default trigger for when no trigger defined in JobPlan
        /// </summary>
        /// <returns></returns>
        private ITrigger GetDefaultTrigger()
        {
            return TriggerBuilder.Create()
                .WithIdentity(DEFAULT_TRIGGER, DEFAULT_GROUP)
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithInterval(TimeSpan.FromMilliseconds(_config.SyncInterval))
                    .RepeatForever())
                .Build();
        }
    }
}
