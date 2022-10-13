using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using FunctionDurableAppTest.Models;
using FunctionDurableAppTest.DataServices;

namespace FunctionDurableAppTest.ActivityFunctions
{
    public class ArchiveAccount
    {
        private IAccountDataService _accountDataService;

        public ArchiveAccount(IAccountDataService accountDataService)
        {
            _accountDataService = accountDataService;
        }

        [FunctionName("ArchiveAccount")]
        public async Task<bool> ArchiveAccountActivity([ActivityTrigger] AccountDetails account, ILogger log)
        {
            var acc=_accountDataService.GetAccountDetailsById(account.AccountId);
            if(acc!=null && acc.ProcessStatus[AppConstants.ProcessArchive])
            {
                return true;
            }

            account.ProcessStatus[AppConstants.ProcessArchive] = true;

            _accountDataService.SaveAccountDetails(account);
            log.LogInformation($"Archive {account.UserName} success!");

            await Task.Delay(100);
            return true;
        }
    }
}
