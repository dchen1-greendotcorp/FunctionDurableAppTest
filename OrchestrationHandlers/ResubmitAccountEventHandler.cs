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
        private readonly ILogger<ResubmitAccountEventHandler> _logger;

        public ResubmitAccountEventHandler(RetryOptions retryOptions, IAccountDataService accountDataService,
            ILogger<ResubmitAccountEventHandler> logger)
        {
            this.retryOptions = retryOptions;
            _accountDataService = accountDataService;
            _logger = logger;
        }
        public string EventName => AppConstants.ResubmitAccount_Event;

        public async Task<OrchestrationResponse> HandleAsync(IDurableOrchestrationContext context, OrchestrationParameters orchestration)
        {
            var account=_accountDataService.GetAccountDetailsById(orchestration.AccountDetails.AccountId);

            try
            {
                foreach (KeyValuePair<string, bool> k in account.ProcessStatus)
                {
                    if (!k.Value)
                    {
                        _logger.LogInformation("Call {Activity}", k.Key);
                        context.CallActivityWithRetryAsync<bool>(k.Key, retryOptions, account).GetAwaiter().GetResult();
                    }
                }

                OrchestrationResponse orchestrationResponse=new OrchestrationResponse()
                {
                    CloseParent=true,
                    AccountDetails=account,
                };
                return orchestrationResponse;
            }
            catch (Exception e)
            {
                _logger.LogError("ResubmitAccountEventHandler met exception: {e}", e);
                OrchestrationResponse orchestrationResponse = new OrchestrationResponse()
                {
                    CloseParent = false,
                    AccountDetails = account,
                };
                return orchestrationResponse;
            }
        }
    }
}
