using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AsyncAwaitBestPractices;
using Microsoft.Extensions.Logging;
using ServiceBusDriver.Client.Services;
using ServiceBusDriver.Shared.Features.Trace;

namespace ServiceBusDriver.Client.CustomHandlers
{
    public class HttpMetricsHandler : DelegatingHandler
    {
        private readonly ILogger<HttpMetricsHandler> _logger;
        private readonly ITraceLogsNotifier _traceLogsNotifier;

        public HttpMetricsHandler(ILogger<HttpMetricsHandler> logger, ITraceLogsNotifier traceLogsNotifier)
        {
            _logger = logger;
            _traceLogsNotifier = traceLogsNotifier;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var sw = Stopwatch.StartNew();
            
            try
            {
                _traceLogsNotifier.AddToQueue(new TraceModel($"Request initiated to {request?.RequestUri?.LocalPath}")).SafeFireAndForget();

                var res = await base.SendAsync(request, cancellationToken);
                 
                _traceLogsNotifier.AddToQueue(new TraceModel($"Request to {request?.RequestUri?.LocalPath} completed in {sw.ElapsedMilliseconds} ms")).SafeFireAndForget();

                if (res.IsSuccessStatusCode)
                {
                    _traceLogsNotifier.AddToQueue(new TraceModel($"Request to {request?.RequestUri?.LocalPath} is successful")).SafeFireAndForget();
                }
                else
                {
                    _traceLogsNotifier.AddToQueue(TraceTypeEnum.ERROR, $"Request to {request?.RequestUri?.LocalPath} failed with Response code {res.StatusCode}").SafeFireAndForget();
                    
                    var error = await res?.Content?.ReadAsStringAsync(cancellationToken);
                    _traceLogsNotifier.AddToQueue(TraceTypeEnum.ERROR, $"Error {error}").SafeFireAndForget();

                }
               
                
                return res;
            }
            catch (Exception)
            {
                _traceLogsNotifier.AddToQueue(new TraceModel($"Request to {request.RequestUri.LocalPath} completed in {sw.ElapsedMilliseconds} ms")).SafeFireAndForget();
                throw;
            }
        }
    }
}