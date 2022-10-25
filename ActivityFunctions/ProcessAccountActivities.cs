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
            if (processed!=null)
            {
                return processed;
            }

            //after this line, business part
            request.Request.SaveAccount = true;

            log.LogInformation($"Save {request.Request.UserName} success!");
            return request;
        }

        [FunctionName(nameof(ArchiveAccountActivity))]
        public async Task<RequestModel<AccountDetails>> ArchiveAccountActivity([ActivityTrigger] IDurableActivityContext context, ILogger log)
        {
            RequestModel<AccountDetails> request = context.GetInput<RequestModel<AccountDetails>>();
            var processed = _activityService.IsActivityComplemetedBefore(request, nameof(ArchiveAccountActivity));
            if (processed != null)
            {
                return processed;
            }

            //after this line, business part
            request.Request.ArchiveAccount = true;

            log.LogInformation($"Archive {request.Request.UserName} success!");
            return request;
        }

        [FunctionName(nameof(NotifyAccountActivity))]
        public async Task<RequestModel<AccountDetails>> NotifyAccountActivity([ActivityTrigger] IDurableActivityContext context, ILogger log)
        {
            RequestModel<AccountDetails> request = context.GetInput<RequestModel<AccountDetails>>();
            var processed = _activityService.IsActivityComplemetedBefore(request, nameof(NotifyAccountActivity));
            if (processed != null)
            {
                return processed;
            }

            //after this line, business part, 
            if (!request.Request.NotifyAccount)
            {
                //demo first time failed
                var errmsg = $"Notify {request.Request.UserName} failed!";
                request.Request.NotifyAccount = true;

                log.LogError(errmsg);
                throw new Exception(errmsg);
            }
            else
            {
                //demo second time success
                log.LogInformation($"Notify {request.Request.UserName} success!");
                return request;
            }
        }
    }
}
