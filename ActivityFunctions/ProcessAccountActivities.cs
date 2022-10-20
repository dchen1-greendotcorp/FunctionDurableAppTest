using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using FunctionDurableAppTest.Models;
using FunctionDurableAppTest.DataServices;

namespace FunctionDurableAppTest.ActivityFunctions
{
    public class ProcessAccountActivities
    {
        private readonly IActivityService<AccountDetails> _activityService;

        public ProcessAccountActivities(IActivityService<AccountDetails> activityService)
        {
            _activityService = activityService;
        }

        [FunctionName(nameof(SaveAccountActivity))]
        public async Task<RequestModel<AccountDetails>> SaveAccountActivity([ActivityTrigger] IDurableActivityContext context, ILogger log)
        {
            RequestModel<AccountDetails> request = context.GetInput<RequestModel<AccountDetails>>();

            var processed = _activityService.IsActivityComplemetedBefore(request, nameof(SaveAccountActivity));
            if (processed)
            {
                return request;
            }

            request.Request.SaveAccount = true;

            log.LogInformation($"Save {request.Request.UserName} success!");
            return request;
        }

        [FunctionName(nameof(ArchiveAccountActivity))]
        public async Task<RequestModel<AccountDetails>> ArchiveAccountActivity([ActivityTrigger] IDurableActivityContext context, ILogger log)
        {
            RequestModel<AccountDetails> request = context.GetInput<RequestModel<AccountDetails>>();
            var processed = _activityService.IsActivityComplemetedBefore(request, nameof(SaveAccountActivity));
            if (processed)
            {
                return request;
            }

            request.Request.ArchiveAccount = true;

            log.LogInformation($"Archive {request.Request.UserName} success!");
            return request;
        }

        [FunctionName(nameof(NotifyAccountActivity))]
        public async Task<RequestModel<AccountDetails>> NotifyAccountActivity([ActivityTrigger] IDurableActivityContext context, ILogger log)
        {
            RequestModel<AccountDetails> request = context.GetInput<RequestModel<AccountDetails>>();
            var processed = _activityService.IsActivityComplemetedBefore(request, nameof(SaveAccountActivity));
            if (processed)
            {
                return request;
            }

            if (!request.Request.NotifyAccount)
            {
                var errmsg = $"Notify {request.Request.UserName} failed!";
                request.Request.NotifyAccount = true;

                log.LogError(errmsg);
                throw new Exception(errmsg);
            }
            else
            {
                log.LogInformation($"Notify {request.Request.UserName} success!");
                return request;
            }
        }
    }
}
