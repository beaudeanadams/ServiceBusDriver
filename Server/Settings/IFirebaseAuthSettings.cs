namespace ServiceBusDriver.Server.Settings
{
    public interface IFirebaseAuthSettings
    {
        string ApiKey { get; set; }
        string AuthDomain { get; set; }
    }
}
