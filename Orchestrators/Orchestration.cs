using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FunctionDurableAppTest.ActivityFunctions;
using FunctionDurableAppTest.DataServices;
using FunctionDurableAppTest.Models;
using FunctionDurableAppTest.OrchestrationHandlers;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;

namespace FunctionDurableAppTest.Orchestrators
{
    public class Orchestration
    {
        private readonly RetryOptions retryOptions;
        private readonly AccountBusinessService _accountBusinessService;
        private readonly IAccountDataService accountDataService;
        private readonly Dictionary<string, IOrchestrationEventHandler> eventHandlerDict = new Dictionary<string, IOrchestrationEventHandler>();

        public Orchestration(RetryOptions retryOptions, IEnumerable<IOrchestrationEventHandler> orchestrationEventHandlers,
            AccountBusinessService accountBusinessService, IAccountDataService accountDataService)
        {
            this.retryOptions = retryOptions;
            _accountBusinessService = accountBusinessService;
            this.accountDataService = accountDataService;
            eventHandlerDict = orchestrationEventHandlers.ToDictionary(c => c.EventName);

            //foreach (var item in orchestrationEventHandlers)
            //{
            //    eventHandlerDict[item.EventName] = item;
            //}
        }

        [FunctionName("Orchestration")]
        public async Task RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context, ILogger log)
        {
            var account = context.GetInput<AccountDetails>();

            try
            {
                account.ProcessInstanceId = context.InstanceId;
                
                var saved=await context.CallActivityAsync<AccountDetails>(nameof(SaveAccount), account);
                if(saved!=null)
                {
                    var archived = await context.CallActivityAsync<AccountDetails>(nameof(ArchiveAccount), saved);
                    if(archived!=null)
                    {
                        var notified = await context.CallActivityAsync<AccountDetails>(nameof(NotifyAccount), account);
                    }
                }
                

                //var returnAccount = await _accountBusinessService.AccountBusiness(context, account); 
                //var savedAccount = await context.CallActivityAsync<AccountDetails>(nameof(SaveAccount), account);
                //if (savedAccount != null)
                //{
                //    var archivedAccount = await context.CallActivityAsync<AccountDetails>(nameof(ArchiveAccount), savedAccount);
                //    if (archivedAccount != null)
                //    {
                //        var notifiedAccount = await context.CallActivityAsync<AccountDetails>(nameof(NotifyAccount), archivedAccount);
                //        return;
                //    }
                //}

            }
            catch (Exception ex)
            {
                log.LogError("Orchestra met error = {ex}", ex);
            }


            Dictionary<string, Task<OrchestrationEventObj>> taskdict = new Dictionary<string, Task<OrchestrationEventObj>>();

            using (var cts = new CancellationTokenSource())
            {
                RegisterTimer(taskdict, context, cts.Token);

                while (true)
                {
                    //register orchestration events
                    RegisterEvents(taskdict, context);

                    //wait for any registration event trigger
                    var taskList = taskdict.Values.Select(c => c).ToList();
                    var taskResult = await Task.WhenAny(taskList);
                    if(taskResult.Result.EventName== AppConstants.ResubmitAccount_Event)
                    {
                        var notifiedAccount = await context.CallActivityAsync<AccountDetails>(nameof(NotifyAccount), account);
                    }

                    IOrchestrationEventHandler eventHandler = eventHandlerDict[taskResult.Result.EventName];

                    //var myaccount=accountDataService.GetAccountDetailsById(account.AccountId);

                    OrchestrationParameters parameters = PrepareOrchestrationParameters(taskResult.Result, account);

                    var response = await eventHandler.HandleAsync( parameters);
                    if (response.CloseParent)
                    {
                        cts.Cancel();
                        break;
                    }
                }
            }
        }

        private OrchestrationParameters PrepareOrchestrationParameters(OrchestrationEventObj eventObj, AccountDetails account)
        {
            OrchestrationParameters parameters = new OrchestrationParameters();
            parameters.AccountDetails = account;
            parameters.OrchestrationEventObj = eventObj;
            return parameters;
        }

        private void RegisterEvents(Dictionary<string, Task<OrchestrationEventObj>> taskdict, IDurableOrchestrationContext context)
        {
            foreach (var evtName in AppConstants.RegisterdEvents)
            {
                if (!taskdict.ContainsKey(evtName))
                {
                    taskdict[evtName] = context.WaitForExternalEvent<OrchestrationEventObj>(evtName);
                }
            }
        }

        private void RegisterTimer(Dictionary<string, Task<OrchestrationEventObj>> taskdict, IDurableOrchestrationContext context, CancellationToken token)
        {
            var duration = context.CurrentUtcDateTime.AddMinutes(10);
            OrchestrationEventObj timerEventObj = new OrchestrationEventObj() { EventName = AppConstants.TaskExpireEvent, EventData = null };

            var waitForExpireTask = context.CreateTimer(duration, timerEventObj, token);

            taskdict[AppConstants.TaskExpireEvent] = waitForExpireTask;
        }
    }
}