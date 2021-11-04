using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ServiceBusDriver.Server.Services.Email.Models;
using ServiceBusDriver.Server.Services.Email.Settings;
using ServiceBusDriver.Server.Services.HttpClient;
using static System.IO.Path;
using static System.Reflection.Assembly;

namespace ServiceBusDriver.Server.Services.Email
{
    public interface IEmailSenderService
    {
        Task<string> SendCodeEmail(string toName, string toEmail, string code, CancellationToken cancellationToken);

    }

    public class EmailSenderService : IEmailSenderService
    {
        private readonly IHttpClientHelper _httpClientHelper;
        private readonly ISendInBlueSettings _sendInBlueSettings;

        public EmailSenderService(IHttpClientHelper httpClientHelper, ISendInBlueSettings sendInBlueSettings)
        {
            _httpClientHelper = httpClientHelper;
            _sendInBlueSettings = sendInBlueSettings;
        }

        public static string VerifyEmailCodeContent()
        {
            var content = ReadFromResourceFile("verifyOtpEmail.html");
            return content;
        }

        public async Task<string> SendCodeEmail(string toName, string toEmail, string code,
                                                CancellationToken cancellationToken)
        {
            var htmlContent = VerifyEmailCodeContent()
                .Replace("{{otp}}", code)
                .Replace("{{display_name}}", toName);

            var email = new EmailRequestModel
            {
                Subject = " Email Verification OTP from Service Bus Driver",
                HtmlContent = htmlContent,
                Sender = new Sender
                {
                    Name = "Service Bus Driver Team",
                    Email = "no-reply@sbdriver.tech"
                },
                To = new List<Destination>
                {
                    new Destination
                    {
                        Name = toName,
                        Email = toEmail
                    }
                }
            };

            return await SendRequest(email, cancellationToken);
        }

        private async Task<string> SendRequest(EmailRequestModel request, CancellationToken cancellationToken)
        {
            var url = $"{HttpClientConstants.SendInBlue.BaseUrl}{HttpClientConstants.Paths.SendTransactionEmailPath}";

            var customerHeaders = new Dictionary<string, string>
            {
                {"api-key", _sendInBlueSettings.ApiKey}
            };

            var response = await _httpClientHelper.PostAsync(
                HttpClientConstants.Clients.SendInBlueClient,
                url,
                request,
                customerHeaders,
                cancellationToken);

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var jObject = JObject.Parse(content);

            return jObject["messageId"].ToString();
        }

        public static string ReadFromResourceFile(string endingFileName)
        {
            var assembly = GetExecutingAssembly();
            var manifestResourceNames = assembly.GetManifestResourceNames();

            foreach (var resourceName in manifestResourceNames)
            {
                var fileNameFromResourceName = GetFileNameFromResourceName(resourceName);
                if (!fileNameFromResourceName.EndsWith(endingFileName)) continue;

                using (var manifestResourceStream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (manifestResourceStream == null) continue;

                    using (var streamReader = new StreamReader(manifestResourceStream))
                    {
                        return streamReader.ReadToEnd();
                    }
                }
            }

            return null;
        }

        // https://stackoverflow.com/a/32176198/3764804
        private static string GetFileNameFromResourceName(string resourceName)
        {
            var stringBuilder = new StringBuilder();
            var escapeDot = false;
            var haveExtension = false;

            for (var resourceNameIndex = resourceName.Length - 1;
                resourceNameIndex >= 0;
                resourceNameIndex--)
            {
                if (resourceName[resourceNameIndex] == '_')
                {
                    escapeDot = true;
                    continue;
                }

                if (resourceName[resourceNameIndex] == '.')
                {
                    if (!escapeDot)
                    {
                        if (haveExtension)
                        {
                            stringBuilder.Append('\\');
                            continue;
                        }

                        haveExtension = true;
                    }
                }
                else
                {
                    escapeDot = false;
                }

                stringBuilder.Append(resourceName[resourceNameIndex]);
            }

            var fileName = GetDirectoryName(stringBuilder.ToString());
            return fileName == null ? null : new string(fileName.Reverse().ToArray());
        }
    }
}