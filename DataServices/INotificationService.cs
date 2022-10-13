using FunctionDurableAppTest.Models;
using System.Threading.Tasks;

namespace FunctionDurableAppTest.DataServices
{
    public interface   INotificationService
    {
        bool NotifyAccount(AccountDetails account);
    }
}
