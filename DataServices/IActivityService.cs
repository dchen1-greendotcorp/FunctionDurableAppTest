using FunctionDurableAppTest.Models;

namespace FunctionDurableAppTest.DataServices
{
    public interface IActivityService<T> where T : IRequest
    {
        RequestModel<T> IsActivityComplemetedBefore(RequestModel<T> requestModel, string activityName) ;
    }
}
