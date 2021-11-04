namespace ServiceBusDriver.Server.Services.Password
{
    public interface IAesEncryptService
    {
        string Encrypt(string text);

        string Decrypt(string cipherText);
    }
}
