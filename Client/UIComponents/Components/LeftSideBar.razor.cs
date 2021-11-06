using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AsyncAwaitBestPractices;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using ServiceBusDriver.Client.Constants;
using ServiceBusDriver.Client.Services;
using ServiceBusDriver.Shared.Features.Instance;
using ServiceBusDriver.Shared.Features.Message;
using ServiceBusDriver.Shared.Features.Queue;
using ServiceBusDriver.Shared.Features.Subscription;
using ServiceBusDriver.Shared.Features.Topic;
using ServiceBusDriver.Shared.Features.Trace;
using ServiceBusDriver.Shared.Tools;

namespace ServiceBusDriver.Client.UIComponents.Components
{
    public partial class LeftSideBar
    {
        private List<InstanceResponseDto> _instances;
        private List<TopicResponseDto> _topics;
        private List<QueueResponseDto> _queues;
        private List<SubscriptionResponseDto> _subscriptions;
        private SubscriptionResponseDto _subscriptionDetails;
        private List<MessageResponseDto> _messages;
        private bool _noInstances = false;

        //Select Boxes
        private string _instanceIdSelectBox;
        private string _topicNameSelectBox;
        private string _queueNameSelectBox;
        private string _subscriptionNameSelectBox;

        //Spinners
        private bool _propertiesSpinner;
        private bool _peekActiveMessageSpinner;
        private bool _peekDlqMessageSpinner;
        private bool _peekLastNMessageSpinner;
        private bool _searchMessageSpinner;

        // Subscription Counts
        private string _activeMsgsText;
        private string _deadLetterTxt;

        //Search Properties
        private string _searchPathInput;
        private string _searchKeyInput;
        private bool _searchActiveQueueRadio = true;
        private bool _searchDlQueueRadio = false;
        private bool _featureIsQueue = false;

        // Peek Counts
        private int _activeMsgCountInput;
        private int _deadLetterMsgCountInput;
        private int _lastNMsgCountInput;

        private bool _ruleTableVisible = false;

        [CascadingParameter]
        public Task<AuthenticationState> AuthenticationState { get; set; }

        protected override async Task OnInitializedAsync()
        {
            //Initializing Variables
            _activeMsgsText = "0";
            _deadLetterTxt = "0";

            var authState = await AuthenticationState;
            if (authState?.User?.Identity != null && authState.User.Identity.IsAuthenticated)
            {
                await GetAllInstance();
            }
        }

        private async Task SetDefaultUiValuesForSubscription()
        {
            _subscriptions = null;
            _subscriptionNameSelectBox = null;
            _activeMsgsText = "0";
            _deadLetterTxt = "0";
            await _propertiesNotifierService.SetSubscriptionProperties(new Dictionary<string, string>());
        }

        private async Task SetDefaultUiValuesForTopic()
        {
            _topics = null;
            _topicNameSelectBox = null;
            await _propertiesNotifierService.SetTopicProperties(new Dictionary<string, string>());
        }

        private async Task SetDefaultUiValuesForQueue()
        {
            _queues = null;
            _queueNameSelectBox = null;
            await _propertiesNotifierService.SetTopicProperties(new Dictionary<string, string>());
        }

        private async Task GetAllInstance()
        {
            _traceLogsNotifier.AddToQueue(new TraceModel("Initiate instance fetch")).SafeFireAndForget();
            _instances = await _instanceHandler.GetInstances();
            _noInstances = _instances == null;
        }

        private async void OnInstanceValueChanged(ChangeEventArgs e)
        {
            try
            {
                await SetDefaultUiValuesForTopic();
                await SetDefaultUiValuesForQueue();
                StateHasChanged();
                if (e.Value != null && !e.Value.ToString().IsNullOrEmpty())
                {
                    _propertiesSpinner = true;
                    _instanceIdSelectBox = e.Value.ToString();

                    var topicFetchTask = _topicHandler.GetTopicsInInstance(_instanceIdSelectBox);
                    var queueFetchTask = _queueHandler.GetQueuesInInstance(_instanceIdSelectBox);

                    await Task.WhenAll(topicFetchTask, queueFetchTask);

                    _topics = topicFetchTask.Result;
                    _queues = queueFetchTask.Result;

                    await SetDefaultUiValuesForSubscription();
                    _propertiesSpinner = false;
                    StateHasChanged();
                }
            }
            catch (SbDriverUiException sbe)
            {
                _toastService.ShowError(sbe.Message);
            }
        }

        // Toggle between Topics or Queues
        private void UpdateFeatureType(bool isQueue)
        {
            _featureIsQueue = isQueue;
        }

        private async void OnTopicsValueChanged(ChangeEventArgs e)
        {
            try
            {
                _queueNameSelectBox = null;
                await _propertiesNotifierService.SetQueueProperties(new Dictionary<string, string>());
                await SetDefaultUiValuesForSubscription();
                StateHasChanged();

                if (e.Value != null && e.Value.ToString().IsNotNullOrWhitespace())
                {
                    _propertiesSpinner = true;
                    _topicNameSelectBox = e.Value.ToString();

                    var selectedTopic = _topics.FirstOrDefault(x => x.Name == _topicNameSelectBox);

                    if (_instanceIdSelectBox.IsNotNullOrWhitespace())
                    {
                        _subscriptions = await _subscriptionHandler.GetSubscriptionsInTopic(_instanceIdSelectBox, _topicNameSelectBox);
                        _propertiesSpinner = false;
                        StateHasChanged();

                        var properties = SetTopicProperties(selectedTopic);

                        await _propertiesNotifierService.SetTopicProperties(properties);
                        _traceLogsNotifier.AddToQueue($"Topics Received for Instance: {_instanceIdSelectBox} , Topic: {_topicNameSelectBox} ").SafeFireAndForget();
                    }
                }
            }
            catch (SbDriverUiException sbe)
            {
                _toastService.ShowError(sbe.Message);
            }
        }

        private async void OnQueueValueChanged(ChangeEventArgs e)
        {
            try
            {
                _topicNameSelectBox = null;
                await _propertiesNotifierService.SetTopicProperties(new Dictionary<string, string>());
                await SetDefaultUiValuesForSubscription();
                StateHasChanged();

                if (e.Value != null && e.Value.ToString().IsNotNullOrWhitespace())
                {
                    _propertiesSpinner = true;
                    _queueNameSelectBox = e.Value.ToString();

                    var selectedQueue = _queues.FirstOrDefault(x => x.Name == (_queueNameSelectBox));
                    _activeMsgsText = selectedQueue.QueueStats.ActiveMessageCount.ToString();
                    _deadLetterTxt = selectedQueue.QueueStats.DeadLetterMessageCount.ToString();

                    if (_instanceIdSelectBox.IsNotNullOrWhitespace())
                    {
                        var properties = SetQueueProperties(selectedQueue);
                        await _propertiesNotifierService.SetQueueProperties(properties);
                        _propertiesSpinner = false;
                        _traceLogsNotifier.AddToQueue($"Queues Received for Instance: {_instanceIdSelectBox} , Queue: {_queueNameSelectBox} ").SafeFireAndForget();
                    }
                }
            }
            catch (SbDriverUiException sbe)
            {
                _toastService.ShowError(sbe.Message);
            }
        }

        private Dictionary<string, string> SetTopicProperties(TopicResponseDto selectedTopic)
        {
            var properties = new Dictionary<string, string>();
            properties.Add(nameof(selectedTopic.TopicStats.SubscriptionCount).ToSpaceSeperated(), selectedTopic.TopicStats.SubscriptionCount.ToString());
            properties.Add(nameof(selectedTopic.TopicStats.SizeInBytes).ToSpaceSeperated(), selectedTopic.TopicStats.SizeInBytes.ToString());
            properties.Add(nameof(selectedTopic.TopicStats.ScheduledMessageCount).ToSpaceSeperated(), selectedTopic.TopicStats.ScheduledMessageCount.ToString());
            properties.Add(nameof(selectedTopic.TopicCheckBoxProperties.EnableBatchedOperations).ToSpaceSeperated(), selectedTopic.TopicCheckBoxProperties.EnableBatchedOperations.ToString());
            properties.Add(nameof(selectedTopic.TopicCheckBoxProperties.EnablePartitioning).ToSpaceSeperated(), selectedTopic.TopicCheckBoxProperties.EnablePartitioning.ToString());
            properties.Add(nameof(selectedTopic.TopicCheckBoxProperties.RequiresDuplicateDetection).ToSpaceSeperated(), selectedTopic.TopicCheckBoxProperties.RequiresDuplicateDetection.ToString());
            properties.Add(nameof(selectedTopic.TopicCheckBoxProperties.SupportOrdering).ToSpaceSeperated(), selectedTopic.TopicCheckBoxProperties.SupportOrdering.ToString());
            properties.Add(nameof(selectedTopic.TopicDateProperties.AccessedAt).ToSpaceSeperated(), selectedTopic.TopicDateProperties.AccessedAt.ToString());
            properties.Add(nameof(selectedTopic.TopicDateProperties.CreatedAt).ToSpaceSeperated(), selectedTopic.TopicDateProperties.CreatedAt.ToString());
            properties.Add(nameof(selectedTopic.TopicDateProperties.UpdatedAt).ToSpaceSeperated(), selectedTopic.TopicDateProperties.UpdatedAt.ToString());
            properties.Add(nameof(selectedTopic.TopicTimeDetails.AutoDeleteOnIdle).ToSpaceSeperated(), selectedTopic.TopicTimeDetails.AutoDeleteOnIdle.ToString());
            properties.Add(nameof(selectedTopic.TopicTimeDetails.DuplicateDetectionHistoryTimeWindow).ToSpaceSeperated(),
                           selectedTopic.TopicTimeDetails.DuplicateDetectionHistoryTimeWindow.ToString());
            properties.Add(nameof(selectedTopic.TopicTimeDetails.DefaultMessageTimeToLive).ToSpaceSeperated(), selectedTopic.TopicTimeDetails.DefaultMessageTimeToLive.ToString());
            return properties;
        }

        private Dictionary<string, string> SetQueueProperties(QueueResponseDto selectedQueue)
        {
            var properties = new Dictionary<string, string>();
            properties.Add(nameof(selectedQueue.QueueStats.SizeInBytes).ToSpaceSeperated(), selectedQueue.QueueStats.SizeInBytes.ToString());
            properties.Add(nameof(selectedQueue.QueueStats.ScheduledMessageCount).ToSpaceSeperated(), selectedQueue.QueueStats.ScheduledMessageCount.ToString());
            properties.Add(nameof(selectedQueue.QueueCheckBoxProperties.EnableBatchedOperations).ToSpaceSeperated(), selectedQueue.QueueCheckBoxProperties.EnableBatchedOperations.ToString());
            properties.Add(nameof(selectedQueue.QueueCheckBoxProperties.EnablePartitioning).ToSpaceSeperated(), selectedQueue.QueueCheckBoxProperties.EnablePartitioning.ToString());
            properties.Add(nameof(selectedQueue.QueueCheckBoxProperties.RequiresDuplicateDetection).ToSpaceSeperated(), selectedQueue.QueueCheckBoxProperties.RequiresDuplicateDetection.ToString());
            properties.Add(nameof(selectedQueue.QueueCheckBoxProperties.SupportOrdering).ToSpaceSeperated(), selectedQueue.QueueCheckBoxProperties.SupportOrdering.ToString());
            properties.Add(nameof(selectedQueue.QueueDateProperties.AccessedAt).ToSpaceSeperated(), selectedQueue.QueueDateProperties.AccessedAt.ToString());
            properties.Add(nameof(selectedQueue.QueueDateProperties.CreatedAt).ToSpaceSeperated(), selectedQueue.QueueDateProperties.CreatedAt.ToString());
            properties.Add(nameof(selectedQueue.QueueDateProperties.UpdatedAt).ToSpaceSeperated(), selectedQueue.QueueDateProperties.UpdatedAt.ToString());
            properties.Add(nameof(selectedQueue.QueueTimeDetails.AutoDeleteOnIdle).ToSpaceSeperated(), selectedQueue.QueueTimeDetails.AutoDeleteOnIdle.ToString());
            properties.Add(nameof(selectedQueue.QueueTimeDetails.DuplicateDetectionHistoryTimeWindow).ToSpaceSeperated(),
                           selectedQueue.QueueTimeDetails.DuplicateDetectionHistoryTimeWindow.ToString());
            properties.Add(nameof(selectedQueue.QueueTimeDetails.DefaultMessageTimeToLive).ToSpaceSeperated(), selectedQueue.QueueTimeDetails.DefaultMessageTimeToLive.ToString());
            return properties;
        }

        private async void OnSubscriptionValueChanged(ChangeEventArgs e)
        {
            try
            {
                if (e.Value != null && e.Value.ToString().IsNotNullOrWhitespace())
                {
                    _subscriptionNameSelectBox = e.Value.ToString();
                    if (_instanceIdSelectBox.IsNotNullOrWhitespace() && _topicNameSelectBox.IsNotNullOrWhitespace())
                    {
                        var selectedSubscription = _subscriptions.FirstOrDefault(x => x.SubscriptionName == _subscriptionNameSelectBox);

                        _propertiesSpinner = true;
                        _subscriptionDetails = await _subscriptionHandler.GetSubscription(_instanceIdSelectBox, _topicNameSelectBox, _subscriptionNameSelectBox);
                        _activeMsgsText = _subscriptionDetails.SubscriptionStats.ActiveMessageCount.ToString();
                        _deadLetterTxt = _subscriptionDetails.SubscriptionStats.DeadLetterMessageCount.ToString();
                        _propertiesSpinner = false;

                        StateHasChanged();

                        var properties = SetSubscriptionProperties(selectedSubscription);

                        await _propertiesNotifierService.SetSubscriptionProperties(properties);
                        _traceLogsNotifier.AddToQueue("Subscription Details Received for Instance:" + _instanceIdSelectBox + ", Topic:" + _topicNameSelectBox + ", Subscription:" +
                                                      _subscriptionNameSelectBox).SafeFireAndForget();
                    }
                    else
                    {
                        _toastService.ShowError(ApiConstants.UiErrorConstants.InstanceAndTopicNotFound);
                    }
                }
            }
            catch (SbDriverUiException sbe)
            {
                _toastService.ShowError(sbe.Message);
            }
        }

        private Dictionary<string, string> SetSubscriptionProperties(SubscriptionResponseDto selectedSubscription)
        {
            var properties = new Dictionary<string, string>();
            properties.Add(nameof(selectedSubscription.MaxDeliveryCount).ToSpaceSeperated(), selectedSubscription.MaxDeliveryCount.ToString());
            properties.Add(nameof(selectedSubscription.EntityStatus).ToSpaceSeperated(), selectedSubscription.EntityStatus);
            properties.Add(nameof(selectedSubscription.ForwardTo).ToSpaceSeperated(), selectedSubscription.ForwardTo);
            properties.Add(nameof(selectedSubscription.ForwardDeadLetteredMessagesTo).ToSpaceSeperated(), selectedSubscription.ForwardDeadLetteredMessagesTo);
            properties.Add(nameof(selectedSubscription.UserMetadata).ToSpaceSeperated(), selectedSubscription.UserMetadata);
            properties.Add(nameof(selectedSubscription.SubscriptionStats.TotalMessageCount).ToSpaceSeperated(), selectedSubscription.SubscriptionStats.TotalMessageCount.ToString());
            properties.Add(nameof(selectedSubscription.SubscriptionStats.ActiveMessageCount).ToSpaceSeperated(), selectedSubscription.SubscriptionStats.ActiveMessageCount.ToString());
            properties.Add(nameof(selectedSubscription.SubscriptionStats.DeadLetterMessageCount).ToSpaceSeperated(), selectedSubscription.SubscriptionStats.DeadLetterMessageCount.ToString());
            properties.Add(nameof(selectedSubscription.SubscriptionStats.TransferMessageCount).ToSpaceSeperated(), selectedSubscription.SubscriptionStats.TransferMessageCount.ToString());
            properties.Add(nameof(selectedSubscription.SubscriptionStats.TransferDeadLetterMessageCount).ToSpaceSeperated(),
                           selectedSubscription.SubscriptionStats.TransferDeadLetterMessageCount.ToString());
            properties.Add(nameof(selectedSubscription.SubscriptionTimeDetails.LockDuration).ToSpaceSeperated(), selectedSubscription.SubscriptionTimeDetails.LockDuration);
            properties.Add(nameof(selectedSubscription.SubscriptionTimeDetails.DefaultMessageTimeToLive).ToSpaceSeperated(), selectedSubscription.SubscriptionTimeDetails.DefaultMessageTimeToLive);
            properties.Add(nameof(selectedSubscription.SubscriptionTimeDetails.AutoDeleteOnIdle).ToSpaceSeperated(), selectedSubscription.SubscriptionTimeDetails.AutoDeleteOnIdle);
            properties.Add(nameof(selectedSubscription.SubscriptionDateProperties.AccessedAt).ToSpaceSeperated(), selectedSubscription.SubscriptionDateProperties.AccessedAt.ToString());
            properties.Add(nameof(selectedSubscription.SubscriptionDateProperties.CreatedAt).ToSpaceSeperated(), selectedSubscription.SubscriptionDateProperties.CreatedAt.ToString());
            properties.Add(nameof(selectedSubscription.SubscriptionDateProperties.UpdatedAt).ToSpaceSeperated(), selectedSubscription.SubscriptionDateProperties.UpdatedAt.ToString());
            properties.Add(nameof(selectedSubscription.SubscriptionCheckBoxProperties.RequiresSession).ToSpaceSeperated(),
                           selectedSubscription.SubscriptionCheckBoxProperties.RequiresSession.ToString());
            properties.Add(nameof(selectedSubscription.SubscriptionCheckBoxProperties.DeadLetteringOnMessageExpiration).ToSpaceSeperated(),
                           selectedSubscription.SubscriptionCheckBoxProperties.DeadLetteringOnMessageExpiration.ToString());
            properties.Add(nameof(selectedSubscription.SubscriptionCheckBoxProperties.EnableDeadLetteringOnFilterEvaluationExceptions).ToSpaceSeperated(),
                           selectedSubscription.SubscriptionCheckBoxProperties.EnableDeadLetteringOnFilterEvaluationExceptions.ToString());
            properties.Add(nameof(selectedSubscription.SubscriptionCheckBoxProperties.EnableBatchedOperations).ToSpaceSeperated(),
                           selectedSubscription.SubscriptionCheckBoxProperties.EnableBatchedOperations.ToString());
            return properties;
        }

        private async void PeekActiveMessages()
        {
            try
            {
                if (ValidatePropertiesAreNotNull() && ValidatePeekParams(_activeMsgsText, _activeMsgCountInput))
                {
                    _peekActiveMessageSpinner = true;
                    _messages = await _messageHandler.GetActiveMessages(_instanceIdSelectBox, _queueNameSelectBox, _topicNameSelectBox, _subscriptionNameSelectBox, _activeMsgCountInput);

                    var typeBreadCrumb = _featureIsQueue ? _queueNameSelectBox : _topicNameSelectBox;
                    var subsBreadCrumb = _featureIsQueue ? null : _subscriptionNameSelectBox;
                    await _messageNotifierService.SetCurrentPropertiesAndMessages(_instanceIdSelectBox, typeBreadCrumb, subsBreadCrumb, "Active", _messages);

                    _peekActiveMessageSpinner = false;
                    StateHasChanged();

                    _traceLogsNotifier.AddToQueue("Message Details Received for Instance:" + _instanceIdSelectBox + ", Topic:" + _topicNameSelectBox + ", Subscription:" + _subscriptionNameSelectBox)
                                      .SafeFireAndForget();
                }
            }
            catch (SbDriverUiException sbe)
            {
                _toastService.ShowError(sbe.Message);
            }
        }

        private async void PeekDeadLetter()
        {
            try
            {
                if (ValidatePropertiesAreNotNull() && ValidatePeekParams(_deadLetterTxt, _deadLetterMsgCountInput))
                {
                    _peekDlqMessageSpinner = true;
                    _messages = await _messageHandler.GetDeadLetterMessages(_instanceIdSelectBox, _queueNameSelectBox, _topicNameSelectBox, _subscriptionNameSelectBox, _deadLetterMsgCountInput);

                    var typeBreadCrumb = _featureIsQueue ? _queueNameSelectBox : _topicNameSelectBox;
                    var subsBreadCrumb = _featureIsQueue ? null : _subscriptionNameSelectBox;
                    await _messageNotifierService.SetCurrentPropertiesAndMessages(_instanceIdSelectBox, typeBreadCrumb, subsBreadCrumb, "Active", _messages);

                    _peekDlqMessageSpinner = false;
                    StateHasChanged();

                    _traceLogsNotifier.AddToQueue("Message Details Received for Instance:" + _instanceIdSelectBox + ", Topic:" + _topicNameSelectBox + ", Subscription:" +
                                                  _subscriptionNameSelectBox).SafeFireAndForget();
                }
            }
            catch (SbDriverUiException sbe)
            {
                _toastService.ShowError(sbe.Message);
            }
        }

        private async void PeekLastNMessages()
        {
            try
            {
                if (ValidatePropertiesAreNotNull() && ValidatePeekParams(_activeMsgsText, _lastNMsgCountInput))
                {
                    _peekLastNMessageSpinner = true;
                    _messages = await _messageHandler.GetLastNMessages(_instanceIdSelectBox, _queueNameSelectBox, _topicNameSelectBox, _subscriptionNameSelectBox,  false, _lastNMsgCountInput);
                    
                    var typeBreadCrumb = _featureIsQueue ? _queueNameSelectBox : _topicNameSelectBox;
                    var subsBreadCrumb = _featureIsQueue ? null : _subscriptionNameSelectBox;
                    await _messageNotifierService.SetCurrentPropertiesAndMessages(_instanceIdSelectBox, typeBreadCrumb, subsBreadCrumb, "Active", _messages);

                    _peekLastNMessageSpinner = false;
                    StateHasChanged();

                    _traceLogsNotifier.AddToQueue("Message Details Received for Instance:" + _instanceIdSelectBox + ", Topic:" + _topicNameSelectBox + ", Subscription:" +
                                                  _subscriptionNameSelectBox).SafeFireAndForget();
                }
            }
            catch (SbDriverUiException sbe)
            {
                _toastService.ShowError(sbe.Message);
            }
        }

        private bool ValidatePeekParams(string messagesInQueue, int messagesToPeek)
        {
            var hasMessages = messagesInQueue != "0";
            var result = true;
            if (hasMessages)
            {
                if (messagesToPeek <= 0)
                {
                    _traceLogsNotifier.AddToQueue(TraceTypeEnum.ERROR, "Peek Count not valid").SafeFireAndForget();
                    _toastService.ShowError("Peek Count not valid");
                    result = false;
                }

                if (messagesToPeek >= 4000)
                {
                    _toastService.ShowWarning($"Operation will take approx {messagesToPeek / 1000} seconds");
                }
            }
            else
            {
                result = false;
                _traceLogsNotifier.AddToQueue(TraceTypeEnum.ERROR, "No Messages In Queue. Cannot Peek").SafeFireAndForget();
                _toastService.ShowError("No Messages In Queue. Cannot Peek");
            }

            return result;
        }



        private void UpdateSearchQueue(bool deadLetterQueue)
        {
            _searchDlQueueRadio = deadLetterQueue;
        }

        private async Task SearchMessages()
        {
            try
            {
                if (ValidatePropertiesAreNotNull())
                {
                    if (_searchKeyInput.IsNullOrWhiteSpace())
                    {
                        _toastService.ShowError("Invalid Search Key");
                    }
                    else
                    {
                        bool hasMessages;
                        if (_searchDlQueueRadio)
                        {
                            hasMessages = _deadLetterTxt != "0";
                        }
                        else
                        {
                            hasMessages = _activeMsgsText != "0";
                        }

                        if (hasMessages)
                        {
                            var totalMessages = Convert.ToInt32(_searchActiveQueueRadio ? _activeMsgsText : _deadLetterTxt);
                            if (totalMessages >= 4000)

                            {
                                _toastService.ShowWarning($"Operation will take approx {totalMessages / 1000} seconds");
                            }


                            _searchMessageSpinner = true;
                            _messages = await _messageHandler.SearchMessages(_instanceIdSelectBox, _queueNameSelectBox, _topicNameSelectBox, _subscriptionNameSelectBox, _searchPathInput, _searchKeyInput,
                                                                             _searchDlQueueRadio);

                            await _messageNotifierService.SetCurrentPropertiesAndMessages(_instanceIdSelectBox, _topicNameSelectBox, _subscriptionNameSelectBox,
                                                                                          _searchDlQueueRadio ? "Dead Letter" : "Active", _messages);

                            _searchMessageSpinner = false;
                            StateHasChanged();

                            _traceLogsNotifier.AddToQueue("Search results received for Instance:" + _instanceIdSelectBox + ", Topic:" + _topicNameSelectBox + ", Subscription:" +
                                                          _subscriptionNameSelectBox).SafeFireAndForget();
                        }
                        else
                        {
                            _toastService.ShowError("No Messages In Queue. Cannot search");
                        }
                    }
                }
            }
            catch (SbDriverUiException sbe)
            {
                _toastService.ShowError(sbe.Message);
            }
        }


        private bool ValidatePropertiesAreNotNull()
        {
            if (_featureIsQueue)
            {
                if (_instanceIdSelectBox.IsNotNullOrWhitespace() && _queueNameSelectBox.IsNotNullOrWhitespace())
                {
                    return true;
                }

                _toastService.ShowError(ApiConstants.UiErrorConstants.InstanceAndQueueNotFound);
            }
            else
            {
                if (_instanceIdSelectBox.IsNotNullOrWhitespace() && _topicNameSelectBox.IsNotNullOrWhitespace() && _subscriptionNameSelectBox.IsNotNullOrWhitespace())
                {
                    return true;
                }

                _toastService.ShowError(ApiConstants.UiErrorConstants.InstanceAndTopicNotFound);
            }

            return false;
        }

        private void ToggleRuleTable()
        {
            _ruleTableVisible = !_ruleTableVisible;
        }
    }
}