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
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Grpc.Core;
using System;
using Azure.Core;
using Newtonsoft.Json.Linq;

namespace FunctionDurableAppTest.TriggerFunctions
{
    public class CreateAccount
    {
        

        [FunctionName("CreateAccount")]
        public async Task<IActionResult> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient client,
            ILogger log)
        {
            var accountDetails = await req.Content.ReadAsAsync<AccountDetails>();

            if (accountDetails == null && string.IsNullOrEmpty(accountDetails.UserName))
            {
                throw new ArgumentException($"not good input");
            }

            AccountDetails account;
            if (string.IsNullOrEmpty(accountDetails.ProcessInstanceId) && string.IsNullOrEmpty(accountDetails.AccountId))
            {
                account = AccountDetails.CreateAccountDetails(accountDetails.UserName);
            }
            else
            {
                account = accountDetails;
                if(string.IsNullOrEmpty(accountDetails.ProcessInstanceId))
                {
                    account.ProcessInstanceId= accountDetails.AccountId;
                }
            }

            DurableOrchestrationStatus status = await client.GetStatusAsync(account.UniqueRequestId, true, true,true);

            RequestModel<AccountDetails> requestModel = RequestModel<AccountDetails>.CreateRequest(account, status);


            await client.StartNewAsync(AppConstants.CreateAcountOrchestration, requestModel.RequestId, requestModel);

            log.LogInformation($"Started orchestration with ID = '{requestModel.RequestId}'.");
            await client.WaitForCompletionOrCreateCheckStatusResponseAsync(req,
                requestModel.RequestId,TimeSpan.FromSeconds(25),TimeSpan.FromSeconds(5));

            status = await client.GetStatusAsync(requestModel.RequestId);

            JToken outPut = null;
            switch (status.RuntimeStatus)
            {
                case OrchestrationRuntimeStatus.Completed:
                    outPut = status.Output;
                    break;
                case OrchestrationRuntimeStatus.Running:
                case OrchestrationRuntimeStatus.Pending:
                case OrchestrationRuntimeStatus.ContinuedAsNew:
                case OrchestrationRuntimeStatus.Unknown:
                default:
                    await client.SuspendAsync(requestModel.RequestId, "Timeout!");
                    break;
            }

            var httpResponse = outPut != null ? new OkObjectResult(outPut) : new OkObjectResult(status);
            return httpResponse;
        }
    }
}