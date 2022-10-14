using Azure;
using Azure.Data.Tables;
using System;

namespace FunctionDurableAppTest.Models
{
    public class AccountEntity : AccountDetails,ITableEntity
    {
        public string RowKey { get; set; } = default!;

        public string PartitionKey { get; set; } = "myaccount";
        public ETag ETag { get; set; } = default!;

        public DateTimeOffset? Timestamp { get; set; } = default!;

        //public string AccountId { get; set; }
        //public string UserName { get; set; }
        //public bool SaveAccount { get; set; }
        //public bool ArchiveAccount { get; set; }
        //public bool NotifyAccount { get; set; }
        //public string ProcessInstanceId { get; set; }

        public AccountEntity()
        {

        }

        public static AccountEntity CreateAccountEntity(AccountDetails accountDetails)
        {
            var ent = new AccountEntity();
            if(string.IsNullOrEmpty(accountDetails.AccountId))
            {
                accountDetails.AccountId = Guid.NewGuid().ToString();
            }
            ent.RowKey = accountDetails.AccountId;
            ent.PartitionKey = "myaccount";

            ent.AccountId = accountDetails.AccountId;
            ent.ProcessInstanceId= accountDetails.ProcessInstanceId;
            ent.UserName= accountDetails.UserName;

            ent.SaveAccount= accountDetails.SaveAccount;
            ent.ArchiveAccount= accountDetails.ArchiveAccount;
            ent.NotifyAccount  = accountDetails.NotifyAccount;

            return ent;
        }
    }
}
