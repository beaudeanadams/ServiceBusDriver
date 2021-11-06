using System.IO;
using System.Linq;

namespace ServiceBusDriver.Core.Tests.Tools
{
    internal class FileHelpers
    {
        public static string ReadResource(string name)
        {
            var assembly = typeof(FileHelpers).Assembly;

            var resourcePath = assembly.GetManifestResourceNames()
                                       .Single(str => str.EndsWith(name));

            var stream = assembly.GetManifestResourceStream(resourcePath);
            var reader = new StreamReader(stream);

            return reader.ReadToEnd();
        }
    }
}
