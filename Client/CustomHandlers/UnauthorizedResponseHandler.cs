using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace ServiceBusDriver.Client.CustomHandlers
{
    public class UnauthorizedResponseHandler : DelegatingHandler
    {
        private readonly ILogger<UnauthorizedResponseHandler> _logger;
        private readonly NavigationManager _navigationManager;

        public UnauthorizedResponseHandler(ILogger<UnauthorizedResponseHandler> logger, NavigationManager navigationManager)
        {
            _logger = logger;
            _navigationManager = navigationManager;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                var res = await base.SendAsync(request, cancellationToken);

                if (!res.IsSuccessStatusCode)
                {
                    _logger.LogError("Request to {0} resulted in Error with Status Code - {1}", request.RequestUri.AbsolutePath, res.StatusCode);

                    // Check if unauthorized
                    if (res.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        _logger.LogError("Request Unauthorized. Navigating to Login");
                        _navigationManager.NavigateTo("/login");
                    }
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}