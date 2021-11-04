using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Azure.Messaging.ServiceBus;
using ServiceBusDriver.Core.Constants;
using ServiceBusDriver.Core.Models.Errors;
using ServiceBusDriver.Core.Models.Features.Instance;

namespace ServiceBusDriver.Core.Tools
{
    // https://github.com/paolosalvatori/ServiceBusExplorer
    public static class ServiceBusNamespaceTool
    {
        public static ServiceBusNamespaceModel GetServiceBusNamespace(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                return null;
            }

            var toLower = connectionString.ToLower();

            var parameters = connectionString.Split(';')
                                             .ToDictionary(s => s.Substring(0, s.IndexOf('=')).ToLower(), s => s.Substring(s.IndexOf('=') + 1));

            if (toLower.Contains(ServiceBusConstants.ConnectionStringEndpoint) &&
                toLower.Contains(ServiceBusConstants.ConnectionStringSharedAccessKeyName) &&
                toLower.Contains(ServiceBusConstants.ConnectionStringSharedAccessKey))
            {
                return GetServiceBusNamespaceUsingSas(connectionString, parameters);
            }


            return null;
        }

        public static ServiceBusNamespaceModel GetServiceBusNamespaceUsingSas(string connectionString, Dictionary<string, string> parameters)
        {
            if (parameters.Count < 3)
            {
                return null;
            }

            var endpoint = parameters.ContainsKey(ServiceBusConstants.ConnectionStringEndpoint) ? parameters[ServiceBusConstants.ConnectionStringEndpoint] : null;

            if (string.IsNullOrWhiteSpace(endpoint))
            {
                return null;
            }

            if (!endpoint.Contains("://"))
            {
                endpoint = "sb://" + endpoint;
            }

            var stsEndpoint = parameters.ContainsKey(ServiceBusConstants.ConnectionStringStsEndpoint) ? parameters[ServiceBusConstants.ConnectionStringStsEndpoint] : null;

            var ns = GetNamespaceNameFromEndpoint(endpoint);

            if (!parameters.ContainsKey(ServiceBusConstants.ConnectionStringSharedAccessKeyName) ||
                string.IsNullOrWhiteSpace(parameters[ServiceBusConstants.ConnectionStringSharedAccessKeyName]))
            {
            }

            var sharedAccessKeyName = parameters[ServiceBusConstants.ConnectionStringSharedAccessKeyName];

            if (!parameters.ContainsKey(ServiceBusConstants.ConnectionStringSharedAccessKey) || string.IsNullOrWhiteSpace(parameters[ServiceBusConstants.ConnectionStringSharedAccessKey]))
            {
                throw new SbDriverException()
                {
                    ErrorMessage = new ErrorMessageModel
                    {
                        UserMessageText = string.Format(CultureInfo.CurrentCulture, ServiceBusConstants.ServiceBusNamespaceSharedAccessKeyIsInvalid,
                            ServiceBusConstants.ConnectionStringSharedAccessKey)
                    }
                };
            }

            var sharedAccessKey = parameters[ServiceBusConstants.ConnectionStringSharedAccessKey];
            var transportType = ServiceBusTransportType.AmqpTcp;
            if (parameters.ContainsKey(ServiceBusConstants.ConnectionStringTransportType))
            {
                Enum.TryParse(parameters[ServiceBusConstants.ConnectionStringTransportType], true, out transportType);
            }

            var entityPath = string.Empty;

            if (parameters.ContainsKey(ServiceBusConstants.ConnectionStringEntityPath))
            {
                entityPath = parameters[ServiceBusConstants.ConnectionStringEntityPath];
            }

            var serviceBusNamespaceModel = new ServiceBusNamespaceModel
            {
                ConnectionStringType = ServiceBusNamespaceType.Cloud,
                ConnectionString = connectionString,
                Uri = endpoint,
                Namespace = ns,
                ServicePath = null,
                TransportType = transportType,
                StsEndpoint = stsEndpoint,
                RuntimePort = null,
                ManagementPort = null,
                SharedAccessKeyName = sharedAccessKeyName,
                SharedAccessKey = sharedAccessKey,
                EntityPath = entityPath
            };

            return serviceBusNamespaceModel;
        }

        public static string GetNamespaceNameFromEndpoint(string endpoint)
        {
            Uri uri;

            try
            {
                uri = new Uri(endpoint);
            }
            catch (Exception)
            {
                return null;
            }

            return uri.Host.Split('.')[0];
        }
    }
}