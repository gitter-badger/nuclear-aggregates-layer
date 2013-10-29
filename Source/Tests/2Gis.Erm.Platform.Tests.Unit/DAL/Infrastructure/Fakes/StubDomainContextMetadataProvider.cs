﻿using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Tests.Unit.DAL.Infrastructure.Fakes.EntityTypes;

namespace DoubleGis.Erm.Platform.Tests.Unit.DAL.Infrastructure.Fakes
{
    public class StubDomainContextMetadataProvider : IDomainContextMetadataProvider
    {
        private static readonly Dictionary<Type, DomainContextMetadata> ContextMappings = new Dictionary<Type, DomainContextMetadata>();

        private static readonly DomainContextMetadata ErmContext = new DomainContextMetadata
            {
                ConnectionStringName = ConnectionStringName.Erm,
                Assembly = null,
                EntityContainerName = "Model.Erm",
                PathToEdmx = "Erm.Erm"
            };

        private static readonly DomainContextMetadata SecurityContext = new DomainContextMetadata
            {
                ConnectionStringName = ConnectionStringName.Erm,
                Assembly = null,
                EntityContainerName = "Model.ErmSecurity",
                PathToEdmx = "ErmSecurity.ErmSecurity"
            };

        static StubDomainContextMetadataProvider()
        {
            ContextMappings.Add(typeof(ErmScopeEntity1), ErmContext);
            ContextMappings.Add(typeof(ErmScopeEntity2), ErmContext);

            ContextMappings.Add(typeof(SecurityScopeEntity1), SecurityContext);
            ContextMappings.Add(typeof(SecurityScopeEntity2), SecurityContext);
        }

        DomainContextMetadata IDomainContextMetadataProvider.GetReadMetadata(Type entityType)
        {
            return ContextMappings[entityType];
        }

        DomainContextMetadata IDomainContextMetadataProvider.GetWriteMetadata(Type entityType)
        {
            return ContextMappings[entityType];
        }
    }
}