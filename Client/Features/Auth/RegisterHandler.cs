using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Newtonsoft.Json;
using ServiceBusDriver.Client.Constants;
using ServiceBusDriver.Core.Models.Errors;
using ServiceBusDriver.Shared.Features.Error;
using ServiceBusDriver.Shared.Features.User;

namespace ServiceBusDriver.Client.Features.Auth
{
    public interface IRegisterHandler
    {
        Task<UserResponseDto> RegisterUser(string email, string password, string teamName);
        Task<LoginResponseDto> RegisterUserAndLogin(string email, string password, string teamName);
        Task<LoginResponseDto> VerifyOtpAndLogin(string id, string otp, string email, string password);
    }

    public class RegisterHandler : IRegisterHandler
    {
        private readonly HttpClient _httpClient;
        private readonly IValidator<AddRequestDto> _validator;
        private readonly ILoginHandler _loginHandler;

        public RegisterHandler(HttpClient httpClient, IValidator<AddRequestDto> validator, ILoginHandler loginHandler)
        {
            _httpClient = httpClient;
            _validator = validator;
            _loginHandler = loginHandler;
        }

        public async Task<LoginResponseDto> RegisterUserAndLogin(string email, string password, string teamName)
        {
            var userResponseDto = await RegisterUser(email, password, teamName);
            _loginHandler.LoginRequestDto = new LoginRequestDto
            {
                Email = email,
                Password = password
            };
            return await _loginHandler.LoginUser();
        }

        public async Task<LoginResponseDto> VerifyOtpAndLogin(string id, string otp, string email, string password)
        {
            var verifyUser = new VerifyOtpRequest
            {
                Id = id,
                Otp = otp
            };

            var response = await _httpClient.PostAsJsonAsync(ApiConstants.PathConstants.VerifyUser, verifyUser);
            var result = await response.Content.ReadAsStringAsync();

            _loginHandler.LoginRequestDto = new LoginRequestDto
            {
                Email = email,
                Password = password
            };
            return await _loginHandler.LoginUser();
        }

        public async Task<UserResponseDto> RegisterUser(string email, string password, string teamName)
        {
            var createUser = new AddRequestDto
            {
                TeamName = teamName,
                Email = email,
                Password = password
            };

            await _validator.ValidateAndThrowAsync(createUser);

            var response = await _httpClient.PostAsJsonAsync(ApiConstants.PathConstants.RegisterUser, createUser);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var userResponseDto = JsonConvert.DeserializeObject<UserResponseDto>(result);
                return userResponseDto;
            }
            else
            {
                var result = await response.Content.ReadAsStringAsync();
                var errorResponse = JsonConvert.DeserializeObject<AppErrorMessageDto>(result);
                var valFailure = new List<ValidationFailure>
                {
                    new("Email", errorResponse.UserMessageText)
                };
                throw new ValidationException(errorResponse.UserMessageText, valFailure);
            }
        }
    }
}
