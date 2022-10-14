using FunctionDurableAppTest.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using Azure.Data.Tables;
using System.Threading.Tasks;

namespace FunctionDurableAppTest.DataServices
{
    public class TableDataService: IAccountDataService
    {
        TableClient _tableClient;
        public TableDataService(IConfiguration configuration)
        {
            TableServiceClient tableServiceClient = new TableServiceClient(configuration["dbconnection"]);
            _tableClient = tableServiceClient.GetTableClient(
                    tableName: "accounts"
                );
            _tableClient.CreateIfNotExists();
        }
       
        //Dictionary<string, AccountDetails> _accountDict = new Dictionary<string, AccountDetails>();

        public async Task<AccountDetails> GetAccountDetailsById(string id)
        {
            try
            {
                var existAcc = await _tableClient.GetEntityAsync<AccountEntity>("myaccount", id);

                return existAcc.Value;
            }
            catch (System.Exception)
            {
                return null;
                //throw;
            }
            
        }

        public async Task InsertAccountDetails(AccountDetails account)
        {
            try
            {
                AccountEntity accountEntity = AccountEntity.CreateAccountEntity(account);

                await _tableClient.AddEntityAsync<AccountEntity>(accountEntity);
                
            }
            catch (System.Exception e)
            {

                //throw;
            }
            
        }

        public async Task UpdateSaveAccountStatus(string accountId, bool status)
        {
            Azure.Response<AccountEntity> existAcc = await _tableClient.GetEntityAsync<AccountEntity>("myaccount", accountId);
            if( existAcc!=null)
            {
                existAcc.Value.SaveAccount = status;

                await _tableClient.UpdateEntityAsync<AccountEntity>(existAcc.Value, Azure.ETag.All);
            }
        }
        public async Task UpdateArchiveAccountStatus(string accountId, bool status)
        {
            Azure.Response<AccountEntity> existAcc = await _tableClient.GetEntityAsync<AccountEntity>("myaccount", accountId);
            if (existAcc != null)
            {
                existAcc.Value.ArchiveAccount = status;

                await _tableClient.UpdateEntityAsync<AccountEntity>(existAcc.Value, Azure.ETag.All);
            }
        }
        public async Task UpdateNotifyAccountStatus(string accountId, bool status)
        {
            Azure.Response<AccountEntity> existAcc = await _tableClient.GetEntityAsync<AccountEntity>("myaccount", accountId);
            if (existAcc != null)
            {
                existAcc.Value.NotifyAccount = status;

                await _tableClient.UpdateEntityAsync<AccountEntity>(existAcc.Value, Azure.ETag.All);
            }
        }
    }
}
