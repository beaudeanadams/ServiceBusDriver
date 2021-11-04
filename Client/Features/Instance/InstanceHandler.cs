using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ServiceBusDriver.Client.Constants;
using ServiceBusDriver.Core.Models.Features.Connection;
using ServiceBusDriver.Shared.Features.Instance;
using ServiceBusDriver.Shared.Models;

namespace ServiceBusDriver.Client.Features.Instance
{
    public interface IInstanceHandler
    {
        Task<List<InstanceResponseDto>> GetInstances();
        Task<InstanceResponseDto> AddInstance(string connectionString);
        Task<ConnectionSettingsModel> TestConnectivity(string connectionString);
        Task<ConnectionSettingsModel> ProcessConnectionString(string connectionString);
    }

    public class InstanceHandler : IInstanceHandler
    {
        private readonly HttpClient _httpClient;

        public InstanceHandler(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<InstanceResponseDto> AddInstance(string connectionString)
        {
            var response = await _httpClient.PostAsJsonAsync(ApiConstants.PathConstants.AddInstance, new AddRequestDto()
            {
                ConnectionString = connectionString
            });

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<InstanceResponseDto>();

            return result;
        }

        public async Task<ConnectionSettingsModel> TestConnectivity(string connectionString)
        {
            var response = await _httpClient.PostAsJsonAsync(ApiConstants.PathConstants.TestConnection, new TestConnectionRequest()
            {
                ConnectionString = connectionString
            });

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<ConnectionSettingsModel>();

            return result;
        }

        public async Task<ConnectionSettingsModel> ProcessConnectionString(string connectionString)
        {
            var response = await _httpClient.PostAsJsonAsync(ApiConstants.PathConstants.ProcessConnectionString, new ProcessConnectionStringRequest()
            {
                ConnectionString = connectionString
            });
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<ConnectionSettingsModel>();

            return result;
        }

        public async Task<List<InstanceResponseDto>> GetInstances()
        {
            var response = await _httpClient.GetFromJsonAsync<List<InstanceResponseDto>>(ApiConstants.PathConstants.ListInstances);

            return response;
        }
    }
}