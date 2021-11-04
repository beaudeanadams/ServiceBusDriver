using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
// ReSharper disable PositionalPropertyUsedProblem

namespace ServiceBusDriver.Server.Services.HttpClient
{
    public class HttpClientHelper : IHttpClientHelper
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerSettings _jsonSerializerSettings;
        private readonly ILogger<HttpClientHelper> _logger;

        public HttpClientHelper(
            IHttpClientFactory httpClientFactory,
            ILogger<HttpClientHelper> logger)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _logger = logger;

            _jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Converters = new List<JsonConverter> { new StringEnumConverter() }
            };
        }

        public async Task<HttpResponseMessage> PostAsync<TRequest>(
            string httpClientName,
            string uri,
            TRequest request,
            IDictionary<string, string> customHeaders,
            CancellationToken cancellationToken)
        {
            return await PostAsync(httpClientName, uri, request, customHeaders, _jsonSerializerSettings,
                cancellationToken);
        }

        public async Task<HttpResponseMessage> PostAsync<TRequest>(
            string httpClientName,
            string uri,
            TRequest request,
            IDictionary<string, string> customHeaders,
            JsonSerializerSettings serializerSettings,
            CancellationToken cancellationToken)
        {
            try
            {
                var requestJson = JsonConvert.SerializeObject(request, serializerSettings);
                var stringContent = new StringContent(requestJson, Encoding.UTF8, MediaTypeNames.Application.Json);

                var httpRequestMessage = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(uri, UriKind.RelativeOrAbsolute),
                    Content = stringContent
                };

                if (customHeaders != null)
                    customHeaders.ToList().ForEach(kv =>
                    {
                        if (!string.IsNullOrEmpty(kv.Value))
                            httpRequestMessage.Headers.Add(kv.Key, kv.Value);
                    });

                var httpClient = _httpClientFactory.CreateClient(httpClientName);
                _logger.LogInformation("Executing: {0}{1}", httpClient.BaseAddress, uri);
                _logger.LogInformation("Headers: {0}", httpRequestMessage.Headers);
                _logger.LogInformation("Body: {0}", requestJson);

                var response = await httpClient.SendAsync(httpRequestMessage, cancellationToken);
                _logger.LogInformation("Headers: {0}", response.Headers);

                _logger.LogInformation("Response Status: {0}", response.StatusCode);

                if (response.Content != null)
                {
                    var content = await response.Content?.ReadAsStringAsync(cancellationToken);
                    _logger.LogInformation("Response: {0}", content);
                }

                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<HttpResponseMessage> GetAsync(string httpClientName, string uri, 
                                                                  IDictionary<string, string> customHeaders, CancellationToken cancellationToken)
        {
            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(uri, UriKind.RelativeOrAbsolute),
            };

            customHeaders?.ToList().ForEach(kv =>
            {
                var (key, value) = kv;
                if (!string.IsNullOrEmpty(value))
                    httpRequestMessage.Headers.Add(key, value);
            });


            var httpClient = _httpClientFactory.CreateClient(httpClientName);
            _logger.LogInformation("Executing: {0}{1}", httpClient.BaseAddress, uri);
            _logger.LogInformation("Headers: {0}", httpRequestMessage.Headers);

            var response = await httpClient.SendAsync(httpRequestMessage, cancellationToken);
            _logger.LogInformation("Headers: {0}", response.Headers);
            _logger.LogInformation("Response Status: {0}", response.StatusCode);

            if (response.Content != null)
            {
                var content = await response.Content?.ReadAsStringAsync(cancellationToken);
                _logger.LogInformation("Response: {0}",content);
            }

            return response;
        }
    }
}
