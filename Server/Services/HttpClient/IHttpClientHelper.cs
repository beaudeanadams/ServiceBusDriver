using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ServiceBusDriver.Server.Services.HttpClient
{
    public interface IHttpClientHelper
    {

        // ------------------- POST -----------------------------

        Task<HttpResponseMessage> PostAsync<TRequest>(string httpClientName, string uri, TRequest request,
                                                      IDictionary<string, string> customHeaders, CancellationToken cancellationToken);

        Task<HttpResponseMessage> PostAsync<TRequest>(string httpClientName, string uri, TRequest request,
                                                      IDictionary<string, string> customHeaders, JsonSerializerSettings serializerSettings,
                                                      CancellationToken cancellationToken);

        Task<HttpResponseMessage> GetAsync(string httpClientName, string uri,
                                                     IDictionary<string, string> customHeaders, CancellationToken cancellationToken);

    }
}