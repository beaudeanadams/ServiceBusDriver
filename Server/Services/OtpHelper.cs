using System;

namespace ServiceBusDriver.Server.Services
{
    public class OtpHelper
    {
        public static string GenerateOtp()
        {
            var generator = new Random();
            return generator.Next(1234, 9999).ToString();
        }
    }
}
