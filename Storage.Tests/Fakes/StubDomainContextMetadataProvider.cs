using System;
using System.Collections.Generic;

using NuClear.Model.Common;
using NuClear.Storage.ConnectionStrings;
using NuClear.Storage.Core;

using Storage.Tests.EntityTypes;

namespace Storage.Tests.Fakes
{
    public class StubDomainContextMetadataProvider : IDomainContextMetadataProvider
    {
        private static readonly Dictionary<Type, DomainContextMetadata> ContextMappings = new Dictionary<Type, DomainContextMetadata>();

        private static readonly DomainContextMetadata Context = new DomainContextMetadata
            {
                ConnectionStringName = ConnectionStringIdentity.Instance,
                EntityContainerName = "ContainerName",
            };

        static StubDomainContextMetadataProvider()
        {
            ContextMappings.Add(typeof(Entity1), Context);
            ContextMappings.Add(typeof(Entity2), Context);
        }

        DomainContextMetadata IDomainContextMetadataProvider.GetReadMetadata(Type entityType)
        {
            return ContextMappings[entityType];
        }

        DomainContextMetadata IDomainContextMetadataProvider.GetWriteMetadata(Type entityType)
        {
            return ContextMappings[entityType];
        }

        private class ConnectionStringIdentity : IdentityBase<ConnectionStringIdentity>, IConnectionStringIdentity
        {
            public override int Id
            {
                get { return 1; }
            }

            public override string Description
            {
                get { return "Test Connection String"; }
            }
        }
    }
}