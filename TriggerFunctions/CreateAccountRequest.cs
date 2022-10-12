using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using FunctionDurableAppTest.DataServices;
using FunctionDurableAppTest.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using FunctionDurableAppTest.Extensions;

namespace FunctionDurableAppTest.TriggerFunctions
{
    public class CreateAccountRequest
    {
        private readonly IAccountDataService _accountDataService;

        public CreateAccountRequest(IAccountDataService accountDataService)
        {
            _accountDataService = accountDataService;
        }

        [FunctionName("CreateAccountRequest")]
        public async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient client,
            ILogger log)
        {
            var accountDetails = await req.Content.ReadAsAsync<AccountDetails>();

            if (accountDetails == null && string.IsNullOrEmpty(accountDetails.UserName))
            {
                return req.CreateErrorResponse(System.Net.HttpStatusCode.BadRequest, "error");
            }

            AccountDetails account;
            if (string.IsNullOrEmpty(accountDetails.ProcessInstanceId) || string.IsNullOrEmpty(accountDetails.AccountId))
            {
                account = AccountDetails.CreateAccountDetails(accountDetails.UserName);
            }
            else
            {
                account = accountDetails;
            }

            if (string.IsNullOrEmpty(account.ProcessInstanceId))
            {
                string instanceId = await client.StartNewAsync("Orchestration", accountDetails);
                log.LogInformation($"Started orchestration with ID = '{instanceId}'.");
            }
            else
            {
                log.LogInformation($"submit event {AppConstants.ResubmitAccount_Event} to orchestration with ID = '{account.ProcessInstanceId}'.");
                var orchEvtObj = new OrchestrationEventObj { EventName = AppConstants.ResubmitAccount_Event, EventData = account };
                await client.RaiseEventAsync(account.ProcessInstanceId, AppConstants.ResubmitAccount_Event, orchEvtObj);
            }

            var result = _accountDataService.GetAccountDetailsById(account.AccountId);
            var json = JsonConvert.SerializeObject(result);

            return req.CreateCustomResponse(System.Net.HttpStatusCode.OK, json);
        }
    }
}