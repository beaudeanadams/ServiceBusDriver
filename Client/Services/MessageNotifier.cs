using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ServiceBusDriver.Shared.Features.Message;

namespace ServiceBusDriver.Client.Services
{
    public class MessageNotifierService
    {
        private List<MessageResponseDto> values = new();
        public IReadOnlyList<MessageResponseDto> ValuesList => values;

        public string CurrentInstance;
        public string CurrentTopic;
        public string CurrentSubscription;
        public string CurrentQueue;
      
        public MessageNotifierService()
        {

        }

        public void SetCurrentProperties(string currentInstance, string currentTopic, string currentSubscription, string currentQueue)
        {
            CurrentInstance = currentInstance;
            CurrentTopic = currentTopic;
            CurrentSubscription = currentSubscription;
            CurrentQueue = currentQueue;
        }

        public async Task SetCurrentPropertiesAndMessages(string currentInstance, string currentTopic, string currentSubscription, string currentQueue, List<MessageResponseDto> message)
        {
            SetCurrentProperties(currentInstance, currentTopic, currentSubscription, currentQueue);
             await AddTolist(message);
        }

        public async Task AddTolist(List<MessageResponseDto> message)
        {
            values = message;
            
            if (Notify != null)
            {
                await Notify?.Invoke();
            }

        }

        public event Func<Task> Notify;
    }
}
