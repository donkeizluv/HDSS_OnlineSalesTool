using OnlineSalesTool.Logic;
using Ninject;
using Ninject.Modules;
using NLog;
using OnlineSalesTool.AppEnum;
using OnlineSalesTool.EFModel;
using Quartz;
using SyncService.SyncLogic.Interface;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SyncService.Job
{
    [DisallowConcurrentExecution]
    public class CLSyncJob : IJob   
    {
        private Logger _logger = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// Must be same with JobPlan key to get correct data
        /// </summary>
        public const string MODULE_KEY = "CLSync";
        private OnlineSalesContext _dbContext;

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.Trace("------------Start sync routine------------");
            var schedulerContext = context.Scheduler.Context;
            //Get data context
            if (!(schedulerContext.Get(MODULE_KEY) is NinjectModule module)) throw new InvalidOperationException("DI Module is not set");
            var kernel = new StandardKernel(module);
            //Resolve DI
            _dbContext = kernel.Get<OnlineSalesContext>();
            var dealerApi = kernel.Get<IDealerAPI>();
            var indusAdapter = kernel.Get<IIndusAdapter>();
            var assigner = kernel.Get<IScheduleMatcher>();

            if (_dbContext == null || dealerApi == null || indusAdapter == null || assigner == null)
                throw new InvalidOperationException("Some dependancy cant be resolved");
            //Things to do in a Sync:
            //Fetch new cases
            //Assign to CA, Queue email notification to CA/ BDS if cant assign

            //Sync/update INDUS status
            //Update final status cases to dealer

            //Request online order number
            //Check & get online order number

            using (_dbContext)
            {
                //Fetch & save new cases
                var newCases = await dealerApi.FetchNewCase();
                _logger.Trace($"New case count: {newCases.Count}");
                //Assign cases to CA
                foreach (var newCase in newCases)
                {
                    (bool result, List<int> userIds, string reason) = await assigner.GetUserMatchedSchedule(newCase.PosCode, DateTime.Now);
                    if(result)
                    {
                        _logger.Trace($"Assign OK, tracking number: {newCase.OrderGuid} to: {string.Concat(userIds.Select(id => id + " "))}");
                        _logger.Trace($"Assign to user id: {userIds.First()}");
                        newCase.AssignUserId = userIds.First();
                    }
                    else //Fail to assign
                    {
                        _logger.Trace($"Assign Failed: tracking number: {newCase.OrderGuid}");
                        //Set stage to NotAssignable
                        //TODO: Queue email to BDS
                        newCase.StageId = (int)Stage.NotAssignable;
                    }
                }
                //Add new case & assign changes to context
                _dbContext.AddRange(newCases);
                //TODO: save here, in case exception happens later, we still have new cases persisted
                //Or just make sure no unhandle ex can raise later ?????
                //Sync INDUS final status
                //Get Processing status
                var tobeReflectList = _dbContext.OnlineOrder.Where(o => o.StageId == (int)Stage.WaitForFinalStatus).ToList();
                _logger.Trace($"To be reflect list count: {tobeReflectList.Count}");
                //Reflect changes
                await indusAdapter.ReflectChanges(tobeReflectList);
                var dirtCasesList = tobeReflectList.Where(o => o.IsDirty).ToList();
                _logger.Trace($"Dirty cases count: {dirtCasesList.Count}");
                foreach (var dirtyCase in dirtCasesList)
                {
                    var stage = (Stage)dirtyCase.StageId;
                    switch (stage)
                    {
                        case Stage.WaitForFinalStatus:
                            //Do nothing, keep tracking
                            _logger.Trace($"Order: {dirtyCase.OrderId} INDUS: Processing -> Do nothing");
                            break;
                        case Stage.Approved:
                            _logger.Trace($"Order: {dirtyCase.OrderId} INDUS: Approved -> Done");
                            //Update final status to dealer
                            //Send request for online order number to dealer
                            await dealerApi.UpdateStatus(dirtyCase);
                            await dealerApi.RequestOrderNumber(dirtyCase);
                            dirtyCase.StageId = (int)Stage.WaitForDealerNumber;
                            break;
                        case Stage.Reject:
                            _logger.Trace($"Order: {dirtyCase.OrderId} INDUS: Reject -> Done");
                            //Update final status to dealer
                            await dealerApi.UpdateStatus(dirtyCase);
                            break;
                        default:
                            _logger.Error($"Invalid stage: {stage.ToString()} at processing dirt case");
                            break;
                    }
                }
            }
        }
    }
}
