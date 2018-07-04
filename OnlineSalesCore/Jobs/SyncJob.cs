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
using OnlineSalesCore.Models;
using OnlineSalesCore.Options;
using OnlineSalesCore.Services;
using OnlineSalesCore.Helper;

namespace OnlineSalesCore.Jobs
{
    //Contract status sync logic
    public class SyncJob : BackgroundService
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
        private const string AMENDED_ACTIVITY = "PAMEND";
        //TODO: Split this to smaller tasks
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if(_options.Disabled)
            {
                _logger.LogDebug("SyncJob is disabled");
                return;
            }
            _logger.LogDebug("SyncJob started");
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
                        var mailService = scope.ServiceProvider.GetService<IMailerService>();
                        _logger.LogDebug("Executing....");
                        //New cases to following/sync
                        //List of WaitForFinalStatus cases except those already in FollowingContracts
                        var waitingCases = await context.OnlineOrder
                            .Where(o => o.StageId == (int)StageEnum.WaitForFinalStatus
                                    && !context.FollowingContracts.Any(f => f.ContractNumber == o.Induscontract))
                            .ToListAsync();
                        _logger.LogDebug($"Cases count: {waitingCases.Count()}");
                        //New tracking contracts
                        var startTracking = waitingCases
                            .Select(w => new FollowingContracts()
                            {
                                ContractNumber = w.Induscontract,
                                StartDate = now
                            });
                        //Stop tracking contracts
                        var stopTracking = new List<string>();
                        //Check following cases for final status
                        var waitForFinalStatus = await context.OnlineOrder
                            .Where(o => o.StageId == (int)StageEnum.WaitForFinalStatus)
                            .Include(o => o.AssignUser)
                            // .ThenInclude(u => u.Manager)
                            .ToListAsync();
                        foreach (var aCase in waitForFinalStatus)
                        {
                            if(string.IsNullOrEmpty(aCase.Induscontract)) continue;
                            //Get activities
                            var activities = await context.ContractActivities
                                .Where(a => a.ContractNumber == aCase.Induscontract).ToListAsync();
                            if (!activities.Any())
                            {
                                _logger.LogInformation($"Case {aCase.OrderId} has no activities");
                                continue;
                            }
                            _logger.LogInformation($"Case {aCase.OrderId} has {activities.Count()}");
                            if (IsReject(activities))
                            {
                                _logger.LogDebug($"Contract {aCase} is Rejected");
                                //Final status Reject
                                aCase.StageId = (int)StageEnum.Reject;
                                stopTracking.Add(aCase.Induscontract);
                                //Notify
                                mailService.MailStageChanged(aCase,
                                    "REJECT",
                                    aCase.AssignUser.Email,
                                    null);
                                continue;
                            }
                            if (IsContractPrinting(activities))
                            {
                                _logger.LogDebug($"Contract {aCase} is ContractPrinting");
                                //Move to next stage
                                aCase.StageId = (int)StageEnum.WaitForOnlineBill;
                                stopTracking.Add(aCase.Induscontract);
                                //Notify
                                mailService.MailStageChanged(aCase,
                                   "CONTRACT PRINTING",
                                   aCase.AssignUser.Email,
                                   null);
                                //Update lastest INDUS data back to order
                                var indusContract = await indus.GetContract(aCase.Induscontract);
                                if(indusContract == null)
                                {
                                    _logger.LogError($"Cant get indus contract data of {aCase.Induscontract}");
                                    continue;
                                }
                                aCase.Name = indusContract.FullName;
                                aCase.Product = indusContract.Product;
                                aCase.Amount = indusContract.Amount;
                                aCase.Paid = indusContract.Paid;
                                aCase.LoanAmount = indusContract.LoanAmount;
                                aCase.Term = indusContract.Term;
                                continue;
                            }
                            if (IsAmended(activities))
                            {
                                _logger.LogDebug($"Contract {aCase} is Amended");
                                var amend = await context.AmendedContracts
                                    .FirstOrDefaultAsync(a => a.ContractNumber == aCase.Induscontract);
                                if (amend == null || string.IsNullOrEmpty(amend.NewContractNumber))
                                {
                                    //May not available, check later in next iteration
                                    _logger.LogDebug($"New contract number for amended case {aCase.Induscontract} is not available");
                                    continue;
                                }
                                //Update new contract number
                                aCase.Induscontract = amend.NewContractNumber;
                                //Follow new contract number
                                startTracking.Append(new FollowingContracts(){
                                    ContractNumber = amend.NewContractNumber,
                                    StartDate = now
                                });
                                //Stop syncing old number
                                stopTracking.Add(aCase.Induscontract);
                                continue;
                            }
                        }
                        //Stop sync
                        var tobeRemoved = context.FollowingContracts
                            .Where(f => stopTracking.Contains(f.ContractNumber));
                        //TODO: use flag to determine tracking instead of remove entry
                        context.RemoveRange(await tobeRemoved.ToListAsync());                            
                        //Add new following case
                        await context.FollowingContracts.AddRangeAsync(startTracking);
                        //Save changes
                        await context.SaveChangesAsync();
                    }
                }
                catch (System.Exception ex)
                {
                    ExceptionHelper.LogException(ex, _logger);
                }
                await Task.Delay(_options.Delay, stoppingToken);
            }
        }
        private bool IsReject(IEnumerable<ContractActivities> activities)
        {
            return activities.Any(a => a.ActivityCode == REJECT_ACTIVITY);
        }
        private bool IsAmended(IEnumerable<ContractActivities> activities)
        {
            return activities.Any(a => a.ActivityCode == AMENDED_ACTIVITY);
        }
        private bool IsContractPrinting(IEnumerable<ContractActivities> activities)
        {
            return activities.Any(a => a.ActivityCode == CONTRACTPRINTING_ACTIVITY);
        }
    }
}
