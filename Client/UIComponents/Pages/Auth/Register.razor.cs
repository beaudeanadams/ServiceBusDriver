using System;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using ServiceBusDriver.Client.Constants;
using ServiceBusDriver.Shared.Tools;

namespace ServiceBusDriver.Client.UIComponents.Pages.Auth
{
    public partial class Register
    {

        [CascadingParameter]
        public Task<AuthenticationState> AuthenticationState { get; set; }

        private string _emailInput;
        private string _teamNameInput;
        private string _passwordInput;
        private string _passwordConfirmInput;
        private string _otpInput;
        private string _userId;

        private bool _userRegistered;


        protected override async Task OnInitializedAsync()
        {
            await _localStorageService.SetItemAsStringAsync(ApiConstants.LocalStorageConstants.JwtTokenKey, string.Empty);
        }

        public async Task VerifyOtp()
        {
            if (_otpInput.IsNullOrEmpty() || _otpInput.Length < 4 || _otpInput.Length > 4)
            {
                _toastService.ShowError("Invalid Otp");
            }
            else
            {
                try
                {
                    var response = await _registerHandler.VerifyOtpAndLogin(_userId, _otpInput, _emailInput, _passwordInput);
                    if (response.Token != string.Empty)
                    {
                        await _localStorageService.SetItemAsStringAsync(ApiConstants.LocalStorageConstants.JwtTokenKey, response.Token);
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
                    _toastService.ShowError(errors.ErrorMessage);
                }
                catch (Exception)
                {
                    _toastService.ShowError(ApiConstants.UiErrorConstants.GenericError);
                }
            }

        }

        public async Task RegisterUser()
        {

            if (_passwordInput != _passwordConfirmInput)
            {
                _toastService.ShowError("Passwords do not match");
            }
            else
            {
                try
                {
                    var response = await _registerHandler.RegisterUser(_emailInput, _passwordInput, _teamNameInput);

                    if (response?.Id != null)
                    {
                        _userId = response.Id;
                        _userRegistered = true;
                        StateHasChanged();
                        _toastService.ShowSuccess("Otp sent to your email. Please enter it to verify email");
                    }
                    else
                    {
                        _toastService.ShowError("Invalid Data");
                    }
                }
                catch (ValidationException e)
                {
                    var errors = e.Errors.Select(err => new
                    {
                        err.PropertyName,
                        err.ErrorMessage
                    }).FirstOrDefault();
                    _toastService.ShowError(errors.ErrorMessage);
                }
                catch (Exception)
                {
                    _toastService.ShowError(ApiConstants.UiErrorConstants.GenericError);
                }
            }
        }

        public async Task RegisterUserAndLogin()
        {

            if (_passwordInput != _passwordConfirmInput)
            {
                _toastService.ShowError("Passwords do not match");
            }
            else
            {
                try
                {
                    var response = await _registerHandler.RegisterUserAndLogin(_emailInput, _passwordInput, _teamNameInput);

                    if (response.Token != string.Empty)
                    {
                        await _localStorageService.SetItemAsStringAsync(ApiConstants.LocalStorageConstants.JwtTokenKey, response.Token);
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
                    _toastService.ShowError(errors.ErrorMessage);
                }
                catch (Exception)
                {
                    _toastService.ShowError(ApiConstants.UiErrorConstants.GenericError);
                }
            }
        }
    }
}
