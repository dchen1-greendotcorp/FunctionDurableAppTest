using FunctionDurableAppTest.Models;
using System.Collections.Generic;
using System.Linq;

namespace FunctionDurableAppTest.DataServices
{
    //public class AccountDataService: IAccountDataService
    //{
       
    //    Dictionary<string, AccountDetails> _accountDict = new Dictionary<string, AccountDetails>();

    //    public AccountDetails GetAccountDetailsById(string id)
    //    {
    //        if(_accountDict.ContainsKey(id))
    //        {
    //            return _accountDict[id];
    //        }
    //        return null;
    //    }

    //    public void InsertAccountDetails(AccountDetails account)
    //    {
    //        if(!_accountDict.ContainsKey(account.AccountId))
    //        {
    //            _accountDict[account.AccountId] = account;
    //        }
    //    }

    //    public void UpdateSaveAccountStatus(string accountId, bool status)
    //    {
    //        if (_accountDict.ContainsKey(accountId))
    //        {
    //            _accountDict[accountId].SaveAccount = status;
    //        }
    //    }
    //    public void UpdateArchiveAccountStatus(string accountId, bool status)
    //    {
    //        if (_accountDict.ContainsKey(accountId))
    //        {
    //            _accountDict[accountId].ArchiveAccount = status;
    //        }
    //    }
    //    public void UpdateNotifyAccountStatus(string accountId, bool status)
    //    {
    //        if (_accountDict.ContainsKey(accountId))
    //        {
    //            _accountDict[accountId].NotifyAccount = status;
    //        }
    //    }
    //}
}
