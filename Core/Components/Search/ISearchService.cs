using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ServiceBusDriver.Core.Models.Features.Message;
using ServiceBusDriver.Core.Models.Features.Search;

namespace ServiceBusDriver.Core.Components.Search
{
    public interface ISearchService
    {
        Task<List<MessageResponse>> Search(SearchCommand command, CancellationToken cancellationToken);
    }
}