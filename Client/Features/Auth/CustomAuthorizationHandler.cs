using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.Extensions.Logging;
using ServiceBusDriver.Client.Constants;

namespace ServiceBusDriver.Client.Features.Auth
{
    public class CustomAuthorizationHandler : DelegatingHandler
    {
        private readonly ILocalStorageService _localStorageService;
        private readonly ILogger<CustomAuthorizationHandler> _logger;

        public CustomAuthorizationHandler(ILocalStorageService localStorageService, ILogger<CustomAuthorizationHandler> logger)
        {
            //injecting local storage service
            _localStorageService = localStorageService;
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            //getting token from the localstorage
            var jwtToken = await _localStorageService.GetItemAsStringAsync(ApiConstants.LocalStorageConstants.JwtTokenKey);

            //adding the token in authorization header
            if (jwtToken != null)
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
            
            //sending the request
            var response =  await base.SendAsync(request, cancellationToken);

            return response;
        }
    }
}