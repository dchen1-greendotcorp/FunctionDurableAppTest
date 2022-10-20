
using FunctionDurableAppTest.Configs;
using FunctionDurableAppTest.DataServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace FunctionDurableAppTest
{
    public static class Registration
    {
        public static IServiceCollection RegistrationServices(this IServiceCollection services, IConfiguration configuration)
        {
 
            services.AddSingleton<IRetryConfiguration, RetryConfiguration>();

            services.AddSingleton(typeof(IActivityService<>), typeof(ActivityService<>));


            services.AddSingleton(sp =>
            {
                IRetryConfiguration retryConfiguration = sp.GetRequiredService<IRetryConfiguration>();

                return new Microsoft.Azure.WebJobs.Extensions.DurableTask.RetryOptions(
                    firstRetryInterval: TimeSpan.FromSeconds(retryConfiguration.FirstRetryInterval),
                    maxNumberOfAttempts: retryConfiguration.MaxNumberOfAttempts)
                { BackoffCoefficient = retryConfiguration.BackoffCoefficient };
            });

            return services;
        }
    }
}
