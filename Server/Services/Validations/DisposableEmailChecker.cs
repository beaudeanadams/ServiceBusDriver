using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Newtonsoft.Json;
using ServiceBusDriver.Server.Services.Email.Models;
using ServiceBusDriver.Server.Services.HttpClient;

namespace ServiceBusDriver.Server.Services.Validations
{
    public interface IDisposableEmailChecker
    {
        Task<bool> CheckIfDisposableEmailDomain(string toEmail,
                                                CancellationToken cancellationToken);
    }

    public class DisposableEmailChecker : IDisposableEmailChecker
    {
        private readonly IHttpClientHelper _httpClientHelper;

        public DisposableEmailChecker(IHttpClientHelper httpClientHelper)
        {
            _httpClientHelper = httpClientHelper;
        }

        public async Task<bool> CheckIfDisposableEmailDomain(string toEmail,
                                                             CancellationToken cancellationToken)
        {
            try
            {
                var email = Convert.ToString(toEmail);

                //some basic validation to avoid majority of exceptions
                if (!string.IsNullOrEmpty(email) && email.Contains("@"))
                {
                    var mailAddress = new MailAddress(email);
                    var uri = new Uri(HttpClientConstants.DisposableEmail.BaseUrl, UriKind.Absolute);
                    uri = new Uri(uri, $"v1/disposable/{mailAddress.Host}");

                    
                    // Reference - https://github.com/ivolo/disposable-email-domains
                    var response = await _httpClientHelper.GetAsync(
                        HttpClientConstants.Clients.DisposableEmailClient,
                        uri.ToString(),
                        null,
                        cancellationToken);

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response?.Content?.ReadAsStringAsync(cancellationToken);
                        var resp = JsonConvert.DeserializeObject<DisposableEmailResponse>(content);

                        if (resp is { Disposable: false }) return true;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    throw new ValidationException("Email format is wrong");
                }
            }
            catch (FormatException)
            {
                throw new ValidationException("Disposable Email Domain Found. Please use valid email");
            }

            throw new ValidationException("Disposable Email Domain Found. Please use valid email");
        }
    }
}