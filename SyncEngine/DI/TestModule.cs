using OnlineSalesTool.Logic;
using Ninject.Modules;
using OnlineSalesTool.EFModel;
using OnlineSalesTool.Logic.Impl;
using SyncService.SyncLogic.Impl.Test;
using SyncService.SyncLogic.Interface;

namespace SyncService.DI
{
    public class CLTestModule : NinjectModule
    {
        private readonly string _connectionStr;
        public CLTestModule(string connectionStr) : base()
        {
            _connectionStr = connectionStr;
        }
        public override void Load()
        {
            //Context should be shared across all instance that requires it, in order to track entity
            var context = new OnlineSalesContext(_connectionStr);
            Bind<OnlineSalesContext>().ToConstant(context);
            Bind<IScheduleMatcher>().To<SimpleScheduleMatcher>().WithConstructorArgument(context);
            Bind<IDealerAPI>().To<FakeDealerAPI>();
            Bind<IIndusAdapter>().To<FakeIndusAdapter>();
        }
    }
}
