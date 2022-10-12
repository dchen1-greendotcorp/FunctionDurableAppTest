using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionDurableAppTest.Configs
{
    public interface IRetryConfiguration
    {
        public double FirstRetryInterval { get; }
        public int MaxNumberOfAttempts { get; }
        public double BackoffCoefficient { get; }
    }
}
