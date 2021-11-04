using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using ServiceBusDriver.Client.Constants;

namespace ServiceBusDriver.Client.UIComponents.BaseLayouts
{
    public partial class MainLayout
    {

        [CascadingParameter]
        public Task<AuthenticationState> AuthenticationState { get; set; }

        private readonly List<string> _pagesWithAnonAccess = new()
        {
            ApiConstants.NavigationConstants.LoginPage,
            ApiConstants.NavigationConstants.RegisterPage
        };

        protected override async Task OnInitializedAsync()
        {
         
            var pathsArray = _navManager.Uri.Split('/');

            if (!_pagesWithAnonAccess.Contains("/" + pathsArray[^1]))
            {
                var authState = await AuthenticationState;
                if (authState?.User?.Identity == null || !authState.User.Identity.IsAuthenticated)
                {
                    _navManager.NavigateTo("/login", true);
                }
                else
                {
                    var user = authState.User;
                    if (user?.Identity != null && !user.Identity.IsAuthenticated)
                    {
                        _navManager.NavigateTo("/login", true);
                    }
                }
            }
        }
    }
}