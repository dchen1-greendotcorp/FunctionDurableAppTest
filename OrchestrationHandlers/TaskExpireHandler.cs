using FunctionDurableAppTest.Models;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System.Threading.Tasks;

namespace FunctionDurableAppTest.OrchestrationHandlers
{
    public class TaskExpireHandler: IOrchestrationEventHandler
    {
        public string EventName => AppConstants.TaskExpireEvent;

        public OrchestrationResponse Handle( OrchestrationParameters orchestration)
        {
            OrchestrationResponse response = new OrchestrationResponse();
            response.CloseParent = true;

            return response;
        }
    }
}
