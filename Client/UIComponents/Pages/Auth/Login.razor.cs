using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using ServiceBusDriver.Client.Constants;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceBusDriver.Client.UIComponents.Pages.Auth
{
    public partial class Login
    {
        private bool _loginSpinner;

        [CascadingParameter]
        public Task<AuthenticationState> AuthenticationState { get; set; }

        protected override async Task OnInitializedAsync()
        {
            // Clear JWT Token if login page is opened
            await _localStorageService.SetItemAsStringAsync(ApiConstants.LocalStorageConstants.JwtTokenKey, string.Empty);
        }

        private async Task LoginWithTest()
        {
            _loginHandler.LoginRequestDto.Email = "test1@test.com";
            _loginHandler.LoginRequestDto.Password = "Test@1234";
            await LoginUser();
        }

        private void RegisterUser()
        {
            _navigationManager.NavigateTo("/register", true);
        }

        private async Task LoginUser()
        {
            try
            {
                _loginSpinner = true;
                var authenticationResponse = await _loginHandler.LoginUser();
                _loginSpinner = false;
                if (authenticationResponse == null)
                {
                    await JsRuntime.InvokeVoidAsync("alert", " Validation Error ! ");
                }

                if (authenticationResponse != null && authenticationResponse.Token != string.Empty)
                {
                    await _localStorageService.SetItemAsStringAsync(ApiConstants.LocalStorageConstants.JwtTokenKey, authenticationResponse.Token);

                    _navigationManager.NavigateTo(ApiConstants.NavigationConstants.Dashboard, true);
                }
                else
                {
                    _toastService.ShowError("Invalid username or password");
                }
            }
            catch (ValidationException e)
            {
                var errors = e.Errors.Select(err => new
                {
                    err.PropertyName,
                    err.ErrorMessage
                }).FirstOrDefault();
                _toastService.ShowError(errors?.ErrorMessage);
            }
            catch (Exception)
            {
                _toastService.ShowError(ApiConstants.UiErrorConstants.GenericError);
            }
            finally
            {
                _loginSpinner = false;
            }
        }

    }
}
