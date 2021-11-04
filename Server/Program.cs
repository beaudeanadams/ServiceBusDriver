using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.Hosting;

namespace ServiceBusDriver.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                    webBuilder.ConfigureAppConfiguration((hostingContext, config) =>
                        {
                            var settings = config.Build();
                            config.AddAzureAppConfiguration(options =>
                            {
                                options.Connect(settings["ConnectionStrings:AppConfig"])
                                       // Load configuration values with no label
                                       .Select(KeyFilter.Any, LabelFilter.Null)
                                       // Override with any configuration values specific to current hosting env
                                       .Select(KeyFilter.Any, string.IsNullOrWhiteSpace(settings["environment"])? LabelFilter.Null: settings["environment"])
                                    .ConfigureRefresh(refresh =>
                                    {
                                        refresh.Register("sentinal", refreshAll: true).SetCacheExpiration(new TimeSpan(1, 0, 0));
                                    });
                            });
                        })
                        .UseStartup<Startup>());
    }
}
