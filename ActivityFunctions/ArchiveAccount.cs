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
        public async Task<AccountDetails> ArchiveAccountActivity([ActivityTrigger] IDurableActivityContext context,  ILogger log)
        {
            AccountDetails account = context.GetInput<AccountDetails>();
            account.ArchiveAccount = true;
            log.LogInformation($"Archive {account.UserName} success!");
            return account;
            //var data = await _accountDataService.GetAccountDetailsById(account.AccountId);
            //if(data.ArchiveAccount)
            //{
            //    return data;
            //}
            //else
            //{
            //    await _accountDataService.UpdateArchiveAccountStatus(account.AccountId, true);
            //    log.LogInformation($"Archive {account.UserName} success!");
            //    data = await _accountDataService.GetAccountDetailsById(account.AccountId);
            //    return data;
            //}
        }
    }
}
