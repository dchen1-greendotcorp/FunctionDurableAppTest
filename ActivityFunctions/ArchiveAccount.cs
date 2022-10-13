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
        public async Task ArchiveAccountActivity([ActivityTrigger] AccountDetails account, ILogger log)
        {
            await _accountDataService.UpdateArchiveAccountStatus(account.AccountId, true);
            log.LogInformation($"Archive {account.UserName} success!");
        }
    }
}
