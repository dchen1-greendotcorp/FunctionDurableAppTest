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
using Microsoft.OpenApi.Models;

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
                outPut["finishOrchestrationSuccess"] = true;
               return new OkObjectResult(outPut);
            }

            string url = req.RequestUri.ToString();

            var baseUrl = url.Substring(0, url.LastIndexOf("CreateAccount")-1);

            if (status == null || status.RuntimeStatus != OrchestrationRuntimeStatus.Running)
            {
                RequestModel<AccountDetails> requestModel = RequestModel<AccountDetails>.CreateRequest(account, status);

                await client.StartNewAsync(AppConstants.CreateAcountOrchestration, requestModel.RequestId, requestModel);
                log.LogInformation($"Started orchestration with ID = '{requestModel.RequestId}'.");

            }

            await client.WaitForCompletionOrCreateCheckStatusResponseAsync(req,
                account.UniqueRequestId, TimeSpan.FromSeconds(25), TimeSpan.FromSeconds(5));

            status = await client.GetStatusAsync(account.UniqueRequestId, true, true, true);
            

            switch (status.RuntimeStatus)
            {
                case OrchestrationRuntimeStatus.Completed:
                    outPut["finishOrchestrationSuccess"] = true;
                    break;
                default:
                    outPut["finishOrchestrationSuccess"] = false;
                    outPut["checkStatusUrl"] = $"{baseUrl}/GetAccountProcessStatus/id/{account.UniqueRequestId}";
                    break;
            }

            //outPut["runningStatus"] = JsonConvert.SerializeObject(status);
            //outPut["data"]=
            OkObjectResult httpResponse =  new OkObjectResult(outPut);
            return httpResponse;
        }

        [OpenApiOperation(operationId: "CreateAccount")]
        [OpenApiParameter(name: "id", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **processor id** parameter")]
        //[OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(JToken))]
        [FunctionName("GetAccountProcessStatus")]
        public async Task<IActionResult> GetAccountProcessStatus(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient client,
            ILogger log)
        {
            if (req.RequestUri.ParseQueryString().Count == 0 || !req.RequestUri.ParseQueryString().HasKeys())
            {
                throw new ArgumentException("Invalid data of request");
            }
            var id = req.RequestUri.ParseQueryString().GetValues("id");
            if (string.IsNullOrWhiteSpace(id[0]))
            {
                throw new ArgumentException("Invalid data of request");
            }

            var instanceId = req.RequestUri.ParseQueryString().GetValues("id")[0];

            var instantce = await client.GetStatusAsync(instanceId, true, true);
            dynamic result = new Dictionary<string,object>();
            result["instanceId"] = id;
            result["status"] = instantce;

            OkObjectResult httpResponse = new OkObjectResult(result);
            return httpResponse;
            //var result = JsonConvert.SerializeObject(instantce, Formatting.Indented,
            //    new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            //log.LogInformation("Check account process instance with id: {instanceId},get result: {result}", instanceId, result);

            //return req.CreateCustomResponse(System.Net.HttpStatusCode.OK, result);
        }
    }
}