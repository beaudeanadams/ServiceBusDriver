using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AsyncAwaitBestPractices;
using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using ServiceBusDriver.Client.Constants;
using ServiceBusDriver.Client.Services;
using ServiceBusDriver.Client.UIComponents.Helpers;
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
        private bool _receiveAndDeleteActiveMessageSpinner;
        private bool _receiveAndDeleteDlMessageSpinner;
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

        // Process topic changed event
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

                        var properties = PropertiesHelper.SetTopicProperties(selectedTopic);

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

        // Process queue changed event
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
                        var properties = PropertiesHelper.SetQueueProperties(selectedQueue);
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


        // Process subscription changed event
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

                        var properties = PropertiesHelper.SetSubscriptionProperties(selectedSubscription);

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

        private async void PeekActiveMessages()
        {
            try
            {
                if (ValidatePropertiesAreNotNull() && ValidateFetchParams(_activeMsgsText, _activeMsgCountInput))
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

        //Show confirm Alert for receive and delete operation
        private async void ReceiveAndDeleteShowConfirmAlert(bool isDeadLetterFetch)
        {
            if (!isDeadLetterFetch)
            {
                var swalHelper = new SweetAlertHelper();
                await swalHelper.ShowSweetAlertConfirm(_swal, null,
                                                       "This action will delete the message from queue",
                                                       true, null, null,
                                                       HandleConfirmAlertResultForActiveFetch);
            }
            else
            {
                var swalHelper = new SweetAlertHelper();
                await swalHelper.ShowSweetAlertConfirm(_swal, null,
                                                       "This action will delete the message from queue",
                                                       true, null, null,
                                                       HandleConfirmAlertResultForDeadLetterReceive);
            }
        }

        // Perform task based on user action on confirm box for receive and delete 
        public async Task HandleConfirmAlertResultForActiveFetch(SweetAlertResult result)
        {
            if (result.IsConfirmed)
            {
                try
                {
                    if (ValidatePropertiesAreNotNull() && ValidateFetchParams(_activeMsgsText, _activeMsgCountInput))
                    {
                        _receiveAndDeleteActiveMessageSpinner = true;
                        _messages = await _messageHandler.GetActiveMessages(_instanceIdSelectBox, _queueNameSelectBox, _topicNameSelectBox, _subscriptionNameSelectBox, _activeMsgCountInput, true);

                        var typeBreadCrumb = _featureIsQueue ? _queueNameSelectBox : _topicNameSelectBox;
                        var subsBreadCrumb = _featureIsQueue ? null : _subscriptionNameSelectBox;
                        await _messageNotifierService.SetCurrentPropertiesAndMessages(_instanceIdSelectBox, typeBreadCrumb, subsBreadCrumb, "Active", _messages);

                        _receiveAndDeleteActiveMessageSpinner = false;
                        OnSubscriptionValueChanged(new ChangeEventArgs()
                        {
                            Value = _subscriptionNameSelectBox
                        });
                        StateHasChanged();

                        _traceLogsNotifier.AddToQueue("Message Details Received for Instance:" + _instanceIdSelectBox + ", Topic:" + _topicNameSelectBox + ", Subscription:" +
                                                      _subscriptionNameSelectBox)
                                          .SafeFireAndForget();
                    }
                }
                catch (SbDriverUiException sbe)
                {
                    _toastService.ShowError(sbe.Message);
                }
            }
            else if(result.Dismiss == DismissReason.Cancel)
            {
                await _swal.FireAsync(
                    "Cancelled",
                    "Messages not fetched. Try Peek instead",
                    SweetAlertIcon.Error);
            }
        }

        private async void PeekDeadLetter()
        {
            try
            {
                if (ValidatePropertiesAreNotNull() && ValidateFetchParams(_deadLetterTxt, _deadLetterMsgCountInput))
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

        public async Task HandleConfirmAlertResultForDeadLetterReceive(SweetAlertResult result)
        {
            if (result.IsConfirmed)
            {
                try
                {
                    if (ValidatePropertiesAreNotNull() && ValidateFetchParams(_deadLetterTxt, _deadLetterMsgCountInput))
                    {
                        _receiveAndDeleteActiveMessageSpinner = true;
                        _messages = await _messageHandler.GetDeadLetterMessages(_instanceIdSelectBox, _queueNameSelectBox, _topicNameSelectBox, _subscriptionNameSelectBox, _deadLetterMsgCountInput,
                                                                                true);

                        var typeBreadCrumb = _featureIsQueue ? _queueNameSelectBox : _topicNameSelectBox;
                        var subsBreadCrumb = _featureIsQueue ? null : _subscriptionNameSelectBox;
                        await _messageNotifierService.SetCurrentPropertiesAndMessages(_instanceIdSelectBox, typeBreadCrumb, subsBreadCrumb, "Active", _messages);

                        _receiveAndDeleteActiveMessageSpinner = false;
                        OnSubscriptionValueChanged(new ChangeEventArgs()
                        {
                            Value = _subscriptionNameSelectBox
                        });
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
            else if(result.Dismiss == DismissReason.Cancel)
            {
                await _swal.FireAsync(
                    "Cancelled",
                    "Messages not fetched. Try Peek instead",
                    SweetAlertIcon.Error);
            }
        }

        private async void PeekLastNMessages()
        {
            try
            {
                if (ValidatePropertiesAreNotNull() && ValidateFetchParams(_activeMsgsText, _lastNMsgCountInput))
                {
                    _peekLastNMessageSpinner = true;
                    _messages = await _messageHandler.GetLastNMessages(_instanceIdSelectBox, _queueNameSelectBox, _topicNameSelectBox, _subscriptionNameSelectBox, false, _lastNMsgCountInput);

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

        private bool ValidateFetchParams(string messagesInQueue, int messagesToPeek)
        {
            var hasMessages = messagesInQueue != "0";
            var result = true;
            if (hasMessages)
            {
                if (messagesToPeek <= 0)
                {
                    _traceLogsNotifier.AddToQueue(TraceTypeEnum.ERROR, "Fetch Count not valid").SafeFireAndForget();
                    _toastService.ShowError("Fetch Count not valid");
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
                _traceLogsNotifier.AddToQueue(TraceTypeEnum.ERROR, "No Messages In Queue. Cannot Fetch").SafeFireAndForget();
                _toastService.ShowError("No Messages In Queue. Cannot Fetch");
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
                            _messages = await _messageHandler.SearchMessages(_instanceIdSelectBox, _queueNameSelectBox, _topicNameSelectBox, _subscriptionNameSelectBox, _searchPathInput,
                                                                             _searchKeyInput,
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