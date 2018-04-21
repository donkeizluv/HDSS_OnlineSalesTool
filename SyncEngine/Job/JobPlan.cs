using Ninject.Modules;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncEngine.Job
{
    public class JobPlan
    {
        public string ModuleKey { get; private set; }
        public NinjectModule Module { get; private set; }
        public IJobDetail Detail { get; private set; }
        public ITrigger Trigger { get; set; }

        public JobPlan(NinjectModule module, IJobDetail detail, string moduleKey)
        {
            Module = module;
            Detail = detail;
            ModuleKey = moduleKey;
        }
    }
}
