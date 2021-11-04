using Microsoft.Extensions.Configuration;

namespace ServiceBusDriver.Server.Settings
{
    public class FirebaseAuthSettings : IFirebaseAuthSettings
    {
        public FirebaseAuthSettings(IConfiguration configuration)
        {
            var firebaseAuth = configuration.GetSection("FirebaseAuth");
            AuthDomain = firebaseAuth.GetSection("AuthDomain").Value;
            ApiKey = firebaseAuth.GetSection("ApiKey").Value;
        }

        public string AuthDomain { get; set; }
        public string ApiKey { get; set; }
    }
}