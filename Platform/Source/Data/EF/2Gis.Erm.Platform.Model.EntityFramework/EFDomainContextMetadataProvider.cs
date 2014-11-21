using System;
using System.Collections.Generic;
using System.Reflection;

using DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.Platform.Model.EntityFramework
{
    public class EFDomainContextMetadataProvider : IDomainContextMetadataProvider
    {
        public const string ErmEntityContainer = "Erm";
        public const string ErmSecurityEntityContainer = "ErmSecurity";

        private readonly DomainContextMetadata _ermDomainContextMetadata = new DomainContextMetadata
        {
            EntityContainerName = "Erm",
            Assembly = Assembly.GetExecutingAssembly(),
            PathToEdmx = "Erm.Erm", // Erm это имя папки в которой лежит Erm.edmx
        };
        private readonly DomainContextMetadata _ermSecurityDomainContextMetadata = new DomainContextMetadata
        {
            EntityContainerName = ErmSecurityEntityContainer,
            Assembly = Assembly.GetExecutingAssembly(),
            PathToEdmx = "ErmSecurity.ErmSecurity", // ErmSecurity это имя папки в которой лежит ErmSecurity.edmx
        };

        public EFDomainContextMetadataProvider()
        {
            ReadConnectionStringNameMap = new Dictionary<string, ConnectionStringName>
            {
                { ErmEntityContainer, ConnectionStringName.Erm },
                { ErmSecurityEntityContainer, ConnectionStringName.Erm },
            };
            WriteConnectionStringNameMap = new Dictionary<string, ConnectionStringName>
            {
                { ErmEntityContainer, ConnectionStringName.Erm },
                { ErmSecurityEntityContainer, ConnectionStringName.Erm },
            };
        }

        public Dictionary<string, ConnectionStringName> ReadConnectionStringNameMap { get; set; }
        public Dictionary<string, ConnectionStringName> WriteConnectionStringNameMap { get; set; }

        DomainContextMetadata IDomainContextMetadataProvider.GetReadMetadata(Type entityType)
        {
            var readMetadata = GetDomainContextMetadata(entityType);
            readMetadata.ConnectionStringName = ReadConnectionStringNameMap[readMetadata.EntityContainerName];

            return readMetadata;
        }

        DomainContextMetadata IDomainContextMetadataProvider.GetWriteMetadata(Type entityType)
        {
            var writeMetadata = GetDomainContextMetadata(entityType);
            writeMetadata.ConnectionStringName = WriteConnectionStringNameMap[writeMetadata.EntityContainerName];

            return writeMetadata;
        }

        private DomainContextMetadata GetDomainContextMetadata(Type entityType)
        {
            if (Array.IndexOf(ErmEntities.Entities, entityType) >= 0)
            {
                return _ermDomainContextMetadata;
            }

            if (Array.IndexOf(ErmSecurityEntities.Entities, entityType) >= 0)
            {
                return _ermSecurityDomainContextMetadata;
            }

            throw new ArgumentOutOfRangeException("entityType");
        }
    }
}