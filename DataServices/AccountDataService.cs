using FunctionDurableAppTest.Models;
using System.Collections.Generic;
using System.Linq;

namespace FunctionDurableAppTest.DataServices
{
    public class AccountDataService: IAccountDataService
    {
        private object _locker = new object();
        Dictionary<string, AccountDetails> _accountDict = new Dictionary<string, AccountDetails>();

        public AccountDetails GetAccountDetailsById(string id)
        {
            if(_accountDict.ContainsKey(id))
            {
                return _accountDict[id];
            }
            return null;
        }

        public void SaveAccountDetails(AccountDetails account)
        {
            lock (_locker)
            {
                if(!account.ProcessStatus.Any())
                {
                    foreach (var c in AppConstants.AccountProcessList)
                    {
                        account.ProcessStatus[c] = false;
                    }
                }
                _accountDict[account.AccountId]=account;
            }
        }
    }
}
