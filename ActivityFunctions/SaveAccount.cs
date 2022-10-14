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
        public async Task<AccountDetails> SaveAccountActivity([ActivityTrigger] IDurableActivityContext context,  ILogger log)
        {
            AccountDetails account= context.GetInput<AccountDetails>();

            var data = await _accountDataService.GetAccountDetailsById(account.AccountId);
            if(data == null)
            {
                account.SaveAccount = true;

                await _accountDataService.InsertAccountDetails(account);
                log.LogInformation($"Save {account.UserName} success!");
                data = await _accountDataService.GetAccountDetailsById(account.AccountId);
            }
            else
            {
                if(!data.SaveAccount)
                {
                    data.SaveAccount = true;
                    await _accountDataService.UpdateSaveAccountStatus(account.AccountId, true);
                }
            }

            return data;
        }
    }
}
