using FunctionDurableAppTest.Models;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System;
using System.Threading.Tasks;

namespace FunctionDurableAppTest.OrchestrationHandlers
{
    public class CancellAccountEventHandler: IOrchestrationEventHandler
    {
        public string EventName => AppConstants.CancellAccount_Event;

        public OrchestrationResponse Handle( OrchestrationParameters orchestration)
        {
            throw new NotImplementedException();
        }
    }
}
