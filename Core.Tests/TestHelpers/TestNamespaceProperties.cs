using System;
using Azure.Messaging.ServiceBus.Administration;

namespace ServiceBusDriver.Core.Tests.TestHelpers
{
    public class TestNamespaceProperties : NamespaceProperties
    {
        /// <summary>
        /// Name of the namespace.
        /// </summary>
        public new string Name { get; set; }

        /// <summary>
        /// The time at which the namespace was created.
        /// </summary>
        public new DateTimeOffset CreatedTime { get; set; }

        /// <summary>
        /// The last time at which the namespace was modified.
        /// </summary>
        public new DateTimeOffset ModifiedTime { get; set; }

        /// <summary>
        /// The SKU/tier of the namespace. Valid only for <see cref="NamespaceType.Messaging"/>
        /// </summary>
        public new MessagingSku MessagingSku { get; set; }

        /// <summary>
        /// The number of messaging units allocated for namespace.
        /// Valid only for <see cref="NamespaceType.Messaging"/> and <see cref="MessagingSku.Premium"/>
        /// </summary>
        public new int MessagingUnits { get; set; }

        /// <summary>
        /// The alias for the namespace.
        /// </summary>
        public new string Alias { get; internal set; }
    }
}