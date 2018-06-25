using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OnlineSalesCore.Const;
using OnlineSalesCore.EFModel;
using OnlineSalesCore.Exceptions;
using OnlineSalesCore.Options;
using OnlineSalesCore.Service;

namespace OnlineSalesCore.BackgroundJobs
{
    //Contract status sync logic
    public class SyncJob  : BackgroundService
    {        
        private readonly ILogger<SyncJob> _logger;
        private readonly SyncOptions _options;
        private readonly IServiceProvider _provider;
        public SyncJob(IOptions<SyncOptions> options,
                            ILogger<SyncJob> logger,
                            IServiceProvider provider)
        {
            _provider = provider;
            _options = options.Value;
            _logger = logger;
        }
        //Indus activity code status
        private const string REJECT_ACTIVITY = "PREJECT";
        private const string CONTRACTPRINTING_ACTIVITY = "SAN_LETT";
        private const string AMENDED_ACTIVITY ="PAMEND";
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("SyncJob started");
            stoppingToken.Register(() => _logger.LogDebug($"Stop signaled"));
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var now = DateTime.Now;
                    using (var scope = _provider.CreateScope())
                    {
                        var context = scope.ServiceProvider.GetService<OnlineSalesContext>();
                        var indus = scope.ServiceProvider.GetService<IIndusService>();
                        _logger.LogInformation("Executing....");
                        //New cases to following/sync
                        //List of WaitForFinalStatus cases except those already in FollowingContracts
                        var waitingCases = await context.OnlineOrder
                            .Where(o => o.StageId == (int)StageEnum.WaitForFinalStatus 
                                    && !context.FollowingContracts.Any(f => f.ContractNumber == o.Induscontract))
                            .ToListAsync();
                        _logger.LogInformation($"Cases count: {waitingCases.Count()}");
                        //Save these to following cases
                        var newFollowing = waitingCases
                            .Select(w => new FollowingContracts(){
                                ContractNumber = w.Induscontract,
                                StartDate = now
                            });
                        await context.FollowingContracts.AddRangeAsync(newFollowing);
                        //Check following cases for final status
                        var waitForFinal = await context.OnlineOrder
                            .Where(o => o.StageId == (int)StageEnum.WaitForFinalStatus)
                            .ToListAsync();
                        var stopSync = new List<string>();
                        foreach (var waitingCase in waitForFinal)
                        {
                            var activities = context.ContractActivities
                                .Where(a => a.ContractNumber == waitingCase.Induscontract);
                            if(!await activities.AnyAsync())
                            {
                                _logger.LogInformation($"Case {waitingCase.OrderId} has no activities");
                                continue;
                            }
                            _logger.LogInformation($"Case {waitingCase.OrderId} has {activities.Count()}");
                            if(IsReject(activities))
                            {
                                //Final status Reject
                                waitingCase.StageId = (int)StageEnum.Reject;
                                stopSync.Add(waitingCase.Induscontract);
                                continue;
                            }
                            if(IsContractPrinting(activities))
                            {
                                //Move to next stage
                                waitingCase.StageId = (int)StageEnum.WaitForOnlineBill;
                                stopSync.Add(waitingCase.Induscontract);
                                //TODO: Update lastest info about contract
                                
                                continue;
                            }
                            if(IsAmended(activities))
                            {
                                var amend = await context.AmendedContracts
                                    .FirstOrDefaultAsync(a => a.ContractNumber == waitingCase.Induscontract);
                                if(amend == null)
                                {
                                    //May not available, check later in next iteration
                                    _logger.LogInformation($"New contract number for amended case {waitingCase.Induscontract} not available");
                                    continue;
                                }
                                //Update new contract number
                                waitingCase.Induscontract = amend.NewContractNumber;
                                //Stop syncing old number
                                stopSync.Add(waitingCase.Induscontract);
                                continue;
                            }
                        }
                        //Stop sync
                        var tobeRemoved = context.FollowingContracts
                            .Where(f => stopSync.Contains(f.ContractNumber));
                        context.RemoveRange(await tobeRemoved.ToListAsync());
                        //Save changes
                        await context.SaveChangesAsync();
                    }
                }
                catch (System.Exception ex)
                {
                    Utility.LogException(ex, _logger);
                }
                await Task.Delay(_options.Delay, stoppingToken);
            }
        }
        private bool IsReject(IQueryable<ContractActivities> activities)
        {
            return activities.Any(a => a.ActivityCode == REJECT_ACTIVITY);
        }
        private bool IsAmended(IQueryable<ContractActivities> activities)
        {
            return activities.Any(a => a.ActivityCode == AMENDED_ACTIVITY);
        }
        private bool IsContractPrinting(IQueryable<ContractActivities> activities)
        {
            return activities.Any(a => a.ActivityCode == CONTRACTPRINTING_ACTIVITY);
        }
    }
}
