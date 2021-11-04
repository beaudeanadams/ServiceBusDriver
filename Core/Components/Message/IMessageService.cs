using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ServiceBusDriver.Core.Models.Features.Message;

namespace ServiceBusDriver.Core.Components.Message
{
    public interface IMessageService
    {
        Task<List<MessageResponse>> FetchMessages(FetchMessagesCommand command, CancellationToken cancellationToken = default);
    }
}