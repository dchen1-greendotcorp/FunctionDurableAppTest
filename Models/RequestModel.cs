using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionDurableAppTest.Models
{
    public class RequestModel<T> where T: IRequest
    {
        public T Request { get; set; }
        public JArray ProcessedHistory { get; set; }

        public string RequestId => Request.UniqueRequestId;

        public static RequestModel<T> CreateRequest(T request, DurableOrchestrationStatus durableOrchestrationStatus)
        {
            RequestModel<T> requestModel = new RequestModel<T>();
            requestModel.Request = request;
            if(durableOrchestrationStatus!=null)
            {
                requestModel.ProcessedHistory = durableOrchestrationStatus.History;
            }

            return requestModel;
        }
    }
}
