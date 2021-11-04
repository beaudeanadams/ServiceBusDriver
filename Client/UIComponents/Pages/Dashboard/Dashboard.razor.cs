using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace ServiceBusDriver.Client.UIComponents.Pages.Dashboard
{
    public partial class Dashboard
    {
        //UI Components

        //private TopicDashboard topicDashboard;
        private async Task OnNotify()
        {
            await InvokeAsync(() => {
                StateHasChanged();
                _jsRuntime.InvokeVoidAsync("ScrollToBottom", "messagelogs");
            });
        }

        protected override void OnInitialized()
        {
            _traceLogsNotifier.AddEventListener(OnNotify);
        }
    }
}