using FunctionDurableAppTest.ActivityFunctions;
using FunctionDurableAppTest.DataServices;
using FunctionDurableAppTest.Models;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FunctionDurableAppTest.OrchestrationHandlers
{
    public class ResubmitAccountEventHandler : IOrchestrationEventHandler
    {
        private readonly RetryOptions retryOptions;
        private readonly IAccountDataService _accountDataService;

        public ResubmitAccountEventHandler(RetryOptions retryOptions, IAccountDataService accountDataService)
        {
            this.retryOptions = retryOptions;
            _accountDataService = accountDataService;
        }
        public string EventName => AppConstants.ResubmitAccount_Event;

        public OrchestrationResponse Handle(OrchestrationParameters orchestration)
        {
            OrchestrationResponse orchestrationResponse = new OrchestrationResponse()
            {
                CloseParent = true,
                AccountDetails = orchestration.AccountDetails,
            };
            return orchestrationResponse;

        }
    }
}
