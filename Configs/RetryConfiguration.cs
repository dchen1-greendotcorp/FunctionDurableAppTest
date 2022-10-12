using Microsoft.Extensions.Configuration;

namespace FunctionDurableAppTest.Configs
{
    public class RetryConfiguration : IRetryConfiguration
    {
        private readonly IConfiguration configuration;

        public RetryConfiguration(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public double FirstRetryInterval => double.Parse(configuration[AppConstants.FirstRetryInterval]);
        public int MaxNumberOfAttempts => int.Parse(configuration[AppConstants.MaxNumberOfAttempts]);
        public double BackoffCoefficient => double.Parse(configuration[AppConstants.BackoffCoefficient]);
    }
}
