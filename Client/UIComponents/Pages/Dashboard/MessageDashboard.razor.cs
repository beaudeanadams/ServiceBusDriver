using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using Newtonsoft.Json.Linq;
using ServiceBusDriver.Shared.Features.Message;
using ServiceBusDriver.Shared.Tools;

namespace ServiceBusDriver.Client.UIComponents.Pages.Dashboard
{
    public partial class MessageDashboard
    {
        //UI Components
        private string _payloadTxtArea;

        //Private Members
        private int _localTimeDifference;
        private MessageResponseDto _selectedMessage;
        
        [CascadingParameter]
        public Task<AuthenticationState> AuthenticationState { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _localTimeDifference = await _jsRuntime.InvokeAsync<int>("GetTimezoneValue");
            _messageNotifierService.Notify += OnNotify;
        }

        private async Task OnNotify()
        {
            await InvokeAsync(StateHasChanged);
        }

        public void Dispose()
        {
            _messageNotifierService.Notify -= OnNotify;
        }

        private async Task SelectRowMessage(MessageResponseDto message)
        {
            _selectedMessage = message;

            var properties = SetMessageProperties();

            await _propertiesNotifierService.SetMessageProperties(properties);

            if (message.ContentType == MediaTypeNames.Application.Json)
            {
                _payloadTxtArea = JToken.Parse(message.Payload).ToString();
            }
            else if (message.ContentType == MediaTypeNames.Application.Xml)
            {
                _payloadTxtArea = message.Payload.FormalXml();
            }
            else if (message.ContentType.IsNullOrEmpty())
            {
                if (message.Payload.IsJson())
                {
                    _payloadTxtArea = JToken.Parse(message.Payload).ToString();
                }
                else if (message.Payload.IsXml())
                {
                    _payloadTxtArea = message.Payload.FormalXml();
                }
                else
                {
                    _payloadTxtArea = message.Payload;
                }
            }
            else
            {
                _payloadTxtArea = message.Payload;
            }
        }

        private Dictionary<string, string> SetMessageProperties()
        {
            var properties = new Dictionary<string, string>();
            properties.Add(nameof(_selectedMessage.MessageId).ToSpaceSeperated(), _selectedMessage.MessageId);
            properties.Add(nameof(_selectedMessage.Subject).ToSpaceSeperated(), _selectedMessage.Subject);
            properties.Add(nameof(_selectedMessage.SequenceNumber).ToSpaceSeperated(), _selectedMessage.SequenceNumber.ToString());
            properties.Add(nameof(_selectedMessage.DeliveryCount).ToSpaceSeperated(), _selectedMessage.DeliveryCount.ToString());
            properties.Add(nameof(_selectedMessage.ContentType).ToSpaceSeperated(), _selectedMessage.ContentType);
            properties.Add(nameof(_selectedMessage.EnqueuedTime).ToSpaceSeperated(), _selectedMessage.EnqueuedTime.ToString());
            properties.Add(nameof(_selectedMessage.TimeToLive).ToSpaceSeperated(), _selectedMessage.TimeToLive);
            properties.Add(nameof(_selectedMessage.CorrelationId).ToSpaceSeperated(), _selectedMessage.CorrelationId);
            properties.Add(nameof(_selectedMessage.EnqueuedSequenceNumber).ToSpaceSeperated(), _selectedMessage.EnqueuedSequenceNumber.ToString());
            properties.Add(nameof(_selectedMessage.MessageFromDeadLetterQueue).ToSpaceSeperated(), _selectedMessage.MessageFromDeadLetterQueue.ToString());
            properties.Add(nameof(_selectedMessage.MessageSecondaryProperties.To).ToSpaceSeperated(), _selectedMessage.MessageSecondaryProperties.To);
            properties.Add(nameof(_selectedMessage.MessageSecondaryProperties.ReplyTo).ToSpaceSeperated(), _selectedMessage.MessageSecondaryProperties.ReplyTo);
            properties.Add(nameof(_selectedMessage.MessageSecondaryProperties.PartitionKey).ToSpaceSeperated(), _selectedMessage.MessageSecondaryProperties.PartitionKey);
            properties.Add(nameof(_selectedMessage.MessageSecondaryProperties.TransactionPartitionKey).ToSpaceSeperated(), _selectedMessage.MessageSecondaryProperties.TransactionPartitionKey);
            properties.Add(nameof(_selectedMessage.MessageSecondaryProperties.SessionId).ToSpaceSeperated(), _selectedMessage.MessageSecondaryProperties.SessionId);
            properties.Add(nameof(_selectedMessage.MessageSecondaryProperties.ReplyToSessionId).ToSpaceSeperated(), _selectedMessage.MessageSecondaryProperties.ReplyToSessionId);
            properties.Add(nameof(_selectedMessage.MessageSecondaryProperties.LockToken).ToSpaceSeperated(), _selectedMessage.MessageSecondaryProperties.LockToken);
            properties.Add(nameof(_selectedMessage.MessageTimeProperties.ScheduledEnqueueTime).ToSpaceSeperated(), _selectedMessage.MessageTimeProperties.ScheduledEnqueueTime.ToString());
            properties.Add(nameof(_selectedMessage.MessageTimeProperties.LockedUntil).ToSpaceSeperated(), _selectedMessage.MessageTimeProperties.LockedUntil.ToString());
            properties.Add(nameof(_selectedMessage.MessageTimeProperties.ExpiresAt).ToSpaceSeperated(), _selectedMessage.MessageTimeProperties.ExpiresAt.ToString());
            properties.Add(nameof(_selectedMessage.DeadLetterProperties.DeadLetterSource).ToSpaceSeperated(), _selectedMessage.DeadLetterProperties.DeadLetterSource);
            properties.Add(nameof(_selectedMessage.DeadLetterProperties.DeadLetterReason).ToSpaceSeperated(), _selectedMessage.DeadLetterProperties.DeadLetterReason);
            properties.Add(nameof(_selectedMessage.DeadLetterProperties.DeadLetterErrorDescription).ToSpaceSeperated(), _selectedMessage.DeadLetterProperties.DeadLetterErrorDescription);
            foreach (var (key, value) in _selectedMessage.ApplicationProperties)
            {
                properties.Add(key, value);
            }

            return properties;
        }
    }
}