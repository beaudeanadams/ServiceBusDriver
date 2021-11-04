using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using ServiceBusDriver.Core.Models.Features.Instance;

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
            try
            {
                var instanceJson = ReadResource("ServiceBusVerifier.Services.Components.Instance.instances.json");
                return JsonConvert.DeserializeObject<List<ServiceBusInstanceModel>>(instanceJson);
            }
            catch (Exception)
            {
                return new List<ServiceBusInstanceModel>
                {
                    new ServiceBusInstanceModel
                    {
                        Id = "8e495aaa-7bd8-44ea-83f5-7c5d166ee7ff",
                        Name = "tst-edc-denovo-sb01",
                        ConnectionString =
                            "Endpoint=sb://tst-edc-boq-sb01.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=awnamRP+v1BATFB5KmBchYpgGtrLYN5lwYuEoVFJ4Y0="
                    },
                    new ServiceBusInstanceModel
                    {
                        Id = "7582be0d-aebc-4a05-9734-451c712891e9",
                        Name = "tst-edc-boq-sb01",
                        ConnectionString =
                            "Endpoint=sb://tst-edc-boq-sb01.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=awnamRP+v1BATFB5KmBchYpgGtrLYN5lwYuEoVFJ4Y0="
                    }
                };
            }
        }

        public static string ReadResource(string name)
        {
            var assembly = typeof(InstanceRepository).Assembly;

            var resourcePath = assembly.GetManifestResourceNames()
                .Single(str => str.EndsWith(name));

            var stream = assembly.GetManifestResourceStream(resourcePath);
            var reader = new StreamReader(stream);

            return reader.ReadToEnd();
        }
    }
}