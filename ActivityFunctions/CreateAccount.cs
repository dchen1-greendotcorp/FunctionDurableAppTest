using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using FunctionDurableAppTest.Models;

namespace FunctionDurableAppTest.ActivityFunctions
{
    public class CreateAccount
    {
        public CreateAccount()
        {

        }

        [FunctionName("CreateAccount")]
        public async Task<bool> CreateAccountActivity([ActivityTrigger] AccountDetails account, ILogger log)
        {
            //var account = context.GetInput<AccountDetails>();

            //create account business
            log.LogInformation($"Saying hello to {account.UserName}");

            await Task.Delay(1000);
            return true;
        }
    }
}
