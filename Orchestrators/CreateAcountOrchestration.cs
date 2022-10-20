using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FunctionDurableAppTest.ActivityFunctions;
using FunctionDurableAppTest.DataServices;
using FunctionDurableAppTest.Models;
using FunctionDurableAppTest.OrchestrationHandlers;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
namespace FunctionDurableAppTest.Orchestrators
{
    public class CreateAcountOrchestration
    {
        private readonly RetryOptions retryOptions;
        public CreateAcountOrchestration(RetryOptions retryOptions,  IAccountDataService accountDataService)
        {
            this.retryOptions = retryOptions;
            //{
            //    eventHandlerDict[item.EventName] = item;
            //}
        }

        [FunctionName("CreateAcountOrchestration")]
        public async Task<RequestModel<AccountDetails>> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context, ILogger log)
        {

            RequestModel<AccountDetails> requestModel = context.GetInput<RequestModel<AccountDetails>>();
            RequestModel<AccountDetails> saved = null;
            RequestModel<AccountDetails> archived = null;
            RequestModel<AccountDetails> notified = null;
            try
            {
                
                saved=await context.CallActivityAsync<RequestModel<AccountDetails>>(nameof(ProcessAccountActivities.SaveAccountActivity), requestModel);
                if(saved!=null)
                {
                    archived = await context.CallActivityAsync<RequestModel<AccountDetails>>(nameof(ProcessAccountActivities.ArchiveAccountActivity), saved);
                    if(archived!=null)
                    {
                        if(requestModel.ProcessedHistory!=null)
                        {
                            archived.Request.NotifyAccount = true;
                        }
                        notified = await context.CallActivityAsync<RequestModel<AccountDetails>>(nameof(ProcessAccountActivities.NotifyAccountActivity), archived);
                    }
                }

            }
            catch (Exception ex)
            {
                log.LogError("Orchestra met error = {ex}", ex);

                throw;
            }
            
            return notified;
            
        }

    }
}