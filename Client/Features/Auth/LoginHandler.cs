using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentValidation;
using Newtonsoft.Json;
using ServiceBusDriver.Client.Constants;
using ServiceBusDriver.Shared.Features.User;

namespace ServiceBusDriver.Client.Features.Auth
{
    public interface ILoginHandler
    {
        public LoginRequestDto LoginRequestDto { get; set; }
        Task<LoginResponseDto> LoginUser();
    }

    public class LoginHandler : ILoginHandler
    {
        private readonly HttpClient _httpClient;
        private readonly IValidator<LoginRequestDto> _validator;
        public LoginHandler(HttpClient httpClient, IValidator<LoginRequestDto> validator)
        {
            _httpClient = httpClient;
            _validator = validator;
        }

        public LoginRequestDto LoginRequestDto { get; set; } = new LoginRequestDto();

        public async Task<LoginResponseDto> LoginUser()
        {
            var validationResult = await _validator.ValidateAsync(LoginRequestDto);
            if (!validationResult.IsValid)
            {
                //TODO Show Error Toast from validationResult.Errors.Message
                Console.WriteLine("Validation Error");
                return null;
            }
            
            var response = await _httpClient.PostAsJsonAsync(ApiConstants.PathConstants.LoginUser, LoginRequestDto);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var loginResponseDto = JsonConvert.DeserializeObject<LoginResponseDto>(result);

                return loginResponseDto;
            }
            else
            { 
                throw new Exception("Unknown Error Occured");
            }

        }
    }
}
