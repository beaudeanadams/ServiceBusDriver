using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace ServiceBusDriver.Client.UIComponents.Components
{
    public partial class RightSideBar
    {

        //Private Members
        private int _localTimeDifference;

        [CascadingParameter]
        public Task<AuthenticationState> AuthenticationState { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _localTimeDifference = await _jsRuntime.InvokeAsync<int>("GetTimezoneValue");

            _propertiesNotifierService.Notify += OnNotify;

        }

        private async Task OnNotify()
        {
            await InvokeAsync(StateHasChanged);
        }

        public void Dispose()
        {
            _propertiesNotifierService.Notify -= OnNotify;
        }


    }
}