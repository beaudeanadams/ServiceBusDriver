using Firebase.Auth;
using Firebase.Auth.Providers;
using ServiceBusDriver.Server.Settings;
using System.IO;
using System.Linq;

namespace ServiceBusDriver.Server.Services.FirebaseAuth
{
    public class FirebaseAuthManager : IFirebaseAuthManager
    {
        private static FirebaseAuthClient _firebaseAuthClient;
        private readonly IFirebaseAuthSettings _firebaseAuthSettings;

        public FirebaseAuthManager(IFirebaseAuthSettings firebaseAuthSettings)
        {
            _firebaseAuthSettings = firebaseAuthSettings;
        }

        public IFirebaseAuthClient GetClient()
        {
            if (_firebaseAuthClient != null) return _firebaseAuthClient;

            var config = new FirebaseAuthConfig
            {
                ApiKey = _firebaseAuthSettings.ApiKey,
                AuthDomain = _firebaseAuthSettings.AuthDomain,
                Providers = new FirebaseAuthProvider[]
                {
                    new EmailProvider()
                },
            };
            _firebaseAuthClient = new FirebaseAuthClient(config);

            return _firebaseAuthClient;
        }
    }
}