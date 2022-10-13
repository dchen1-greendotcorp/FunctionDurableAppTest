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
 
        //private Dictionary<string, bool> _notification= new Dictionary<string, bool>();
        public bool NotifyAccount(AccountDetails account)
        {
                throw new NotImplementedException();
            
        }
    }
}
