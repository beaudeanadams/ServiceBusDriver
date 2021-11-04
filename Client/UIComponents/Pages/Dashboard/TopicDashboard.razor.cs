using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Threading.Tasks;

namespace ServiceBusDriver.Client.UIComponents.Pages.Dashboard
{


    public partial class TopicDashboard
    {
        public IReadOnlyDictionary<string, string> topicProperties;

        [CascadingParameter]
        public Task<AuthenticationState> AuthenticationState { get; set; }

        protected override void OnInitialized()
        {
            _propertiesNotifierService.Notify += OnNotify;

        }

        private async Task OnNotify()
        {
            topicProperties = _propertiesNotifierService.TopicProperties;
            await InvokeAsync(StateHasChanged);
        }
    }
}
