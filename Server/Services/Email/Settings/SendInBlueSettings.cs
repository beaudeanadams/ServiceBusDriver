using Microsoft.Extensions.Configuration;

namespace ServiceBusDriver.Server.Services.Email.Settings
{
    public class SendInBlueSettings : ISendInBlueSettings
    {
        public SendInBlueSettings(IConfiguration configuration)
        {
            configuration.Bind("SendInBlue", this);
        }

        public string ApiKey { get; set; }
    }
}
