using FunctionDurableAppTest.Models;
using System.Collections.Generic;

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
                _accountDict[account.AccountId]=account;
            }
        }
    }
}
