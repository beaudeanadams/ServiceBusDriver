namespace ServiceBusDriver.Db.Cosmos
{
    public interface ICosmosSettings
    {
        string Account { get; set; }
        string Key { get; set; }
        string Database { get; set; }
    }
}
