using FunctionDurableAppTest.ActivityFunctions;
using FunctionDurableAppTest.Models;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionDurableAppTest.Orchestrators
{
    public class AccountBusinessService
    {
        public async Task<AccountDetails> AccountBusiness(IDurableOrchestrationContext context,AccountDetails accountDetails)
        {
            var savedAccount = await context.CallActivityAsync<AccountDetails>(nameof(SaveAccount), accountDetails);
            if (savedAccount != null)
            {
                var archivedAccount = await context.CallActivityAsync<AccountDetails>(nameof(ArchiveAccount), savedAccount);
                if (archivedAccount != null)
                {
                    var notifiedAccount = await context.CallActivityAsync<AccountDetails>(nameof(NotifyAccount), archivedAccount);
                    return notifiedAccount;
                }
            }
            return null;
        }
    }
}
