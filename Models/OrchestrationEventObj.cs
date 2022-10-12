using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionDurableAppTest.Models
{
    public class OrchestrationEventObj
    {
        public string EventName { get; set; }
        public dynamic EventData { get; set; }
    }
}
