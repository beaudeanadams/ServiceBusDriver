using System;
using Newtonsoft.Json.Linq;

namespace ServiceBusDriver.Core.Tools
{
    public static class JsonExtensions
    {
        /// <summary>
        /// Checks if the string is in JSON format.
        /// </summary>
        /// <param name="item">The string that must be deserialized.</param>
        /// <returns>Returns true if the object is in JSON format, false otherwise.</returns>
        public static bool IsJson(this string item)
        {
            if (string.IsNullOrEmpty(item))
            {
                return false;
            }

            try
            {
                JToken.Parse(item);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}