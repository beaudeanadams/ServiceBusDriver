namespace ServiceBusDriver.Server.Settings
{
    public interface ISettings
    {
        string AesKey { get; }
        string Environment { get; }
    }
}