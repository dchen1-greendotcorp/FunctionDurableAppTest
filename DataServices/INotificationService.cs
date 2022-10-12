using FunctionDurableAppTest.Models;
using System.Threading.Tasks;

namespace FunctionDurableAppTest.DataServices
{
    public interface   INotificationService
    {
        Task<bool> NotifyAccount(AccountDetails account);
    }
}
