using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FunctionDurableAppTest.ActivityFunctions;
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
        private readonly Dictionary<string, IOrchestrationEventHandler> eventHandlerDict;

        public Orchestration(RetryOptions retryOptions, IEnumerable<IOrchestrationEventHandler> orchestrationEventHandlers)
        {
            this.retryOptions = retryOptions;
            this.eventHandlerDict = orchestrationEventHandlers.ToDictionary(c => c.EventName);
        }

        [FunctionName("Orchestration")]
        public async Task RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var account = context.GetInput<AccountDetails>();

            account.ProcessInstanceId = context.InstanceId;

            await context.CallActivityWithRetryAsync<bool>(nameof(CreateAccount), retryOptions, account);

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

                    IOrchestrationEventHandler eventHandler = eventHandlerDict[taskResult.Result.EventName];

                    OrchestrationParameters parameters = PrepareOrchestrationParameters(taskResult.Result, account);

                    var response = await eventHandler.HandleAsync(context, parameters);
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
            var duration = context.CurrentUtcDateTime.AddHours(72);
            OrchestrationEventObj timerEventObj = new OrchestrationEventObj() { EventName = AppConstants.TaskExpireEvent, EventData = null };

            var waitForExpireTask = context.CreateTimer(duration, timerEventObj, token);

            taskdict[AppConstants.TaskExpireEvent] = waitForExpireTask;
        }
    }
}