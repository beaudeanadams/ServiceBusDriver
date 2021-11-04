namespace ServiceBusDriver.Server.Services.AuthContext
{
    public interface IClaimsManager
    {
        string GetEmail();
        string GetUserId();
        string GetEmailVerified();
    }
}