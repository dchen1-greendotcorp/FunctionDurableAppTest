using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using FunctionDurableAppTest.Models;
using FunctionDurableAppTest.DataServices;
using System;

namespace FunctionDurableAppTest.ActivityFunctions
{
    public class NotifyAccount
    {
        //private readonly INotificationService _notificationService;
        private readonly IAccountDataService _accountDataService;

        public NotifyAccount(IAccountDataService accountDataService)
        {
            _accountDataService = accountDataService;
        }

        [FunctionName("NotifyAccount")]
        public async Task<AccountDetails> NotifyAccountActivity([ActivityTrigger] IDurableActivityContext context, ILogger log)
        {
            string errmsg = string.Empty;
            AccountDetails account = context.GetInput<AccountDetails>();
            var existacc = await _accountDataService.GetAccountDetailsById(account.AccountId);

            if (!existacc.NotifyAccount)
            {
                await _accountDataService.UpdateNotifyAccountStatus(account.AccountId, true);
                errmsg = $"Notify {account.UserName} failed!";
                log.LogError(errmsg);
                throw new Exception(errmsg);
            }
            else
            {
                log.LogInformation($"Notify {account.UserName} success!");
                return existacc;
            }

        }
    }
}
