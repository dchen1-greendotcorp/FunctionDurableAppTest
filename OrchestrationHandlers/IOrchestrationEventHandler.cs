using FunctionDurableAppTest.Models;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionDurableAppTest.OrchestrationHandlers
{
    public interface IOrchestrationEventHandler
    {
        string EventName { get; }
        Task<OrchestrationResponse> HandleAsync(IDurableOrchestrationContext context, OrchestrationParameters orchestration);
    }
}
