using FunctionDurableAppTest.Models;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionDurableAppTest.DataServices
{
    public interface IAccountDataService
    {
        AccountDetails GetAccountDetailsById(string id);
        void SaveAccountDetails(AccountDetails account);
    }
}
