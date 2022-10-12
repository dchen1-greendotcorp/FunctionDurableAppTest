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
        //public string Status { get; set; }

        public string ProcessInstanceId { get; set; }

        public Dictionary<string,bool> ProcessStatus { get; set; }=new Dictionary<string,bool>();


        public static AccountDetails CreateAccountDetails(string username)
        {
            AccountDetails accountDetails = new AccountDetails();

            accountDetails.UserName = username;
            accountDetails.AccountId = Guid.NewGuid().ToString();

            foreach(var c in AppConstants.AccountProcessList)
            {
                accountDetails.ProcessStatus[c] = false;
            }

            return accountDetails;
        }
    }
}
