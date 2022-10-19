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

namespace FunctionDurableAppTest.TriggerFunctions
{
    public class CreateAccountRequest
    {
        private readonly IAccountDataService accountDataService;

        public CreateAccountRequest(IAccountDataService accountDataService)
        {
            this.accountDataService = accountDataService;
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

            var res = await client.GetStatusAsync(account.ProcessInstanceId, true, true);
            if (res == null)
            {
                string instanceId = await client.StartNewAsync("Orchestration", account.AccountId, account);
                string info = $"Started orchestration with process ID = '{instanceId}', and accountId = '{account.AccountId}' .";
                log.LogInformation(info);
                return req.CreateCustomResponse(System.Net.HttpStatusCode.OK, info);
            }
            else if (res.RuntimeStatus == OrchestrationRuntimeStatus.Running)
            {
                string info = $"submit event {AppConstants.ResubmitAccount_Event} to orchestration with ID = '{account.ProcessInstanceId}'.";
                log.LogInformation(info);
                var orchEvtObj = new OrchestrationEventObj { EventName = AppConstants.ResubmitAccount_Event, EventData = account };
                await client.RaiseEventAsync(account.ProcessInstanceId, AppConstants.ResubmitAccount_Event, orchEvtObj);

                string data = JsonConvert.SerializeObject(res, Formatting.Indented, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
                return req.CreateErrorResponse(System.Net.HttpStatusCode.OK, data);
            }
            else
            {
                string data= JsonConvert.SerializeObject(res,Formatting.Indented,new JsonSerializerSettings {
                    NullValueHandling= NullValueHandling.Ignore,ReferenceLoopHandling= ReferenceLoopHandling.Ignore });
                return req.CreateErrorResponse(System.Net.HttpStatusCode.OK, data);
            }
            
            //if (string.IsNullOrEmpty(account.ProcessInstanceId))
            //{

            //    string instanceId = await client.StartNewAsync("Orchestration", account.AccountId, account);
            //    string info = $"Started orchestration with process ID = '{instanceId}', and accountId = '{account.AccountId}' .";
            //    log.LogInformation(info);
            //    return req.CreateCustomResponse(System.Net.HttpStatusCode.OK, info);
            //}
            //else
            //{
            //    var res = await client.GetStatusAsync(account.ProcessInstanceId, true, true);
            //    if (res.RuntimeStatus == OrchestrationRuntimeStatus.Running)
            //    {
            //        string info = $"submit event {AppConstants.ResubmitAccount_Event} to orchestration with ID = '{account.ProcessInstanceId}'.";
            //        log.LogInformation(info);
            //        var orchEvtObj = new OrchestrationEventObj { EventName = AppConstants.ResubmitAccount_Event, EventData = account };
            //        await client.RaiseEventAsync(account.ProcessInstanceId, AppConstants.ResubmitAccount_Event, orchEvtObj);

            //        return req.CreateCustomResponse(System.Net.HttpStatusCode.OK, info);
            //    }

            //    if (res.RuntimeStatus == OrchestrationRuntimeStatus.Completed)
            //    {
            //        var acc = await accountDataService.GetAccountDetailsById(account.AccountId);
            //        var result = JsonConvert.SerializeObject(acc, Formatting.Indented,
            //            new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            //        string format = string.Format("System find account with id: {AccountId}, get result: {result}", account.AccountId, result);
            //        log.LogInformation(format);

            //        return req.CreateCustomResponse(System.Net.HttpStatusCode.OK, format);
            //    }

            //    var res1 = JsonConvert.SerializeObject(res, Formatting.Indented,
            //            new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            //    var format1 = string.Format("System find account process with instance: {AccountId}, get status result: {result}", account.ProcessInstanceId, res1);
            //    log.LogInformation(format1);

            //    return req.CreateCustomResponse(System.Net.HttpStatusCode.OK, format1);
            //}

        }
    }
}