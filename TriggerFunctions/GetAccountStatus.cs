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

namespace FunctionDurableAppTest.TriggerFunctions
{
    public class GetAccountStatus
    {
        private readonly IAccountDataService accountDataService;

        public GetAccountStatus(IAccountDataService accountDataService)
        {
            this.accountDataService = accountDataService;
        }

        [FunctionName("GetAccountStatus")]
        public HttpResponseMessage HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient client,
            ILogger log)
        {
            if (req.RequestUri.ParseQueryString().Count == 0 || !req.RequestUri.ParseQueryString().HasKeys())
            {
                return req.CreateCustomResponse(System.Net.HttpStatusCode.BadRequest, "Invalid data of request");
            }
            var id = req.RequestUri.ParseQueryString().GetValues("id");
            if (string.IsNullOrWhiteSpace(id[0]))
            {
                return req.CreateCustomResponse(System.Net.HttpStatusCode.BadRequest, "Invalid query string of parameters");
            }

            var accountId = req.RequestUri.ParseQueryString().GetValues("id")[0];

            var account=accountDataService.GetAccountDetailsById(accountId);

            if(account==null)
            {
                return req.CreateCustomResponse(System.Net.HttpStatusCode.OK, $"Cannot find account with given id: {accountId}");
            }

            var result = JsonConvert.SerializeObject(account, Formatting.Indented,
                new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            string formate = string.Format("System find account with id: {instanceId}, get result: {result}", accountId, result);

            log.LogInformation(formate);

            return req.CreateCustomResponse(System.Net.HttpStatusCode.OK, formate);
        }

    }
}