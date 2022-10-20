
using Microsoft.Azure.Functions.Extensions.DependencyInjection;

using Microsoft.Extensions.Configuration;
using System.IO;

[assembly: FunctionsStartup(typeof(FunctionDurableAppTest.Startup))]
namespace FunctionDurableAppTest
{
    public class Startup : FunctionsStartup
    {

        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            FunctionsHostBuilderContext context = builder.GetContext();

            builder.ConfigurationBuilder
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            var config = builder.GetContext().Configuration;
            //builder.Services.AddDurableClientFactory();
            builder.Services.RegistrationServices(config);
            

            //builder.Services.AddGDApplicationInsights(config);

            //builder.Services.AddLogging(loggingBuilder =>
            //{
            //    //loggingBuilder.ClearProviders();

            //    loggingBuilder.AddFilter<GDApplicationInsightsLoggerProvider>(
            //           "", LogLevel.Information);
            //    loggingBuilder.AddFilter<GDApplicationInsightsLoggerProvider>(
            //           "", LogLevel.Trace);

            //    loggingBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);//Trace
            //});

            //var serviceProvider = builder.Services.BuildServiceProvider();

            //var GDLoggerProvider = serviceProvider.GetRequiredService<GDApplicationInsightsLoggerProvider>();

            //var logger = GDLoggerProvider.CreateLogger("Startup");

            //logger.LogInformation("Got Here in Startup");

        }
    }
}

