using System;
using System.Threading.Tasks;
using AsyncAwaitBestPractices;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using ServiceBusDriver.Core.Models.Features.Connection;
using ServiceBusDriver.Shared.Features.Trace;
using ServiceBusDriver.Shared.Tools;

namespace ServiceBusDriver.Client.UIComponents.Components.TopMenuComponents
{
    public partial class AddInstance
    {
        private string _connectionString;

        //UI Spinners
        private bool _processConnectionSpinner;
        private bool _testConnectionSpinner;
        private bool _addInstanceSpinner;

        private bool processSuccessful;
        private bool connectivitySuccessful;

        [CascadingParameter]
        public Task<AuthenticationState> AuthenticationState { get; set; }

        private ConnectionSettingsModel _connectionSettingsModel;

        private void ResetConnectionString()
        {
            _connectionSettingsModel = null;
            processSuccessful = false;
            connectivitySuccessful = false;
        }

        private async Task ProcessConnectionString()
        {
            if (_connectionString.IsNullOrEmpty())
            {
                _toastService.ShowError("Connection string cannot be empty");
                return; 
            }
            
            _processConnectionSpinner = true;
            processSuccessful = false;
            try
            {
                _connectionSettingsModel = await _instanceHandler.ProcessConnectionString(_connectionString);
                processSuccessful = true;
            }
            catch (Exception e)
            {
                _toastService.ShowError(e.Message);
                _traceLogsNotifier.AddToQueue(TraceTypeEnum.ERROR, e.Message).SafeFireAndForget();
            }

            _processConnectionSpinner = false;
        }

        private async Task AddNewInstance()
        {
            if (_connectionString.IsNullOrEmpty())
            {
                _toastService.ShowError("Connection string cannot be empty");
                return; 
            }
            _addInstanceSpinner = true;
            try
            {
                var response = await _instanceHandler.AddInstance(_connectionString);
                _toastService.ShowSuccess($"New Instance Added - {response.Name}. Please refresh this window");
                _navigationManager.NavigateTo("/" ,true);
                
            }
            catch (Exception e)
            {
                _toastService.ShowError(e.Message);
                _traceLogsNotifier.AddToQueue(TraceTypeEnum.ERROR, e.Message).SafeFireAndForget();
            }

            _addInstanceSpinner = false;
        }

        private async Task TestConnection()
        {
            if (_connectionString.IsNullOrEmpty())
            {
                _toastService.ShowError("Connection string cannot be empty");
                return; 
            }
            _testConnectionSpinner = true;
            connectivitySuccessful = false;
            try
            {
                _connectionSettingsModel = await _instanceHandler.TestConnectivity(_connectionString);
                connectivitySuccessful = true;
            }
            catch (Exception e)
            {
                _toastService.ShowError(e.Message);
                _traceLogsNotifier.AddToQueue(TraceTypeEnum.ERROR, e.Message).SafeFireAndForget();
            }

            _testConnectionSpinner = false;
        }
    }
}