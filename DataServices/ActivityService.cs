using FunctionDurableAppTest.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionDurableAppTest.DataServices
{
    public class ActivityService<T>: IActivityService<T> where T : IRequest
    {
        public RequestModel<T> IsActivityComplemetedBefore(RequestModel<T> requestModel, string activityName) 
        {
            if(requestModel!=null && requestModel.ProcessedHistory!=null)
            {
                var result = requestModel.ProcessedHistory?.Where(v =>
                   v["EventType"].ToString() == "TaskCompleted" && v["FunctionName"].ToString() == activityName &&
                   !string.IsNullOrWhiteSpace(v["Result"].ToString())).ToList();

                if (result != null && result.Any())
                {
                    var completeEvent = result.First();
                    Newtonsoft.Json.Linq.JToken typedCompletedEvent = completeEvent["Result"];

                    var data=JsonConvert.DeserializeObject<RequestModel<T>>(typedCompletedEvent.ToString());
                    data.ProcessedHistory = requestModel.ProcessedHistory;
                    return data;
                }
            }
            return null;
        }
    }
    public interface IActivityService<T> where T : IRequest
    {
        RequestModel<T> IsActivityComplemetedBefore(RequestModel<T> requestModel, string activityName) ;
    }
}
