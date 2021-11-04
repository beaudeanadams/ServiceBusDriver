using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using ServiceBusDriver.Shared.Features.User;

namespace ServiceBusDriver.Client.Features.Auth
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {

        private readonly HttpClient _httpClient;
        private readonly NavigationManager _navigationManager;
        private readonly ILocalStorageService _localStorageService;

        public CustomAuthenticationStateProvider(HttpClient httpClient, ILocalStorageService localStorageService, NavigationManager navigationManager)
        {
            _httpClient = httpClient;
            _localStorageService = localStorageService;
            _navigationManager = navigationManager;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var currentUser = await GetUserByJwtAsync(); 

            if (currentUser != null && currentUser.Email != null)
            {
                //create a claims
                var claimEmailAddress = new Claim(ClaimTypes.Name, currentUser.Email);
                var claimNameIdentifier = new Claim(ClaimTypes.NameIdentifier, currentUser.Id);
                var claimTeamIdentifier = new Claim(ClaimTypes.GroupSid, currentUser.TeamName);
                //create claimsIdentity
                var claimsIdentity = new ClaimsIdentity(new[] { claimEmailAddress, claimNameIdentifier, claimTeamIdentifier }, "serverAuth");
                //create claimsPrincipal
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                return new AuthenticationState(claimsPrincipal);
            }
            else
            {
                return new AuthenticationState(new ClaimsPrincipal());
            }
        }

        public async Task<UserResponseDto> GetUserByJwtAsync()
        {
            //pulling the token from localStorage
            var jwtToken = await _localStorageService.GetItemAsStringAsync("jwt_token");
            if (string.IsNullOrEmpty(jwtToken)) return null;

            //preparing the http request
            try
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Get, "api/user/me");
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
                //requestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                //making the http request
                var response = await _httpClient.SendAsync(requestMessage);

                if (response.IsSuccessStatusCode)
                {
                    var returnedUser = await response.Content.ReadFromJsonAsync<UserResponseDto>();

                    //returning the user if found
                    if (returnedUser != null)
                    {
                        return await Task.FromResult(returnedUser);
                    }
                    else return null;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }            
        }
    }
}
