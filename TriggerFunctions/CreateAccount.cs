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
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using System.Net;

namespace FunctionDurableAppTest.TriggerFunctions
{
    public class CreateAccount
    {

        [OpenApiOperation(operationId: "CreateAccount")]
        [OpenApiRequestBody("application/json", typeof(AccountDetails))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json",
            bodyType: typeof(JToken))]

        [FunctionName("CreateAccount")]
        public async Task<IActionResult> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestMessage req,
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
            JToken outPut = JToken.Parse("{}");

            DurableOrchestrationStatus status = await client.GetStatusAsync(account.UniqueRequestId, true, true,true);
            if(status!=null && status.RuntimeStatus== OrchestrationRuntimeStatus.Completed)
            {
                //return new OkObjectResult(status);
                outPut["finishOrchestrationSuccess"] =true;
               return new OkObjectResult(outPut);
            }

            RequestModel<AccountDetails> requestModel = RequestModel<AccountDetails>.CreateRequest(account, status);

            await client.StartNewAsync(AppConstants.CreateAcountOrchestration, requestModel.RequestId, requestModel);

            log.LogInformation($"Started orchestration with ID = '{requestModel.RequestId}'.");
            await client.WaitForCompletionOrCreateCheckStatusResponseAsync(req,
                requestModel.RequestId,TimeSpan.FromSeconds(25),TimeSpan.FromSeconds(5));

            status = await client.GetStatusAsync(requestModel.RequestId, true, true, true);
            
            switch (status.RuntimeStatus)
            {
                case OrchestrationRuntimeStatus.Completed:
                    outPut["finishOrchestrationSuccess"] = true;
                    
                    break;
                case OrchestrationRuntimeStatus.Running:
                case OrchestrationRuntimeStatus.Pending:
                case OrchestrationRuntimeStatus.ContinuedAsNew:
                case OrchestrationRuntimeStatus.Unknown:
                default:
                    outPut["finishOrchestrationSuccess"] = false;
                    break;
            }

            //outPut["runningStatus"] = JsonConvert.SerializeObject(status);

            OkObjectResult httpResponse =  new OkObjectResult(outPut);
            return httpResponse;
        }
    }
}