using FunctionDurableAppTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionDurableAppTest.DataServices
{
    public class NotificationService: INotificationService
    {
        private Dictionary<string, bool> _notification= new Dictionary<string, bool>();
        public Task<bool> NotifyAccount(AccountDetails account)
        {
            if(_notification.ContainsKey(account.AccountId))
            {
                _notification[account.AccountId] = true;
            }
            else
            {
                _notification[account.AccountId] = false;
            }

            return Task.FromResult(_notification.ContainsKey(account.AccountId));
        }
    }
}
