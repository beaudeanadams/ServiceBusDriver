using Firebase.Auth;

namespace ServiceBusDriver.Server.Services.FirebaseAuth
{
    public interface IFirebaseAuthManager
    {
        IFirebaseAuthClient GetClient();
    }
}