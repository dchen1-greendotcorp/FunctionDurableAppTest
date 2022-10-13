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
    public class SaveAccount
    {
        private IAccountDataService _accountDataService;

        public SaveAccount(IAccountDataService accountDataService)
        {
            _accountDataService = accountDataService;
        }

        [FunctionName("SaveAccount")]
        public async Task<bool> SaveAccountActivity([ActivityTrigger] AccountDetails account, ILogger log)
        {
            var acc = _accountDataService.GetAccountDetailsById(account.AccountId);
            if (acc != null && acc.ProcessStatus[AppConstants.ProcessSave])
            {
                return true;
            }

            account.ProcessStatus[AppConstants.ProcessSave] = true;

            _accountDataService.SaveAccountDetails(account);
            log.LogInformation($"Save {account.UserName} success!");

            await Task.Delay(1000);
            return true;
        }
    }
}
