using System;
using System.Threading.Tasks;
using FunctionDurableAppTest.ActivityFunctions;
using FunctionDurableAppTest.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
namespace FunctionDurableAppTest.Orchestrators
{
    public class CreateAcountOrchestration
    {
        private readonly RetryOptions retryOptions;
        public CreateAcountOrchestration(RetryOptions retryOptions)
        {
            this.retryOptions = retryOptions;
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
                        #region second try good prerequisite
                        if (requestModel.ProcessedHistory!=null)
                        {
                            archived.Request.NotifyAccount = true;
                        }
                        #endregion

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