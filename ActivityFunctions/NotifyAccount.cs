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
        private readonly INotificationService _notificationService;
        private readonly IAccountDataService _accountDataService;

        public NotifyAccount(INotificationService notificationService,IAccountDataService accountDataService)
        {
            _notificationService = notificationService;
            _accountDataService = accountDataService;
        }

        [FunctionName("NotifyAccount")]
        public async Task<bool> NotifyAccountActivity([ActivityTrigger] AccountDetails account, ILogger log)
        {
            var result=await _notificationService.NotifyAccount(account)
                .ConfigureAwait(false);
            if(result)
            {
                account.ProcessStatus[AppConstants.ProcessNotification] = true;
                _accountDataService.SaveAccountDetails(account);
                log.LogInformation($"Notify {account.UserName} success!");
                await Task.Delay(1000);
                return true;
            }
            else
            {
                log.LogError($"Notify {account.UserName} failed!");

                throw new Exception($"Notify {account.UserName} failed!");
            }
        }
    }
}
