
using System;

namespace ServiceBusDriver.Shared.Tools
{
    public static class DateTimeExtensions
    {

        public static DateTimeOffset ToLocalTime(this DateTimeOffset dateTime, int difference)
        {
            var userTime = TimeSpan.FromMinutes(-difference);
            var localTime = dateTime.ToOffset(userTime);
            
            return localTime; // Just because I dont like to add the extra 'string.'
        }
        
    }
}
