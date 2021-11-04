using System;
using System.Net.Http;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Blazored.Toast;
using FluentValidation;
using MatBlazor;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServiceBusDriver.Client.CustomHandlers;
using ServiceBusDriver.Client.Features.Auth;
using ServiceBusDriver.Client.Features.Instance;
using ServiceBusDriver.Client.Features.Message;
using ServiceBusDriver.Client.Features.Subscription;
using ServiceBusDriver.Client.Features.Topic;
using ServiceBusDriver.Client.Services;
using ServiceBusDriver.Shared.Features.User.Validators;

namespace ServiceBusDriver.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.Logging.SetMinimumLevel(LogLevel.None);

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            // Authentication Components
            builder.Services.AddOptions();
            builder.Services.AddAuthorizationCore();
            builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
            builder.Services.AddTransient<CustomAuthorizationHandler>();
            builder.Services.AddTransient<HttpMetricsHandler>();
            builder.Services.AddTransient<UnauthorizedResponseHandler>();

            builder.Services.AddScoped<MessageNotifierService>();
            builder.Services.AddScoped<PropertiesNotifierService>();
            builder.Services.AddScoped<ITraceLogsNotifier,TraceLogsNotifier>();

            builder.Services.AddValidatorsFromAssembly(typeof(LoginRequestDtoValidator).Assembly);

            // Third Party Components
            builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddBlazoredToast();

            // UI Components
            builder.Services.AddMatBlazor();

            AddHttpClients(builder);

            await builder.Build().RunAsync();
        }

        private static void AddHttpClients(WebAssemblyHostBuilder builder)
        {
            var baseAddress = new Uri(builder.HostEnvironment.BaseAddress);

            builder.Services.AddHttpClient<ILoginHandler, LoginHandler>
                ("LoginClient", client => client.BaseAddress = baseAddress)
                .AddHttpMessageHandler<HttpMetricsHandler>();

            builder.Services.AddHttpClient<IRegisterHandler, RegisterHandler>
               ("RegisterClient", client => client.BaseAddress = baseAddress)
               .AddHttpMessageHandler<HttpMetricsHandler>();

            builder.Services.AddHttpClient<IInstanceHandler, InstanceHandler>
                    ("InstanceClient", client => client.BaseAddress = baseAddress)
                .AddHttpMessageHandler<CustomAuthorizationHandler>()
                .AddHttpMessageHandler<UnauthorizedResponseHandler>()
                .AddHttpMessageHandler<HttpMetricsHandler>();

            builder.Services.AddHttpClient<ITopicHandler, TopicHandler>
                    ("TopicClient", client => client.BaseAddress = baseAddress)
                .AddHttpMessageHandler<CustomAuthorizationHandler>()
                .AddHttpMessageHandler<UnauthorizedResponseHandler>()
                .AddHttpMessageHandler<HttpMetricsHandler>();

            builder.Services.AddHttpClient<ISubscriptionHandler, SubscriptionHandler>
                    ("SubscriptionClient", client => client.BaseAddress = baseAddress)
                .AddHttpMessageHandler<CustomAuthorizationHandler>()
                .AddHttpMessageHandler<UnauthorizedResponseHandler>()
                .AddHttpMessageHandler<HttpMetricsHandler>();

            builder.Services.AddHttpClient<IMessageHandler, MessageHandler>
                    ("MessageClient", client => client.BaseAddress = baseAddress)
                .AddHttpMessageHandler<CustomAuthorizationHandler>()
                .AddHttpMessageHandler<UnauthorizedResponseHandler>()
                .AddHttpMessageHandler<HttpMetricsHandler>();

        }
    }
}