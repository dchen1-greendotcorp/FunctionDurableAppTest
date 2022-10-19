using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionDurableAppTest.Models
{
    public class AccountDetails
    {
        public string AccountId { get; set; }
        public string UserName { get; set; }
        public string ProcessInstanceId { get; set; }

        public bool SaveAccount { get; set; }
        public bool ArchiveAccount { get; set; }
        public bool NotifyAccount { get; set; }

        public static AccountDetails CreateAccountDetails(string username)
        {
            AccountDetails accountDetails = new AccountDetails();

            accountDetails.UserName = username;
            accountDetails.AccountId = Guid.NewGuid().ToString();
            accountDetails.ProcessInstanceId = accountDetails.AccountId;

            return accountDetails;
        }
    }
}
