using Azure.Core;
using FunctionDurableAppTest.Configs;
using FunctionDurableAppTest.DataServices;
using FunctionDurableAppTest.OrchestrationHandlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionDurableAppTest
{
    public static class Registration
    {
        public static IServiceCollection RegistrationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IAccountDataService, AccountDataService>();
            services.AddSingleton<INotificationService, NotificationService>();

            services.AddSingleton<IOrchestrationEventHandler, CancellAccountEventHandler>();
            services.AddSingleton<IOrchestrationEventHandler, TaskExpireHandler>();
            services.AddSingleton<IOrchestrationEventHandler, ResubmitAccountEventHandler>();

            services.AddSingleton<IRetryConfiguration, RetryConfiguration>();

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
