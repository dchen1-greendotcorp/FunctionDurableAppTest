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
    public class GetAccountProcessStatus
    {

        [FunctionName("GetAccountProcessStatus")]
        public async Task<HttpResponseMessage> HttpStart(
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

            var instanceId = req.RequestUri.ParseQueryString().GetValues("id")[0];

            var instantce = await client.GetStatusAsync(instanceId, true, true);

            var result = JsonConvert.SerializeObject(instantce, Formatting.Indented, 
                new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            log.LogInformation("Check account process instance with id: {instanceId},get result: {result}", instanceId, result);

            return req.CreateCustomResponse(System.Net.HttpStatusCode.OK, result);
        }

    }
}