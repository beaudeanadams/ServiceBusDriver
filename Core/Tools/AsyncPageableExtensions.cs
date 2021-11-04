using System.Collections.Generic;
using System.Threading.Tasks;
using Azure;

namespace ServiceBusDriver.Core.Tools
{
    public static class AsyncPageableExtensions
    {
        public static async Task<List<T>> GetItems<T>(this AsyncPageable<T> items)
        {
            var collection = new List<T>();

            await foreach (var item in items)
            {
                collection.Add(item);
            }

            return collection;
        }
    }
}