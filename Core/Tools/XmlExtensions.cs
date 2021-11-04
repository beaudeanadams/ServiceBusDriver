using System;
using System.IO;
using System.Xml;

namespace ServiceBusDriver.Core.Tools
{
    public static class XmlExtensions
    {
        /// <summary>
        /// Checks whether the input text is a valid xml string.
        /// </summary>
        /// <param name="text">The text to check.</param>
        /// <returns>Yes if the text is a valid xml string, false otherwise.</returns>
        public static bool IsXml(this string text)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(text))
                {
                    return false;
                }

                using (var stringReader = new StringReader(text))
                {
                    using var xmlReader = XmlReader.Create(stringReader);
                    while (xmlReader.Read())
                    {
                    }
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}