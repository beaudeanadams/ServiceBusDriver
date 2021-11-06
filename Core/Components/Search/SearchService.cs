using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ServiceBusDriver.Core.Components.Message;
using ServiceBusDriver.Core.Models.Features.Message;
using ServiceBusDriver.Core.Models.Features.Search;
using ServiceBusDriver.Core.Tools;

namespace ServiceBusDriver.Core.Components.Search
{
    public class SearchService : ISearchService
    {
        private readonly IMessageService _messageService;
        private readonly ILogger<SearchService> _logger;

        public SearchService(IMessageService messageService, ILogger<SearchService> logger)
        {
            _messageService = messageService;
            _logger = logger;
        }


        public async Task<List<MessageResponse>> Search(SearchCommand command, CancellationToken cancellationToken)
        {
            _logger.LogTrace("Start {0}", nameof(Search));
            _logger.LogInformation("Searching for {0}", command.Value);

            var messages = await _messageService.FetchMessages(new FetchMessagesCommand
            {
                InstanceId = command.InstanceId,
                QueueName = command.QueueName,
                TopicName = command.TopicName,
                SubscriptionName = command.SubscriptionName,
                FetchAll = command.MaxMessages == 0 ? true : false,
                DeadLetterQueue = command.SearchDeadLetter,
                OrderByDescending = false,
                PrefetchCount = command.PrefetchCount,
                MaxMessages = command.MaxMessages
            }, cancellationToken);

            var response = new List<MessageResponse>();

            var watch = System.Diagnostics.Stopwatch.StartNew();

            foreach (var message in messages)
            {
                if (message.Message.ContentType == MediaTypeNames.Application.Json)
                {
                    // Search a key
                    if (!string.IsNullOrWhiteSpace(command.KeyPath))
                    {
                        var value = GetValueFromJson(message, command.KeyPath);

                        if (!string.IsNullOrWhiteSpace(value) && value == command.Value)
                        {
                            response.Add(message);
                        }
                    }
                    else
                    {
                        //Free text search
                        if (message.Payload.Contains(command.Value, StringComparison.InvariantCultureIgnoreCase))
                        {
                            response.Add(message);
                        }
                    }
                }
                else if (message.Message.ContentType == MediaTypeNames.Application.Xml)
                {
                    if (!string.IsNullOrWhiteSpace(command.KeyPath))
                    {
                        var value = GetValueFromXml(message, command.KeyPath);

                        if (!string.IsNullOrWhiteSpace(value) && value == command.Value)
                        {
                            response.Add(message);
                        }
                    }
                    else
                    {
                        if (message.Payload.Contains(command.Value, StringComparison.InvariantCultureIgnoreCase))
                        {
                            response.Add(message);
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(command.KeyPath))
                    {
                        if (message.Payload.IsJson())
                        {
                            var value = GetValueFromJson(message, command.KeyPath);

                            if (!string.IsNullOrWhiteSpace(value) && value == command.Value)
                            {
                                response.Add(message);
                            }
                        }
                        else if (message.Payload.IsXml())
                        {
                            var value = GetValueFromXml(message, command.KeyPath);

                            if (!string.IsNullOrWhiteSpace(value) && value == command.Value)
                            {
                                response.Add(message);
                            }
                        }
                        else
                        {
                            _logger.LogError("Unable to determine content type of message with Id {0}", message.Message.MessageId);
                        }
                    }
                    else
                    {
                        if (message.Payload.Contains(command.Value, StringComparison.InvariantCultureIgnoreCase))
                        {
                            response.Add(message);
                        }
                    }
                }
            }

            _logger.LogTrace("Finish {0}", nameof(Search));

            watch.Stop();
            _logger.LogInformation("Performance - Search completed on {0} Messages and fetched {1} messages in {2}", messages.Count, response.Count, watch.ElapsedMilliseconds);

            return response;
        }

        private string GetValueFromJson(MessageResponse message, string path)
        {
            try
            {
                var jObj = JObject.Parse(message.Payload);
                var token = jObj.SelectToken(path);

                return token?.ToString();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unable to process message with Id {0}", message.Message.MessageId);
                return null;
            }
        }

        private string GetValueFromXml(MessageResponse message, string path)
        {
            try
            {
                var xmlDoc = XDocument.Parse(message.Payload);
                var element = xmlDoc.XPathSelectElement(path);

                return element?.Value;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unable to process message with Id {0}. Error Raised {1}", message.Message.MessageId, e.Message);
                return null;
            }
        }
    }
}