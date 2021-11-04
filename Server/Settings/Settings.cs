using Microsoft.Extensions.Configuration;

namespace ServiceBusDriver.Server.Settings
{
    public class Settings : ISettings
    {
        private readonly IConfiguration _configuration;

        public Settings(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string AesKey => GetValue("AesKey");

        public string GetValue(string key)
        {
            return _configuration.GetSection(key).Value;
        }
    }
}