using ServiceBusDriver.Core.Models.Features.Instance;
using System.Collections.Generic;
using ServiceBusDriver.Core.Constants;

namespace ServiceBusDriver.Core.Components.Instance
{
    public class InstanceRepository
    {
        private static List<ServiceBusInstanceModel> _serviceBusInstances;

        public static List<ServiceBusInstanceModel> GetInstances()
        {
            return _serviceBusInstances ??= GetInstanceData();
        }

        public static List<ServiceBusInstanceModel> GetInstanceData()
        {
            // Override method to return correct values
            return DemoDataConstants.DummyInstances;

        }
    }
}