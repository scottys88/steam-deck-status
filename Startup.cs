using System;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

[assembly: FunctionsStartup(typeof(SteamDeckStatus.Startup))]

namespace SteamDeckStatus
{
    class Startup : FunctionsStartup
    {
        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            
            string cs = Environment.GetEnvironmentVariable("ConnectionString");
            builder.ConfigurationBuilder.AddAzureAppConfiguration(cs);
        }
        public override void Configure(IFunctionsHostBuilder builder)
        {            
        }
    }
}