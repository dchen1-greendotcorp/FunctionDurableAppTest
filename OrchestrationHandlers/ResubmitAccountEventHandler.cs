using FunctionDurableAppTest.Models;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System;
using System.Threading.Tasks;

namespace FunctionDurableAppTest.OrchestrationHandlers
{
    public class ResubmitAccountEventHandler : IOrchestrationEventHandler
    {
        public string EventName => AppConstants.ResubmitAccount_Event;

        public Task<OrchestrationResponse> HandleAsync(IDurableOrchestrationContext context, OrchestrationParameters orchestration)
        {
            throw new NotImplementedException();
        }
    }
}
