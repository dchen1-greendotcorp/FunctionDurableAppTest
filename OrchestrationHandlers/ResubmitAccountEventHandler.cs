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

        public async Task<OrchestrationResponse> HandleAsync(IDurableOrchestrationContext context, OrchestrationParameters orchestration)
        {
            var account=_accountDataService.GetAccountDetailsById(orchestration.AccountDetails.AccountId);

            try
            {
                foreach (KeyValuePair<string, bool> k in account.ProcessStatus)
                {
                    if (!k.Value)
                    {
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
