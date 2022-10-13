using FunctionDurableAppTest.Models;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionDurableAppTest.DataServices
{
    public interface IAccountDataService
    {
        Task<AccountDetails> GetAccountDetailsById(string id);
        Task InsertAccountDetails(AccountDetails account);

        Task UpdateSaveAccountStatus(string accountId, bool status);
        Task UpdateArchiveAccountStatus(string accountId, bool status);

        Task UpdateNotifyAccountStatus(string accountId, bool status);
    }
}
