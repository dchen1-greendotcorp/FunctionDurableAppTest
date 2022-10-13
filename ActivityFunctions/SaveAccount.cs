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
        public async Task SaveAccountActivity([ActivityTrigger] AccountDetails account, ILogger log)
        {
            await _accountDataService.UpdateSaveAccountStatus(account.AccountId, true);
            log.LogInformation($"Save {account.UserName} success!");
        }
    }
}
